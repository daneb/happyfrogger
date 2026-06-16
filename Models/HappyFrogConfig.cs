namespace HappyFrog.Models;

public class HappyFrogConfig
{
    public string MarkdownFilesPath { get; set; } = "MarkdownFiles";
    public string OutputPath { get; set; } = "Output";
    public string TemplatesPath { get; set; } = "Templates";
    public SiteMetadata Site { get; set; } = new SiteMetadata();
    public BuildOptions Build { get; set; } = new BuildOptions();
    public List<BookConfig> Books { get; set; } = new List<BookConfig>();
}

public class SiteMetadata
{
    public string Title { get; set; } = "HappyFrogger Blog";
    public string Description { get; set; } = "A lightweight static site generator";
    public string Author { get; set; } = "";
    public string BaseUrl { get; set; } = "";
    public string DefaultSocialImage { get; set; } = "";
    public string TwitterHandle { get; set; } = "";
}

public class BuildOptions
{
    public bool GenerateCategoryPages { get; set; } = true;
    public bool GenerateLandingPage { get; set; } = true;
    public string HtmlExtension { get; set; } = ".html";
    public bool IncludeDrafts { get; set; } = false;
    public List<string> Categories { get; set; } = new List<string> { "tech", "faith", "creative" };
    public int WordsPerMinute { get; set; } = 200;
    public RssOptions Rss { get; set; } = new RssOptions();
    public SitemapOptions Sitemap { get; set; } = new SitemapOptions();
    public TocOptions Toc { get; set; } = new TocOptions();
}

public class BookConfig
{
    public string Id { get; set; } = "";
    public string Title { get; set; } = "";
    public string Description { get; set; } = "";
    public string Path { get; set; } = "";
    public string OutputPath { get; set; } = "";
    public string CoverImage { get; set; } = "";
    /// <summary>Category page this book appears on (e.g. "faith")</summary>
    public string Category { get; set; } = "";
    /// <summary>A future book that has no folder yet — shown on the shelf as "Planned".</summary>
    public bool Planned { get; set; } = false;
}

public class RssOptions
{
    public bool Enabled { get; set; } = true;
    public string Path { get; set; } = "feed.xml";
    public int ItemCount { get; set; } = 20;
    public bool FullContent { get; set; } = true;
}

public class SitemapOptions
{
    public bool Enabled { get; set; } = true;
    public string Path { get; set; } = "sitemap.xml";
    public string ChangeFrequency { get; set; } = "weekly";
    public decimal DefaultPriority { get; set; } = 0.5m;
}

public class TocOptions
{
    public bool EnabledByDefault { get; set; } = true;
    public int MinHeadings { get; set; } = 3;
    public int MaxLevel { get; set; } = 3;
    public string Title { get; set; } = "Table of Contents";
}
