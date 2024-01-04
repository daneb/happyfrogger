using Markdig;
using System;
using System.IO;

class Program
{
    static void Main(string[] args)
    {
        var markdownFiles = Directory.GetFiles("MarkdownFiles", "*.md");

        foreach (var file in markdownFiles)
        {
            string markdown = File.ReadAllText(file);
            string html = Markdown.ToHtml(markdown);

            // Optionally, insert 'html' into a template here

            string outputFilename = Path.Combine("Output", Path.GetFileNameWithoutExtension(file) + ".html");
            File.WriteAllText(outputFilename, html);
        }

        Console.WriteLine("Conversion complete.");
    }
}
