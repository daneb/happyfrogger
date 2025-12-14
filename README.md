# HappyFrogger 🐸

A lightweight static site generator built with C# and Razor, designed for developers who want a simple yet powerful blogging platform.

![HappyFrogger](happyblogger.png)

## Features

- 📝 Write in Markdown with front matter support
- 🎨 Beautiful styling with Tailwind CSS
- 🖼️ Templating with Razor pages
- 📊 Categories and subcategories for content organization
- 📋 GitHub Gist embedding support
- 🚀 Fast and efficient static site generation
- 📱 Fully responsive design
- 🔄 Live reload during development
- ⚙️ Flexible JSON configuration system
- 📁 Configurable input/output directories
- 📝 Draft post support
- 📡 RSS 2.0 feed generation with configurable options

## Prerequisites

- .NET 8.0 or higher
- Node.js (for Tailwind CSS)
- Basic knowledge of C# and Markdown

## Quick Start

1. Clone the repository
```bash
git clone https://github.com/yourusername/happyfrogger.git
cd happyfrogger
```

2. Install Node.js dependencies
```bash
npm install
```

3. Build the project
```bash
dotnet build
```

4. Start writing! Create a new markdown file in `MarkdownFiles/` with front matter:
```markdown
---
title: "My First Post"
date: 2024-01-14
category: tech
subcategory: csharp
description: A sample post
slug: my-first-post
---

Your content here...
```

## Project Structure

```
HappyFrogger/
├── MarkdownFiles/        # Your markdown content
├── Output/              # Generated static site
├── Templates/           # Razor templates
│   ├── BlogTemplate.cshtml
│   ├── CategoryTemplate.cshtml
│   └── LandingTemplate.cshtml
├── Models/              # C# model classes
│   ├── BlogPostModel.cs
│   ├── CategoryPageModel.cs
│   ├── LandingPageModel.cs
│   ├── FrontMatter.cs
│   └── HappyFrogConfig.cs
├── Program.cs          # Main generation logic
├── happyfrog.config.json # Configuration file
├── styles.css          # Tailwind entry point
└── tailwind.config.js  # Tailwind configuration
```

## Writing Content

### Front Matter
Each markdown file requires front matter at the top:
```yaml
---
title: "Post Title"
date: YYYY-MM-DD
category: tech|faith|creative
subcategory: your-subcategory
description: Brief description
slug: url-friendly-title
status: published  # or "draft" to exclude from builds
---
```

### Categories
The site supports three main categories:
- `tech`: Technical articles and tutorials
- `faith`: Faith-based content and reflections
- `creative`: Creative writing and personal posts

### Embedding Gists
To embed a GitHub Gist:
```markdown
[gist:gist-id]
```

## Configuration

HappyFrogger uses a `happyfrog.config.json` file for flexible configuration. This allows you to customize paths, build options, and site metadata without modifying code.

### Configuration File Structure

```json
{
  "markdownFilesPath": "MarkdownFiles",
  "outputPath": "Output",
  "templatesPath": "Templates",
  "site": {
    "title": "HappyFrogger Blog",
    "description": "A lightweight static site generator",
    "author": "Your Name",
    "baseUrl": "https://yourdomain.com"
  },
  "build": {
    "generateCategoryPages": true,
    "generateLandingPage": true,
    "htmlExtension": ".html",
    "includeDrafts": false,
    "categories": ["tech", "faith", "creative"],
    "rss": {
      "enabled": true,
      "path": "feed.xml",
      "itemCount": 20,
      "fullContent": true
    }
  }
}
```

### Configuration Options

#### Paths
- **markdownFilesPath**: Directory containing your markdown files (default: `"MarkdownFiles"`)
- **outputPath**: Directory where HTML files will be generated (default: `"Output"`)
- **templatesPath**: Directory containing Razor templates (default: `"Templates"`)

#### Site Metadata
- **title**: Your site's title
- **description**: Site description for meta tags
- **author**: Content author name
- **baseUrl**: Base URL for your site (useful for RSS feeds, sitemaps)

#### Build Options
- **generateCategoryPages**: Enable/disable category page generation (default: `true`)
- **generateLandingPage**: Enable/disable landing page generation (default: `true`)
- **htmlExtension**: File extension for generated HTML files (default: `".html"`)
- **includeDrafts**: Include posts with `status: draft` in builds (default: `false`)
- **categories**: Array of categories to generate pages for (default: `["tech", "faith", "creative"]`)
- **rss**: RSS feed configuration options
  - **enabled**: Enable/disable RSS feed generation (default: `true`)
  - **path**: Output path for RSS feed file (default: `"feed.xml"`)
  - **itemCount**: Maximum number of posts in feed, 0 for all (default: `20`)
  - **fullContent**: Include full HTML content or just description (default: `true`)

### Path Validation

When you run HappyFrogger, it will validate all configured paths and display:
- ✓ Green checkmarks for valid paths
- ✗ Red crosses for missing paths
- File counts for markdown directories
- Automatic creation of output directory if missing

Example output:
```
═══════════════════════════════════════════════
   HappyFrogger - Static Site Generator 🐸
═══════════════════════════════════════════════

Configuration loaded from: happyfrog.config.json

Configuration Check:
─────────────────────────────────────────────
✓ Templates Path: /path/to/Templates
✓ Markdown Files Path: MarkdownFiles (23 files)
✓ Output Path: Output
─────────────────────────────────────────────

Build Settings:
  • Generate Landing Page: True
  • Generate Category Pages: True
  • Include Drafts: False
  • Categories: tech, faith, creative
```

## Development

### Watching for Changes

1. Start Tailwind CSS watcher:
```bash
npm run watch:css
```

2. Build CSS for production:
```bash
npm run build:css
```

### Template Customization

Templates are located in the `Templates/` directory:
- `BlogTemplate.cshtml`: Individual post template
- `CategoryTemplate.cshtml`: Category page template
- `LandingTemplate.cshtml`: Home page template

## Deployment

1. Generate the site:
```bash
dotnet run
```

2. The static site will be generated in the `Output/` directory
3. Copy all files in `Output/` to the GitHub Pages repository or your web host
3. Deploy the contents of `Output/` to your web host

## Contributing

Contributions are welcome! Please feel free to submit a Pull Request.

1. Fork the repository
2. Create your feature branch (`git checkout -b feature/AmazingFeature`)
3. Commit your changes (`git commit -m 'Add some AmazingFeature'`)
4. Push to the branch (`git push origin feature/AmazingFeature`)
5. Open a Pull Request

## License

This project is licensed under the MIT License - see the LICENSE file for details

## Acknowledgments

- [Markdig](https://github.com/xoofx/markdig) for Markdown processing
- [RazorLight](https://github.com/toddams/RazorLight) for template generation
- [Tailwind CSS](https://tailwindcss.com/) for styling
- [YamlDotNet](https://github.com/aaubry/YamlDotNet) for YAML processing

## Changelog

See [CHANGELOG.md](CHANGELOG.md) for a detailed history of changes.

## Future Enhancements

- [ ] Dynamic index.html generation
- [ ] Automatic deployment to GitHub Pages
- [ ] Incremental builds for unchanged files
- [ ] RSS feed generation
- [ ] Search functionality
- [ ] Tags support
- [ ] Image optimization
- [ ] Watch mode for automatic rebuilds

## Support

For support, please open an issue in the GitHub repository or reach out to the maintainers.
