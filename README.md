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

## Prerequisites

- .NET 6.0 or higher
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
├── Program.cs          # Main generation logic
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

## Future Enhancements

- [ ] Dynamic index.html generation
- [ ] Automatic deployment to GitHub Pages
- [ ] Incremental builds for unchanged files
- [ ] RSS feed generation
- [ ] Search functionality
- [ ] Tags support
- [ ] Image optimization

## Support

For support, please open an issue in the GitHub repository or reach out to the maintainers.
