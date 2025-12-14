# ADR 002: Reading Time Calculation

**Status**: Accepted
**Date**: 2025-12-14
**Deciders**: Development Team
**Technical Story**: Phase 2 - Reading Time Calculation (v2.2.0)

---

## Context and Problem Statement

Blog readers benefit from knowing how long an article will take to read. This helps them decide whether to read now or save for later. We needed a simple, accurate way to calculate and display reading time.

---

## Decision Drivers

- Must be accurate enough for typical blog posts
- Should account for HTML content (strip tags before counting)
- Must be configurable (words per minute may vary by audience)
- Should display consistently across all templates
- Must not significantly impact build performance

---

## Considered Options

### Option 1: Character Count Based
- Divide character count by average reading speed
- **Rejected**: Less accurate, doesn't account for word complexity

### Option 2: Word Count from HTML (Chosen)
- Strip HTML tags, count words, divide by WPM
- **Pros**: Industry standard, simple, accurate
- **Cons**: Doesn't account for code blocks or images

### Option 3: Third-Party Library
- Use readability calculation library
- **Rejected**: Overkill for our needs, adds dependency

---

## Decision Outcome

**Chosen**: Word count from HTML with configurable WPM

### Implementation

**Calculation Method**:
```csharp
1. Strip HTML tags using regex: @"<[^>]+>"
2. Normalize whitespace
3. Split by whitespace to count words
4. Calculate: Math.Ceiling(wordCount / wordsPerMinute)
5. Minimum 1 minute (avoid "0 min read")
```

**Configuration**:
- `wordsPerMinute` in BuildOptions (default: 200)
- Industry standard: 200-250 WPM for average readers

**Display Format**:
- "X min read" on all pages
- Shown alongside publish date
- Consistent placement across templates

---

## Consequences

### Positive
- ✅ Simple, fast calculation
- ✅ Configurable per-site
- ✅ Industry-standard accuracy
- ✅ No external dependencies
- ✅ Negligible performance impact

### Negative
- ⚠️ Doesn't account for images/videos (would increase reading time)
- ⚠️ Code blocks counted as regular text
- ⚠️ Same WPM for all post types (technical vs. casual)

### Neutral
- 📊 Adds one property to BlogPostModel
- 📊 Adds ~15 lines of code

---

## Files Changed

- `Models/BlogPostModel.cs` - Added `ReadingTimeMinutes` property
- `Models/HappyFrogConfig.cs` - Added `WordsPerMinute` config
- `Program.cs` - Added `CalculateReadingTime()` method
- `Templates/BlogTemplate.cshtml` - Display reading time
- `Templates/CategoryTemplate.cshtml` - Display in listings
- `Templates/LandingTemplate.cshtml` - Display on previews

---

## Notes

- Reading time calculated once during build (not runtime)
- Plain text extraction is fast (regex-based)
- Future: Could make WPM configurable per-category if needed
