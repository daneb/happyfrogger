# 🔄 HappyFrogger Book Chapters Flow

## How It Works Now

```
┌─────────────────────────────────────────────────────────────┐
│                    Markdown Files                            │
│                (../blog/markdownfiles/)                      │
│                                                              │
│  mans-guide-contents.md                                      │
│  ┌──────────────────────────────┐                           │
│  │ ---                          │                           │
│  │ title: "Table of Contents"   │                           │
│  │ category: faith              │                           │
│  │ subcategory: book        ← KEY!                          │
│  │ chapter_number: 0        ← DETERMINES ORDER              │
│  │ status: published        ← MUST BE PUBLISHED!            │
│  │ slug: chapter-0.html         │                           │
│  │ previous_chapter: intro.html │                           │
│  │ next_chapter: chapter-1.html │                           │
│  │ study_resources:             │                           │
│  │   - title: "Resource"        │                           │
│  │     description: "..."       │                           │
│  │ ---                          │                           │
│  │ # Content here               │                           │
│  └──────────────────────────────┘                           │
└─────────────────────────────────────────────────────────────┘
                         │
                         ▼
┌─────────────────────────────────────────────────────────────┐
│                  Program.cs (Build Process)                  │
│                                                              │
│  1. Load Configuration                                       │
│  2. Parse Front Matter with YamlDotNet                       │
│     ↓                                                        │
│  3. Extract Book Properties:                                 │
│     • ChapterNumber                                          │
│     • Progress                                               │
│     • PreviousChapter                                        │
│     • NextChapter                                            │
│     • StudyResources                                         │
│     ↓                                                        │
│  4. Convert Markdown → HTML                                  │
│     ↓                                                        │
│  5. Create BlogPostModel with ALL properties                 │
│     ↓                                                        │
│  6. Skip if status != "published" && !includeDrafts          │
│     ↓                                                        │
│  7. Render Templates                                         │
└─────────────────────────────────────────────────────────────┘
                         │
                         ▼
┌─────────────────────────────────────────────────────────────┐
│              CategoryTemplate.cshtml (faith.html)            │
│                                                              │
│  Filters posts where SubCategory == "book"                   │
│  ↓                                                           │
│  Sorts by ChapterNumber (not PublishDate!)                   │
│  ↓                                                           │
│  Displays:                                                   │
│  ┌────────────────────────────────────────┐                 │
│  │ 📚 The Man's Guide to Biblical Truth   │                 │
│  │ ┌────────────────────────────────┐     │                 │
│  │ │ Progress: ████████░░░░ 12%     │     │                 │
│  │ └────────────────────────────────┘     │                 │
│  │                                        │                 │
│  │ Available Chapters                     │                 │
│  │ ┌──┬───────────────────────────┐       │                 │
│  │ │ 0│ Table of Contents    →    │       │                 │
│  │ └──┴───────────────────────────┘       │                 │
│  │ ┌──┬───────────────────────────┐       │                 │
│  │ │ 1│ The Absent Man       →    │       │                 │
│  │ └──┴───────────────────────────┘       │                 │
│  │ ┌──┬───────────────────────────┐       │                 │
│  │ │ 2│ Unlocking Your Bible →    │       │                 │
│  │ └──┴───────────────────────────┘       │                 │
│  └────────────────────────────────────────┘                 │
└─────────────────────────────────────────────────────────────┘
                         │
                         ▼
┌─────────────────────────────────────────────────────────────┐
│           BlogTemplate.cshtml (Individual Chapters)          │
│                                                              │
│  ┌────────────────────────────────────────┐                 │
│  │ ← Back to Faith         Chapter 1      │                 │
│  │ The Absent Man                         │                 │
│  └────────────────────────────────────────┘                 │
│                                                              │
│  [Table of Contents if enabled]                             │
│  [Chapter Content]                                           │
│                                                              │
│  If SubCategory == "book":                                   │
│  ┌────────────────────────────────────────┐                 │
│  │ ← Previous  │  All Chapters  │  Next → │                 │
│  │   Chapter 0  │               │ Chapter 2│                 │
│  └────────────────────────────────────────┘                 │
│                                                              │
│  If StudyResources exists:                                   │
│  ┌────────────────────────────────────────┐                 │
│  │ 📖 Additional Study Resources           │                 │
│  │ ┌────────────────────────────────────┐ │                 │
│  │ │ Resource Title                      │ │                 │
│  │ │ Description here...                 │ │                 │
│  │ └────────────────────────────────────┘ │                 │
│  └────────────────────────────────────────┘                 │
└─────────────────────────────────────────────────────────────┘
```

## Key Data Flow

### Front Matter → Models
```
FrontMatter.cs                BlogPostModel.cs
━━━━━━━━━━━━━━                ━━━━━━━━━━━━━━━━
title               →         Title
category            →         Category
subcategory         →         SubCategory
chapter_number      →         ChapterNumber
progress            →         Progress
previous_chapter    →         PreviousChapter
next_chapter        →         NextChapter
study_resources     →         StudyResources
```

### Filtering Logic
```
All Posts
    │
    ├─ Category Filter
    │   └─ Posts where Category == "faith"
    │       │
    │       ├─ SubCategory Filter
    │       │   └─ Posts where SubCategory == "book"
    │       │       │
    │       │       └─ Sort by ChapterNumber
    │       │           └─ Display in "Available Chapters"
    │       │
    │       └─ Other Faith Posts
    │           └─ Display in year groups
    │
    └─ Other Categories (tech, creative)
        └─ Display in their own sections
```

## Critical Points

### ⚠️ Must Be Correct
1. `subcategory: book` (exact, lowercase)
2. `status: published` (not draft)
3. `chapter_number: X` (determines order)
4. Slug includes `.html` extension

### ✅ Optional But Recommended
1. `description` (shows under title)
2. `previous_chapter` / `next_chapter` (enables navigation)
3. `study_resources` (additional materials)
4. `progress` (individual chapter completion)

## Build Order

```
1. Load config
2. For each .md file:
   a. Parse front matter
   b. Extract ALL properties (including book ones)
   c. Convert markdown → HTML
   d. Create BlogPostModel
   e. Check status (skip if draft and includeDrafts=false)
   f. Render individual post page
3. Generate category pages
   a. Filter posts by category
   b. For faith category:
      - Find posts with subcategory="book"
      - Sort by chapter_number
      - Display in special book section
   c. Display other posts in year groups
4. Generate landing page
5. Generate RSS feed
6. Generate sitemap
```

## Troubleshooting Flow

```
Chapters not showing?
    │
    ├─ Check: Are files marked published?
    │   │
    │   ├─ NO → Change status: draft → published
    │   │
    │   └─ YES → Continue
    │
    ├─ Check: Is subcategory exactly "book"?
    │   │
    │   ├─ NO → Fix typo/capitalization
    │   │
    │   └─ YES → Continue
    │
    ├─ Check: Are files in correct directory?
    │   │
    │   ├─ NO → Move to markdownFilesPath location
    │   │
    │   └─ YES → Continue
    │
    └─ Check: Any build errors?
        │
        ├─ YES → Read error, fix issue
        │
        └─ NO → Check faith.html source for
                "Available Chapters" section
```

This visual guide should help you understand exactly how your book chapters flow through the system!
