using System.Reflection;
using System.Text;
using System.Text.Json;
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
        Console.WriteLine("═══════════════════════════════════════════════");
        Console.WriteLine("   HappyFrogger - Static Site Generator 🐸");
        Console.WriteLine("═══════════════════════════════════════════════\n");

        // Load configuration
        var config = LoadConfiguration();

        string basePath = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
        string templatesPath = Path.Combine(basePath, config.TemplatesPath);

        // Validate and display paths
        Console.WriteLine("Configuration Check:");
        Console.WriteLine("─────────────────────────────────────────────");

        bool allPathsValid = true;

        // Check Templates Path
        if (Directory.Exists(templatesPath))
        {
            Console.WriteLine($"✓ Templates Path: {templatesPath}");
        }
        else
        {
            Console.WriteLine($"✗ Templates Path: {templatesPath} [NOT FOUND]");
            allPathsValid = false;
        }

        // Check Markdown Files Path
        if (Directory.Exists(config.MarkdownFilesPath))
        {
            var fileCount = Directory.GetFiles(config.MarkdownFilesPath, "*.md").Length;
            Console.WriteLine($"✓ Markdown Files Path: {config.MarkdownFilesPath} ({fileCount} files)");
        }
        else
        {
            Console.WriteLine($"✗ Markdown Files Path: {config.MarkdownFilesPath} [NOT FOUND]");
            allPathsValid = false;
        }

        // Check/Create Output Path
        if (!Directory.Exists(config.OutputPath))
        {
            try
            {
                Directory.CreateDirectory(config.OutputPath);
                Console.WriteLine($"✓ Output Path: {config.OutputPath} [CREATED]");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"✗ Output Path: {config.OutputPath} [CANNOT CREATE: {ex.Message}]");
                allPathsValid = false;
            }
        }
        else
        {
            Console.WriteLine($"✓ Output Path: {config.OutputPath}");
        }

        Console.WriteLine("─────────────────────────────────────────────\n");

        if (!allPathsValid)
        {
            Console.WriteLine("❌ Error: Some required paths are missing or invalid.");
            Console.WriteLine("Please check your configuration in happyfrog.config.json\n");
            return;
        }

        Console.WriteLine("Build Settings:");
        Console.WriteLine($"  • Generate Landing Page: {config.Build.GenerateLandingPage}");
        Console.WriteLine($"  • Generate Category Pages: {config.Build.GenerateCategoryPages}");
        Console.WriteLine($"  • Include Drafts: {config.Build.IncludeDrafts}");
        Console.WriteLine($"  • Categories: {string.Join(", ", config.Build.Categories)}");
        Console.WriteLine();

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

        var markdownFiles = Directory.GetFiles(config.MarkdownFilesPath, "*.md");
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
                    string markdownWithGist = ConvertMarkdownToHtmlWithGist(markdown);
                    string htmlContent = Markdown.ToHtml(markdownWithGist, pipeline);

                    var post = new BlogPostModel
                    {
                        Title = metadata.Title,
                        PublishDate = metadata.PublishDate,
                        Category = metadata.Category,
                        SubCategory = metadata.SubCategory,
                        Content = htmlContent,
                        Slug = metadata.Slug ?? GenerateSlug(metadata.Title),
                        Description = metadata.Description,
                        Status = metadata.Status
                    };

                    // Skip drafts if not configured to include them
                    if (post.Status != "published" && !config.Build.IncludeDrafts)
                    {
                        Console.WriteLine($"Skipping draft: {post.Title}");
                        continue;
                    }

                    allPosts.Add(post);

                    // Generate individual post page
                    string result = await engine.CompileRenderAsync("BlogTemplate.cshtml", post);
                    string outputFilename = Path.Combine(config.OutputPath, post.Slug);
                    if (!post.Slug.EndsWith(config.Build.HtmlExtension, StringComparison.OrdinalIgnoreCase))
                    {
                        outputFilename += config.Build.HtmlExtension;
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
        if (config.Build.GenerateLandingPage)
        {
            var landingModel = new LandingPageModel
            {
                TechPosts = allPosts.Where(p => p.Category == "tech").OrderByDescending(p => p.PublishDate),
                FaithPosts = allPosts.Where(p => p.Category == "faith").OrderByDescending(p => p.PublishDate),
                CreativePosts = allPosts.Where(p => p.Category == "creative").OrderByDescending(p => p.PublishDate)
            };

            string landingPage = await engine.CompileRenderAsync("LandingTemplate.cshtml", landingModel);
            File.WriteAllText(Path.Combine(config.OutputPath, "index.html"), landingPage);
        }

        // Generate category pages
        if (config.Build.GenerateCategoryPages)
        {
            foreach (var category in config.Build.Categories)
            {
                var categoryPosts = allPosts.Where(p => p.Category == category);
                await GenerateCategoryPage(engine, categoryPosts, category, $"{category}{config.Build.HtmlExtension}", config.OutputPath);
            }
        }

        // Build summary
        Console.WriteLine("\n═══════════════════════════════════════════════");
        Console.WriteLine("   Build Complete! 🎉");
        Console.WriteLine("═══════════════════════════════════════════════");
        Console.WriteLine($"  • Posts Generated: {allPosts.Count}");
        Console.WriteLine($"  • Landing Page: {(config.Build.GenerateLandingPage ? "✓" : "✗")}");
        Console.WriteLine($"  • Category Pages: {(config.Build.GenerateCategoryPages ? config.Build.Categories.Count : 0)}");
        Console.WriteLine($"  • Output Location: {Path.GetFullPath(config.OutputPath)}");
        Console.WriteLine("═══════════════════════════════════════════════\n");
    }

    private static HappyFrogConfig LoadConfiguration()
    {
        const string configFileName = "happyfrog.config.json";

        // Try to load from current directory first
        string configPath = configFileName;

        // If not found, try the executable directory
        if (!File.Exists(configPath))
        {
            string basePath = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
            configPath = Path.Combine(basePath, configFileName);
        }

        if (File.Exists(configPath))
        {
            try
            {
                string jsonContent = File.ReadAllText(configPath);
                var options = new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true,
                    ReadCommentHandling = JsonCommentHandling.Skip,
                    AllowTrailingCommas = true
                };

                var config = JsonSerializer.Deserialize<HappyFrogConfig>(jsonContent, options);
                Console.WriteLine($"Configuration loaded from: {configPath}");
                return config ?? new HappyFrogConfig();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Warning: Error loading configuration from {configPath}: {ex.Message}");
                Console.WriteLine("Using default configuration.");
            }
        }
        else
        {
            Console.WriteLine($"Configuration file not found at {configPath}. Using default configuration.");
        }

        return new HappyFrogConfig();
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
        string outputFile,
        string outputPath)
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
                string fullOutputPath = Path.Combine(outputPath, outputFile);
                File.WriteAllText(fullOutputPath, result);

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
