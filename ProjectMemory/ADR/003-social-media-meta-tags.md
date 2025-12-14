# ADR 003: Social Media Meta Tags

**Status**: Accepted
**Date**: 2025-12-14
**Deciders**: Development Team
**Technical Story**: Phase 3 - Social Media Meta Tags (v2.3.0)

---

## Context and Problem Statement

When blog posts are shared on social media platforms (Facebook, Twitter, LinkedIn), the preview appearance is controlled by meta tags. Without proper meta tags, shares look unprofessional with missing images, titles, or descriptions.

---

## Decision Drivers

- Must support Open Graph (Facebook, LinkedIn)
- Must support Twitter Cards
- Should allow per-post image customization
- Must work without external API calls
- Should be simple to configure

---

## Considered Options

### Option 1: Manual Meta Tags in Templates (Chosen)
- Add meta tags directly to Razor templates
- **Pros**: Simple, no dependencies, full control
- **Cons**: Duplicated across templates

### Option 2: Shared Partial View
- Create `_MetaTags.cshtml` partial
- **Rejected**: Over-engineering for 3 templates

### Option 3: Third-Party Library
- Use meta tag generation library
- **Rejected**: Adds complexity, not needed

---

## Decision Outcome

**Chosen**: Manual meta tags in each template with model-driven values

### Implementation

**Open Graph Tags** (Facebook, LinkedIn):
```html
<meta property="og:type" content="article">
<meta property="og:title" content="@Model.Title">
<meta property="og:description" content="@Model.Description">
<meta property="og:url" content="full-url">
<meta property="og:image" content="@Model.SocialImage">
<meta property="og:site_name" content="Site Name">
<meta property="article:published_time" content="ISO-8601">
<meta property="article:author" content="Author">
<meta property="article:section" content="Category">
```

**Twitter Card Tags**:
```html
<meta name="twitter:card" content="summary_large_image">
<meta name="twitter:title" content="@Model.Title">
<meta name="twitter:description" content="@Model.Description">
<meta name="twitter:image" content="@Model.SocialImage">
```

**Configuration**:
- `site.defaultSocialImage` - Fallback image URL
- `site.twitterHandle` - Optional Twitter username
- `frontMatter.socialImage` - Per-post override

---

## Consequences

### Positive
- ✅ Professional social media previews
- ✅ Better click-through rates on shares
- ✅ Per-post image customization
- ✅ No external dependencies
- ✅ Standards compliant (OG + Twitter)

### Negative
- ⚠️ No automatic image generation
- ⚠️ Requires manual social image creation
- ⚠️ Meta tags duplicated across 3 templates
- ⚠️ Hardcoded baseURL in templates (should use config)

### Neutral
- 📊 Adds ~15 lines to each template
- 📊 Adds 2 config properties
- 📊 Adds 1 front matter field

---

## Files Changed

- `Models/HappyFrogConfig.cs` - Added social config
- `Models/FrontMatter.cs` - Added `socialImage`
- `Models/BlogPostModel.cs` - Added `SocialImage`
- `Program.cs` - Pass through social image
- `Templates/BlogTemplate.cshtml` - Article meta tags
- `Templates/LandingTemplate.cshtml` - Site-level tags
- `Templates/CategoryTemplate.cshtml` - Category tags

---

## Validation

**Testing Methods**:
- Facebook Sharing Debugger: https://developers.facebook.com/tools/debug/
- Twitter Card Validator: https://cards-dev.twitter.com/validator
- LinkedIn Post Inspector: https://www.linkedin.com/post-inspector/

**Expected Results**:
- Rich previews with title, description, image
- Proper card type (article vs. summary)
- Correct published dates and author

---

## Future Improvements

- Use `config.Site.BaseUrl` instead of hardcoded URLs
- Auto-generate social images from title/content
- Add `og:image:width` and `og:image:height` for better rendering
- Support multiple images per post
- Add `twitter:site` from config

---

## Notes

- Social images should be at least 1200x630px for best results
- Images must be absolute URLs (not relative)
- Twitter falls back to OG tags if twitter: tags missing
- Meta description also improves SEO (search engines use it)
