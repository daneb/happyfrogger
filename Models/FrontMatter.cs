using YamlDotNet.Serialization;

namespace HappyFrog.Models;

public class FrontMatter
{
    [YamlMember(Alias = "title")]
    public string Title { get; set; }

    [YamlMember(Alias = "date")]
    public DateTime PublishDate { get; set; }

    [YamlMember(Alias = "category")]
    public string Category { get; set; }

    [YamlMember(Alias = "subcategory")]
    public string SubCategory { get; set; }

    [YamlMember(Alias = "description")]
    public string Description { get; set; }

    [YamlMember(Alias = "slug")]
    public string Slug { get; set; }
    
    [YamlMember(Alias = "status")]
    public string Status { get; set; } = "published"; // Default to published if not specified
}