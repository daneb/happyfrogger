/** @type {import('tailwindcss').Config} */
module.exports = {
  content: [
    './Templates/BlogTemplate.cshtml',
    './Output/*.html'
  ],
  theme: {
    extend: {},
  },
  plugins: [
    'tailwindcss/typography'
  ],
}

