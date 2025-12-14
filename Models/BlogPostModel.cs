using YamlDotNet.Serialization;

namespace HappyFrog.Models;

public class BlogPostModel
{
    public string Title { get; set; }
    public DateTime PublishDate { get; set; }
    public string Category { get; set; }
    public string SubCategory { get; set; } // For finer categorization within main sections
    public string Content { get; set; }
    public string Slug { get; set; } // URL-friendly version of the title
    public string Description { get; set; } // Short excerpt or description
    public int ReadingTimeMinutes { get; set; } // Estimated reading time in minutes
    public string SocialImage { get; set; } // Social media image URL
    public string TableOfContents { get; set; } // Generated HTML for table of contents

    [YamlMember(Alias = "status")]
    public string Status { get; set; } = "published"; // Default to published if not specified
}

