using System.Text.Json;
using System.Text.Json.Serialization;
using HappyFrog.Models;

namespace HappyFrog;

public class BookProcessor
{
    private static readonly JsonSerializerOptions Json = new()
    {
        PropertyNameCaseInsensitive = true,
        ReadCommentHandling = JsonCommentHandling.Skip,
        AllowTrailingCommas = true,
        Converters = { new BookChapterRefConverter() }
    };

    private readonly HappyFrogConfig _config;
    private readonly PostProcessor _posts;

    public BookProcessor(HappyFrogConfig config, PostProcessor posts)
    {
        _config = config;
        _posts = posts;
    }

    public record ProcessedBook(
        BookConfig Config,
        BookManifest Manifest,
        List<BlogPostModel> Chapters,
        BookIndexModel Index);

    /// <summary>Process every configured (non-planned) book exactly once.</summary>
    public List<ProcessedBook> ProcessBooks()
    {
        var result = new List<ProcessedBook>();

        foreach (var book in _config.Books.Where(b => !b.Planned))
        {
            if (!Directory.Exists(book.Path))
            {
                Console.WriteLine($"  Warning: book path not found: {book.Path}");
                continue;
            }

            var manifest = LoadManifest(book);
            var written = _posts.ProcessDirectory(book.Path)
                                .GroupBy(p => p.Slug)
                                .ToDictionary(g => g.Key, g => g.First());

            var index = BuildIndex(book, manifest, written);
            var orderedChapters = written.Values
                .OrderBy(c => c.ChapterNumber ?? int.MaxValue)
                .ThenBy(c => c.PublishDate)
                .ToList();

            result.Add(new ProcessedBook(book, manifest, orderedChapters, index));
            Console.WriteLine($"  Book '{manifest.Title}': {index.ChaptersAvailable}/{index.ChaptersTotal} chapters");
        }

        // Build the cross-book shelf once (real books + planned ones), attach to each index.
        var shelf = result
            .Select(r => new ShelfItem
            {
                Title = r.Manifest.Title,
                BaseUrl = $"books/{r.Config.Id}/",
                Status = r.Manifest.Status.ToUpper()
            })
            .Concat(_config.Books
                .Where(b => b.Planned)
                .Select(b => new ShelfItem { Title = b.Title, Planned = true }))
            .ToList();

        foreach (var r in result)
            r.Index.OtherBooks = shelf.Where(s => s.Title != r.Manifest.Title).ToList();

        return result;
    }

    private static BookManifest LoadManifest(BookConfig book)
    {
        var path = Path.Combine(book.Path, "book.json");
        if (File.Exists(path))
        {
            var m = JsonSerializer.Deserialize<BookManifest>(File.ReadAllText(path), Json);
            if (m != null)
            {
                if (string.IsNullOrEmpty(m.Category)) m.Category = book.Category;
                if (string.IsNullOrEmpty(m.Id)) m.Id = book.Id;
                return m;
            }
        }

        // Fallback when a book has no manifest: one unnamed part, chapters discovered from disk.
        Console.WriteLine($"  Note: no book.json for '{book.Id}'; using config defaults.");
        return new BookManifest
        {
            Id = book.Id,
            Title = book.Title,
            Subtitle = book.Description,
            Category = book.Category
        };
    }

    private static BookIndexModel BuildIndex(
        BookConfig book, BookManifest m, Dictionary<string, BlogPostModel> written)
    {
        var parts = new List<BookPart>();
        int n = 0, available = 0;
        string? first = null;

        // If the manifest declares no parts, synthesise one from whatever is on disk.
        var partConfigs = m.Parts.Count > 0
            ? m.Parts
            : new List<BookPartConfig>
            {
                new()
                {
                    Label = "Chapters",
                    Title = "",
                    Chapters = written.Values
                        .OrderBy(c => c.ChapterNumber ?? int.MaxValue)
                        .Select(c => new BookChapterRef { Slug = c.Slug })
                        .ToList()
                }
            };

        foreach (var pc in partConfigs)
        {
            var part = new BookPart { Label = pc.Label, Title = pc.Title, Epigraph = pc.Epigraph };

            foreach (var cref in pc.Chapters)
            {
                n++;
                var has = written.TryGetValue(cref.Slug, out var post);
                if (has)
                {
                    available++;
                    first ??= cref.Slug;
                }

                part.Chapters.Add(new BookChapter
                {
                    ChapterNumber = post?.ChapterNumber ?? n,
                    Slug = cref.Slug,
                    Title = post?.Title ?? cref.Title ?? Humanise(cref.Slug),
                    Description = post?.Description ?? cref.Description ?? "",
                    ReadingTimeMinutes = post?.ReadingTimeMinutes ?? 0,
                    Available = has
                });
            }

            parts.Add(part);
        }

        return new BookIndexModel
        {
            Category = m.Category,
            CategoryUrl = $"../../{m.Category}.html",
            CategoryLabel = CategoryInfo.Categories.TryGetValue(m.Category, out var info) ? info.Title : m.Category,
            Title = m.Title,
            Description = m.Subtitle,
            CoverImage = book.CoverImage,
            ChaptersTotal = m.TotalChapters ?? n,
            ChaptersAvailable = available,
            Progress = (m.TotalChapters ?? n) == 0 ? 0 : (int)Math.Round(available / (double)(m.TotalChapters ?? n) * 100),
            FirstChapter = first,
            Parts = parts
        };
    }

    private static string Humanise(string slug) =>
        string.Join(' ', slug.Split('-').Select(w => w.Length == 0 ? w : char.ToUpper(w[0]) + w[1..]));
}

/// <summary>Lets book.json chapters be either "slug" or { "slug": ..., "title": ..., "description": ... }.</summary>
public class BookChapterRefConverter : JsonConverter<BookChapterRef>
{
    public override BookChapterRef Read(ref Utf8JsonReader reader, Type type, JsonSerializerOptions options)
    {
        if (reader.TokenType == JsonTokenType.String)
            return new BookChapterRef { Slug = reader.GetString() ?? "" };

        using var doc = JsonDocument.ParseValue(ref reader);
        var el = doc.RootElement;
        return new BookChapterRef
        {
            Slug = el.TryGetProperty("slug", out var s) ? s.GetString() ?? "" : "",
            Title = el.TryGetProperty("title", out var t) ? t.GetString() : null,
            Description = el.TryGetProperty("description", out var d) ? d.GetString() : null
        };
    }

    public override void Write(Utf8JsonWriter writer, BookChapterRef value, JsonSerializerOptions options) =>
        writer.WriteStringValue(value.Slug);
}
