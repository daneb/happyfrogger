namespace HappyFrog.Models;
// New model for the landing page
public class LandingPageModel
{
    public IEnumerable<BlogPostModel> TechPosts { get; set; }
    public IEnumerable<BlogPostModel> FaithPosts { get; set; }
    public IEnumerable<BlogPostModel> CreativePosts { get; set; }
}