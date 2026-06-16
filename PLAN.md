# HappyFrogger — v3.0 Improvement Plan

This document tracks the planned improvements to HappyFrogger. Work items are ordered by dependency — items earlier in the list unblock items later.

---

## Planned Improvements

### 1. Remove Dumpify (quick win, no risk)

Dumpify is a debug-only utility currently shipping in the release build. It should be removed.

**Changes:**
- `HappyFrog.csproj` — remove `<PackageReference Include="Dumpify" Version="0.6.4" />`
- `Program.cs` — remove `using Dumpify;`

---

### 2. Replace RazorLight → Scriban

RazorLight requires Roslyn compilation at startup (slow cold start, `PreserveCompilationContext` flag). Scriban is a lightweight, fast template engine with no compilation step.

**Changes:**
- `HappyFrog.csproj`:
  - Remove `RazorLight` package reference
  - Remove `<PreserveCompilationContext>true</PreserveCompilationContext>`
  - Add `Scriban` package (`dotnet add package Scriban`)
- `Templates/` — rewrite all 3 templates from `.cshtml` (Razor) to `.html` (Scriban)
  - Razor `@Model.Title` → Scriban `{{ model.title }}`
  - Razor `@foreach` → Scriban `{{ for post in model.posts }}...{{ end }}`
  - Razor `@if` → Scriban `{{ if condition }}...{{ end }}`
  - Razor `@Html.Raw(...)` → Scriban `{{ value | html.escape }}` or raw output
- `Program.cs` — replace `RazorLightEngineBuilder` setup with `Scriban.Template.Parse()`

**Template files to rewrite:**
- `Templates/BlogTemplate.cshtml` → `Templates/BlogTemplate.html`
- `Templates/CategoryTemplate.cshtml` → `Templates/CategoryTemplate.html`
- `Templates/LandingTemplate.cshtml` → `Templates/LandingTemplate.html`

**Scriban rendering pattern:**
```csharp
var templateText = File.ReadAllText("Templates/BlogTemplate.html");
var template = Template.Parse(templateText);
var result = template.Render(new { model = post }, member => member.Name);
```

---

### 3. Switch Tailwind to Standalone CLI (removes Node dependency)

The current setup requires Node.js + npm just to build Tailwind CSS. Tailwind v3 provides a standalone CLI binary with zero dependencies.

**Steps:**
1. Download the Tailwind standalone CLI for your platform from the Tailwind CSS GitHub releases:
   - macOS ARM: `tailwindcss-macos-arm64`
   - macOS x64: `tailwindcss-macos-x64`
   - Linux x64: `tailwindcss-linux-x64`
2. Place binary in `tools/tailwindcss` (chmod +x on macOS/Linux)
3. Add `tools/` to `.gitignore` (or commit the binary — it's ~35MB self-contained)
4. Create `input.css` (Tailwind directives only):
   ```css
   @tailwind base;
   @tailwind components;
   @tailwind utilities;
   ```
5. Build command: `./tools/tailwindcss -i input.css -o output/styles.css --minify`
6. Add a build script (`build.sh` / `build.ps1`) that:
   a. Runs Tailwind CLI to generate CSS
   b. Runs `dotnet run` to generate the site
7. Remove `package.json`, `package-lock.json`, `node_modules/`

**Changes:**
- Delete `package.json`, `tailwind.config.js`
- Add `input.css`
- Add `tools/tailwindcss` binary (gitignored or committed)
- Add `build.sh` / `build.ps1`
- Update `.gitignore`

---

### 4. Books Folder Isolation

Books content (chapters, study resources) is currently mixed with blog posts in `markdownFiles/`. This makes it hard to manage multiple books and creates conceptual clutter.

**Proposed structure:**
```
../blog/
  markdownfiles/          ← blog posts only (tech, faith, creative)
  books/
    biblical-understanding/   ← one folder per book
      chapter-01.md
      chapter-02.md
      ...
```

**Config changes** — add `books` array to `happyfrog.config.json`:
```json
{
  "books": [
    {
      "id": "biblical-understanding",
      "title": "Unlocking your Biblical Understanding",
      "description": "...",
      "path": "../blog/books/biblical-understanding",
      "outputPath": "../blog/books/biblical-understanding",
      "coverImage": "/images/book-cover.jpg"
    }
  ]
}
```

**Model changes:**
- Add `BookConfig` class to `HappyFrogConfig.cs`
- `BookChapterModel` in `CategoryPageModel.cs` currently duplicates fields from `BlogPostModel` — clean this up by ensuring `BlogPostModel` is the single source of book-specific fields and `BookChapterModel` is removed

**Processing changes:**
- Process book chapters separately from blog posts
- Output book pages to `books/{book-id}/` subfolder
- Generate a book index page (`books/biblical-understanding/index.html`) listing all chapters with progress
- Blog post category pages (e.g., `faith.html`) no longer need to contain book chapters — link to the book index instead

---

### 5. Refactor Program.cs

`Program.cs` is 485 lines with pipeline setup, post processing, page generation, and asset generation all mixed together. Split into focused classes.

**New file structure:**
```
HappyFrog/
  Program.cs              ← ~80 lines: CLI arg parsing, config loading, path validation, orchestration
  PostProcessor.cs        ← reads .md files, extracts front matter, builds BlogPostModel list
  PageGenerator.cs        ← renders posts, landing page, category pages, book index pages
  AssetGenerator.cs       ← RSS feed, sitemap.xml, robots.txt (wraps existing generators)
  BookProcessor.cs        ← reads book folders, builds chapter lists, generates book pages
  DevServer.cs            ← --serve mode: HttpListener + FileSystemWatcher
  RssFeedGenerator.cs     ← unchanged
  SitemapGenerator.cs     ← unchanged
  Models/
    ...                   ← unchanged
  Templates/
    ...                   ← updated (Scriban)
```

**PostProcessor.cs responsibilities:**
- `BuildMarkdownPipeline()` — Markdig pipeline setup
- `ProcessFile(string filePath, HappyFrogConfig config)` → `BlogPostModel?`
- `ReplaceGistPlaceholders(string markdown)` → `string`
- `GenerateTableOfContents(string html, TocOptions options)` → `string`
- `CalculateReadingTime(string html, int wpm)` → `int`
- `ExtractFrontMatter(string filePath)` → `string`
- `GenerateSlug(string title)` → `string`

**PageGenerator.cs responsibilities:**
- Constructor takes `ITemplateEngine` (interface over Scriban)
- `RenderPost(BlogPostModel post)` → `string`
- `RenderLandingPage(LandingPageModel model)` → `string`
- `RenderCategoryPage(CategoryPageModel model)` → `string`
- `RenderBookIndex(BookIndexModel model)` → `string`

**AssetGenerator.cs responsibilities:**
- Thin wrapper: calls `RssFeedGenerator` and `SitemapGenerator`
- `Generate(IEnumerable<BlogPostModel> posts, HappyFrogConfig config)` — writes all assets

**BookProcessor.cs responsibilities:**
- `ProcessBook(BookConfig bookConfig, PostProcessor postProcessor)` → `IEnumerable<BlogPostModel>`
- Reads from `bookConfig.Path`, processes chapters, sorts by chapter number
- Returns ordered chapter list

**Program.cs (orchestrator) flow:**
```
1. Parse CLI args (--serve, --drafts, --config <path>)
2. LoadConfiguration()
3. ValidatePaths()
4. new PostProcessor() → processes blog posts
5. new BookProcessor() → processes each book
6. new PageGenerator() → renders all pages to disk
7. new AssetGenerator() → writes RSS, sitemap, robots.txt
8. if --serve: new DevServer(config).Start()
```

---

### 6. Add --serve Mode

A local development server for live preview without needing a separate tool.

**CLI usage:**
```
dotnet run -- --serve          # build + serve on localhost:4000
dotnet run -- --serve --port 8080
dotnet run -- --serve --drafts  # include drafts in preview
```

**DevServer.cs implementation:**
- `HttpListener` serving static files from `config.OutputPath`
- `FileSystemWatcher` watching `config.MarkdownFilesPath` and each book path
- On file change: trigger incremental rebuild (only changed file + dependent pages)
- MIME type mapping for `.html`, `.css`, `.js`, `.xml`, `.txt`, `.png`, `.jpg`, etc.
- Console output: `Serving at http://localhost:4000 — watching for changes...`
- Graceful Ctrl+C shutdown

**Key implementation notes:**
- `HttpListener` requires `http://localhost:{port}/` prefix registered
- Serve `index.html` for directory requests
- Return 404 HTML for missing files (not an exception)
- Rebuild is async — don't block the listener thread

---

## Implementation Order

| # | Task | Effort | Risk |
|---|------|--------|------|
| 1 | Remove Dumpify | 5 min | None |
| 2 | Replace RazorLight → Scriban | 3–4 hrs | Medium (template rewrite) |
| 3 | Books folder isolation | 2–3 hrs | Low |
| 4 | Refactor Program.cs | 3–4 hrs | Low (logic unchanged) |
| 5 | Tailwind standalone CLI | 1–2 hrs | Low |
| 6 | --serve mode | 3–4 hrs | Low |

**Total estimated effort: 2–3 working days**

Start with items 1–3 as they are independent. Item 4 (refactor) is easier after item 2 (Scriban) is done since the template engine code changes anyway. Item 6 (serve) depends on item 4 (refactor) being clean.

---

## Version Target

These changes collectively constitute **v3.0.0**.

Suggested changelog entries:
- Breaking: Tailwind now requires standalone CLI binary instead of npm
- Breaking: Book content moved to dedicated `books/` folder with config-driven paths
- Changed: Templates now use Scriban syntax (`.html`) instead of Razor (`.cshtml`)
- Added: `--serve` flag for local development server with live reload
- Removed: Dumpify dependency
- Refactored: `Program.cs` split into `PostProcessor`, `PageGenerator`, `AssetGenerator`, `BookProcessor`, `DevServer`
