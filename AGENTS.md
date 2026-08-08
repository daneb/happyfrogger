# AGENTS.md — repo conventions

## Context rule (token economy)

Never read whole files; use targeted retrieval (`ctx map` / `ctx sym` /
`ctx grep` once available — until then, ripgrep with tight line budgets).
Escalate to full source only when targeted retrieval is insufficient.

## Conventions

- <build / run commands>
- <style notes the linter cannot express>
- <directory guide — five lines max>

## Plumb

Work arrives as tasks under `.tasks/<name>/` (SPEC.md + PLAN.md). Advancement
is by gate, not judgment: this repo's own lint/typecheck/tests must pass, the
diff stays within the plan's declared Scope, and under the diff budget set in
`.plumb.toml`. Do not commit — merges happen through the gate layer.
