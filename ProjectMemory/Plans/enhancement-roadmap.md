# HappyFrogger Enhancement Roadmap

**Version**: 2.0.0 → 3.0.0
**Created**: 2025-12-14
**Status**: Planning

---

## Overview

This roadmap outlines the phased implementation of key features to enhance HappyFrogger's capabilities, bringing it in line with modern static site generators while maintaining its simplicity and C# foundation.

---

## Phase 1: RSS Feed Generation 📡

**Priority**: CRITICAL
**Estimated Effort**: Low-Medium
**Target Version**: 2.1.0

### Goals
- Generate RSS 2.0 compliant feed at `/feed.xml` or `/rss.xml`
- Include all published posts (not drafts)
- Support full content or excerpt-based feeds

### Technical Requirements
1. Create `RssFeedGenerator` class in Models or separate namespace
2. XML generation using `System.Xml.Linq` (XDocument)
3. Configuration options in `happyfrog.config.json`:
   - `feedEnabled` (bool, default: true)
   - `feedPath` (string, default: "feed.xml")
   - `feedItemCount` (int, default: 20, 0 = all)
   - `feedFullContent` (bool, default: true)

### Implementation Tasks
- [ ] Create RSS feed model/generator class
- [ ] Add RSS configuration to HappyFrogConfig
- [ ] Generate feed.xml in build pipeline (after posts are processed)
- [ ] Add `<link rel="alternate">` to all HTML templates
- [ ] Test feed with RSS validator
- [ ] Update documentation

### RSS Feed Structure
```xml
<?xml version="1.0" encoding="UTF-8"?>
<rss version="2.0">
  <channel>
    <title>Site Title</title>
    <link>Base URL</link>
    <description>Site Description</description>
    <lastBuildDate>RFC-822 date</lastBuildDate>
    <item>
      <title>Post Title</title>
      <link>Full URL to post</link>
      <description>Content or excerpt</description>
      <pubDate>RFC-822 date</pubDate>
      <guid>Full URL to post</guid>
      <category>Category Name</category>
    </item>
  </channel>
</rss>
```

### Success Criteria
- ✓ Valid RSS 2.0 feed generated
- ✓ Feed includes last 20 posts (or all if < 20)
- ✓ Feed validates at https://validator.w3.org/feed/
- ✓ Feed accessible at configured path
- ✓ All templates link to feed

---

## Phase 2: Reading Time Calculation ⏱️

**Priority**: HIGH
**Estimated Effort**: Very Low
**Target Version**: 2.2.0

### Goals
- Calculate and display estimated reading time for each post
- Show reading time in post headers and category listings
- Use industry-standard calculation (200 words/minute average)

### Technical Requirements
1. Add `ReadingTimeMinutes` property to `BlogPostModel`
2. Calculate during post processing (before rendering)
3. Word count from plain text (strip HTML)
4. Display in templates with appropriate formatting

### Implementation Tasks
- [ ] Add `CalculateReadingTime(string content)` helper method in Program.cs
- [ ] Add `ReadingTimeMinutes` to BlogPostModel
- [ ] Calculate during post creation (strip HTML tags, count words)
- [ ] Update BlogTemplate.cshtml to display reading time
- [ ] Update CategoryTemplate.cshtml to display reading time in listings
- [ ] Update LandingTemplate.cshtml to display reading time
- [ ] Add configuration option for words-per-minute (default: 200)

### Calculation Logic
```csharp
// Strip HTML tags
var plainText = Regex.Replace(htmlContent, @"<[^>]+>", "");
// Count words
var wordCount = plainText.Split(new[] { ' ', '\t', '\n', '\r' },
                StringSplitOptions.RemoveEmptyEntries).Length;
// Calculate minutes (round up)
var minutes = (int)Math.Ceiling(wordCount / (double)wordsPerMinute);
```

### Display Format
- "5 min read" for posts 5 minutes or longer
- "1 min read" for short posts
- Optional: "< 1 min read" for very short posts

### Success Criteria
- ✓ Reading time displayed on all post pages
- ✓ Reading time shown in category listings
- ✓ Reading time shown on landing page
- ✓ Configurable words-per-minute setting
- ✓ Accurate calculations tested with sample posts

---

## Phase 3: Social Media Meta Tags 📱

**Priority**: HIGH
**Estimated Effort**: Low
**Target Version**: 2.3.0

### Goals
- Add Open Graph (OG) tags for Facebook, LinkedIn, etc.
- Add Twitter Card tags for rich Twitter previews
- Support custom images per post (optional)
- Improve social media sharing experience

### Technical Requirements
1. Add meta tags to all Razor templates
2. Support post-specific social images (from front matter)
3. Fallback to site-wide defaults
4. Configuration in `happyfrog.config.json` for defaults

### Implementation Tasks
- [ ] Add social media configuration to HappyFrogConfig:
  - `defaultSocialImage` (URL)
  - `twitterHandle` (optional)
  - `facebookAppId` (optional)
- [ ] Add `socialImage` to FrontMatter (optional per-post override)
- [ ] Create partial Razor template for meta tags
- [ ] Update BlogTemplate.cshtml with OG/Twitter tags
- [ ] Update LandingTemplate.cshtml with site-level tags
- [ ] Update CategoryTemplate.cshtml with category-level tags
- [ ] Test with social media validators

### Meta Tags to Include

**Open Graph (Facebook, LinkedIn)**:
```html
<meta property="og:title" content="Post Title" />
<meta property="og:description" content="Post Description" />
<meta property="og:type" content="article" />
<meta property="og:url" content="Full URL" />
<meta property="og:image" content="Image URL" />
<meta property="og:site_name" content="Site Title" />
<meta property="article:published_time" content="ISO 8601 date" />
<meta property="article:author" content="Author Name" />
<meta property="article:section" content="Category" />
```

**Twitter Cards**:
```html
<meta name="twitter:card" content="summary_large_image" />
<meta name="twitter:title" content="Post Title" />
<meta name="twitter:description" content="Post Description" />
<meta name="twitter:image" content="Image URL" />
<meta name="twitter:creator" content="@handle" />
```

### Front Matter Addition
```yaml
socialImage: "https://example.com/images/post-image.jpg"  # optional
```

### Success Criteria
- ✓ OG tags present on all pages
- ✓ Twitter Card tags present on all pages
- ✓ Per-post image overrides work
- ✓ Validates at https://cards-dev.twitter.com/validator
- ✓ Validates at https://developers.facebook.com/tools/debug/
- ✓ Beautiful previews when sharing links

---

## Phase 4: Sitemap.xml Generation 🗺️

**Priority**: CRITICAL (SEO)
**Estimated Effort**: Low
**Target Version**: 2.4.0

### Goals
- Generate XML sitemap for search engine crawlers
- Include all pages (posts, categories, landing page)
- Support priority and change frequency hints
- Auto-update on each build

### Technical Requirements
1. Create `SitemapGenerator` class
2. XML generation using `System.Xml.Linq`
3. Include all generated HTML files
4. Configuration for sitemap customization

### Implementation Tasks
- [ ] Create SitemapGenerator class
- [ ] Add sitemap configuration to HappyFrogConfig:
  - `sitemapEnabled` (bool, default: true)
  - `sitemapPath` (string, default: "sitemap.xml")
  - `defaultChangeFrequency` (string, default: "weekly")
  - `defaultPriority` (decimal, default: 0.5)
- [ ] Generate sitemap.xml in build pipeline (after all pages generated)
- [ ] Add `<link rel="sitemap">` to templates
- [ ] Test with sitemap validator
- [ ] Update documentation
- [ ] Add robots.txt reference to sitemap

### Sitemap Structure
```xml
<?xml version="1.0" encoding="UTF-8"?>
<urlset xmlns="http://www.sitemaps.org/schemas/sitemap/0.9">
  <url>
    <loc>https://example.com/</loc>
    <lastmod>2025-12-14</lastmod>
    <changefreq>daily</changefreq>
    <priority>1.0</priority>
  </url>
  <url>
    <loc>https://example.com/post-slug.html</loc>
    <lastmod>2025-12-14</lastmod>
    <changefreq>monthly</changefreq>
    <priority>0.8</priority>
  </url>
  <url>
    <loc>https://example.com/tech.html</loc>
    <lastmod>2025-12-14</lastmod>
    <changefreq>weekly</changefreq>
    <priority>0.7</priority>
  </url>
</urlset>
```

### Priority Scheme
- Landing page: 1.0
- Category pages: 0.7
- Recent posts (< 30 days): 0.8
- Older posts: 0.6

### Change Frequency
- Landing page: daily
- Category pages: weekly
- Posts: monthly

### Additional: robots.txt
Create `robots.txt` in output:
```
User-agent: *
Allow: /
Sitemap: https://yourdomain.com/sitemap.xml
```

### Success Criteria
- ✓ Valid XML sitemap generated
- ✓ Includes all pages (posts, categories, landing)
- ✓ Validates at https://www.xml-sitemaps.com/validate-xml-sitemap.html
- ✓ robots.txt references sitemap
- ✓ Configurable priorities and frequencies
- ✓ baseUrl required in config (validation check)

---

## Phase 5: Table of Contents Auto-Generation 📑

**Priority**: MEDIUM
**Estimated Effort**: Medium
**Target Version**: 2.5.0

### Goals
- Automatically generate table of contents from markdown headers
- Insert TOC at configurable location (start of post or custom marker)
- Sticky TOC for longer posts (optional)
- Anchor links for easy navigation

### Technical Requirements
1. Parse markdown headers (H2, H3, H4) before HTML conversion
2. Generate nested TOC structure
3. Add anchor IDs to headers during HTML generation
4. Insert TOC in rendered HTML
5. Configuration per-post and globally

### Implementation Tasks
- [ ] Add TOC configuration to HappyFrogConfig:
  - `tocEnabled` (bool, default: false)
  - `tocMinHeadings` (int, default: 3) - minimum headings to show TOC
  - `tocDepth` (int, default: 3) - how many heading levels to include
  - `tocPosition` (string: "top" | "marker", default: "top")
- [ ] Add `toc` to FrontMatter (bool, optional per-post override)
- [ ] Create `TableOfContentsGenerator` class
- [ ] Parse markdown for headers before HTML conversion
- [ ] Generate TOC HTML with nested lists
- [ ] Configure Markdig to add header IDs
- [ ] Insert TOC marker support: `[TOC]` or `[[TOC]]`
- [ ] Update BlogTemplate.cshtml with TOC display
- [ ] Add CSS for TOC styling (sticky option)
- [ ] Test with various post lengths

### Header Parsing Logic
```csharp
// Using Markdig pipeline with HeadingIdExtension
var pipeline = new MarkdownPipelineBuilder()
    .UseYamlFrontMatter()
    .UseAdvancedExtensions()
    .UseAutoIdentifiers() // Adds IDs to headings
    .Build();
```

### TOC HTML Structure
```html
<nav class="table-of-contents">
  <h2>Table of Contents</h2>
  <ul>
    <li><a href="#heading-1">First Section</a></li>
    <li><a href="#heading-2">Second Section</a>
      <ul>
        <li><a href="#heading-2-1">Subsection</a></li>
      </ul>
    </li>
  </ul>
</nav>
```

### Front Matter Addition
```yaml
toc: true  # Enable TOC for this post (overrides global setting)
```

### Configuration Options
```json
{
  "build": {
    "tableOfContents": {
      "enabled": false,
      "minHeadings": 3,
      "depth": 3,
      "position": "top"
    }
  }
}
```

### Styling Options
- Inline TOC at top of post
- Sticky sidebar TOC (desktop)
- Collapsible TOC (mobile)
- Highlight current section on scroll (advanced)

### Success Criteria
- ✓ TOC generated for posts with sufficient headings
- ✓ Anchor links work correctly
- ✓ Nested structure reflects heading hierarchy
- ✓ Per-post override via front matter
- ✓ TOC marker `[TOC]` support
- ✓ Responsive design (mobile/desktop)
- ✓ Optional sticky positioning

---

## Post-Phase 5: Future Considerations

After completing Phases 1-5, consider these next priorities:

### Tier 2 Features (Version 3.x)
1. **Dark Mode Toggle** (2.6.0)
2. **Tag System** (2.7.0)
3. **Related Posts** (2.8.0)
4. **Search (JSON Index)** (2.9.0)
5. **Watch Mode** (2.10.0)

### Tier 3 Features (Version 4.x)
6. Pagination
7. Archive Pages
8. Image Optimization
9. Shortcodes System
10. Series/Collections

---

## Version Numbering Strategy

- **2.1.x** - RSS Feed
- **2.2.x** - Reading Time
- **2.3.x** - Social Meta Tags
- **2.4.x** - Sitemap
- **2.5.x** - Table of Contents
- **3.0.0** - Major release with all Phase 1-5 features complete

---

## Configuration File Evolution

As features are added, the `happyfrog.config.json` will grow:

```json
{
  "version": "3.0.0",
  "markdownFilesPath": "MarkdownFiles",
  "outputPath": "Output",
  "templatesPath": "Templates",

  "site": {
    "title": "HappyFrogger Blog",
    "description": "A lightweight static site generator",
    "author": "Author Name",
    "baseUrl": "https://yourdomain.com",
    "defaultSocialImage": "https://yourdomain.com/default-og.jpg",
    "twitterHandle": "@yourhandle"
  },

  "build": {
    "generateCategoryPages": true,
    "generateLandingPage": true,
    "htmlExtension": ".html",
    "includeDrafts": false,
    "categories": ["tech", "faith", "creative"],
    "wordsPerMinute": 200,

    "rss": {
      "enabled": true,
      "path": "feed.xml",
      "itemCount": 20,
      "fullContent": true
    },

    "sitemap": {
      "enabled": true,
      "path": "sitemap.xml",
      "changeFrequency": "weekly",
      "priority": 0.5
    },

    "tableOfContents": {
      "enabled": false,
      "minHeadings": 3,
      "depth": 3,
      "position": "top"
    }
  }
}
```

---

## Testing Strategy

For each phase:
1. **Unit Tests** - Test core functionality in isolation
2. **Integration Tests** - Test full build pipeline
3. **Validation** - Use external validators (RSS, sitemap, social cards)
4. **Manual Testing** - Visual inspection of output
5. **Documentation** - Update README.md and CHANGELOG.md

---

## Documentation Updates Required

After each phase:
- [ ] Update README.md with new features
- [ ] Update CHANGELOG.md with version history
- [ ] Update happyfrog.config.json with new options
- [ ] Add inline code comments for new functionality
- [ ] Consider creating wiki pages for advanced features

---

## Breaking Changes Considerations

**Minimal Breaking Changes Expected**:
- All features are additive
- Configuration has sensible defaults
- Existing builds should work without modification

**Potential Breaking Changes**:
- Phase 4 (Sitemap) requires `baseUrl` to be set (validation added)
- Configuration file structure may evolve (backwards compatible)

---

## Success Metrics

By version 3.0.0, HappyFrogger should:
- ✓ Generate valid RSS feeds
- ✓ Display reading times on all posts
- ✓ Include proper social media meta tags
- ✓ Generate search engine sitemaps
- ✓ Auto-generate table of contents
- ✓ Maintain sub-second build times for < 100 posts
- ✓ Pass all W3C validators
- ✓ Have comprehensive documentation

---

## Timeline Estimate

**Aggressive**: 1-2 weeks (all phases)
**Moderate**: 3-4 weeks (one phase per week)
**Relaxed**: 1-2 months (as needed)

---

## Notes

- Each phase builds on the previous
- Phases can be implemented in order or cherry-picked
- All features maintain the simplicity and C# foundation
- Configuration-driven approach allows easy feature toggling
- Zero external service dependencies (all generated at build time)

---

**Next Steps**:
1. Review and approve roadmap
2. Start Phase 1: RSS Feed Generation
3. Iterate through phases sequentially
