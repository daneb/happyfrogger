namespace HappyFrog.Models;

public class HappyFrogConfig
{
    /// <summary>
    /// Path to the directory containing markdown files
    /// </summary>
    public string MarkdownFilesPath { get; set; } = "MarkdownFiles";

    /// <summary>
    /// Path to the output directory for generated HTML files
    /// </summary>
    public string OutputPath { get; set; } = "Output";

    /// <summary>
    /// Path to the templates directory
    /// </summary>
    public string TemplatesPath { get; set; } = "Templates";

    /// <summary>
    /// Site metadata
    /// </summary>
    public SiteMetadata Site { get; set; } = new SiteMetadata();

    /// <summary>
    /// Build options
    /// </summary>
    public BuildOptions Build { get; set; } = new BuildOptions();
}

public class SiteMetadata
{
    /// <summary>
    /// Site title
    /// </summary>
    public string Title { get; set; } = "HappyFrogger Blog";

    /// <summary>
    /// Site description
    /// </summary>
    public string Description { get; set; } = "A lightweight static site generator";

    /// <summary>
    /// Site author
    /// </summary>
    public string Author { get; set; } = "";

    /// <summary>
    /// Base URL for the site (useful for RSS feeds, sitemaps, etc.)
    /// </summary>
    public string BaseUrl { get; set; } = "";

    /// <summary>
    /// Default social media image URL (absolute URL)
    /// </summary>
    public string DefaultSocialImage { get; set; } = "";

    /// <summary>
    /// Twitter handle (without @)
    /// </summary>
    public string TwitterHandle { get; set; } = "";
}

public class BuildOptions
{
    /// <summary>
    /// Whether to generate category pages
    /// </summary>
    public bool GenerateCategoryPages { get; set; } = true;

    /// <summary>
    /// Whether to generate landing page
    /// </summary>
    public bool GenerateLandingPage { get; set; } = true;

    /// <summary>
    /// File extension for generated HTML files
    /// </summary>
    public string HtmlExtension { get; set; } = ".html";

    /// <summary>
    /// Whether to include drafts (posts with status != "published")
    /// </summary>
    public bool IncludeDrafts { get; set; } = false;

    /// <summary>
    /// Categories to generate pages for
    /// </summary>
    public List<string> Categories { get; set; } = new List<string> { "tech", "faith", "creative" };

    /// <summary>
    /// Average words per minute for reading time calculation
    /// </summary>
    public int WordsPerMinute { get; set; } = 200;

    /// <summary>
    /// RSS feed configuration
    /// </summary>
    public RssOptions Rss { get; set; } = new RssOptions();

    /// <summary>
    /// Sitemap configuration
    /// </summary>
    public SitemapOptions Sitemap { get; set; } = new SitemapOptions();

    /// <summary>
    /// Table of Contents configuration
    /// </summary>
    public TocOptions Toc { get; set; } = new TocOptions();
}

public class RssOptions
{
    /// <summary>
    /// Whether to generate RSS feed
    /// </summary>
    public bool Enabled { get; set; } = true;

    /// <summary>
    /// Path to the RSS feed file (relative to output directory)
    /// </summary>
    public string Path { get; set; } = "feed.xml";

    /// <summary>
    /// Number of items to include in feed (0 = all)
    /// </summary>
    public int ItemCount { get; set; } = 20;

    /// <summary>
    /// Whether to include full content or just description in feed
    /// </summary>
    public bool FullContent { get; set; } = true;
}

public class SitemapOptions
{
    /// <summary>
    /// Whether to generate sitemap.xml
    /// </summary>
    public bool Enabled { get; set; } = true;

    /// <summary>
    /// Path to the sitemap file (relative to output directory)
    /// </summary>
    public string Path { get; set; } = "sitemap.xml";

    /// <summary>
    /// Default change frequency for pages
    /// </summary>
    public string ChangeFrequency { get; set; } = "weekly";

    /// <summary>
    /// Default priority for pages (0.0 to 1.0)
    /// </summary>
    public decimal DefaultPriority { get; set; } = 0.5m;
}

public class TocOptions
{
    /// <summary>
    /// Whether to enable table of contents generation globally
    /// </summary>
    public bool EnabledByDefault { get; set; } = true;

    /// <summary>
    /// Minimum number of headings required to generate TOC
    /// </summary>
    public int MinHeadings { get; set; } = 3;

    /// <summary>
    /// Maximum heading level to include (2-6, where 2=H2, 3=H3, etc.)
    /// </summary>
    public int MaxLevel { get; set; } = 3;

    /// <summary>
    /// Title for the table of contents section
    /// </summary>
    public string Title { get; set; } = "Table of Contents";
}
