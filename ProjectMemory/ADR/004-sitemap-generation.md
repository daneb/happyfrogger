# ADR 004: Sitemap.xml Generation

**Status**: Accepted
**Date**: 2025-12-14
**Deciders**: Development Team
**Technical Story**: Phase 4 - Sitemap.xml Generation (v2.4.0)

---

## Context and Problem Statement

Search engines need a sitemap to efficiently discover and index all pages on a website. Without a sitemap, search engines may miss pages or take longer to index new content. We needed to generate a valid XML sitemap that includes all blog posts and static pages.

---

## Decision Drivers

- Must be valid XML sitemap protocol 0.9
- Should include all pages (posts, categories, landing)
- Must support priority and change frequency hints
- Should generate robots.txt with sitemap reference
- Must require baseUrl configuration (absolute URLs needed)
- Should prioritize recent content

---

## Considered Options

### Option 1: Manual XML Generation with System.Xml.Linq (Chosen)
- **Pros**: No dependencies, full control, simple spec
- **Cons**: Manual XML structure

### Option 2: Third-Party Sitemap Library
- **Rejected**: Adds dependency, spec is simple enough

### Option 3: Generate from File System Scan
- **Rejected**: Doesn't have access to post metadata (dates, priorities)

---

## Decision Outcome

**Chosen**: Manual XML generation using System.Xml.Linq

### Implementation

**SitemapGenerator Class**:
- Generates sitemap.xml with all pages
- Calculates dynamic priority based on post age
- Generates robots.txt with sitemap reference
- Uses W3C datetime format (ISO 8601)

**Priority Scheme**:
- Landing page: 1.0 (highest)
- Category pages: 0.7
- Recent posts (< 30 days): 0.8
- Medium age (30-90 days): 0.7
- Older posts (> 90 days): 0.6

**Change Frequency**:
- Landing page: daily
- Category pages: weekly
- Recent posts (< 30 days): weekly
- Older posts: monthly

**Configuration**:
```json
{
  "sitemap": {
    "enabled": true,
    "path": "sitemap.xml",
    "changeFrequency": "weekly",
    "defaultPriority": 0.5
  }
}
```

---

## Consequences

### Positive
- ✅ Better SEO and search engine indexing
- ✅ Faster discovery of new content
- ✅ Dynamic priorities favor recent posts
- ✅ Valid sitemap protocol 0.9
- ✅ Includes robots.txt with sitemap reference
- ✅ No external dependencies

### Negative
- ⚠️ Requires baseUrl to be set (throws error if missing)
- ⚠️ Static priorities (could be more sophisticated)
- ⚠️ No image sitemap support
- ⚠️ No video sitemap support
- ⚠️ Regenerated on every build (no incremental updates)

### Neutral
- 📊 Adds ~140 lines of code (SitemapGenerator class)
- 📊 Adds 4 configuration options
- 📊 Generates 2 files (sitemap.xml + robots.txt)

---

## Files Changed

- `Models/HappyFrogConfig.cs` - Added SitemapOptions
- `SitemapGenerator.cs` - New generator class
- `Program.cs` - Integrated sitemap and robots.txt generation

---

## XML Structure

```xml
<?xml version="1.0" encoding="UTF-8"?>
<urlset xmlns="http://www.sitemaps.org/schemas/sitemap/0.9">
  <url>
    <loc>https://example.com/</loc>
    <lastmod>2025-12-14T12:00:00Z</lastmod>
    <changefreq>daily</changefreq>
    <priority>1.0</priority>
  </url>
  <!-- More URLs... -->
</urlset>
```

---

## Validation

**Testing Methods**:
- XML Sitemap Validator: https://www.xml-sitemaps.com/validate-xml-sitemap.html
- Google Search Console: Submit sitemap
- Bing Webmaster Tools: Submit sitemap

**Expected Results**:
- Valid XML structure
- All pages included
- Proper W3C datetime format
- robots.txt references sitemap

---

## Future Improvements

- Support sitemap index for large sites (> 50,000 URLs)
- Add image sitemap entries
- Add video sitemap entries
- Support lastmod from file modification time
- Add sitemap compression (.xml.gz)
- Submit to search engines via API

---

## Notes

- Sitemap spec limits: 50,000 URLs and 50MB per file
- Current implementation handles both constraints easily
- robots.txt placed at site root (critical for crawlers)
- W3C datetime format: `YYYY-MM-DDTHH:MM:SSZ`
