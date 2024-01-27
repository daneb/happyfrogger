---
title: Build your own AI code-reviewer
date: 2024-01-27T00:00:00-04:00
category: blog
---

![CoPilot](images/copilot.jpg)

## Build Your Own AI Code-Reviewer

In this journey, I embarked on creating a plugin for my IDE, designed to highlight code snippets and provide insights on their function, quality improvement, and refactoring potential.

### Why Build Your Own?

Here are compelling reasons to consider:

1. A local LLM (Large Language Model) guarantees your privacy.
2. The absence of a suitable plugin for your specific IDE.
3. Desire to customize the LLM's role and focus.
4. Interest in leveraging open-source LLMs for experimentation.
5. Curiosity to explore various LLMs.

If these resonate with you, prepare for a rewarding endeavor!

### The Steps Involved

1. **Choosing the Right LLM**
2. **Setting Up the LLM**
3. **Mastering LLM Interactions**
4. **Crafting Your IDE Plugin**
5. **Refining the Outcome**

### Choosing the Right LLM

Limited by my hardware, I sought LLMs that were compact, efficient, and specialized in coding. I discovered a helpful list by [continuedev](https://github.com/continuedev/what-llm-to-use) and chose [Deepseek Coder](https://deepseek.com/).

### Setting Up the LLM

My go-to was [LM Studio](https://lmstudio.ai/). Keep these points in mind:

- LLMs come in Base and Instruct types: Base excels in completing code, while Instruct is adept at writing from scratch.
- [Quantization](https://lmstudio.ai/quantization) affects the LLM's performance, balancing inference quality, speed, and resource usage.

> Tip: For a 7B model, a q4_K_S quantization is recommended.

### Interacting with the LLM

LM Studio facilitates interactions similarly to the OpenAI API. Here's a sample interaction using curl or [Httpie](https://httpie.io):

```json
{ 
  "messages": [ 
    { "role": "system", "content": "Review my code as an expert senior developer. Be critical. Focus on readability and maintainability. Refactor if needed." },
    { "role": "user", "content": <code snippet> }
  ], 
  "temperature": 0.7, 
  "max_tokens": -1,
  "stream": false
}
```

Define the LLM's behavior as 'system' and input your code snippets as 'user'.

> Note: Code must be properly escaped for POST HTTP requests.

### Building Your IDE Plugin

I developed a plugin for LazyVim using [neovim](https://neovim.io). Here's the process:

- Initialize your plugin on GitHub.
- Start with a simple echo function.
- Install the plugin in LazyVim (`~/.config/nvim/lua/plugins/`).

```lua
# reviewer.lua
return {
  {
    "daneb/reviewer.nvim",
  },
}
```

- Enhance the plugin functionality (see [Reviewer.nvim](https://github.com/daneb/reviewer.nvim)).
- Use LazyVim to sync plugin updates.
- Implement keymapping (located in `~/.config/nvim/lua/config/keymaps.lua`).

```lua
vim.api.nvim_set_keymap(
  "v",
  "<leader>lm",
  ':lua require("reviewer").main_function()<CR>',
  { noremap = true, silent = true, desc = "AI Reviewer" }
)
```

### Tweaking the Result

![Result](/images/reviewer.jpg)

Your plugin, now functional in your IDE, can be aesthetically enhanced with features like window sizing, colors, and borders.

Challenges remain, such as ensuring proper code snippet escape, particularly with regex. Initially, I used a curl call instead of the built-in neovim HTTP API. 

It's a work in progress, but it's an accessible, enjoyable, and fulfilling project!

