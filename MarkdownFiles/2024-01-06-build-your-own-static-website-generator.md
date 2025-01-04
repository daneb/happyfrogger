---
title: Build your own static web site generator
date: 2024-01-14T00:00:00-04:00
category: tech
subcategory: diy
description: Its quick and easy to build a blog engine
slug: static-website-gen
---

![HappyFrogger](images/happyfrogger.jpg)

# How I Built My Own Static Site Generator for a Personal Blog

In the ever-evolving landscape of web development, the allure of simplicity often leads to innovative solutions. This
was precisely my experience as I ventured into creating my own static site generator for my personal blog. It definitely
doesn't have everything, but it's enough, done across 2 days and is only 132 lines of C#. Here’s
a [glimpse into my journey](https://github.com/daneb/happyfrogger), offering insights and practical steps that might
inspire your own custom project.

## Step 1: Embracing Markdown and C# for Content and Processing

The cornerstone of my project was the utilization of Markdown for content creation, chosen for its simplicity and
flexibility. Coupled with C#, a language I was already comfortable with, I used the `Markdig` library to process
Markdown files into HTML. This setup provided a straightforward way to write and transform blog content.

## Step 2: Implementing Templating with Razor

Consistency in appearance is key to any blog's identity. Leveraging the power of the Razor templating engine, commonly
used in ASP.NET, I could craft dynamic templates. These templates ensured a uniform layout and style across various blog
posts, embedding the essence of the blog’s personality into every page.

## Step 3: Styling with Tailwind CSS

Aesthetic appeal and responsive design are non-negotiable in today’s web. Tailwind CSS was my choice for its
utility-first approach, enabling me to elegantly style the blog. The integration of Tailwind CSS guaranteed not only
functionality but also visual appeal, ensuring the blog's seamless presentation on diverse devices.

## Step 4: Front Matter Parsing with YamlDotNet

A crucial feature for any blog is the ability to categorize and manage metadata for each post. YamlDotNet came to the
rescue, allowing me to parse Front Matter in Markdown files. This addition enabled me to define properties like title,
date, and categories directly within the Markdown, greatly enhancing content manageability and organization.

## Step 5: Embedding GitHub Gists

To enrich the technical aspect of the blog, embedding code snippets was essential. GitHub Gists provided a simple yet
effective way to incorporate these snippets. By placing placeholders in the Markdown content, which were later replaced
with Gist embed codes during HTML generation, I could seamlessly integrate dynamic code examples into the posts.

## Step 6: Deployment on GitHub Pages

The final piece of the puzzle was deploying the blog. GitHub Pages offered an efficient and straightforward hosting
solution. After setting up a repository and configuring the GitHub Pages settings, I pushed the static HTML files,
bringing the blog to life on the web.

## Overcoming Challenges

The journey wasn’t without its challenges. Ensuring that Tailwind CSS properly styled the dynamically generated HTML
content required some tweaking, solved by configuring the JIT Compiler and the correct loading sequence. Deploying the
site with a custom domain on GitHub Pages needed careful DNS configuration and repository setup, including a `.nojekyll`
file to bypass Jekyll processing.

## Conclusion

Crafting a static site generator from scratch was not only immensely satisfying but also provided a tailor-made solution
for my blogging needs. It allowed complete control over the design, functionality, and content, resulting in a unique
and personal blogging platform. For those inspired to embark on a similar endeavor, remember to keep things simple,
leverage familiar tools, and enjoy the creative process. Your perfect blogging platform might just be a few code lines
away.

Happy coding and blogging!
