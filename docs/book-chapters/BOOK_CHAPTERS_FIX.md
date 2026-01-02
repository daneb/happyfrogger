# Book Chapters Display Fix

## Problem Identified

Your book chapters weren't appearing on the Faith page due to three main issues:

### 1. Missing Front Matter Properties
The `FrontMatter.cs` model didn't include book-specific fields like:
- `chapter_number`
- `progress`
- `previous_chapter`
- `next_chapter`
- `study_resources`

### 2. Draft Status Filtering
Your book chapter markdown files have `status: draft`, but your config has `includeDrafts: false`, which means they were being skipped during the build process.

### 3. Slug Handling
The template was adding `.html` extension when it's already included in the Slug property.

## Changes Made

### 1. Updated `Models/FrontMatter.cs`
Added book-specific properties and a new `StudyResource` class:

```csharp
// Book-specific properties
[YamlMember(Alias = "chapter_number")]
public int? ChapterNumber { get; set; }

[YamlMember(Alias = "progress")]
public int? Progress { get; set; }

[YamlMember(Alias = "previous_chapter")]
public string PreviousChapter { get; set; }

[YamlMember(Alias = "next_chapter")]
public string NextChapter { get; set; }

[YamlMember(Alias = "study_resources")]
public List<StudyResource> StudyResources { get; set; }
```

### 2. Updated `Models/BlogPostModel.cs`
Added the same book-specific properties to the model that represents rendered blog posts.

### 3. Updated `Program.cs`
Modified the post creation logic to copy book-specific properties from front matter to the blog post model.

### 4. Updated `Templates/CategoryTemplate.cshtml`
- **Fixed sorting**: Changed from `OrderBy(p => p.PublishDate)` to `OrderBy(p => p.ChapterNumber ?? int.MaxValue)` so chapters appear in the correct order
- **Fixed slug links**: Removed redundant `.html` extension (changed from `@(post.Slug).html` to `@post.Slug`)
- **Added chapter numbers**: Displays chapter number in a blue circle badge if available

## Next Steps to See Chapters

### Option 1: Change Draft Status (Recommended for Testing)
In your markdown files, change:
```yaml
status: draft
```
to:
```yaml
status: published
```

### Option 2: Enable Draft Mode in Config
In `happyfrog.config.json`, change:
```json
"includeDrafts": false
```
to:
```json
"includeDrafts": true
```

### Option 3: Rebuild and Deploy
After choosing one of the options above:

1. Rebuild the project:
   ```bash
   dotnet build
   dotnet run
   ```

2. The chapters should now appear on your Faith page under "The Man's Guide to Biblical Truth" section

## Expected Result

After the rebuild, you should see:
- Chapters listed in order by chapter number (0, 1, 2, etc.)
- Chapter numbers displayed in blue circular badges
- Proper progress bar based on number of chapters
- Clickable links to each chapter
- Chapter descriptions displayed under titles

## Example Chapter Front Matter

Here's what a properly formatted chapter should look like:

```yaml
---
title: "The Absent Man"
date: 2025-01-04
category: faith
subcategory: book
chapter_number: 1
progress: 10
status: published  # Change from draft to published
description: "Why good men matter"
slug: the-absent-man.html
previous_chapter: mans-guide-chapter-0
next_chapter: dispensations
study_resources:
  - title: "Biblical Manhood Resources"
    description: "Additional study materials on biblical masculinity"
---
```

## Future Enhancements

You might want to consider adding:

1. **Previous/Next Navigation**: Use the `previous_chapter` and `next_chapter` fields to add navigation at the bottom of each chapter
2. **Study Resources Section**: Display the `study_resources` list on individual chapter pages
3. **Progress Tracking**: Use the individual chapter `progress` field instead of calculating based on chapter count
4. **Chapter Filtering**: Make the filter buttons (All, book, devotion, sermon) actually functional with JavaScript

## Testing Checklist

- [ ] Rebuild the project: `dotnet run`
- [ ] Check that chapters appear on faith.html page
- [ ] Verify chapters are in correct order (by chapter number)
- [ ] Confirm chapter numbers display correctly
- [ ] Test chapter links work properly
- [ ] Verify progress bar displays
- [ ] Check that chapter descriptions show up

## Questions or Issues?

If chapters still don't appear after these changes:

1. **Check the markdown file location**: Ensure your markdown files are in the path specified by `markdownFilesPath` in your config
2. **Verify subcategory spelling**: Make sure it's exactly `subcategory: book` (lowercase, no typo)
3. **Check the build output**: Look for any error messages during the build
4. **Inspect the generated faith.html**: Open it and search for "Available Chapters" to see if the section is rendered

Remember: The YamlDotNet deserializer will ignore any properties that don't have matching `[YamlMember]` attributes, which is why we had to add these to the FrontMatter model.
