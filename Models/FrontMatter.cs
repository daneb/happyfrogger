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

    [YamlMember(Alias = "socialImage")]
    public string SocialImage { get; set; } // Optional per-post social media image

    [YamlMember(Alias = "toc")]
    public bool? Toc { get; set; } // Optional per-post TOC override (null = use default)

    // Book-specific properties
    [YamlMember(Alias = "chapter_number")]
    public int? ChapterNumber { get; set; }

    [YamlMember(Alias = "progress")]
    public int? Progress { get; set; }

    [YamlMember(Alias = "previous_chapter")]
    public string PreviousChapter { get; set; }

    [YamlMember(Alias = "next_chapter")]
    public string NextChapter { get; set; }

    [YamlMember(Alias = "study_resources")]
    public List<StudyResource> StudyResources { get; set; }
}