using System.Xml.Linq;
using System.Globalization;
using HappyFrog.Models;

namespace HappyFrog;

/// <summary>
/// Generates sitemap.xml for search engine crawlers
/// </summary>
public class SitemapGenerator
{
    private readonly HappyFrogConfig _config;
    private readonly XNamespace _namespace = "http://www.sitemaps.org/schemas/sitemap/0.9";

    public SitemapGenerator(HappyFrogConfig config)
    {
        _config = config;
    }

    /// <summary>
    /// Generate sitemap.xml from blog posts and static pages
    /// </summary>
    public string Generate(IEnumerable<BlogPostModel> posts)
    {
        // Validate base URL is set
        if (string.IsNullOrWhiteSpace(_config.Site.BaseUrl))
        {
            throw new InvalidOperationException(
                "Sitemap generation requires 'baseUrl' to be set in site configuration.");
        }

        var baseUrl = _config.Site.BaseUrl.TrimEnd('/');
        var now = DateTime.UtcNow;

        var urlSet = new XElement(_namespace + "urlset");

        // Add landing page (highest priority)
        urlSet.Add(CreateUrlElement(
            url: $"{baseUrl}/",
            lastMod: now,
            changeFreq: "daily",
            priority: 1.0m
        ));

        // Add category pages
        foreach (var category in _config.Build.Categories)
        {
            urlSet.Add(CreateUrlElement(
                url: $"{baseUrl}/{category}{_config.Build.HtmlExtension}",
                lastMod: now,
                changeFreq: "weekly",
                priority: 0.7m
            ));
        }

        // Add blog posts (sorted by publish date, newest first)
        var sortedPosts = posts.OrderByDescending(p => p.PublishDate);

        foreach (var post in sortedPosts)
        {
            var postUrl = $"{baseUrl}/{post.Slug}";
            if (!post.Slug.EndsWith(_config.Build.HtmlExtension))
            {
                postUrl += _config.Build.HtmlExtension;
            }

            // Calculate priority based on post age
            var priority = CalculatePostPriority(post.PublishDate, now);

            // Determine change frequency based on post age
            var changeFreq = post.PublishDate > now.AddMonths(-1) ? "weekly" : "monthly";

            urlSet.Add(CreateUrlElement(
                url: postUrl,
                lastMod: post.PublishDate,
                changeFreq: changeFreq,
                priority: priority
            ));
        }

        var document = new XDocument(
            new XDeclaration("1.0", "UTF-8", null),
            urlSet
        );

        // Return formatted XML
        using var stringWriter = new StringWriter();
        document.Save(stringWriter);
        return stringWriter.ToString();
    }

    /// <summary>
    /// Create a URL element for the sitemap
    /// </summary>
    private XElement CreateUrlElement(string url, DateTime lastMod, string changeFreq, decimal priority)
    {
        return new XElement(_namespace + "url",
            new XElement(_namespace + "loc", url),
            new XElement(_namespace + "lastmod", ToW3CDateTime(lastMod)),
            new XElement(_namespace + "changefreq", changeFreq),
            new XElement(_namespace + "priority", priority.ToString("0.0", CultureInfo.InvariantCulture))
        );
    }

    /// <summary>
    /// Calculate priority for a post based on its age
    /// Recent posts get higher priority
    /// </summary>
    private decimal CalculatePostPriority(DateTime publishDate, DateTime now)
    {
        var age = now - publishDate;

        if (age.TotalDays < 30)
            return 0.8m; // Recent posts (< 1 month)
        else if (age.TotalDays < 90)
            return 0.7m; // Medium age (1-3 months)
        else
            return 0.6m; // Older posts (> 3 months)
    }

    /// <summary>
    /// Convert DateTime to W3C format (required by sitemap spec)
    /// Example: 2025-12-14T12:00:00Z
    /// </summary>
    private string ToW3CDateTime(DateTime dateTime)
    {
        // Ensure UTC
        if (dateTime.Kind != DateTimeKind.Utc)
            dateTime = dateTime.ToUniversalTime();

        return dateTime.ToString("yyyy-MM-dd'T'HH:mm:ss'Z'", CultureInfo.InvariantCulture);
    }

    /// <summary>
    /// Generate robots.txt content
    /// </summary>
    public string GenerateRobotsTxt()
    {
        var baseUrl = _config.Site.BaseUrl.TrimEnd('/');
        var sitemapUrl = $"{baseUrl}/{_config.Build.Sitemap.Path}";

        return $@"User-agent: *
Allow: /

Sitemap: {sitemapUrl}
";
    }
}
