# ADR-0001: v3.0 Architecture Overhaul

**Date:** 2026-04-06
**Status:** Accepted
**Deciders:** Dane Balia

---

## Context

HappyFrogger reached v2.5.0 as a working static site generator with a single `Program.cs` file of 485 lines. Several pain points accumulated:

1. **RazorLight** required Roslyn compilation at startup (`PreserveCompilationContext`), adding cold-start overhead and a heavyweight dependency for what is ultimately string interpolation into HTML.
2. **Node.js/npm** was required solely to compile Tailwind CSS, adding a second runtime dependency to what should be a single-binary .NET tool.
3. **`Program.cs`** mixed pipeline setup, markdown processing, page rendering, RSS/sitemap generation, and output file writing in one monolithic method — difficult to extend and test in isolation.
4. **Book content** (chapters, study resources, chapter navigation) was co-located with regular blog posts in the same `markdownfiles/` directory. This caused:
   - Category pages requiring special-case logic (`SubCategory == "book"` filtering)
   - No clean way to add a second book
   - Chapter files mixed with hundreds of blog posts
5. **No local development server** — every preview required a manual build, a file browser, and no live reload.
6. **Dumpify** (a debug introspection utility) was shipping in the production binary.

---

## Decisions

### 1. Replace RazorLight with Scriban

**Chosen:** [Scriban](https://github.com/scriban/scriban) `5.9.0`

Scriban is a lightweight, fast template language with no compilation step. Templates are plain `.html` files using `{{ }}` syntax. Properties are exposed as `snake_case` via `StandardMemberRenamer`.

**Alternatives considered:**
- **Keep RazorLight** — Familiar C#/Razor syntax but requires `PreserveCompilationContext`, slow cold start, and Roslyn as a transitive dependency.
- **Fluid** — Clean Liquid syntax, good performance. Rejected because Scriban is more mature for .NET server-side rendering and has better array/date function support.
- **Handlebars.Net** — Lacks built-in date formatting and array operations needed for post listings.

**Trade-offs:**
- Templates must be rewritten from `.cshtml` to `.html` (one-time migration cost).
- Scriban has NuGet audit warnings for template-injection vulnerabilities. These are design characteristics of all template engines when user-supplied content reaches templates — not applicable here since all templates are developer-authored.

**Template variable naming:** All model properties are exposed in `snake_case` (e.g. `PublishDate` → `publish_date`, `ReadingTimeMinutes` → `reading_time_minutes`) via `StandardMemberRenamer.Default` on the `TemplateContext`.

---

### 2. Switch Tailwind CSS to Standalone CLI

**Chosen:** Tailwind CSS standalone CLI binary (platform-specific, placed at `tools/tailwindcss`)

The standalone CLI is a self-contained binary (~35 MB) with zero Node.js dependency. It is downloaded once per machine and gitignored. Build scripts (`build.sh`, `build.ps1`) run it before `dotnet run`.

**Alternatives considered:**
- **Keep npm/Node** — Works, but requires two runtimes for what is a .NET tool. Adds `node_modules/` to the repo surface.
- **Inline CSS / CDN Tailwind** — Would remove the build step entirely but loses tree-shaking, meaning large CSS payloads in production and no custom config (colours, typography plugin).
- **Tailwind v4 with CSS-first config** — v4's standalone CLI is still in active development; v3 is stable and well-supported.

**Consequence:** First-time setup requires a manual binary download. `build.sh`/`build.ps1` print a clear error with the download URL if the binary is absent.

---

### 3. Decompose Program.cs into Focused Classes

**Chosen:** Five new classes, each with a single responsibility:

| Class | Responsibility |
|---|---|
| `PostProcessor` | Reads `.md` files, extracts YAML front matter, runs Markdig, generates TOC, normalises slugs |
| `PageGenerator` | Renders all page types via Scriban; owns template cache |
| `AssetGenerator` | Delegates to `RssFeedGenerator` and `SitemapGenerator`; writes `robots.txt` |
| `BookProcessor` | Reads book folders from config, orders chapters, returns `(BookConfig, chapters)` pairs |
| `DevServer` | `HttpListener` + `FileSystemWatcher` + debounced rebuild for local preview |

`Program.cs` shrinks to ~130 lines: argument parsing, config loading, path validation, and top-level orchestration.

**Slug normalisation:** `GenerateSlug` no longer appends `.html`. The slug stored in `BlogPostModel.Slug` is always the bare slug (e.g. `my-post`). Templates append `.html` explicitly (`{{ slug }}.html`), and `PageGenerator` appends `.html` when writing files. This removes a class of double-extension bugs present in the v2 templates.

---

### 4. Isolate Book Content into Dedicated Folders

**Chosen:** Books are configured as a top-level `books[]` array in `happyfrog.config.json`. Each entry specifies its own `path` (markdown source) and `outputPath` (HTML destination).

```
../blog/
  markdownfiles/          ← blog posts only
  books/
    biblical-understanding/
      index.html          ← generated book index page
      chapter-01.html
      chapter-02.html
```

**Motivation:** Mixing book chapters with blog posts required `SubCategory == "book"` filtering scattered across category page logic and templates. A second book would have required code changes. Isolation makes the content structure self-evident and configuration-driven.

**Book category linking:** `BookConfig.Category` determines which category page shows the book section. The category page renders a `BookSectionModel` per configured book, with `BaseUrl = "books/{id}/"` used to prefix chapter links from the category page.

**CSS relative paths:** Book chapter pages sit two levels deep (`books/{id}/chapter.html`), so `PageGenerator.RenderPost` receives `cssPath = "../../output.css"` and `homeUrl = "../../index.html"` for book chapters.

**New template:** `BookIndexTemplate.html` generates a standalone chapter listing page at `books/{id}/index.html`.

---

### 5. Add `--serve` Mode

**Chosen:** `System.Net.HttpListener` + `System.IO.FileSystemWatcher` implemented in `DevServer.cs`.

```bash
dotnet run -- --serve [--port 8080] [--drafts]
```

After the initial build, `DevServer` starts an HTTP server over the output directory and watches all configured markdown paths (blog + each book folder) for changes. A 500ms debounce timer prevents redundant rebuilds on rapid saves.

**Alternatives considered:**
- **`dotnet watch run`** — Restarts the whole process on code changes, not content changes. Not suitable for a content-watch workflow.
- **`live-server` (npm)** — Would reintroduce the Node dependency we are removing.
- **Kestrel/ASP.NET Core** — Correct for a production server; heavyweight for a dev-only static file server.

**`HttpListener` limitation:** On macOS, `HttpListener` with `localhost` requires no special permissions. On Windows, non-admin users may need to register the URL prefix with `netsh http add urlacl`. This is a known limitation and is acceptable for a local dev tool.

---

## Consequences

**Positive:**
- Single .NET runtime required; no Node.js.
- Templates are plain HTML — editable in any text editor with syntax highlighting.
- Book content is isolated; adding a second book requires only a config entry and a new folder.
- `Program.cs` is readable and extensible; each concern has a clear home.
- Local development is significantly faster with live reload.
- Dumpify removed from production binary.

**Negative / watch items:**
- Existing book chapter `.md` files must be moved from `markdownfiles/` to `books/{id}/` to use the new structure. Existing files left in `markdownfiles/` with `subcategory: book` will still process as regular posts.
- Templates are rewritten; any custom template modifications from v2 must be re-applied to the new `.html` files.
- Tailwind CLI binary must be downloaded manually on each new machine.
- `HttpListener` on Windows may require a one-time `netsh` command for non-admin users.
