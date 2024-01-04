using Markdig;
using RazorLight;
using System.Reflection;

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

        var markdownFiles = Directory.GetFiles("MarkdownFiles", "*.md");

        foreach (var file in markdownFiles)
        {
            string markdown = File.ReadAllText(file);
            string htmlContent = Markdown.ToHtml(markdown);

            var model = new BlogPostModel
            {
                Title = "Your Blog Post Title", // Replace with actual data
                PublishDate = DateTime.Now, // Replace with actual data
                Category = "Category", // Replace with actual data
                Content = htmlContent
            };

            string result = await engine.CompileRenderAsync("BlogTemplate.cshtml", model);
            string outputFilename = Path.Combine("Output", Path.GetFileNameWithoutExtension(file) + ".html");
            File.WriteAllText(outputFilename, result);
        }

        Console.WriteLine("Conversion complete.");
    }
}
