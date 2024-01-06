# HappyFrogger

A static content generation framework for the web.

![HappyFrogger](happyblogger.png) 

## Features

1. Templates with [Microsoft Razor](https://github.com/microsoft/razor)
2. Markdown support with [Markdown-it](https://markdown-it.github.io/)
3. Styling with [Tailwind](https://tailwindcss.com/)
4. Blog supports [Gist](https://gist.github.com)
5. Front Matters support through [YamlDotNet](https://github.com/aaubry/YamlDotNet)
5. Built with [C#](https://dotnet.microsoft.com) 

## TO ADD

1. Dynamic Index.html (manually defined at present)
2. Copy of output to release folder for github pages
3. How to not generate markdown for files un-changed

## Generating Tailwind for static HTML

1. Install Tailwind

```sh
npm install tailwindcss @tailwindcss/typography
```

2. Initialize tailwind in the directory

```sh
npx tailwindcss init
```

3. Modify your tailwind config file

```js
module.exports = {
  content: ["./path/to/your/html/**/*.html"],
  theme: {
    extend: {},
  },
  plugins: [
    require('@tailwindcss/typography'),
    // Other plugins can be added here
  ],
}

4. Create a CSS file (e.g., styles.css) that includes Tailwind.

```css
@tailwind base;
@tailwind components;
@tailwind utilities;
```

5. Build the stylesheet from the classes defined in the HTML.

```sh
npx tailwindcss -i ./styles.css -o ./output.css --watch
```

6. For now, copy the output.css to Output. This should be referenced in all templates/HTML files.

```html
  <link href="/output.css" rel="stylesheet">
```


