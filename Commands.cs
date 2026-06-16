using HappyFrog.Models;

namespace HappyFrog;

public static class Commands
{
    /// <summary>
    ///   hf new "My Title" -c tech [-s subcat] [--book biblical-understanding]
    /// Scaffolds a markdown file with front matter so you can start writing immediately.
    /// </summary>
    public static int New(string[] args, HappyFrogConfig config)
    {
        var title = args.FirstOrDefault(a => !a.StartsWith('-'));
        if (string.IsNullOrWhiteSpace(title))
        {
            Console.WriteLine("Usage: hf new \"Title\" -c <category> [-s <subcategory>] [--book <bookId>]");
            return 1;
        }

        string category = Opt(args, "-c", "--category") ?? "tech";
        string? sub     = Opt(args, "-s", "--subcategory");
        string? bookId  = Opt(args, "--book");

        var slug = Slug(title);

        string dir;
        if (bookId is null)
        {
            dir = config.MarkdownFilesPath;
        }
        else
        {
            var book = config.Books.FirstOrDefault(b => b.Id == bookId);
            if (book is null) { Console.WriteLine($"Unknown book id: {bookId}"); return 1; }
            dir = book.Path;
            sub ??= "book";
        }

        Directory.CreateDirectory(dir);
        var file = Path.Combine(dir, slug + ".md");
        if (File.Exists(file)) { Console.WriteLine($"Already exists: {file}"); return 1; }

        var subLine = sub is null ? "" : $"subcategory: {sub}\n";
        var frontMatter =
$@"---
title: ""{title}""
date: {DateTime.Now:yyyy-MM-dd}
category: {category}
{subLine}description: """"
slug: {slug}
status: draft
---

Write here…
";
        File.WriteAllText(file, frontMatter);

        Console.WriteLine($"✓ Created {file}  (status: draft)");
        Console.WriteLine("  Edit it, run `hf serve` to preview, then set status: published and run `hf` to build.");
        return 0;
    }

    private static string? Opt(string[] a, params string[] keys)
    {
        foreach (var k in keys)
        {
            var i = Array.IndexOf(a, k);
            if (i >= 0 && i + 1 < a.Length) return a[i + 1];
        }
        return null;
    }

    private static string Slug(string title)
    {
        var chars = title.ToLowerInvariant()
            .Select(c => char.IsLetterOrDigit(c) ? c : (c == ' ' || c == '-') ? '-' : '\0')
            .Where(c => c != '\0');
        var slug = new string(chars.ToArray());
        while (slug.Contains("--")) slug = slug.Replace("--", "-");
        return slug.Trim('-');
    }
}
