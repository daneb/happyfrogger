using HappyFrog.Models;
using Scriban;
using Scriban.Runtime;

namespace HappyFrog;

public class PageGenerator
{
    private readonly HappyFrogConfig _config;
    private readonly string _templatesPath;
    private readonly Dictionary<string, Template> _cache = new();

    public PageGenerator(HappyFrogConfig config, string templatesPath)
    {
        _config = config;
        _templatesPath = templatesPath;
    }

    public void RenderPost(BlogPostModel post, string outputPath,
        string cssPath = "assets/theme.css",
        string jsPath = "assets/theme.js",
        string? backUrl = null,
        string? backLabel = null,
        string homeUrl = "index.html")
    {
        var extra = new Dictionary<string, object?>
        {
            ["css_path"]   = cssPath,
            ["js_path"]    = jsPath,
            ["home_url"]   = homeUrl,
            ["back_url"]   = backUrl ?? $"{post.Category}.html",
            ["back_label"] = backLabel ?? Capitalise(post.Category),
        };
        File.WriteAllText(outputPath, Render("BlogTemplate.html", post, extra));
    }

    public void RenderLandingPage(LandingPageModel model, string outputPath)
    {
        File.WriteAllText(outputPath, Render("LandingTemplate.html", model));
    }

    public void RenderCategoryPage(CategoryPageModel model, string outputPath)
    {
        File.WriteAllText(outputPath, Render("CategoryTemplate.html", model));
    }

    public void RenderBookIndex(BookIndexModel model, string outputPath)
    {
        var extra = new Dictionary<string, object?>
        {
            ["css_path"]  = "../../assets/theme.css",
            ["js_path"]   = "../../assets/theme.js",
            ["home_url"]  = "../../index.html",
            ["root_url"]  = "../../",
        };
        File.WriteAllText(outputPath, Render("BookIndexTemplate.html", model, extra));
    }

    // ── internals ────────────────────────────────────────────────────────────

    private string Render<T>(string templateName, T model, Dictionary<string, object?>? extra = null)
    {
        var template = GetTemplate(templateName);

        var scriptObject = new ScriptObject();
        scriptObject.Import(model!, renamer: StandardMemberRenamer.Default);

        // Global helpers available in every template
        scriptObject["current_year"]     = DateTime.Now.Year;
        scriptObject["site_title"]       = _config.Site.Title;
        scriptObject["site_author"]      = _config.Site.Author;
        scriptObject["site_description"] = _config.Site.Description;
        scriptObject["base_url"]         = _config.Site.BaseUrl;
        scriptObject["css_path"]         = "assets/theme.css";
        scriptObject["js_path"]          = "assets/theme.js";
        scriptObject["home_url"]         = "index.html";

        if (extra != null)
            foreach (var (k, v) in extra)
                scriptObject[k] = v;

        var context = new TemplateContext { MemberRenamer = StandardMemberRenamer.Default };
        context.PushGlobal(scriptObject);

        return template.Render(context);
    }

    private Template GetTemplate(string name)
    {
        if (_cache.TryGetValue(name, out var cached))
            return cached;

        var path = Path.Combine(_templatesPath, name);
        var text = File.ReadAllText(path);
        var template = Template.Parse(text);

        if (template.HasErrors)
            throw new InvalidOperationException(
                $"Template '{name}' has errors:\n" +
                string.Join("\n", template.Messages));

        _cache[name] = template;
        return template;
    }

    private static string Capitalise(string s) =>
        string.IsNullOrEmpty(s) ? s : char.ToUpper(s[0]) + s[1..];
}
