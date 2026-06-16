namespace HappyFrog.Models;

public class CategoryPageModel
{
    public string Category { get; set; } = "";
    public string Title { get; set; } = "";
    public string Description { get; set; } = "";
    public IEnumerable<string> SubCategories { get; set; } = Enumerable.Empty<string>();
    /// <summary>Non-book posts grouped by year, descending.</summary>
    public List<YearGroup> PostsByYear { get; set; } = new();
    /// <summary>Book sections for this category (one per book).</summary>
    public List<BookSectionModel> Books { get; set; } = new();
}

public class YearGroup
{
    public int Year { get; set; }
    public List<BlogPostModel> Posts { get; set; } = new();
}

public class BookSectionModel
{
    public string Title { get; set; } = "";
    public string Description { get; set; } = "";
    /// <summary>Relative URL prefix from the category page to the book folder, e.g. "books/biblical-understanding/"</summary>
    public string BaseUrl { get; set; } = "";
    public int Progress { get; set; }
    public List<BlogPostModel> Chapters { get; set; } = new();
}

// NOTE: the richer BookIndexModel now lives in BookModels.cs.

public class StudyResource
{
    [YamlDotNet.Serialization.YamlMember(Alias = "title")]
    public string Title { get; set; } = "";

    [YamlDotNet.Serialization.YamlMember(Alias = "description")]
    public string Description { get; set; } = "";

    [YamlDotNet.Serialization.YamlMember(Alias = "url")]
    public string Url { get; set; } = "";
}

public static class CategoryInfo
{
    public static readonly Dictionary<string, (string Title, string Description)> Categories =
        new Dictionary<string, (string Title, string Description)>
        {
            { "tech",     ("Tech Insights",   "Deep dives into software development, system design, and tech career insights.") },
            { "faith",    ("Faith & Wisdom",  "Explorations in Christian living, Biblical insights, and my upcoming book.") },
            { "creative", ("Creative Corner", "A collection of poems, reflections, and personal essays.") }
        };
}
