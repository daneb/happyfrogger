using HappyFrog;
using HappyFrog.Models;

namespace HappyFrog.Tests;

public class BookProcessorTests : IDisposable
{
    private readonly string _tempDir;
    private readonly HappyFrogConfig _config;
    private readonly PostProcessor _posts;

    public BookProcessorTests()
    {
        _tempDir = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString());
        Directory.CreateDirectory(_tempDir);

        _config = new HappyFrogConfig
        {
            OutputPath = _tempDir,
            Build = new BuildOptions { IncludeDrafts = true, WordsPerMinute = 200 }
        };
        _posts = new PostProcessor(_config);
    }

    public void Dispose() => Directory.Delete(_tempDir, recursive: true);

    // ── helpers ───────────────────────────────────────────────────────────────

    private string CreateBookDir(string bookId)
    {
        var dir = Path.Combine(_tempDir, bookId);
        Directory.CreateDirectory(dir);
        return dir;
    }

    private void WriteChapter(string dir, string slug, int chapterNumber = 1)
    {
        File.WriteAllText(Path.Combine(dir, $"{slug}.md"), $"""
            ---
            title: Chapter {chapterNumber}
            date: 2025-01-01
            category: faith
            status: published
            slug: {slug}
            chapter_number: {chapterNumber}
            ---
            Content here.
            """);
    }

    private void WriteManifest(string dir, object manifest)
    {
        var json = System.Text.Json.JsonSerializer.Serialize(manifest,
            new System.Text.Json.JsonSerializerOptions { PropertyNamingPolicy = System.Text.Json.JsonNamingPolicy.CamelCase });
        File.WriteAllText(Path.Combine(dir, "book.json"), json);
    }

    private BookProcessor.ProcessedBook ProcessSingleBook(BookConfig bookConfig)
    {
        _config.Books = [bookConfig];
        var processor = new BookProcessor(_config, _posts);
        var books = processor.ProcessBooks();
        Assert.Single(books);
        return books[0];
    }

    // ── progress calculation ──────────────────────────────────────────────────

    [Fact]
    public void Progress_WithAllChaptersWritten_Is100Percent()
    {
        var dir = CreateBookDir("book1");
        WriteChapter(dir, "ch1", 1);
        WriteChapter(dir, "ch2", 2);
        WriteManifest(dir, new { id = "book1", title = "Book 1", category = "faith",
            parts = new[] { new { label = "Part I", title = "", chapters = new[] { "ch1", "ch2" } } } });

        var book = ProcessSingleBook(new BookConfig { Id = "book1", Path = dir, OutputPath = dir, Category = "faith" });
        Assert.Equal(100, book.Index.Progress);
    }

    [Fact]
    public void Progress_WithTotalChaptersOverride_UsesConfiguredTotal()
    {
        var dir = CreateBookDir("book2");
        WriteChapter(dir, "ch1", 1);
        WriteManifest(dir, new { id = "book2", title = "Book 2", category = "faith", totalChapters = 16,
            parts = new[] { new { label = "Part I", title = "", chapters = new[] { "ch1" } } } });

        var book = ProcessSingleBook(new BookConfig { Id = "book2", Path = dir, OutputPath = dir, Category = "faith" });

        Assert.Equal(16, book.Index.ChaptersTotal);
        Assert.Equal(6, book.Index.Progress); // 1/16 = 6.25 → 6
    }

    [Fact]
    public void Progress_WithNoChapters_IsZero()
    {
        var dir = CreateBookDir("book3");
        WriteManifest(dir, new { id = "book3", title = "Book 3", category = "faith",
            parts = new[] { new { label = "Part I", title = "",
                chapters = new[] { new { slug = "unwritten", title = "Unwritten" } } } } });

        var book = ProcessSingleBook(new BookConfig { Id = "book3", Path = dir, OutputPath = dir, Category = "faith" });

        Assert.Equal(0, book.Index.Progress);
        Assert.Equal(0, book.Index.ChaptersAvailable);
    }

    [Fact]
    public void ChaptersAvailable_CountsOnlyWrittenChapters()
    {
        var dir = CreateBookDir("book4");
        WriteChapter(dir, "ch1", 1);
        WriteManifest(dir, new { id = "book4", title = "Book 4", category = "faith",
            parts = new[] { new { label = "Part I", title = "",
                chapters = new object[] {
                    "ch1",
                    new { slug = "ch2-coming", title = "Coming Soon" }
                } } } });

        var book = ProcessSingleBook(new BookConfig { Id = "book4", Path = dir, OutputPath = dir, Category = "faith" });

        Assert.Equal(1, book.Index.ChaptersAvailable);
        Assert.Equal(2, book.Index.ChaptersTotal);
        Assert.Equal(50, book.Index.Progress);
    }

    // ── cover image ───────────────────────────────────────────────────────────

    [Fact]
    public void CoverImage_IsPassedThroughFromBookConfig()
    {
        var dir = CreateBookDir("book5");
        WriteChapter(dir, "ch1", 1);
        WriteManifest(dir, new { id = "book5", title = "Book 5", category = "faith",
            parts = new[] { new { label = "Part I", title = "", chapters = new[] { "ch1" } } } });

        var book = ProcessSingleBook(new BookConfig
        {
            Id = "book5", Path = dir, OutputPath = dir,
            Category = "faith", CoverImage = "books/book5/cover.png"
        });

        Assert.Equal("books/book5/cover.png", book.Index.CoverImage);
    }
}
