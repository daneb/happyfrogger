namespace HappyFrog.Models;

public class CategoryPageModel
{
    public string Category { get; set; }
    public string Title { get; set; }
    public string Description { get; set; }
    public IEnumerable<string> SubCategories { get; set; }
    public IEnumerable<BlogPostModel> Posts { get; set; }
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