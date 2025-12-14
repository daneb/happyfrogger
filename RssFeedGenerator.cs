using System.Xml.Linq;
using System.Text;
using System.Globalization;
using HappyFrog.Models;

namespace HappyFrog;

/// <summary>
/// Generates RSS 2.0 compliant feed from blog posts
/// </summary>
public class RssFeedGenerator
{
    private readonly HappyFrogConfig _config;
    private readonly XNamespace _namespace = "http://www.w3.org/2005/Atom";

    public RssFeedGenerator(HappyFrogConfig config)
    {
        _config = config;
    }

    /// <summary>
    /// Generate RSS feed from a list of blog posts
    /// </summary>
    /// <param name="posts">List of published blog posts</param>
    /// <returns>RSS feed XML as string</returns>
    public string Generate(IEnumerable<BlogPostModel> posts)
    {
        // Validate base URL is set
        if (string.IsNullOrWhiteSpace(_config.Site.BaseUrl))
        {
            throw new InvalidOperationException(
                "RSS feed generation requires 'baseUrl' to be set in site configuration.");
        }

        // Sort posts by publish date (newest first) and limit items
        var feedPosts = posts
            .OrderByDescending(p => p.PublishDate)
            .Take(_config.Build.Rss.ItemCount > 0 ? _config.Build.Rss.ItemCount : int.MaxValue)
            .ToList();

        var baseUrl = _config.Site.BaseUrl.TrimEnd('/');
        var feedUrl = $"{baseUrl}/{_config.Build.Rss.Path}";

        // Create RSS document
        var rss = new XElement("rss",
            new XAttribute("version", "2.0"),
            new XAttribute(XNamespace.Xmlns + "atom", _namespace),
            new XElement("channel",
                new XElement("title", _config.Site.Title),
                new XElement("link", baseUrl),
                new XElement("description", _config.Site.Description),
                new XElement("language", "en-us"),
                new XElement("lastBuildDate", ToRfc822DateTime(DateTime.UtcNow)),
                new XElement("generator", "HappyFrogger v2.1.0"),
                // Atom self-reference link
                new XElement(_namespace + "link",
                    new XAttribute("href", feedUrl),
                    new XAttribute("rel", "self"),
                    new XAttribute("type", "application/rss+xml")),
                // Generate items
                feedPosts.Select(post => CreateFeedItem(post, baseUrl))
            )
        );

        var document = new XDocument(
            new XDeclaration("1.0", "UTF-8", null),
            rss
        );

        // Return formatted XML
        using var stringWriter = new StringWriter();
        document.Save(stringWriter);
        return stringWriter.ToString();
    }

    /// <summary>
    /// Create an RSS item element from a blog post
    /// </summary>
    private XElement CreateFeedItem(BlogPostModel post, string baseUrl)
    {
        var postUrl = $"{baseUrl}/{post.Slug}";
        if (!post.Slug.EndsWith(_config.Build.HtmlExtension))
        {
            postUrl += _config.Build.HtmlExtension;
        }

        var item = new XElement("item",
            new XElement("title", post.Title),
            new XElement("link", postUrl),
            new XElement("guid",
                new XAttribute("isPermaLink", "true"),
                postUrl),
            new XElement("pubDate", ToRfc822DateTime(post.PublishDate)),
            new XElement("category", post.Category)
        );

        // Add subcategory if present
        if (!string.IsNullOrWhiteSpace(post.SubCategory))
        {
            item.Add(new XElement("category", post.SubCategory));
        }

        // Add author if configured
        if (!string.IsNullOrWhiteSpace(_config.Site.Author))
        {
            item.Add(new XElement("author", _config.Site.Author));
        }

        // Add description or full content based on configuration
        if (_config.Build.Rss.FullContent)
        {
            // Use CDATA section for full HTML content
            item.Add(new XElement("description", new XCData(post.Content)));
        }
        else
        {
            // Use description field only
            var description = !string.IsNullOrWhiteSpace(post.Description)
                ? post.Description
                : GetContentExcerpt(post.Content, 200);
            item.Add(new XElement("description", description));
        }

        return item;
    }

    /// <summary>
    /// Extract a plain text excerpt from HTML content
    /// </summary>
    private string GetContentExcerpt(string htmlContent, int maxLength)
    {
        // Strip HTML tags
        var plainText = System.Text.RegularExpressions.Regex.Replace(
            htmlContent, @"<[^>]+>", " ");

        // Normalize whitespace
        plainText = System.Text.RegularExpressions.Regex.Replace(
            plainText, @"\s+", " ").Trim();

        // Truncate to max length
        if (plainText.Length <= maxLength)
            return plainText;

        var truncated = plainText.Substring(0, maxLength);
        var lastSpace = truncated.LastIndexOf(' ');

        if (lastSpace > 0)
            truncated = truncated.Substring(0, lastSpace);

        return truncated + "...";
    }

    /// <summary>
    /// Convert DateTime to RFC 822 format (required by RSS 2.0)
    /// Example: Mon, 14 Dec 2025 12:00:00 GMT
    /// </summary>
    private string ToRfc822DateTime(DateTime dateTime)
    {
        // Ensure UTC
        if (dateTime.Kind != DateTimeKind.Utc)
            dateTime = dateTime.ToUniversalTime();

        return dateTime.ToString("ddd, dd MMM yyyy HH:mm:ss 'GMT'", CultureInfo.InvariantCulture);
    }
}
