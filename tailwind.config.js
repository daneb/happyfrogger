/** @type {import('tailwindcss').Config} */
module.exports = {
  content: [
    './Templates/*.cshtml',
    './Output/*.html'
  ],
  theme: {
    extend: {
      colors: {
        'primary-blue': '#0052FF',  // The blue color used in your design
        'crimson': '#DC143C', // sub-sections
      },
      maxWidth: {
        'content': '800px'  // Custom max-width for content
      }
    },
  },
  plugins: [
    require('@tailwindcss/typography'),
    require('tailwind-children'),
  ],
}