using HappyFrog.Models;

namespace HappyFrog;

public class AssetGenerator
{
    private readonly HappyFrogConfig _config;
    private readonly string _templatesPath;

    public AssetGenerator(HappyFrogConfig config, string templatesPath)
    {
        _config = config;
        _templatesPath = templatesPath;
    }

    public void Generate(IEnumerable<BlogPostModel> allPosts)
    {
        CopyStaticAssets();

        new ImageProcessor(_config.Build.Images, _config.OutputPath).ProcessImages();

        if (_config.Build.Rss.Enabled)
        {
            try
            {
                var feedGenerator = new RssFeedGenerator(_config);
                var feedXml = feedGenerator.Generate(allPosts);
                File.WriteAllText(Path.Combine(_config.OutputPath, _config.Build.Rss.Path), feedXml);
                Console.WriteLine($"  Generated RSS feed: {_config.Build.Rss.Path}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"  Warning: Failed to generate RSS feed: {ex.Message}");
            }
        }

        if (_config.Build.Sitemap.Enabled)
        {
            try
            {
                var sitemapGenerator = new SitemapGenerator(_config);

                File.WriteAllText(Path.Combine(_config.OutputPath, _config.Build.Sitemap.Path), sitemapGenerator.Generate(allPosts));
                Console.WriteLine($"  Generated sitemap: {_config.Build.Sitemap.Path}");

                File.WriteAllText(Path.Combine(_config.OutputPath, "robots.txt"), sitemapGenerator.GenerateRobotsTxt());
                Console.WriteLine("  Generated robots.txt");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"  Warning: Failed to generate sitemap: {ex.Message}");
            }
        }
    }

    /// <summary>Copy the hand-authored design system (theme.css + theme.js) into Output/assets.</summary>
    private void CopyStaticAssets()
    {
        var src = Path.Combine(_templatesPath, "assets");
        var dest = Path.Combine(_config.OutputPath, "assets");

        if (!Directory.Exists(src))
        {
            Console.WriteLine($"  Warning: assets folder not found at {src}");
            return;
        }

        Directory.CreateDirectory(dest);
        foreach (var file in Directory.GetFiles(src))
            File.Copy(file, Path.Combine(dest, Path.GetFileName(file)), overwrite: true);

        Console.WriteLine("  Copied theme assets to Output/assets");
    }
}
