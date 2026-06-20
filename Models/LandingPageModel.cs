namespace HappyFrog.Models;

/// <summary>
/// Landing page is now data-driven: a list of sections (one per category)
/// plus an optional featured book. Renders with LandingTemplate.html.
/// </summary>
public class LandingPageModel
{
    public FeaturedBook? FeaturedBook { get; set; }
    public List<LandingSection> Sections { get; set; } = new();
}

public class LandingSection
{
    /// <summary>"tech" | "faith" | "creative" — also drives the body's is-{category} mood class.</summary>
    public string Category { get; set; } = "";
    public string Heading { get; set; } = "";   // e.g. "Tech Insights"
    public string Cta { get; set; } = "";        // e.g. "View all"
    public List<BlogPostModel> Posts { get; set; } = new();
}

public class FeaturedBook
{
    public string Title { get; set; } = "";
    public string Description { get; set; } = "";
    public string BaseUrl { get; set; } = "";    // e.g. "books/biblical-understanding/"
    public string CoverImage { get; set; } = ""; // relative to output root
    public int Progress { get; set; }
}
