# Changelog

All notable changes to HappyFrogger will be documented in this file.

The format is based on [Keep a Changelog](https://keepachangelog.com/en/1.0.0/),
and this project adheres to [Semantic Versioning](https://semver.org/spec/v2.0.0.html).

## [2.5.0] - 2025-12-14

### Added
- **Table of Contents Auto-Generation**: Automatic TOC for posts with multiple headings
  - Automatically generates TOC when post has 3+ headings (configurable)
  - GitHub-style anchor IDs for all headings (e.g., #currently-reading)
  - Clickable links for quick navigation within posts
  - Semantic `<nav>` element for accessibility
  - Supports heading levels H2-H6 with nested structure
  - Clean styling with left border accent and hover effects
- **TOC Configuration**: Added to build config
  - `enabledByDefault` (bool, default: true) - Global TOC toggle
  - `minHeadings` (int, default: 3) - Minimum headings required
  - `maxLevel` (int, default: 3) - Maximum heading level (2=H2, 3=H3, etc.)
  - `title` (string, default: "Table of Contents") - TOC section title
- **Per-Post TOC Control**: Optional `toc: true|false` in front matter
  - Overrides global default setting
  - Allows disabling TOC for specific posts
- **AutoIdentifiers**: Markdig extension for automatic heading IDs
  - Uses GitHub-compatible anchor ID generation
  - Enables deep linking to specific sections
- **TableOfContents Property**: Added to BlogPostModel for storing generated HTML

### Changed
- **Markdown Processing**: Consolidated to single conversion pass
  - Fixed double-conversion issue with Gist placeholders
  - Now uses single `Markdown.ToHtml()` call with full pipeline
  - AutoIdentifiers properly applied during conversion
- **Gist Handling**: Refactored `ConvertMarkdownToHtmlWithGist` to `ReplaceGistPlaceholders`
  - Now replaces placeholders before HTML conversion (not after)
  - Ensures AutoIdentifiers work correctly

### Fixed
- Double HTML conversion bug that prevented AutoIdentifiers from working
- TOC generation now correctly parses heading IDs from rendered HTML

## [2.4.0] - 2025-12-14

### Added
- **Sitemap.xml Generation**: SEO-optimized XML sitemap
  - Automatically generates `sitemap.xml` during build
  - Includes all pages (landing, categories, posts)
  - Dynamic priority based on post age (recent posts get higher priority)
  - Change frequency hints (daily/weekly/monthly)
  - W3C datetime format (ISO 8601)
  - Validates that `baseUrl` is set before generation
- **robots.txt Generation**: Automatic robots.txt with sitemap reference
  - References sitemap.xml location
  - Allows all crawlers by default
- **SitemapGenerator Class**: Dedicated class for sitemap generation

### Changed
- Configuration file now supports `build.sitemap` section

## [2.3.0] - 2025-12-14

### Added
- **Social Media Meta Tags**: Open Graph and Twitter Card support
  - Open Graph tags for Facebook, LinkedIn sharing
  - Twitter Card tags for rich Twitter previews
  - Per-post social image customization via front matter
  - Default social image configuration
  - Article metadata (published time, author, section)
- **Social Configuration**: Added to site config
  - `defaultSocialImage` setting
  - `twitterHandle` setting
- **socialImage Front Matter**: Optional per-post override

### Changed
- All templates now include comprehensive meta tags
- BlogTemplate: Full article meta tags with OG and Twitter
- LandingTemplate: Site-level meta tags
- CategoryTemplate: Category-level meta tags

## [2.2.0] - 2025-12-14

### Added
- **Reading Time Calculation**: Automatic reading time estimation
  - Displays "X min read" on all pages
  - Calculated from word count (strips HTML)
  - Configurable words-per-minute (default: 200)
  - Shows in post headers, category listings, and landing page
- **ReadingTimeMinutes Property**: Added to BlogPostModel
- **CalculateReadingTime Method**: Helper method in Program.cs

### Changed
- Configuration file now supports `wordsPerMinute` in build options

## [2.1.0] - 2025-12-14

### Added
- **RSS Feed Generation**: Full RSS 2.0 compliant feed support
  - Automatically generates `feed.xml` during build
  - Configurable via `build.rss` section in config
  - Options: enable/disable, item count limit, full content vs. excerpts
  - Includes proper RFC 822 date formatting
  - CDATA escaping for HTML content
  - Atom self-reference link
  - Author and category metadata
  - Validates that `baseUrl` is set before generation
- **RSS Discovery Links**: All HTML templates now include `<link rel="alternate">` tags
- **Enhanced Build Summary**: Build output now shows RSS feed generation status
- **RssFeedGenerator Class**: New dedicated class for RSS generation with:
  - Content excerpt generation (strips HTML, truncates intelligently)
  - RFC 822 date conversion
  - Configurable item limits
  - Full content or description-only modes

### Changed
- Configuration file now supports `build.rss` section with:
  - `enabled` (bool, default: true)
  - `path` (string, default: "feed.xml")
  - `itemCount` (int, default: 20, 0 = all posts)
  - `fullContent` (bool, default: true)

### Fixed
- N/A

## [2.0.0] - 2025-12-14

### Added
- **Configuration System**: Introduced `happyfrog.config.json` for flexible project configuration
  - Configurable markdown files path (`markdownFilesPath`)
  - Configurable output directory (`outputPath`)
  - Configurable templates directory (`templatesPath`)
  - Site metadata configuration (title, description, author, baseUrl)
  - Build options configuration (category pages, landing page, HTML extension, draft inclusion)
  - Ability to configure custom categories list
- **Path Validation**: Added comprehensive path validation on startup
  - Displays checkmarks (✓) for valid paths and crosses (✗) for invalid ones
  - Shows markdown file count when path is valid
  - Automatically creates output directory if it doesn't exist
  - Exits gracefully with helpful error messages if required paths are missing
- **Enhanced CLI Output**: Improved command-line interface with:
  - Branded header with HappyFrogger logo
  - Configuration check section with visual indicators
  - Build settings display showing enabled features
  - Build completion summary with statistics
  - Better visual separation using box-drawing characters
- **Draft Management**: Posts can now be excluded from builds
  - Set `status: draft` in front matter to mark posts as drafts
  - Control draft inclusion via `includeDrafts` setting in config
  - Draft posts are logged as skipped during build
- **Configuration Model**: New `HappyFrogConfig` class with full IntelliSense support
  - Type-safe configuration with default values
  - Nested configuration objects for better organization
  - JSON configuration with support for comments and trailing commas

### Changed
- **Target Framework**: Updated from .NET 7.0 to .NET 8.0
  - Aligns with current LTS version
  - .NET 7.0 is out of support
- **Configuration Loading**:
  - Configuration now loads from `happyfrog.config.json` instead of hardcoded values
  - Falls back to sensible defaults if config file is missing
  - Searches current directory first, then executable directory
- **Category Generation**: Category pages are now generated dynamically based on config
  - Categories defined in configuration file rather than hardcoded
  - Easy to add or remove categories without code changes

### Fixed
- Improved null reference handling in configuration loading
- Better error handling for missing or invalid configuration files

## [1.0.0] - 2024-01-14

### Added
- Initial release of HappyFrogger
- Markdown to HTML conversion using Markdig
- Razor template support with RazorLight
- YAML front matter parsing with YamlDotNet
- Three default categories: tech, faith, creative
- GitHub Gist embedding support via `[gist:id]` syntax
- Responsive design with Tailwind CSS
- Category pages generation
- Landing page with categorized posts
- Automatic slug generation from titles
- Subcategory support
- Post metadata (title, date, description, category, subcategory)
