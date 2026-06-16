---
title: "4 Pitfalls in Using AI to Build Software"
date: 2025-01-08
category: tech
subcategory: ai
description: "AI is a productivity multiplier — if you avoid the traps that quietly cap its value."
slug: four-pitfalls-ai
status: published
---

AI has evolved into a clear productivity multiplier for developers. Yet some remain skeptical, frustrated by its limitations. The issue often isn't the tool — it's misaligned expectations.

Through working extensively with AI, I've observed common patterns that hold developers back. By naming them, we can better align our expectations and get more meaningful outcomes.

## Pitfall #1 — Poor problem framing

Problem definition matters. Keywords change the nature of the implementation entirely. If I drew a circle and asked *"what is this familiar object?"* you would say a circle. Ask *"what is this object?"* and doubt creeps in.

> How you frame the problem decides the shape of the answer — long before the model ever responds.

## Pitfall #2 — Over-reliance on AI

Treat the model as a collaborator, not an oracle. The strongest results come from a tight loop where you supply judgement and it supplies breadth.

```
Role: senior reviewer
Context: [paste the failing test]
Task: explain the root cause,
      then propose the smallest fix.
```

## Pitfall #3 — Insufficient context

Models reason over what you give them. Thin context yields generic answers. Bring the failing test, the constraint, the prior decision.

## Pitfall #4 — Ineffective iteration

Iterate in small, verifiable steps. Each turn should leave you with something you can run, read, and reason about.
