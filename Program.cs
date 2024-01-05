using Markdig;
using RazorLight;
using System.Reflection;
using YamlDotNet.Serialization;
using YamlDotNet.Serialization.NamingConventions;
using YamlDotNet.Core;
using YamlDotNet.Core.Events;
using Dumpify;
using System.Text;
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


        var pipeline = new MarkdownPipelineBuilder()
             .UseYamlFrontMatter()
             .Build();

        var markdownFiles = Directory.GetFiles("MarkdownFiles", "*.md");

        foreach (var file in markdownFiles)
        {

            try
            {
                string fileContent = File.ReadAllText(file);

                // Seperate out front matter
                var frontMatter = ExtractFrontMatter(file);

                if (!string.IsNullOrEmpty(frontMatter))
                {

                    frontMatter.Dump();

                    var deserializer = new DeserializerBuilder()
                        .WithNamingConvention(CamelCaseNamingConvention.Instance)
                        .Build();

                    deserializer.DumpDebug();

                    var metadata = deserializer.Deserialize<FrontMatter>(frontMatter);
                    metadata.Dump();

                    string markdown = File.ReadAllText(file).Replace(frontMatter, "").TrimStart('-', ' ', '\r', '\n');
                    string htmlContent = Markdown.ToHtml(markdown);

                    var model = new BlogPostModel
                    {
                        Title = metadata.Title, // Replace with actual data
                        PublishDate = metadata.PublishDate, // Replace with actual data
                        Category = metadata.Category, // Replace with actual data
                        Content = htmlContent
                    };

                    string result = await engine.CompileRenderAsync("BlogTemplate.cshtml", model);
                    string outputFilename = Path.Combine("Output", Path.GetFileNameWithoutExtension(file) + ".html");
                    File.WriteAllText(outputFilename, result);

                    Console.WriteLine("Conversion complete.");
                }
                else
                {
                    Console.WriteLine("No front matter found.");
                }


            }
            catch (Exception e)
            {
                Console.WriteLine($"Exception while processing file {file}: {e.StackTrace.Dump()}");
            }

        }

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
}
