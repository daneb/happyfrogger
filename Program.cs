using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;
using Dumpify;
using HappyFrog.Models;
using Markdig;
using RazorLight;
using YamlDotNet.Core;
using YamlDotNet.Core.Events;
using YamlDotNet.Serialization;
using YamlDotNet.Serialization.NamingConventions;

class Program
{
    static async Task Main(string[] args)
    {
        string basePath = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
        string templatesPath = Path.Combine(basePath, "Templates");

        Console.WriteLine(templatesPath);

        var engine = new RazorLightEngineBuilder()
                        .UseFileSystemProject(templatesPath)
                        .UseMemoryCachingProvider()
                        .Build();

        var deserializer = new DeserializerBuilder()
            .WithNamingConvention(CamelCaseNamingConvention.Instance)
            .IgnoreUnmatchedProperties()  // Useful if front matter has extra fields
            .Build();
       
        var pipeline = new MarkdownPipelineBuilder()
             .UseYamlFrontMatter()
             .UseAdvancedExtensions()
             .Build();

        var markdownFiles = Directory.GetFiles("MarkdownFiles", "*.md");
        var allPosts = new List<BlogPostModel>();

        foreach (var file in markdownFiles)
        {
            try
            {
                var frontMatter = ExtractFrontMatter(file);
                if (!string.IsNullOrEmpty(frontMatter))
                {
                    var metadata = deserializer.Deserialize<FrontMatter>(frontMatter);
                    string markdown = File.ReadAllText(file).Replace(frontMatter, "").TrimStart('-', ' ', '\r', '\n');
                    string htmlContent = Markdown.ToHtml(markdown, pipeline);

                    var post = new BlogPostModel
                    {
                        Title = metadata.Title,
                        PublishDate = metadata.PublishDate,
                        Category = metadata.Category,
                        SubCategory = metadata.SubCategory,
                        Content = htmlContent,
                        Slug = metadata.Slug ?? GenerateSlug(metadata.Title),
                        Description = metadata.Description
                    };

                    allPosts.Add(post);

                    // Generate individual post page
                    string result = await engine.CompileRenderAsync("BlogTemplate.cshtml", post);
                    string outputFilename = Path.Combine("Output", post.Slug);
                    if (!post.Slug.EndsWith(".html", StringComparison.OrdinalIgnoreCase))
                    {
                        outputFilename += ".html";
                    }

                    File.WriteAllText(outputFilename, result);
                }
            }
            catch (Exception e)
            {
                Console.WriteLine($"Error processing {file}: {e.Message}");
            }
        }

        // Generate landing page
        var landingModel = new LandingPageModel
        {
            TechPosts = allPosts.Where(p => p.Category == "tech").OrderByDescending(p => p.PublishDate),
            FaithPosts = allPosts.Where(p => p.Category == "faith").OrderByDescending(p => p.PublishDate),
            CreativePosts = allPosts.Where(p => p.Category == "creative").OrderByDescending(p => p.PublishDate)
        };

        string landingPage = await engine.CompileRenderAsync("LandingTemplate.cshtml", landingModel);
        File.WriteAllText(Path.Combine("Output", "index.html"), landingPage);

        // Generate category pages
        await GenerateCategoryPage(engine, allPosts.Where(p => p.Category == "tech"), "tech", "tech.html");
        await GenerateCategoryPage(engine, allPosts.Where(p => p.Category == "faith"), "faith", "faith.html");
        await GenerateCategoryPage(engine, allPosts.Where(p => p.Category == "creative"), "creative", "creative.html");

    }

    private static string ExtractFrontMatter(string filePath)
    {
        var lines = File.ReadAllLines(filePath);
        var sb = new StringBuilder();
        var insideFrontMatter = false;

        foreach (var line in lines)
        {
            if (line.Trim() == "---")
            {
                if (insideFrontMatter)
                {
                    break;
                }
                insideFrontMatter = true;
                continue;
            }

            if (insideFrontMatter)
            {
                sb.AppendLine(line);
            }
        }

        return sb.ToString().Trim();
    }

    private static string GenerateSlug(string title)
    {
        // Convert title to URL-friendly format
        return title.ToLower()
                   .Replace(" ", "-")
                   .Replace("&", "and")
                   .Replace("'", "")
                   .Replace("\"", "")
                   .Replace("?", "")
                   .Replace("!", "")
                   .Replace(":", "")
                   .Replace(";", "")
                   .Replace("/", "-") + ".html";
    }
    
    private static async Task GenerateCategoryPage(
        RazorLightEngine engine,
        IEnumerable<BlogPostModel> posts,
        string category,
        string outputFile)
    {
        if (CategoryInfo.Categories.TryGetValue(category, out var categoryInfo))
        {
            var model = new CategoryPageModel
            {
                Category = category,
                Title = categoryInfo.Title,
                Description = categoryInfo.Description,
                Posts = posts.OrderByDescending(p => p.PublishDate),
                SubCategories = posts
                    .Where(p => !string.IsNullOrEmpty(p.SubCategory))
                    .Select(p => p.SubCategory)
                    .Distinct()
                    .OrderBy(s => s)
            };

            try
            {
                string result = await engine.CompileRenderAsync("CategoryTemplate.cshtml", model);
                string outputPath = Path.Combine("Output", outputFile);
                File.WriteAllText(outputPath, result);
            
                Console.WriteLine($"Generated category page: {outputFile}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error generating category page {category}: {ex.Message}");
            }
        }
        else
        {
            Console.WriteLine($"Warning: Category '{category}' not found in CategoryInfo.");
        }
    }


    private static string ConvertMarkdownToHtmlWithGist(string markdownContent)
    {
        // Replace Gist placeholders
        var replacedMarkdown = Regex.Replace(markdownContent, @"\[gist:(.*?)\]", m =>
        {
            string gistId = m.Groups[1].Value;
            return $"<script src=\"https://gist.github.com/{gistId}.js\"></script>";
        });

        // Convert to HTML
        return Markdown.ToHtml(replacedMarkdown);
    }

}
