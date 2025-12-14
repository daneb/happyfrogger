# Changelog

All notable changes to HappyFrogger will be documented in this file.

The format is based on [Keep a Changelog](https://keepachangelog.com/en/1.0.0/),
and this project adheres to [Semantic Versioning](https://semver.org/spec/v2.0.0.html).

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
