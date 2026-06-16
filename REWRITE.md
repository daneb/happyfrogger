# HappyFrog → HappyFrog 2 — Engine Rewrite Spec

A focused rewrite of your static generator around three goals you named:

1. **Write → publish, fast.** One command to scaffold a post, one to preview live, one to ship.
2. **Books are first-class** — grouped into *Parts*, support *planned/coming-soon* chapters, and a *shelf* built for **multiple books**.
3. **Fewer moving parts.** Drop the Tailwind + Node build step entirely; ship one hand-authored `theme.css`. Faster cold builds, no `npm install`, no `output.css` regeneration.

Everything below is additive or a drop-in replacement for existing files. Variable names assume your current Scriban `StandardMemberRenamer` (PascalCase → `snake_case`), so `FeaturedBook` → `featured_book`, etc. The redesigned templates in `redesign/Templates/` already expect these names.

> **The complete, ready-to-compile files are in `redesign/src/`** — not just the snippets below. Copy them straight over your existing files (same namespaces/paths). The sections below explain *what changed and why*; `src/` is what you actually drop in.
>
> ```
> redesign/
> ├── src/
> │   ├── Program.cs                 (replace)
> │   ├── BookProcessor.cs           (replace — manifest-driven, runs once)
> │   ├── PageGenerator.cs           (replace — js_path global, theme.css)
> │   ├── AssetGenerator.cs          (replace — copies theme assets; new ctor arg)
> │   ├── Commands.cs                (new — `hf new` scaffolder)
> │   └── Models/
> │       ├── LandingPageModel.cs    (replace — Sections + FeaturedBook)
> │       ├── CategoryPageModel.cs   (replace — old BookIndexModel removed)
> │       ├── BookModels.cs          (new — manifest + book index view models)
> │       └── HappyFrogConfig.cs     (replace — BookConfig.Planned added)
> ├── Templates/                     (4 templates + assets/theme.css + assets/theme.js)
> └── example/                       (happyfrog.config.json + books/.../book.json)
> ```
>
> One ctor change to wire up: `AssetGenerator` now takes `(config, templatesPath)` — `Program.cs` in `src/` already passes it.

---

## 0. What changes at a glance

| Area | Before | After |
|---|---|---|
| Styling | Tailwind `input.css` → `output.css` via Node | Single static `assets/theme.css` (+ `theme.js`) — no build step |
| Landing data | `TechPosts` / `FaithPosts` / `CreativePosts` fixed fields | `Sections` list (data-driven) + `FeaturedBook` |
| Books | flat `Chapters`, ordering quirks, `ProcessBooks()` called repeatedly | `book.json` manifest with **Parts** + planned chapters; processed **once** |
| Book index | one book, generic list | parts, epigraphs, coming-soon rows, multi-book **shelf** |
| Authoring | hand-create `.md` + front matter | `hf new "Title" -c tech` scaffolder |
| Dark mode | none | `data-theme` + `theme.js`, no-flash head snippet |

Delete after migration: `tailwind.config.js`, `input.css`, `output.css`, `styles.css`, the `build:css`/`watch:css` npm scripts, and the `package.json` Tailwind dependency. Copy `redesign/assets/theme.css` and `theme.js` to your output's `assets/` (and have `AssetGenerator` copy them on build — see §5).

---

## 1. Models

### 1a. Landing — data-driven sections

Replace `Models/LandingPageModel.cs`:

```csharp
namespace HappyFrog.Models;

public class LandingPageModel
{
    public FeaturedBook? FeaturedBook { get; set; }
    public List<LandingSection> Sections { get; set; } = new();
}

public class LandingSection
{
    public string Category { get; set; } = "";   // "tech" | "faith" | "creative"  -> body class is-{category}
    public string Heading  { get; set; } = "";    // e.g. "Building software"
    public string Cta      { get; set; } = "";    // e.g. "All articles"
    public List<BlogPostModel> Posts { get; set; } = new();
}

public class FeaturedBook
{
    public string Title { get; set; } = "";
    public string Description { get; set; } = "";
    public string BaseUrl { get; set; } = "";     // e.g. "books/biblical-understanding/"
    public int Progress { get; set; }
}
```

### 1b. Book manifest + index

New `Models/BookModels.cs`:

```csharp
namespace HappyFrog.Models;

/// <summary>Loaded from each book folder's book.json (see §2).</summary>
public class BookManifest
{
    public string Id { get; set; } = "";
    public string Title { get; set; } = "";
    public string Subtitle { get; set; } = "";   // shown as the hero "description"
    public string Category { get; set; } = "";    // category page this book belongs to
    public string Cover { get; set; } = "";
    public string Status { get; set; } = "In progress";
    public List<BookPartConfig> Parts { get; set; } = new();
}

public class BookPartConfig
{
    public string Label { get; set; } = "";        // "Part I"
    public string Title { get; set; } = "";
    public string Epigraph { get; set; } = "";
    /// <summary>Chapter slugs (or {slug,title,description}) in reading order.</summary>
    public List<BookChapterRef> Chapters { get; set; } = new();
}

public class BookChapterRef
{
    public string Slug { get; set; } = "";
    public string? Title { get; set; }            // fallback title for not-yet-written chapters
    public string? Description { get; set; }
}

// ---- view models the BookIndexTemplate renders ----
public class BookIndexModel
{
    public string Category { get; set; } = "";
    public string CategoryUrl { get; set; } = "";   // "../../faith.html"
    public string CategoryLabel { get; set; } = ""; // "Faith & Wisdom"
    public string Title { get; set; } = "";
    public string Description { get; set; } = "";
    public int Progress { get; set; }
    public int ChaptersAvailable { get; set; }
    public int ChaptersTotal { get; set; }
    public string? FirstChapter { get; set; }       // slug of first available chapter
    public List<BookPart> Parts { get; set; } = new();
    public List<ShelfItem> OtherBooks { get; set; } = new();
}

public class BookPart
{
    public string Label { get; set; } = "";
    public string Title { get; set; } = "";
    public string Epigraph { get; set; } = "";
    public List<BookChapter> Chapters { get; set; } = new();
}

public class BookChapter
{
    public int ChapterNumber { get; set; }
    public string Slug { get; set; } = "";
    public string Title { get; set; } = "";
    public string Description { get; set; } = "";
    public int ReadingTimeMinutes { get; set; }
    public bool Available { get; set; }             // false => "Coming soon"
}

public class ShelfItem
{
    public string Title { get; set; } = "";
    public string BaseUrl { get; set; } = "";
    public string Status { get; set; } = "";
    public bool Planned { get; set; }               // true => dashed placeholder cover
}
```

Front matter gains one optional field for chapters — **`part`** is no longer needed (parts come from `book.json`), but keep `chapter_number` for ordering fallback. Add to `Models/FrontMatter.cs` only if you want per-file epigraphs; otherwise no change.

---

## 2. The `book.json` manifest (per book folder)

Drop one into each book directory (e.g. `../blog/books/biblical-understanding/book.json`). This is what makes books "treated differently" — parts, epigraphs, and chapters you haven't written yet:

```json
{
  "id": "biblical-understanding",
  "title": "A Dad's Memoir of Biblical Truth",
  "subtitle": "A journey through Scripture to equip the everyday man — written one chapter at a time, in the open.",
  "category": "faith",
  "status": "In progress",
  "parts": [
    {
      "label": "Part I",
      "title": "Understanding the Foundation",
      "epigraph": "Getting oriented with God's Word",
      "chapters": [
        "the-absent-man",
        "unlocking-your-bible-understanding",
        "gods-programme",
        "why-bible-versions-matter"
      ]
    },
    {
      "label": "Part II",
      "title": "The Journey of Salvation",
      "epigraph": "The core of Christian faith and life",
      "chapters": [
        { "slug": "the-gospel-message", "title": "The Gospel Message", "description": "What the good news actually is." },
        { "slug": "the-cross-of-salvation", "title": "The Cross of Salvation", "description": "The centre of everything." }
      ]
    }
  ]
}
```

A chapter is **available** when a matching published `.md` exists in the folder; otherwise it renders as a dimmed *Coming soon* row using the manifest title/description. `progress = round(available / total * 100)`. This replaces the old `Math.Min(100, chapters.Count * 2)` guesswork.

`happyfrog.config.json` keeps a lightweight `books` list (id + path + which category page hosts it). Planned future books can be declared there with `"planned": true` to populate the shelf.

---

## 3. `BookProcessor` — manifest-driven, processed once

Replace `BookProcessor.cs`:

```csharp
using System.Text.Json;
using HappyFrog.Models;

namespace HappyFrog;

public class BookProcessor
{
    private static readonly JsonSerializerOptions Json = new()
    { PropertyNameCaseInsensitive = true, ReadCommentHandling = JsonCommentHandling.Skip, AllowTrailingCommas = true };

    private readonly HappyFrogConfig _config;
    private readonly PostProcessor _posts;

    public BookProcessor(HappyFrogConfig config, PostProcessor posts)
    { _config = config; _posts = posts; }

    public record ProcessedBook(BookConfig Config, BookManifest Manifest,
                                List<BlogPostModel> Chapters, BookIndexModel Index);

    /// <summary>Process every configured book exactly once.</summary>
    public List<ProcessedBook> ProcessBooks()
    {
        var result = new List<ProcessedBook>();

        foreach (var book in _config.Books.Where(b => !b.Planned))
        {
            if (!Directory.Exists(book.Path))
            { Console.WriteLine($"  Warning: book path not found: {book.Path}"); continue; }

            var manifest = LoadManifest(book);
            var written  = _posts.ProcessDirectory(book.Path)
                                 .ToDictionary(p => p.Slug, p => p);

            var index = BuildIndex(book, manifest, written);
            result.Add(new ProcessedBook(book, manifest,
                written.Values.OrderBy(c => c.ChapterNumber ?? int.MaxValue).ToList(), index));

            Console.WriteLine($"  Book '{manifest.Title}': {index.ChaptersAvailable}/{index.ChaptersTotal} chapters");
        }

        // Build the cross-book shelf once, then attach to each index.
        var shelf = result.Select(r => new ShelfItem {
            Title = r.Manifest.Title, BaseUrl = $"books/{r.Config.Id}/", Status = r.Manifest.Status.ToUpper()
        }).Concat(_config.Books.Where(b => b.Planned).Select(b => new ShelfItem {
            Title = b.Title, Planned = true
        })).ToList();

        foreach (var r in result)
            r.Index.OtherBooks = shelf.Where(s => s.Title != r.Manifest.Title).ToList();

        return result;
    }

    private static BookManifest LoadManifest(BookConfig book)
    {
        var path = Path.Combine(book.Path, "book.json");
        if (File.Exists(path))
            return JsonSerializer.Deserialize<BookManifest>(File.ReadAllText(path), Json)!;
        // Fallback: single unnamed part from config when no manifest present.
        return new BookManifest { Id = book.Id, Title = book.Title, Subtitle = book.Description, Category = book.Category };
    }

    private static BookIndexModel BuildIndex(BookConfig book, BookManifest m,
                                             Dictionary<string, BlogPostModel> written)
    {
        var parts = new List<BookPart>();
        int n = 0, available = 0; string? first = null;

        foreach (var pc in m.Parts)
        {
            var part = new BookPart { Label = pc.Label, Title = pc.Title, Epigraph = pc.Epigraph };
            foreach (var cref in pc.Chapters)
            {
                n++;
                var has = written.TryGetValue(cref.Slug, out var post);
                if (has) { available++; first ??= cref.Slug; }
                part.Chapters.Add(new BookChapter {
                    ChapterNumber = post?.ChapterNumber ?? n,
                    Slug = cref.Slug,
                    Title = post?.Title ?? cref.Title ?? cref.Slug,
                    Description = post?.Description ?? cref.Description ?? "",
                    ReadingTimeMinutes = post?.ReadingTimeMinutes ?? 0,
                    Available = has
                });
            }
            parts.Add(part);
        }

        return new BookIndexModel {
            Category = m.Category,
            CategoryUrl = $"../../{m.Category}.html",
            CategoryLabel = CategoryInfo.Categories.TryGetValue(m.Category, out var i) ? i.Title : m.Category,
            Title = m.Title, Description = m.Subtitle,
            ChaptersTotal = n, ChaptersAvailable = available,
            Progress = n == 0 ? 0 : (int)Math.Round(available / (double)n * 100),
            FirstChapter = first, Parts = parts
        };
    }
}
```

Add `public bool Planned { get; set; }` to `BookConfig` in `HappyFrogConfig.cs`.

---

## 4. `Program.cs` — the build loop, cleaned up

The book section now runs once and the landing/category models are assembled from real data. Replace the `RunBuild()` body's book + landing + category steps with:

```csharp
// 2. Books (processed once)
Console.WriteLine("Processing books...");
var bookProcessor = new BookProcessor(config, postProcessor);
var books = bookProcessor.ProcessBooks();

foreach (var b in books)
{
    Directory.CreateDirectory(b.Config.OutputPath);
    foreach (var chapter in b.Chapters)
        pageGenerator.RenderPost(chapter, Path.Combine(b.Config.OutputPath, chapter.Slug + ".html"),
            cssPath: "../../assets/theme.css", jsPath: "../../assets/theme.js",
            backUrl: "index.html", backLabel: "All chapters", homeUrl: "../../index.html");

    pageGenerator.RenderBookIndex(b.Index, Path.Combine(b.Config.OutputPath, "index.html"));
}

// 3. Landing — data-driven sections + featured book
if (config.Build.GenerateLandingPage)
{
    var featured = books.FirstOrDefault();
    var landing = new LandingPageModel {
        FeaturedBook = featured is null ? null : new FeaturedBook {
            Title = featured.Index.Title, Description = featured.Index.Description,
            BaseUrl = $"books/{featured.Config.Id}/", Progress = featured.Index.Progress
        },
        Sections = config.Build.Categories.Select(cat => new LandingSection {
            Category = cat,
            Heading  = CategoryInfo.Categories.TryGetValue(cat, out var i) ? i.Title : cat,
            Cta      = "All " + cat,
            Posts    = allPosts.Where(p => p.Category == cat).Take(3).ToList()
        }).ToList()
    };
    pageGenerator.RenderLandingPage(landing, Path.Combine(config.OutputPath, "index.html"));
}

// 4. Category pages
if (config.Build.GenerateCategoryPages)
{
    foreach (var category in config.Build.Categories)
    {
        if (!CategoryInfo.Categories.TryGetValue(category, out var info)) continue;
        var categoryPosts = allPosts.Where(p => p.Category == category).ToList();

        var model = new CategoryPageModel {
            Category = category, Title = info.Title, Description = info.Description,
            PostsByYear = categoryPosts.GroupBy(p => p.PublishDate.Year).OrderByDescending(g => g.Key)
                .Select(g => new YearGroup { Year = g.Key, Posts = g.OrderByDescending(p => p.PublishDate).ToList() }).ToList(),
            Books = books.Where(b => b.Config.Category == category)
                .Select(b => new BookSectionModel {
                    Title = b.Index.Title, Description = b.Index.Description,
                    BaseUrl = $"books/{b.Config.Id}/", Progress = b.Index.Progress, Chapters = b.Chapters
                }).ToList(),
            SubCategories = categoryPosts.Where(p => !string.IsNullOrEmpty(p.SubCategory))
                .Select(p => p.SubCategory!).Distinct().OrderBy(s => s)
        };
        pageGenerator.RenderCategoryPage(model, Path.Combine(config.OutputPath, $"{category}.html"));
    }
}
```

Note the `|| true` filter and the repeated `ProcessBooks()` calls from the old loop are gone.

---

## 5. `PageGenerator` — `js_path` global + `css_path` for the new asset

Two small changes. (1) Add a `js_path` global and point `css_path` at `assets/theme.css`. (2) Thread `jsPath` through `RenderPost`.

```csharp
// in Render<T>(...)
scriptObject["css_path"] = "assets/theme.css";
scriptObject["js_path"]  = "assets/theme.js";
scriptObject["site_description"] = _config.Site.Description;

// RenderPost signature
public void RenderPost(BlogPostModel post, string outputPath,
    string cssPath = "assets/theme.css", string jsPath = "assets/theme.js",
    string? backUrl = null, string? backLabel = null, string homeUrl = "index.html")
{
    var extra = new Dictionary<string, object?> {
        ["css_path"] = cssPath, ["js_path"] = jsPath, ["home_url"] = homeUrl,
        ["back_url"] = backUrl ?? $"{post.Category}.html",
        ["back_label"] = backLabel ?? Capitalise(post.Category),
    };
    File.WriteAllText(outputPath, Render("BlogTemplate.html", post, extra));
}

// RenderBookIndex: nested two levels deep
public void RenderBookIndex(BookIndexModel model, string outputPath)
{
    var extra = new Dictionary<string, object?> {
        ["css_path"] = "../../assets/theme.css", ["js_path"] = "../../assets/theme.js",
        ["home_url"] = "../../index.html",
    };
    File.WriteAllText(outputPath, Render("BookIndexTemplate.html", model, extra));
}
```

In `AssetGenerator.Generate(...)`, copy the static assets into output once:

```csharp
var assetsOut = Path.Combine(_config.OutputPath, "assets");
Directory.CreateDirectory(assetsOut);
foreach (var f in new[] { "theme.css", "theme.js" })
    File.Copy(Path.Combine(_templatesAssetsPath, f), Path.Combine(assetsOut, f), overwrite: true);
```

---

## 6. Fast write → publish

### `hf new` — scaffold a post in one command

New `Commands.cs`:

```csharp
using HappyFrog.Models;

namespace HappyFrog;

public static class Commands
{
    /// <summary>hf new "My Title" -c tech [-s subcat] [--book biblical-understanding]</summary>
    public static int New(string[] args, HappyFrogConfig config)
    {
        var title = args.FirstOrDefault(a => !a.StartsWith('-'));
        if (string.IsNullOrWhiteSpace(title)) { Console.WriteLine("Usage: hf new \"Title\" -c <category>"); return 1; }

        string category = Opt(args, "-c", "--category") ?? "tech";
        string? sub     = Opt(args, "-s", "--subcategory");
        string? bookId  = Opt(args, "--book");

        var slug = Slug(title);
        var dir  = bookId is null
            ? config.MarkdownFilesPath
            : config.Books.First(b => b.Id == bookId).Path;
        Directory.CreateDirectory(dir);
        var file = Path.Combine(dir, slug + ".md");
        if (File.Exists(file)) { Console.WriteLine($"Already exists: {file}"); return 1; }

        var fm = $"""
        ---
        title: "{title}"
        date: {DateTime.Now:yyyy-MM-dd}
        category: {category}
        {(sub is null ? "" : $"subcategory: {sub}\n")}description: ""
        slug: {slug}
        status: draft
        ---

        Write here…
        """;
        File.WriteAllText(file, fm);
        Console.WriteLine($"✓ Created {file}  (status: draft)");
        Console.WriteLine("  Edit it, then run `hf serve` to preview or set status: published and `hf` to build.");
        return 0;
    }

    static string? Opt(string[] a, params string[] keys)
    {
        foreach (var k in keys) { var i = Array.IndexOf(a, k); if (i >= 0 && i + 1 < a.Length) return a[i + 1]; }
        return null;
    }
    static string Slug(string t) => string.Concat(t.ToLower().Select(c =>
        char.IsLetterOrDigit(c) ? c : c is ' ' or '-' ? '-' : '\0')).Replace("--", "-").Trim('-');
}
```

Wire it at the very top of `Program.cs` before the build runs:

```csharp
if (args.Length > 0 && args[0] == "new")
    return Commands.New(args[1..], LoadConfiguration());
```

### Workflow

```bash
hf new "Why I switched editors" -c tech   # scaffold a draft
hf serve                                   # live preview at :4000 (already implemented)
# flip status: published, then:
hf                                         # build to Output/
```

`hf serve` already rebuilds on change (your `DevServer`). For one-touch deploy, add an optional `deploy.command` to config and run it after a successful `hf publish` (alias for build with `IncludeDrafts=false`). Because there's no Tailwind step anymore, the build is just: read markdown → render Scriban → copy assets.

---

## 7. Migration checklist

1. Copy `redesign/assets/theme.css` + `theme.js` next to your templates (e.g. `Templates/assets/`); set `_templatesAssetsPath` in `AssetGenerator`.
2. Replace the four templates in `Templates/` with the ones in `redesign/Templates/`.
3. Apply the model + processor + `Program.cs` + `PageGenerator` changes above.
4. Add a `book.json` to each book folder.
5. Remove Tailwind: delete `input.css`, `output.css`, `tailwind.config.js`, `styles.css`, and the npm `*:css` scripts.
6. `dotnet run` → verify `Output/`. Open `index.html`.

Net effect: same Markdown-in front matter you already write, a cleaner build with no Node dependency, books that look like books, and `hf new` to start writing in seconds.
