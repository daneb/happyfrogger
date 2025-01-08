---
title: 🔍 4 Pitfalls in using AI to build software 
date: 2025-01-08T00:00:00-04:00
category: tech
subcategory: ai
slug: four-pitfalls-ai
---

AI has evolved into a clear productivity multiplier for developers. Yet some remain skeptical, frustrated by its limitations. The issue often isn't the tool - it's misaligned expectations. Like any powerful technology, success comes from leveraging its strengths rather than fixating on what it can't do.

Through working extensively with AI, I've observed common patterns that hold developers back from realizing its full potential. By understanding these traps, we can better align our expectations and achieve more meaningful outcomes.

## 🎯 Pitfall #1: Poor problem framing.
Problem definition matters. Keywords play such a huge impact it can change the nature of the implementation with or without AI. For example, if I drew a circle, and I stated, **what is this familiar object?** You would immediately state it's a circle. But if I had instead asked, **what is this object?** Perhaps you would find it odd why I would be asking some a basic question, one I already know the answer to. Doubt would creep, perhaps some red flags, resulting in you perhaps asking more questions. Consider a third approach, what if I asked you instead, **what is this unfamiliar object?** This would certainly make you step back and think. How I framed the problem, impacted how you derived your outcome.

### How to identify:
- Not thinking through what you are asking, but just asking
- Asking for complete solutions without breaking down complex problems
- Jumping straight to implementation without discussing architecture
- Being vague about edge cases

## 🏗️ Pitfall #2: Over-reliance on AI
Picture a chef asking a kitchen assistant to "just fix dinner service." Even with access to every ingredient and recipe, the assistant can't replace the chef's experience, judgment, and understanding of their kitchen's unique challenges. Similarly, feeding an AI your entire codebase won't magically solve architectural problems or replace developer intuition.

### How to identify:
* Expecting AI to understand under-stated requirements
* Not reviewing generated code for best practices or logic flaws*
* Blindly implementing without understanding
* Using AI for critical security or data handling without review

## 🤖 Pitfall #3: Providing insufficient context
This is the equivalent of asking a construction company to quote you on building a 3 bedroom house, but forgetting to highlight that the location is on a hill, in a remote location with poor to little infrastructure. You just not going to get the right result.

### How to identify:
* Not sharing relevant dependencies or environment details
- Omitting important business rules or constraints
- Not explaining how the code fits into the larger system
- Failing to specify performance requirements or scale

## 🔄 Pitfall #4: Ineffective iteration
Fred is building his dream bathroom with a view of the mountains. Initially, he tells the builder "Move the toilet to the left and bath to the right" to capture that perfect vista. The builder does exactly that, rearranging the plumbing at considerable effort. The next day, Fred has another thought: "Actually, what if we just kept everything where it was and moved the window instead?" The builder, trying to accommodate, moves the window. But Fred had forgotten a crucial detail from his first design - the original window position had needed special tinted glass to manage the intense morning sun. When Fred visits the site, he finds a beautiful mountain view...and a bathroom that turns into a greenhouse every sunrise.

### How to identify:
- Not providing feedback on what worked/didn't work
- Starting over instead of building on previous responses
- Not clarifying misunderstandings in the AI's assumptions
- Failing to narrow down issues when debugging

## Suggestions:

### Improve your prompt engineering
One aspect of this is prompt engineering, and the second around structured communication. There is a plenitude of improvement and thinking around this on the internet, and a great resource in the [Prompt Engineering Guide](https://www.promptingguide.ai/).

### See AI as a collaborative junior developer
The real power comes from treating AI as a collaborative junior developer - one who can suggest approaches, spot patterns, and help with implementation details, but needs your guidance and domain expertise to create truly effective solutions. Like any good mentoring relationship, success comes from clear communication, thoughtful questions, and combining both parties' strengths.

###  Improve Problem Definition
Here's a simple template for an improved problem definition:

```
Here's what I'm trying to achieve: [clear goal]
Current environment: [versions, dependencies]
Constraints: [performance, security, etc.]
What I've tried: [previous attempts]
Specific questions: [what you need help with]
```

For more complex problems:

```
1. Context
- System: [architecture, tech stack, scale]
- Dependencies: [versions, integrations]
- Constraints: [performance, security, compliance]

2. Current State
- Existing code: [relevant snippets]
- Current behavior: [what's happening]
- Error messages: [if applicable]

3. Desired Outcome
- Expected behavior
- Success criteria
- Non-functional requirements

4. Attempted Solutions
- Approaches tried
- Results observed
- Specific blockers

5. Specific Questions
- Targeted questions about implementation
- Areas needing clarification
- Potential concerns
```

Include diagrams or flow-charts as well.
### Improving Iterative Development

```
Initial code: [code snippet]
Issues found: [specific problems]
Required changes: [clear modifications needed]
New requirements: [additional features]
```

### Improving development experience

Keep AI and context awareness separate from your IDE. While mastering these fundamentals, limit in-IDE AI to Copilot for routine code completion. Use standalone AI tools like Claude (Project Mode) or ChatGPT externally. This separation offers three key advantages:

1. Control over AI model quality
2. Better context management
3. Easier recovery from context-related mistakes