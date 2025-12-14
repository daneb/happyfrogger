# ADR 005: Table of Contents Auto-Generation

**Status:** Accepted
**Date:** 2025-12-14
**Version:** 2.5.0

## Context

Long-form blog posts benefit from navigation aids that help readers quickly jump to sections of interest. A table of contents (TOC) improves user experience by providing an overview of post structure and enabling quick navigation to specific sections.

## Decision

Implement automatic table of contents generation with the following approach:

### Configuration
- Add `TocOptions` to `HappyFrogConfig.cs` with:
  - `EnabledByDefault`: Global toggle (default: true)
  - `MinHeadings`: Minimum headings required to generate TOC (default: 3)
  - `MaxLevel`: Maximum heading level to include (default: 3, meaning H2-H3)
  - `Title`: TOC section title (default: "Table of Contents")

### Per-Post Control
- Add optional `toc: true|false` field in YAML front matter
- Per-post setting overrides global default
- Allows flexibility for posts that don't need TOC

### Implementation
1. **Markdig Pipeline**: Use `UseAutoIdentifiers(AutoIdentifierOptions.GitHub)` to add URL-friendly IDs to all headings
2. **TOC Generation**: Parse rendered HTML using regex to extract headings with IDs
3. **HTML Structure**: Generate semantic `<nav>` element with nested `<ul>` for hierarchy
4. **Styling**: Add clean, accessible CSS with left border accent and hover effects

### Technical Details
- Only generate TOC if heading count meets `MinHeadings` threshold
- Support heading levels H2-H6 (H1 reserved for post title)
- Generate GitHub-style kebab-case IDs (e.g., "currently-reading")
- Strip HTML tags from heading text for TOC links
- Support nested heading structure with proper indentation

## Consequences

### Positive
- Improves navigation for long posts
- Automatic generation reduces manual maintenance
- Per-post control provides flexibility
- Semantic HTML enhances accessibility
- GitHub-compatible anchor IDs for consistent URL fragments

### Negative
- Adds processing overhead for each post
- Regex-based parsing could be fragile for complex HTML
- TOC appears for all posts meeting minimum heading count (may not always be desired)

### Risks Mitigated
- Minimum heading threshold prevents TOC on short posts
- Per-post override allows disabling when needed
- Empty TOC check prevents rendering empty navigation

## Alternatives Considered

1. **Client-side JavaScript TOC**: Would reduce build time but requires JavaScript enabled
2. **Manual TOC in Markdown**: More control but requires maintenance
3. **Fixed TOC Position**: Could use sticky positioning but opted for inline to maintain simplicity

## Notes

- AutoIdentifiers must be applied during initial markdown-to-HTML conversion (not on pre-converted HTML)
- Previous double-conversion bug was fixed by consolidating to single `Markdown.ToHtml()` call with pipeline
- TOC rendered before post content for logical reading order
