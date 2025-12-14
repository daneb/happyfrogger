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
}
