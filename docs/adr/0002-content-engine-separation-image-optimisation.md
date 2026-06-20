# ADR-0002: Content/Engine Separation, Image Optimisation, and Book Enhancements

**Date:** 2026-06-20
**Status:** Accepted
**Deciders:** Dane Balia

---

## Context

Three unrelated pain points motivated this set of changes:

1. **`happyfrog.config.json` lived inside the engine repo.** This forced site-specific configuration (paths, book IDs, author details) to be committed alongside generator code, making it impossible to share the engine without leaking personal site config, and creating confusion about which of the multiple copies of the file was canonical.

2. **Images were not processed during the build.** Large, unoptimised images were served directly from the output directory. There was no resize or compression step.

3. **Book progress always showed 100%** when every chapter listed in `book.json` had a corresponding markdown file. The `totalChapters` denominator was derived entirely from the manifest, giving no way to express "I have written 3 of a planned 16 chapters."

4. **Book cover images were never rendered.** `BookConfig.CoverImage` existed in the model but was never passed to the landing page view model or template — the landing page always rendered a CSS-styled placeholder div.

---

## Decisions

### 1. Move `happyfrog.config.json` to the Content Repository

**Chosen:** `happyfrog.config.json` lives in the content (blog) repository root, not in the engine repo. The engine's copy is gitignored. The tool is run from the content directory, where `LoadConfiguration` already checks the working directory first.

```
blog/
  happyfrog.config.json    ← site config lives here
  markdownfiles/
  images/
  books/
```

```bash
# from the blog directory
dotnet run --project ../happyfrogger
```

**Motivation:** The engine (`happyfrogger`) and the content (`blog`) have different change rates and different audiences. Coupling them via a committed config file required workarounds (multiple copies, relative path hacks). Separation makes `happyfrogger` a reusable tool with no embedded site assumptions.

**Consequence:** All paths in the config are now relative to the blog directory. `templatesPath` continues to resolve from the assembly location, so it still finds the bundled `Templates/` folder regardless of working directory.

**`.csproj` change:** Removed the `<None Update="happyfrog.config.json">` item — there is no longer a config to copy to the output directory.

---

### 2. Image Optimisation Pipeline

**Chosen:** `SixLabors.ImageSharp 3.1.6` processes images during the build step. Configuration is via a new `build.images` section:

```json
"images": {
  "enabled": true,
  "sourcePath": "images",
  "outputSubPath": "images",
  "maxWidth": 1200,
  "quality": 85
}
```

`ImageProcessor.ProcessImages()` is called from `AssetGenerator.Generate()`. For each supported image (`.jpg`, `.jpeg`, `.png`, `.webp`):
- If `width <= maxWidth`: copied as-is (or skipped when source = destination).
- If `width > maxWidth`: resized proportionally and saved with the configured quality.

**Same-directory handling:** When `sourcePath` and the computed destination resolve to the same file (source and output are the same directory), copying to self would fail. The processor detects this via `Path.GetFullPath` comparison. Images within the size limit are skipped entirely; images needing a resize are written to a `.tmp` file first, then atomically moved over the original. The encoder is selected from the original file's extension, not the `.tmp` suffix.

**Alternatives considered:**
- **External tooling (`squoosh-cli`, `sharp`)** — Would keep the engine dependency-free but adds a Node.js or separate binary requirement. Conflicts with the project's single-runtime goal.
- **WebP conversion** — Considered but deferred; browser support is now universal but changing filenames would break existing markdown image references.

**Unsupported formats:** GIF files are not processed by ImageSharp in this pipeline (animated GIF support requires additional handling). Files with unrecognised extensions are ignored.

---

### 3. `totalChapters` Override in `book.json`

**Chosen:** An optional `totalChapters` field in `book.json` overrides the auto-counted total used for the progress calculation.

```json
{
  "totalChapters": 16
}
```

Progress is then `available / totalChapters * 100`. If omitted, the denominator is the number of chapters listed in `parts[]` (prior behaviour).

**Motivation:** A book author may plan 16 chapters but only list 3 in the manifest so far. Without this override, progress reports 100% whenever all listed chapters are written. `ChaptersTotal` in the view model also reflects the configured value so the display is consistent.

**Alternative considered:** `completionPercentage` — a direct percentage override. Rejected because it is static; `totalChapters` keeps the progress dynamic as chapters are published.

---

### 4. Book Cover Image on the Landing Page

**Chosen:** `CoverImage` from `BookConfig` is now propagated to `FeaturedBook` (landing page view model) and rendered as an `<img>` when set. The CSS placeholder div is retained as a fallback.

```html
{{ if featured_book.cover_image != "" }}
<img src="{{ featured_book.cover_image }}" ...>
{{ else }}
<div class="book-cover">...</div>
{{ end }}
```

**Root cause:** `CoverImage` was defined in `HappyFrogConfig.BookConfig` but never mapped into any view model. `FeaturedBook` gained a `CoverImage` property; `Program.cs` passes `featured.Config.CoverImage` when building the landing model.

---

## Consequences

**Positive:**
- Engine repository contains no site-specific data; can be shared or published without sanitisation.
- Large images are automatically downscaled at build time without manual intervention.
- Book progress is honest — reflects planned scope, not just what has been written so far.
- Book cover images render on the landing page.

**Negative / watch items:**
- Users upgrading from a setup where `happyfrog.config.json` lived in the engine directory must move the file to their content directory and adjust all paths to be content-relative.
- Image processing adds build time proportional to the number of oversized images. No caching between builds — every build re-evaluates all images (acceptable at current content scale).
- `CoverImage` path in `BookConfig` is relative to the output root, not the book folder. Authors must use the full relative path (e.g. `books/biblical-understanding/cover.png`).
