using YamlDotNet.Serialization;

namespace HappyFrog.Models;

public class CategoryPageModel
{
    public string Category { get; set; }
    public string Title { get; set; }
    public string Description { get; set; }
    public IEnumerable<string> SubCategories { get; set; }
    public IEnumerable<BlogPostModel> Posts { get; set; }
}

public class BookChapterModel : BlogPostModel
{
    [YamlMember(Alias = "chapter_number")]
    public int ChapterNumber { get; set; }

    [YamlMember(Alias = "progress")]
    public int Progress { get; set; }

    [YamlMember(Alias = "previous_chapter")]
    public string PreviousChapter { get; set; }

    [YamlMember(Alias = "next_chapter")]
    public string NextChapter { get; set; }

    [YamlMember(Alias = "study_resources")]
    public List<StudyResource> StudyResources { get; set; }

    public string TableOfContents { get; set; }
}

public class StudyResource
{
    [YamlMember(Alias = "title")]
    public string Title { get; set; }

    [YamlMember(Alias = "description")]
    public string Description { get; set; }

    [YamlMember(Alias = "url")]
    public string Url { get; set; }
}

public static class CategoryInfo
{
    public static readonly Dictionary<string, (string Title, string Description)> Categories = 
        new Dictionary<string, (string Title, string Description)>
        {
            { "tech", ("Tech Insights", "Deep dives into software development, system design, and tech career insights.") },
            { "faith", ("Faith & Wisdom", "Explorations in Christian living, Biblical insights, and my upcoming book.") },
            { "creative", ("Creative Corner", "A collection of poems, reflections, and personal essays.") }
        };
}