# HappyFrog 🐸

A lightweight static site generator in C# / .NET 8. Write Markdown with front matter, run one command, get a fast, warm, reading-first site — with first-class support for **books** (grouped into Parts, with coming-soon chapters and a multi-book shelf) and light/dark themes.

No Node. No Tailwind. No build step for CSS — the design system is one hand-authored `theme.css`.

---

## Quick start

HappyFrogger separates the **engine** (this repo) from your **content** (your blog repo). The config and all content live in your blog directory; the engine is run from there.

```bash
# 1. Clone the engine alongside your blog
git clone https://github.com/daneb/happyfrogger
# expects: ../my-blog/ and ../happyfrogger/ as siblings

# 2. Put happyfrog.config.json in your blog directory (see Configuration below)

# 3. Build the site (from the happyfrogger directory)
./build.sh

# Or point at any blog directory:
./build.sh --blog /path/to/my-blog

# 4. Preview with live reload
./build.sh -- --serve
```

The script `cd`s into the blog directory, so the engine picks up `happyfrog.config.json` and resolves all paths relative to the content repo. Generated HTML lands in your configured `outputPath`. Deploy that folder anywhere static (GitHub Pages, Netlify, S3, …).

**Alternative — run dotnet directly** (if you prefer not to use the script):

```bash
cd ../my-blog
dotnet run --project ../happyfrogger -- new "My First Post" -c tech  # scaffold
dotnet run --project ../happyfrogger                                  # build
dotnet run --project ../happyfrogger -- --serve                       # build + watch
```

> Tip: `alias hf="dotnet run --project /absolute/path/to/happyfrogger --"` from your blog directory so the commands read `hf new …`, `hf --serve`, `hf`.

---

## Writing

### A post

```markdown
---
title: "Why I switched editors"
date: 2025-06-01
category: tech          # tech | faith | creative
subcategory: tooling    # optional — shows as a tag / filter chip
description: "A short, honest account."
slug: why-i-switched-editors
status: published       # or "draft" to exclude from builds
toc: true               # optional override; default from config
---

Your Markdown here…
```

`hf new "Title" -c tech` writes this front matter for you (as a draft) so you can jump straight to writing.

### Categories

Three sections, each with its own colour mood on the site: `tech`, `faith`, `creative`. Titles/descriptions live in `CategoryInfo` (`Models/CategoryPageModel.cs`).

### Embedding Gists

```markdown
[gist:GIST_ID]
```

### Tables of contents

If a post has at least `toc.minHeadings` headings, a contents list is generated automatically and rendered in the article's sticky side rail. Override per-post with `toc: true|false`.

---

## Books

Books are more than a folder of posts. Each book has a **`book.json` manifest** that defines its Parts, epigraphs, and chapter order — including chapters you haven't written yet (they render as dimmed *Coming soon* rows).

### 1. Configure the book in `happyfrog.config.json`

Paths are relative to your **blog directory** (where you run the tool from).

```json
"books": [
  {
    "id": "my-book",
    "title": "My Book Title",
    "path": "src/books/my-book",
    "outputPath": "books/my-book",
    "coverImage": "books/my-book/cover.png",
    "category": "faith"
  },
  { "id": "future-book", "title": "Future Book", "category": "faith", "planned": true }
]
```

- `path` — where the markdown source lives (relative to blog dir, e.g. inside `src/books/`)
- `outputPath` — where the generated HTML is written (relative to blog dir)
- `coverImage` — path to cover image relative to the output root; rendered on the landing page. Omit to use the CSS placeholder.
- A `"planned": true` entry has no folder yet and appears on the **shelf** of other books.

### 2. Add a `book.json` to the book folder

```json
{
  "id": "biblical-understanding",
  "title": "A Dad's Memoir of Biblical Truth",
  "subtitle": "A journey through Scripture to equip the everyday man.",
  "category": "faith",
  "status": "In progress",
  "totalChapters": 16,
  "parts": [
    {
      "label": "Part I",
      "title": "Understanding the Foundation",
      "epigraph": "Getting oriented with God's Word",
      "chapters": ["the-absent-man", "unlocking-your-bible-understanding", "gods-programme", "why-bible-versions-matter"]
    },
    {
      "label": "Part II",
      "title": "The Journey of Salvation",
      "chapters": [
        { "slug": "the-gospel-message", "title": "The Gospel Message", "description": "What the good news actually is." }
      ]
    }
  ]
}
```

- `totalChapters` — optional. Sets the denominator for the progress bar (`available / totalChapters`). Useful when you haven't listed all planned chapters in `parts[]` yet. Omit to derive the total from the manifest automatically.
- A chapter is **available** when a published `.md` with that slug exists in the folder; otherwise it shows as *Coming soon* using the manifest's title/description.

### 3. Write chapters

Ordinary posts in the book folder, with `subcategory: book` and a `chapter_number`:

```markdown
---
title: "The Absent Man"
date: 2025-01-04
category: faith
subcategory: book
slug: the-absent-man
status: published
chapter_number: 1
next_chapter: unlocking-your-bible-understanding
---
```

A complete, runnable example (manifest + four written chapters + two coming-soon) lives in [`example/`](example/).

---

## Project structure

```
happyfrogger/               ← engine (this repo)
├── build.sh                 # wrapper: resolves blog dir, runs Tailwind + dotnet
├── Templates/
│   ├── LandingTemplate.html
│   ├── BlogTemplate.html
│   ├── BookIndexTemplate.html
│   ├── CategoryTemplate.html
│   └── assets/
│       ├── theme.css        # the whole design system (light + dark)
│       └── theme.js         # theme toggle, reading progress, active TOC
├── Models/                  # C# data + view models
├── Program.cs               # build pipeline + CLI
├── PostProcessor.cs         # Markdown → HTML, front matter, TOC, reading time
├── BookProcessor.cs         # book.json → parts, chapters, shelf
├── PageGenerator.cs         # Scriban rendering
├── AssetGenerator.cs        # copies theme assets, processes images, RSS, sitemap
├── ImageProcessor.cs        # resizes/compresses images via ImageSharp
└── Commands.cs              # `hf new`

my-blog/                    ← content (separate repo)
├── happyfrog.config.json    # site config — lives here, not in the engine
├── src/
│   ├── posts/               # blog post markdown files
│   └── books/
│       └── my-book/         # book folder (path set in config)
│           ├── book.json    # manifest: parts, chapters, totalChapters
│           └── chapter-01.md
├── images/                  # source images (optimised on build if enabled)
├── books/                   # generated book HTML + cover images (outputPath)
└── (*.html, index.html…)    # generated site output (outputPath: ".")
```

Templates are [Scriban](https://github.com/scriban/scriban). Model properties map to `snake_case` in templates (`PublishDate` → `publish_date`).

---

## Theming

The site ships light and dark. `theme.js` respects the OS preference on first visit, remembers the visitor's choice (`localStorage`), and a no-flash snippet in each template `<head>` sets the theme before first paint. To retune colours, edit the CSS custom properties at the top of `theme.css` — the section moods (`--tech`, `--faith`, `--creative`) and the warm accent are all there.

---

## Configuration

`happyfrog.config.json` lives in your **content directory** (not the engine repo). All paths are relative to that directory. See [`example/happyfrog.config.json`](example/happyfrog.config.json) for a complete file.

| Key | Purpose |
|---|---|
| `markdownFilesPath` | path to markdown source files, relative to blog dir (e.g. `"src/posts"`) |
| `outputPath` | where generated HTML is written (e.g. `"."` for blog root) |
| `templatesPath` | resolved from the executable — leave as `"Templates"` |
| `site.{title,description,author,baseUrl}` | metadata + masthead copy |
| `build.categories` | the sections to generate |
| `build.includeDrafts` | include `status: draft` posts (or pass `--drafts`) |
| `build.toc` | TOC thresholds and title |
| `build.images.enabled` | turn image optimisation on/off |
| `build.images.sourcePath` | folder of original images |
| `build.images.maxWidth` | images wider than this are resized down (default 1200) |
| `build.images.quality` | JPEG/WebP compression quality, 1–100 (default 85) |
| `books[]` | book folders + planned future books |
| `books[].coverImage` | path to cover image relative to output root |

---

## CLI

Run from the **happyfrogger directory** using `build.sh`, or from your **blog directory** using `dotnet run --project`:

| Command | Does |
|---|---|
| `./build.sh` | build the site (blog dir defaults to `../blog`) |
| `./build.sh --blog /path/to/blog` | build with an explicit blog directory |
| `./build.sh -- --serve [--port 4000]` | build + watch + live-reload preview |
| `./build.sh -- --drafts` | include drafts in the build |
| `dotnet run --project ../happyfrogger` | build (run from blog directory) |
| `dotnet run --project ../happyfrogger -- --serve` | build + watch (run from blog directory) |
| `dotnet run --project ../happyfrogger -- new "Title" -c tech [-s sub] [--book id]` | scaffold a draft post or book chapter |

---

## Dependencies

- [Markdig](https://github.com/xoofx/markdig) — Markdown
- [Scriban](https://github.com/scriban/scriban) — templating
- [YamlDotNet](https://github.com/aaubry/YamlDotNet) — front matter
- [SixLabors.ImageSharp](https://github.com/SixLabors/ImageSharp) — image resizing and compression

Tailwind and Node are **no longer required** — delete `input.css`, `output.css`, `tailwind.config.js`, and the npm `*:css` scripts.

## License

MIT.
