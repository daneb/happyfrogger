using System.Text;
using System.Text.RegularExpressions;
using HappyFrog.Models;
using Markdig;
using Markdig.Extensions.AutoIdentifiers;
using YamlDotNet.Serialization;
using YamlDotNet.Serialization.NamingConventions;

namespace HappyFrog;

public class PostProcessor
{
    private readonly MarkdownPipeline _pipeline;
    private readonly IDeserializer _deserializer;
    private readonly HappyFrogConfig _config;

    public PostProcessor(HappyFrogConfig config)
    {
        _config = config;

        _pipeline = new MarkdownPipelineBuilder()
            .UseYamlFrontMatter()
            .UseAdvancedExtensions()
            .UseAutoIdentifiers(AutoIdentifierOptions.GitHub)
            .Build();

        _deserializer = new DeserializerBuilder()
            .IgnoreUnmatchedProperties()
            .Build();
    }

    public List<BlogPostModel> ProcessDirectory(string path)
    {
        var posts = new List<BlogPostModel>();
        foreach (var file in Directory.GetFiles(path, "*.md"))
        {
            try
            {
                var post = ProcessFile(file);
                if (post != null)
                    posts.Add(post);
            }
            catch (Exception e)
            {
                Console.WriteLine($"  Error processing {Path.GetFileName(file)}: {e.Message}");
            }
        }
        return posts;
    }

    public BlogPostModel? ProcessFile(string filePath)
    {
        var frontMatterYaml = ExtractFrontMatter(filePath);
        if (string.IsNullOrEmpty(frontMatterYaml))
            return null;

        var metadata = _deserializer.Deserialize<FrontMatter>(frontMatterYaml);
        string markdown = File.ReadAllText(filePath).Replace(frontMatterYaml, "").TrimStart('-', ' ', '\r', '\n');
        markdown = ReplaceGistPlaceholders(markdown);

        string htmlContent = Markdown.ToHtml(markdown, _pipeline);

        bool shouldGenerateToc = metadata.Toc ?? _config.Build.Toc.EnabledByDefault;
        string tableOfContents = shouldGenerateToc
            ? GenerateTableOfContents(htmlContent, _config.Build.Toc)
            : string.Empty;

        // Normalise slug: never store .html suffix — templates append it
        var rawSlug = metadata.Slug ?? GenerateSlug(metadata.Title);
        var slug = rawSlug.EndsWith(".html", StringComparison.OrdinalIgnoreCase)
            ? rawSlug[..^5]
            : rawSlug;

        var post = new BlogPostModel
        {
            Title = metadata.Title,
            PublishDate = metadata.PublishDate,
            Category = metadata.Category,
            SubCategory = metadata.SubCategory,
            Content = htmlContent,
            Slug = slug,
            Description = metadata.Description,
            Status = metadata.Status,
            ReadingTimeMinutes = CalculateReadingTime(htmlContent, _config.Build.WordsPerMinute),
            SocialImage = string.IsNullOrEmpty(metadata.SocialImage) ? null : metadata.SocialImage,
            TableOfContents = tableOfContents,
            ChapterNumber = metadata.ChapterNumber,
            Progress = metadata.Progress,
            PreviousChapter = metadata.PreviousChapter,
            NextChapter = metadata.NextChapter,
            StudyResources = metadata.StudyResources
        };

        if (post.Status != "published" && !_config.Build.IncludeDrafts)
        {
            Console.WriteLine($"  Skipping draft: {post.Title}");
            return null;
        }

        return post;
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
                if (insideFrontMatter) break;
                insideFrontMatter = true;
                continue;
            }
            if (insideFrontMatter)
                sb.AppendLine(line);
        }

        return sb.ToString().Trim();
    }

    private static string GenerateSlug(string title)
    {
        return title.ToLower()
            .Replace(" ", "-")
            .Replace("&", "and")
            .Replace("'", "")
            .Replace("\"", "")
            .Replace("?", "")
            .Replace("!", "")
            .Replace(":", "")
            .Replace(";", "")
            .Replace("/", "-");
    }

    private static int CalculateReadingTime(string htmlContent, int wordsPerMinute)
    {
        var plainText = Regex.Replace(htmlContent, @"<[^>]+>", " ");
        plainText = Regex.Replace(plainText, @"\s+", " ").Trim();
        var wordCount = plainText.Split(new[] { ' ', '\t', '\n', '\r' }, StringSplitOptions.RemoveEmptyEntries).Length;
        return Math.Max(1, (int)Math.Ceiling((double)wordCount / wordsPerMinute));
    }

    private static string ReplaceGistPlaceholders(string markdownContent)
    {
        return Regex.Replace(markdownContent, @"\[gist:(.*?)\]", m =>
        {
            string gistId = m.Groups[1].Value;
            return $"<script src=\"https://gist.github.com/{gistId}.js\"></script>";
        });
    }

    private static string GenerateTableOfContents(string htmlContent, TocOptions options)
    {
        var headingPattern = @"<h([2-6])[^>]*id=""([^""]*)""[^>]*>(.*?)</h\1>";
        var matches = Regex.Matches(htmlContent, headingPattern, RegexOptions.IgnoreCase);

        if (matches.Count < options.MinHeadings)
            return string.Empty;

        var tocBuilder = new StringBuilder();
        tocBuilder.AppendLine("<nav class=\"toc\" role=\"navigation\">");
        tocBuilder.AppendLine($"  <h2 class=\"toc-title\">{options.Title}</h2>");
        tocBuilder.AppendLine("  <ul class=\"toc-list\">");

        int lastLevel = 2;

        foreach (Match match in matches)
        {
            int level = int.Parse(match.Groups[1].Value);
            if (level > options.MaxLevel) continue;

            string id = match.Groups[2].Value;
            string text = Regex.Replace(match.Groups[3].Value, @"<[^>]+>", "");

            if (level > lastLevel)
            {
                for (int i = lastLevel; i < level; i++)
                    tocBuilder.AppendLine($"{new string(' ', (i - 1) * 2)}  <ul>");
            }
            else if (level < lastLevel)
            {
                for (int i = level; i < lastLevel; i++)
                {
                    tocBuilder.AppendLine($"{new string(' ', (i - 1) * 2)}  </ul>");
                    tocBuilder.AppendLine($"{new string(' ', (i - 1) * 2)}  </li>");
                }
            }
            else if (lastLevel > 2 || tocBuilder.ToString().Contains("<li>"))
            {
                tocBuilder.AppendLine($"{new string(' ', (level - 2) * 2)}    </li>");
            }

            tocBuilder.AppendLine($"{new string(' ', (level - 2) * 2)}    <li><a href=\"#{id}\">{text}</a>");
            lastLevel = level;
        }

        for (int i = 2; i < lastLevel; i++)
        {
            tocBuilder.AppendLine($"{new string(' ', (i - 2) * 2)}  </ul>");
            tocBuilder.AppendLine($"{new string(' ', (i - 2) * 2)}  </li>");
        }

        tocBuilder.AppendLine("    </li>");
        tocBuilder.AppendLine("  </ul>");
        tocBuilder.AppendLine("</nav>");

        return tocBuilder.ToString();
    }
}
