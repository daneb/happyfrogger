# HappyFrog 🐸

A lightweight static site generator in C# / .NET 8. Write Markdown with front matter, run one command, get a fast, warm, reading-first site — with first-class support for **books** (grouped into Parts, with coming-soon chapters and a multi-book shelf) and light/dark themes.

No Node. No Tailwind. No build step for CSS — the design system is one hand-authored `theme.css`.

---

## Quick start

```bash
dotnet build

# scaffold a new draft and start writing
dotnet run -- new "My First Post" -c tech

# build the site
dotnet run

# build + live-reload preview at http://localhost:4000
dotnet run -- --serve
```

Generated HTML lands in your configured `outputPath`. Deploy that folder anywhere static (GitHub Pages, Netlify, S3, …).

> Tip: alias `hf="dotnet run --"` so the commands read `hf new …`, `hf serve`, `hf`.

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

```json
"books": [
  {
    "id": "biblical-understanding",
    "title": "A Dad's Memoir of Biblical Truth",
    "path": "../blog/books/biblical-understanding",
    "outputPath": "../blog/books/biblical-understanding",
    "category": "faith"
  },
  { "id": "untitled-next", "title": "Untitled (next book)", "category": "faith", "planned": true }
]
```

A `"planned": true` entry has no folder yet and appears on the **shelf** of other books.

### 2. Add a `book.json` to the book folder

```json
{
  "id": "biblical-understanding",
  "title": "A Dad's Memoir of Biblical Truth",
  "subtitle": "A journey through Scripture to equip the everyday man.",
  "category": "faith",
  "status": "In progress",
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

A chapter is **available** when a published `.md` with that slug exists in the folder; otherwise it shows as *Coming soon* using the manifest's title/description. Progress is computed as `available / total`.

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
HappyFrog/
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
├── AssetGenerator.cs        # copies theme assets + RSS + sitemap
├── Commands.cs              # `hf new`
└── happyfrog.config.json
```

Templates are [Scriban](https://github.com/scriban/scriban). Model properties map to `snake_case` in templates (`PublishDate` → `publish_date`).

---

## Theming

The site ships light and dark. `theme.js` respects the OS preference on first visit, remembers the visitor's choice (`localStorage`), and a no-flash snippet in each template `<head>` sets the theme before first paint. To retune colours, edit the CSS custom properties at the top of `theme.css` — the section moods (`--tech`, `--faith`, `--creative`) and the warm accent are all there.

---

## Configuration

`happyfrog.config.json` controls paths, site metadata, categories, books, RSS, sitemap, and TOC behaviour. See [`example/happyfrog.config.json`](example/happyfrog.config.json) for a complete file.

| Key | Purpose |
|---|---|
| `markdownFilesPath` / `outputPath` / `templatesPath` | where content, output, and templates live |
| `site.{title,description,author,baseUrl}` | metadata + masthead copy |
| `build.categories` | the sections to generate |
| `build.includeDrafts` | include `status: draft` posts (or pass `--drafts`) |
| `build.toc` | TOC thresholds and title |
| `books[]` | book folders + planned future books |

---

## CLI

| Command | Does |
|---|---|
| `dotnet run` | build the site |
| `dotnet run -- --serve [--port 4000]` | build + watch + live-reload preview |
| `dotnet run -- --drafts` | include drafts in the build |
| `dotnet run -- new "Title" -c tech [-s sub] [--book id]` | scaffold a draft post (or book chapter) |

---

## Dependencies

- [Markdig](https://github.com/xoofx/markdig) — Markdown
- [Scriban](https://github.com/scriban/scriban) — templating
- [YamlDotNet](https://github.com/aaubry/YamlDotNet) — front matter

Tailwind and Node are **no longer required** — delete `input.css`, `output.css`, `tailwind.config.js`, and the npm `*:css` scripts.

## License

MIT.
