# HappyFrogger Best Practices for Book Chapters

## Quick Reference

### Creating a New Chapter

1. **Create your markdown file** in your markdownfiles directory
2. **Use this template** for the front matter:

```yaml
---
title: "Your Chapter Title"
date: 2025-01-04
category: faith
subcategory: book
chapter_number: 2  # Sequential number
progress: 5        # Percentage complete (optional)
status: published  # Use 'published' to make visible
description: "Brief description of the chapter"
slug: your-chapter-slug.html
previous_chapter: previous-slug
next_chapter: next-slug
study_resources:
  - title: "Resource 1 Title"
    description: "What this resource covers"
  - title: "Resource 2 Title"
    description: "What this resource covers"
---

# Your Chapter Content Here

Write your chapter content in markdown below the front matter.
```

### Front Matter Field Guide

| Field | Required | Type | Description |
|-------|----------|------|-------------|
| `title` | Yes | string | Chapter title |
| `date` | Yes | datetime | Publication date |
| `category` | Yes | string | Must be "faith" for book chapters |
| `subcategory` | Yes | string | Must be "book" to appear in book section |
| `chapter_number` | Recommended | integer | Determines display order |
| `progress` | Optional | integer | Individual chapter completion % |
| `status` | Yes | string | Use "published" or "draft" |
| `description` | Recommended | string | Shows under chapter title |
| `slug` | Optional | string | Auto-generated if not provided |
| `previous_chapter` | Optional | string | Slug of previous chapter |
| `next_chapter` | Optional | string | Slug of next chapter |
| `study_resources` | Optional | array | List of additional resources |

### Common Pitfalls

1. **Forgetting `.html` extension in slug**
   - ❌ `slug: my-chapter`
   - ✅ `slug: my-chapter.html`

2. **Wrong subcategory spelling**
   - ❌ `subcategory: Book` (capital B)
   - ❌ `subcategory: books` (plural)
   - ✅ `subcategory: book` (lowercase, singular)

3. **Leaving status as draft**
   - ❌ `status: draft` (won't appear on site)
   - ✅ `status: published` (will appear on site)

4. **Non-sequential chapter numbers**
   - Chapters display in order of `chapter_number`
   - Missing numbers create gaps in the sequence
   - Use 0 for introduction/table of contents

### Build and Test

```bash
# Navigate to project directory
cd /Users/dane.balia2/Documents/Repos/Personal/happyfrogger

# Build and run
dotnet run

# Check output
# Open ../blog/faith.html in your browser
# Look for "The Man's Guide to Biblical Truth" section
```

### Troubleshooting

**Chapters not appearing?**
1. Check `status: published` in front matter
2. Verify `subcategory: book` (exact spelling)
3. Ensure `category: faith`
4. Check build output for errors
5. Verify markdown file is in correct directory

**Chapters in wrong order?**
1. Check `chapter_number` values
2. Ensure they're sequential integers
3. Rebuild the project

**Links not working?**
1. Verify slug has `.html` extension
2. Check that linked chapters exist
3. Ensure slugs match between files

## File Structure

```
happyfrogger/
├── Models/
│   ├── FrontMatter.cs       # Defines all front matter fields
│   ├── BlogPostModel.cs     # Defines rendered post properties
│   └── ...
├── Templates/
│   ├── CategoryTemplate.cshtml  # Where book chapters display
│   ├── BlogTemplate.cshtml      # Individual chapter template
│   └── ...
├── Program.cs               # Main build logic
└── happyfrog.config.json   # Configuration
```

## Notes

- The deserializer (YamlDotNet) only parses fields with `[YamlMember]` attributes
- All book-specific fields are now supported in FrontMatter.cs
- Chapters sort by `chapter_number`, not by date
- Progress bar calculated from total chapter count (can be enhanced)
