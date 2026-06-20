using System.Reflection;
using System.Text.Json;
using HappyFrog;
using HappyFrog.Models;

Console.WriteLine("═══════════════════════════════════════════════");
Console.WriteLine("   HappyFrogger - Static Site Generator");
Console.WriteLine("═══════════════════════════════════════════════\n");

// ── Sub-commands (run before the build pipeline) ──────────────────────────────
if (args.Length > 0 && args[0] == "new")
    return Commands.New(args[1..], LoadConfiguration());

// ── CLI arguments ─────────────────────────────────────────────────────────────
bool serveMode     = args.Contains("--serve") || args.Contains("-s");
bool includeDrafts = args.Contains("--drafts");
int  port          = 4000;
var  portIdx       = Array.IndexOf(args, "--port");
if (portIdx >= 0 && portIdx + 1 < args.Length && int.TryParse(args[portIdx + 1], out int p))
    port = p;

// ── Config ────────────────────────────────────────────────────────────────────
var config = LoadConfiguration();
if (includeDrafts) config.Build.IncludeDrafts = true;

string basePath      = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location)!;
string templatesPath = Path.Combine(basePath, config.TemplatesPath);

if (!ValidatePaths(config, templatesPath)) return 1;

PrintBuildSettings(config);

// ── Build (runs once on startup and again on each file change in serve mode) ──
async Task RunBuild()
{
    var postProcessor  = new PostProcessor(config);
    var pageGenerator  = new PageGenerator(config, templatesPath);
    var assetGenerator = new AssetGenerator(config, templatesPath);

    // 1. Blog posts
    Console.WriteLine("Processing blog posts...");
    var allPosts = postProcessor.ProcessDirectory(config.MarkdownFilesPath)
        .OrderByDescending(p => p.PublishDate)
        .ToList();

    foreach (var post in allPosts)
        pageGenerator.RenderPost(post, Path.Combine(config.OutputPath, post.Slug + ".html"));
    Console.WriteLine($"  {allPosts.Count} post(s) generated");

    // 2. Books — processed exactly once
    Console.WriteLine("Processing books...");
    var bookProcessor = new BookProcessor(config, postProcessor);
    var books = bookProcessor.ProcessBooks();

    foreach (var b in books)
    {
        Directory.CreateDirectory(b.Config.OutputPath);

        foreach (var chapter in b.Chapters)
            pageGenerator.RenderPost(
                chapter, Path.Combine(b.Config.OutputPath, chapter.Slug + ".html"),
                cssPath:   "../../assets/theme.css",
                jsPath:    "../../assets/theme.js",
                backUrl:   "index.html",
                backLabel: "All chapters",
                homeUrl:   "../../index.html");

        pageGenerator.RenderBookIndex(b.Index, Path.Combine(b.Config.OutputPath, "index.html"));
    }

    // 3. Landing page — data-driven sections + featured book
    if (config.Build.GenerateLandingPage)
    {
        var featured = books.FirstOrDefault();
        var landing = new LandingPageModel
        {
            FeaturedBook = featured is null ? null : new FeaturedBook
            {
                Title       = featured.Index.Title,
                Description = featured.Index.Description,
                BaseUrl     = $"books/{featured.Config.Id}/",
                CoverImage  = featured.Config.CoverImage,
                Progress    = featured.Index.Progress
            },
            Sections = config.Build.Categories.Select(cat => new LandingSection
            {
                Category = cat,
                Heading  = CategoryInfo.Categories.TryGetValue(cat, out var i) ? i.Title : Capitalise(cat),
                Cta      = "View all",
                Posts    = allPosts.Where(p => p.Category == cat).Take(3).ToList()
            }).ToList()
        };
        pageGenerator.RenderLandingPage(landing, Path.Combine(config.OutputPath, "index.html"));
        Console.WriteLine("  Landing page generated");
    }

    // 4. Category pages
    if (config.Build.GenerateCategoryPages)
    {
        foreach (var category in config.Build.Categories)
        {
            if (!CategoryInfo.Categories.TryGetValue(category, out var info)) continue;

            var categoryPosts = allPosts.Where(p => p.Category == category).ToList();

            var model = new CategoryPageModel
            {
                Category    = category,
                Title       = info.Title,
                Description = info.Description,
                PostsByYear = categoryPosts
                    .GroupBy(p => p.PublishDate.Year)
                    .OrderByDescending(g => g.Key)
                    .Select(g => new YearGroup
                    {
                        Year  = g.Key,
                        Posts = g.OrderByDescending(p => p.PublishDate).ToList()
                    })
                    .ToList(),
                Books = books
                    .Where(b => b.Config.Category == category)
                    .Select(b => new BookSectionModel
                    {
                        Title       = b.Index.Title,
                        Description = b.Index.Description,
                        BaseUrl     = $"books/{b.Config.Id}/",
                        Progress    = b.Index.Progress,
                        Chapters    = b.Chapters
                    })
                    .ToList(),
                SubCategories = categoryPosts
                    .Where(p => !string.IsNullOrEmpty(p.SubCategory))
                    .Select(p => p.SubCategory!)
                    .Distinct()
                    .OrderBy(s => s)
            };
            pageGenerator.RenderCategoryPage(model, Path.Combine(config.OutputPath, $"{category}.html"));
            Console.WriteLine($"  Category page generated: {category}.html");
        }
    }

    // 5. Assets (theme.css/js, RSS, sitemap, robots.txt)
    Console.WriteLine("Generating assets...");
    assetGenerator.Generate(allPosts);

    PrintBuildSummary(allPosts.Count, config);
}

await RunBuild();

// ── Serve mode ────────────────────────────────────────────────────────────────
if (serveMode)
{
    using var cts = new CancellationTokenSource();
    Console.CancelKeyPress += (_, e) => { e.Cancel = true; cts.Cancel(); };

    var server = new DevServer(config, port);
    await server.StartAsync(RunBuild, cts.Token);
}

return 0;

// ── Helpers ───────────────────────────────────────────────────────────────────
static string Capitalise(string s) =>
    string.IsNullOrEmpty(s) ? s : char.ToUpper(s[0]) + s[1..];

static HappyFrogConfig LoadConfiguration()
{
    const string configFileName = "happyfrog.config.json";
    string configPath = configFileName;

    if (!File.Exists(configPath))
    {
        string basePath = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location)!;
        configPath = Path.Combine(basePath, configFileName);
    }

    if (File.Exists(configPath))
    {
        try
        {
            var options = new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true,
                ReadCommentHandling = JsonCommentHandling.Skip,
                AllowTrailingCommas = true
            };
            var config = JsonSerializer.Deserialize<HappyFrogConfig>(File.ReadAllText(configPath), options);
            Console.WriteLine($"Configuration loaded from: {configPath}");
            return config ?? new HappyFrogConfig();
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Warning: Error loading config: {ex.Message}. Using defaults.");
        }
    }
    else
    {
        Console.WriteLine($"Config not found at {configPath}. Using defaults.");
    }

    return new HappyFrogConfig();
}

static bool ValidatePaths(HappyFrogConfig config, string templatesPath)
{
    Console.WriteLine("Configuration Check:");
    Console.WriteLine("─────────────────────────────────────────────");
    bool valid = true;

    Check(Directory.Exists(templatesPath), ref valid, "Templates", templatesPath);
    Check(Directory.Exists(config.MarkdownFilesPath), ref valid, "Markdown", config.MarkdownFilesPath);

    if (!Directory.Exists(config.OutputPath))
    {
        try
        {
            Directory.CreateDirectory(config.OutputPath);
            Console.WriteLine($"✓ Output:    {config.OutputPath} [CREATED]");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"✗ Output:    {config.OutputPath} [CANNOT CREATE: {ex.Message}]");
            valid = false;
        }
    }
    else
    {
        Console.WriteLine($"✓ Output:    {config.OutputPath}");
    }

    Console.WriteLine("─────────────────────────────────────────────\n");
    if (!valid)
        Console.WriteLine("❌ Some required paths are missing. Check happyfrog.config.json\n");
    return valid;

    static void Check(bool exists, ref bool valid, string label, string path)
    {
        if (exists) Console.WriteLine($"✓ {label,-12}{path}");
        else { Console.WriteLine($"✗ {label,-12}{path} [NOT FOUND]"); valid = false; }
    }
}

static void PrintBuildSettings(HappyFrogConfig config)
{
    Console.WriteLine("Build Settings:");
    Console.WriteLine($"  • Landing Page:   {config.Build.GenerateLandingPage}");
    Console.WriteLine($"  • Category Pages: {config.Build.GenerateCategoryPages}");
    Console.WriteLine($"  • Include Drafts: {config.Build.IncludeDrafts}");
    Console.WriteLine($"  • Categories:     {string.Join(", ", config.Build.Categories)}");
    Console.WriteLine($"  • Books:          {config.Books.Count}");
    Console.WriteLine();
}

static void PrintBuildSummary(int postCount, HappyFrogConfig config)
{
    Console.WriteLine("\n═══════════════════════════════════════════════");
    Console.WriteLine("   Build Complete!");
    Console.WriteLine("═══════════════════════════════════════════════");
    Console.WriteLine($"  • Posts:    {postCount}");
    Console.WriteLine($"  • Landing:  {(config.Build.GenerateLandingPage ? "✓" : "✗")}");
    Console.WriteLine($"  • Category: {(config.Build.GenerateCategoryPages ? config.Build.Categories.Count : 0)} page(s)");
    Console.WriteLine($"  • RSS:      {(config.Build.Rss.Enabled ? "✓" : "✗")}");
    Console.WriteLine($"  • Sitemap:  {(config.Build.Sitemap.Enabled ? "✓" : "✗")}");
    Console.WriteLine($"  • Output:   {Path.GetFullPath(config.OutputPath)}");
    Console.WriteLine("═══════════════════════════════════════════════\n");
}
