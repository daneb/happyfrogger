# 📋 Complete Example: Before & After

## Before (Not Working)

### Markdown File
```yaml
---
title: "The Absent Man"
date: 2025-01-04
category: faith
subcategory: book
chapter_number: 1
progress: 10
status: draft  # ❌ This prevents it from appearing
description: "Why good men matter"
slug: the-absent-man
previous_chapter: introduction
next_chapter: the-absent-man
study_resources:
  - title: "Biblical Manhood Resources"
    description: "Additional study materials"
---

# The Absent Man

Content here...
```

### What Happened
❌ Chapter didn't appear on faith.html
❌ "Available Chapters" section was empty
❌ Front matter was partially ignored

### Why It Failed
1. `status: draft` - filtered out by build process
2. `slug: the-absent-man` - missing `.html` extension
3. Chapter-specific properties weren't parsed
4. Template had bugs in sorting and linking

---

## After (Working)

### Markdown File
```yaml
---
title: "The Absent Man"
date: 2025-01-04
category: faith
subcategory: book
chapter_number: 1
progress: 10
status: published  # ✅ Changed to published
description: "Why good men matter"
slug: the-absent-man.html  # ✅ Added .html extension
previous_chapter: mans-guide-chapter-0.html  # ✅ Full slug with extension
next_chapter: dispensations.html  # ✅ Full slug with extension
study_resources:
  - title: "Biblical Manhood Resources"
    description: "Additional study materials on biblical masculinity"
  - title: "Prayer Guide for Men"
    description: "Daily prayers focused on spiritual growth"
---

# The Absent Man

Why do good men matter? Let's explore this critical question...

## The Problem

In today's culture, masculine leadership is under attack...

## Biblical Foundation

The Bible has much to say about godly manhood...
```

### What Happens Now
✅ Chapter appears on faith.html
✅ Listed in "Available Chapters" section
✅ Shows chapter number badge (1)
✅ Proper navigation (Previous/Next)
✅ Study resources displayed
✅ All properties preserved

---

## Visual Comparison

### Faith.html Page - Before
```
Faith & Wisdom
└── 2025
    ├── Article 1
    └── Article 2
```

### Faith.html Page - After
```
Faith & Wisdom
├── 📚 The Man's Guide to Biblical Truth
│   ├── Progress: ████░░░░░░ 12% Complete
│   └── Available Chapters
│       ├── [0] Table of Contents
│       ├── [1] The Absent Man
│       ├── [2] Unlocking Your Bible
│       └── [3] God's Programme
│
└── 2025
    ├── Article 1
    └── Article 2
```

### Individual Chapter Page - Before
```
┌─────────────────────────────┐
│ The Absent Man              │
├─────────────────────────────┤
│                             │
│ Content...                  │
│                             │
└─────────────────────────────┘
```

### Individual Chapter Page - After
```
┌─────────────────────────────┐
│ ← Back to Faith             │
│ The Absent Man     [DRAFT]  │
│ faith / book                │
│ January 4, 2025 • 5 min     │
├─────────────────────────────┤
│ Table of Contents           │
│ 1. The Problem              │
│ 2. Biblical Foundation      │
├─────────────────────────────┤
│                             │
│ Content...                  │
│                             │
├─────────────────────────────┤
│ ← Prev  │ All Chapters │ Next →│
│ Intro   │              │ Ch. 2 │
├─────────────────────────────┤
│ 📖 Additional Study Resources│
│ ┌───────────────────────┐  │
│ │ Biblical Manhood      │  │
│ │ Resources             │  │
│ └───────────────────────┘  │
│ ┌───────────────────────┐  │
│ │ Prayer Guide for Men  │  │
│ └───────────────────────┘  │
└─────────────────────────────┘
```

---

## Code Changes Summary

### Models/FrontMatter.cs
```diff
  public class FrontMatter
  {
      [YamlMember(Alias = "title")]
      public string Title { get; set; }
      // ... existing properties ...
+
+     // Book-specific properties
+     [YamlMember(Alias = "chapter_number")]
+     public int? ChapterNumber { get; set; }
+
+     [YamlMember(Alias = "progress")]
+     public int? Progress { get; set; }
+
+     [YamlMember(Alias = "previous_chapter")]
+     public string PreviousChapter { get; set; }
+
+     [YamlMember(Alias = "next_chapter")]
+     public string NextChapter { get; set; }
+
+     [YamlMember(Alias = "study_resources")]
+     public List<StudyResource> StudyResources { get; set; }
  }
+
+ public class StudyResource
+ {
+     [YamlMember(Alias = "title")]
+     public string Title { get; set; }
+
+     [YamlMember(Alias = "description")]
+     public string Description { get; set; }
+ }
```

### Templates/CategoryTemplate.cshtml
```diff
- @foreach (var post in bookPosts.OrderBy(p => p.PublishDate))
+ @foreach (var post in bookPosts.OrderBy(p => p.ChapterNumber ?? int.MaxValue))
  {
-     <a href="@(post.Slug).html" class="block group">
+     <a href="@post.Slug" class="block group">
          <div class="flex justify-between items-start p-4 hover:bg-gray-50 rounded-lg">
-             <div>
+             <div class="flex items-start space-x-3">
+                 @if (post.ChapterNumber.HasValue)
+                 {
+                 <span class="flex-shrink-0 w-8 h-8 flex items-center justify-center bg-primary-blue text-white rounded-full text-sm font-semibold">
+                     @post.ChapterNumber
+                 </span>
+                 }
+                 <div>
                      <h4 class="font-medium group-hover:text-primary-blue">@post.Title</h4>
                      <p class="text-sm text-gray-500 mt-1">@post.Description</p>
+                 </div>
              </div>
```

### Templates/BlogTemplate.cshtml
```diff
  </article>
+
+ @* Chapter Navigation for Book Posts *@
+ @if (Model.SubCategory == "book" && (!string.IsNullOrEmpty(Model.PreviousChapter) || !string.IsNullOrEmpty(Model.NextChapter)))
+ {
+     <div class="mt-12 pt-8 border-t border-gray-200">
+         <nav class="flex justify-between items-center">
+             [Previous/Next navigation code]
+         </nav>
+     </div>
+ }
+
+ @* Study Resources Section *@
+ @if (Model.StudyResources != null && Model.StudyResources.Any())
+ {
+     <div class="mt-8 p-6 bg-blue-50 rounded-lg border border-blue-100">
+         [Study resources display code]
+     </div>
+ }
</main>
```

---

## Complete Chapter Template

Here's a complete, working example you can copy:

```yaml
---
title: "Understanding Salvation"
date: 2025-01-05
category: faith
subcategory: book
chapter_number: 4
progress: 15
status: published
description: "The core message of the Gospel and what it means for your life"
slug: understanding-salvation.html
previous_chapter: gods-programme.html
next_chapter: blessings-in-salvation.html
study_resources:
  - title: "Romans Road to Salvation"
    description: "A guided study through key verses in Romans"
  - title: "Assurance of Salvation"
    description: "How to know you are saved"
  - title: "Memory Verses"
    description: "Key salvation verses to memorize"
toc: true
---

# Understanding Salvation

## What is Salvation?

Salvation is God's free gift of eternal life...

## The Gospel Message

### We Are All Sinners
Romans 3:23 tells us...

### Sin Has Consequences
The wages of sin is death...

### Christ Died for Us
But God demonstrates His own love...

## How to Be Saved

Salvation is by grace through faith...

## What Happens Next?

After salvation, you are a new creature...

## Conclusion

The Gospel is the power of God unto salvation...
```

---

## Verification Checklist

After making these changes, verify:

✅ **Build Process**
- [ ] No errors during `dotnet run`
- [ ] All chapters processed
- [ ] faith.html generated successfully

✅ **Category Page (faith.html)**
- [ ] Book section appears
- [ ] Progress bar displays
- [ ] Chapters listed in order
- [ ] Chapter numbers show in badges
- [ ] Descriptions visible
- [ ] Links are clickable

✅ **Individual Chapters**
- [ ] Page renders correctly
- [ ] Table of contents (if enabled)
- [ ] Previous/Next navigation
- [ ] "All Chapters" link works
- [ ] Study resources section (if applicable)
- [ ] Content displays properly

✅ **Navigation Flow**
- [ ] Can click from faith.html to chapter
- [ ] Can navigate between chapters
- [ ] Can return to faith.html
- [ ] Can reach all chapters

---

This complete example shows exactly what changed and what the results should be. Copy the working example to create new chapters!
