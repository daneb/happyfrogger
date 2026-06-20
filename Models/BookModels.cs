namespace HappyFrog.Models;

// ──────────────────────────────────────────────────────────────────────────
//  Book manifest — one book.json per book folder. Defines Parts, epigraphs,
//  and chapters (including ones not yet written → "Coming soon").
// ──────────────────────────────────────────────────────────────────────────

public class BookManifest
{
    public string Id { get; set; } = "";
    public string Title { get; set; } = "";
    public string Subtitle { get; set; } = "";    // shown as the hero description
    public string Category { get; set; } = "";     // category page that hosts this book
    public string Cover { get; set; } = "";
    public string Status { get; set; } = "In progress";
    public int? TotalChapters { get; set; }
    public List<BookPartConfig> Parts { get; set; } = new();
}

public class BookPartConfig
{
    public string Label { get; set; } = "";        // "Part I"
    public string Title { get; set; } = "";
    public string Epigraph { get; set; } = "";
    public List<BookChapterRef> Chapters { get; set; } = new();
}

/// <summary>
/// A chapter reference in the manifest. JSON may be a bare string (slug) or an
/// object {slug,title,description}; see BookChapterRefConverter.
/// </summary>
public class BookChapterRef
{
    public string Slug { get; set; } = "";
    public string? Title { get; set; }             // fallback title for unwritten chapters
    public string? Description { get; set; }
}

// ──────────────────────────────────────────────────────────────────────────
//  View models the BookIndexTemplate.html renders.
// ──────────────────────────────────────────────────────────────────────────

public class BookIndexModel
{
    public string Category { get; set; } = "";
    public string CategoryUrl { get; set; } = "";   // "../../faith.html"
    public string CategoryLabel { get; set; } = ""; // "Faith & Wisdom"
    public string Title { get; set; } = "";
    public string Description { get; set; } = "";
    public string CoverImage { get; set; } = "";
    public int Progress { get; set; }
    public int ChaptersAvailable { get; set; }
    public int ChaptersTotal { get; set; }
    public string? FirstChapter { get; set; }        // slug of first available chapter
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
    public bool Available { get; set; }              // false => "Coming soon"
}

public class ShelfItem
{
    public string Title { get; set; } = "";
    public string BaseUrl { get; set; } = "";
    public string Status { get; set; } = "";
    public bool Planned { get; set; }                // true => dashed placeholder cover
}
