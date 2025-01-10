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
    
    [YamlMember(Alias = "status")]
    public string Status { get; set; } = "published"; // Default to published if not specified
}

