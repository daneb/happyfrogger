using HappyFrog;
using HappyFrog.Models;

namespace HappyFrog.Tests;

public class PostProcessorTests : IDisposable
{
    private readonly string _tempDir;
    private readonly PostProcessor _sut;

    public PostProcessorTests()
    {
        _tempDir = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString());
        Directory.CreateDirectory(_tempDir);

        _sut = new PostProcessor(new HappyFrogConfig
        {
            Build = new BuildOptions
            {
                IncludeDrafts = true,
                WordsPerMinute = 200,
                Toc = new TocOptions { EnabledByDefault = false }
            }
        });
    }

    public void Dispose() => Directory.Delete(_tempDir, recursive: true);

    // ── helpers ───────────────────────────────────────────────────────────────

    private string WritePost(string frontMatter, string body = "Hello world.")
    {
        var path = Path.Combine(_tempDir, Guid.NewGuid() + ".md");
        File.WriteAllText(path, $"---\n{frontMatter.Trim()}\n---\n\n{body}");
        return path;
    }

    private BlogPostModel ProcessPost(string frontMatter, string body = "Hello world.")
    {
        var path = WritePost(frontMatter, body);
        var post = _sut.ProcessFile(path);
        Assert.NotNull(post);
        return post;
    }

    // ── slug normalisation ────────────────────────────────────────────────────

    [Fact]
    public void Slug_WithHtmlSuffix_IsStripped()
    {
        var post = ProcessPost("title: Test\ndate: 2025-01-01\ncategory: tech\nstatus: published\nslug: my-post.html");
        Assert.Equal("my-post", post.Slug);
    }

    [Fact]
    public void Slug_WithoutHtmlSuffix_IsUnchanged()
    {
        var post = ProcessPost("title: Test\ndate: 2025-01-01\ncategory: tech\nstatus: published\nslug: my-post");
        Assert.Equal("my-post", post.Slug);
    }

    [Fact]
    public void Slug_WhenAbsent_IsGeneratedFromTitle()
    {
        var post = ProcessPost("title: My Great Post\ndate: 2025-01-01\ncategory: tech\nstatus: published");
        Assert.Equal("my-great-post", post.Slug);
    }

    // ── chapter navigation normalisation ─────────────────────────────────────

    [Fact]
    public void PreviousChapter_WithHtmlSuffix_IsStripped()
    {
        var post = ProcessPost("title: Ch2\ndate: 2025-01-01\ncategory: faith\nstatus: published\nslug: ch2\nprevious_chapter: ch1.html");
        Assert.Equal("ch1", post.PreviousChapter);
    }

    [Fact]
    public void NextChapter_WithHtmlSuffix_IsStripped()
    {
        var post = ProcessPost("title: Ch2\ndate: 2025-01-01\ncategory: faith\nstatus: published\nslug: ch2\nnext_chapter: ch3.html");
        Assert.Equal("ch3", post.NextChapter);
    }

    [Fact]
    public void PreviousChapter_WithoutHtmlSuffix_IsUnchanged()
    {
        var post = ProcessPost("title: Ch2\ndate: 2025-01-01\ncategory: faith\nstatus: published\nslug: ch2\nprevious_chapter: ch1");
        Assert.Equal("ch1", post.PreviousChapter);
    }

    [Fact]
    public void ChapterNav_WhenAbsent_IsNull()
    {
        var post = ProcessPost("title: Ch1\ndate: 2025-01-01\ncategory: faith\nstatus: published\nslug: ch1");
        Assert.Null(post.PreviousChapter);
        Assert.Null(post.NextChapter);
    }

    // ── study resources ───────────────────────────────────────────────────────

    [Fact]
    public void StudyResources_AuthorIsDeserialized()
    {
        var post = ProcessPost("""
            title: Chapter
            date: 2025-01-01
            category: faith
            status: published
            slug: chapter
            study_resources:
              - title: "The Wiersbe Bible Commentary"
                author: "Warren Wiersbe"
                description: "Comprehensive biblical commentary."
            """);

        Assert.Single(post.StudyResources);
        Assert.Equal("Warren Wiersbe", post.StudyResources[0].Author);
    }

    [Fact]
    public void StudyResources_MultipleResourcesDeserializedInOrder()
    {
        var post = ProcessPost("""
            title: Chapter
            date: 2025-01-01
            category: faith
            status: published
            slug: chapter
            study_resources:
              - title: "Book A"
                author: "Author A"
                description: "Description A."
              - title: "Book B"
                author: "Author B"
                description: "Description B."
            """);

        Assert.Equal(2, post.StudyResources.Count);
        Assert.Equal("Author A", post.StudyResources[0].Author);
        Assert.Equal("Author B", post.StudyResources[1].Author);
    }

    [Fact]
    public void StudyResources_AuthorIsOptional()
    {
        var post = ProcessPost("""
            title: Chapter
            date: 2025-01-01
            category: faith
            status: published
            slug: chapter
            study_resources:
              - title: "A Resource"
                description: "No author listed."
            """);

        Assert.Single(post.StudyResources);
        Assert.Equal("", post.StudyResources[0].Author);
    }

    // ── draft filtering ───────────────────────────────────────────────────────

    [Fact]
    public void DraftPost_WhenIncludeDraftsFalse_IsExcluded()
    {
        var processor = new PostProcessor(new HappyFrogConfig
        {
            Build = new BuildOptions { IncludeDrafts = false, WordsPerMinute = 200 }
        });
        var path = WritePost("title: Draft\ndate: 2025-01-01\ncategory: tech\nstatus: draft\nslug: draft");
        Assert.Null(processor.ProcessFile(path));
    }

    [Fact]
    public void DraftPost_WhenIncludeDraftsTrue_IsIncluded()
    {
        var path = WritePost("title: Draft\ndate: 2025-01-01\ncategory: tech\nstatus: draft\nslug: draft");
        var post = _sut.ProcessFile(path);
        Assert.NotNull(post);
        Assert.Equal("draft", post.Status);
    }

    // ── reading time ──────────────────────────────────────────────────────────

    [Fact]
    public void ReadingTime_IsAtLeastOneMinute()
    {
        var post = ProcessPost("title: Short\ndate: 2025-01-01\ncategory: tech\nstatus: published\nslug: short", "Hi.");
        Assert.True(post.ReadingTimeMinutes >= 1);
    }

    [Fact]
    public void ReadingTime_ScalesWithWordCount()
    {
        var shortPost = ProcessPost("title: S\ndate: 2025-01-01\ncategory: tech\nstatus: published\nslug: s", "word ");
        var longPost = ProcessPost("title: L\ndate: 2025-01-01\ncategory: tech\nstatus: published\nslug: l",
            string.Join(" ", Enumerable.Repeat("word", 600)));
        Assert.True(longPost.ReadingTimeMinutes > shortPost.ReadingTimeMinutes);
    }
}
