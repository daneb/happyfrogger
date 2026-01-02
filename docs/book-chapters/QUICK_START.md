# ✅ Quick Start Checklist

## Get Your Chapters Displaying in 3 Steps

### Step 1: Update Chapter Status
Open each chapter markdown file and change:
```diff
- status: draft
+ status: published
```

**Files to update:**
- `mans-guide-contents.md`
- `the-absent-man.md`
- `dispensations.md`
- `gods-programme.md`
- (any other book chapter files)

### Step 2: Rebuild the Site
```bash
cd /Users/dane.balia2/Documents/Repos/Personal/happyfrogger
dotnet run
```

Look for output like:
```
✓ Posts Generated: XX
✓ Landing Page: ✓
✓ Category Pages: 3
```

### Step 3: Check the Output
Open your faith.html page and look for:
- "The Man's Guide to Biblical Truth" section
- List of chapters with blue number badges
- Progress bar showing completion

---

## That's It! 🎉

If chapters appear: You're done!

If chapters don't appear, check:
1. Did you change `status: draft` to `status: published`?
2. Is `subcategory: book` (lowercase, no typo)?
3. Are the markdown files in the correct directory?
4. Any error messages when running `dotnet run`?

---

## What's New

Your chapters now have:
- ✨ Proper sorting by chapter number
- 🔢 Visual chapter number badges
- ⬅️➡️ Previous/Next navigation
- 📚 Study resources section
- 🎯 Progress tracking

---

## Quick Reference

**Create a new chapter:**
1. Copy existing chapter markdown file
2. Update front matter (title, chapter_number, etc.)
3. Write content
4. Set `status: published`
5. Run `dotnet run`

**Front matter template:**
```yaml
---
title: "Chapter Title"
date: 2025-01-04
category: faith
subcategory: book
chapter_number: 1
status: published
description: "Brief description"
slug: chapter-slug.html
previous_chapter: previous-slug.html
next_chapter: next-slug.html
---
```

---

## Documentation

For more details, see:
- `SUMMARY.md` - Complete overview of changes
- `BOOK_CHAPTERS_FIX.md` - Detailed technical explanation  
- `BOOK_CHAPTERS_GUIDE.md` - Best practices and troubleshooting

---

**Questions?** Review the documentation above or check for error messages in the build output.
