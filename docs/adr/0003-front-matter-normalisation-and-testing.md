# ADR-0003: Front Matter Normalisation and Test Suite

**Date:** 2026-06-21
**Status:** Accepted
**Deciders:** Dane Balia

---

## Context

Two bugs were discovered after the content/engine separation (ADR-0002):

1. **Double `.html` extension on chapter navigation links.** Front matter fields `previous_chapter` and `next_chapter` stored values with an `.html` suffix (e.g. `previous_chapter: ch1.html`). The `BlogTemplate.html` template appends `.html` when building the `href`, resulting in `ch1.html.html` on the rendered page. The same issue already existed for `slug:` and had been fixed there, but the pattern was not applied consistently to navigation fields.

2. **`StudyResource.Author` not deserialised.** The `StudyResource` model (defined in `CategoryPageModel.cs`) was missing an `Author` property. The YAML front matter allowed specifying an author per resource, and the template conditionally rendered it — but since the property did not exist on the model, the YAML deserialiser silently dropped the value and the template condition was never true.

In addition, no test suite existed to catch regressions in front matter processing, slug normalisation, or book progress calculation.

---

## Decisions

### 1. Consistent `.html` Stripping via `StripHtml` Helper

**Chosen:** Extract a private `StripHtml(string? value)` helper in `PostProcessor` and apply it to `PreviousChapter` and `NextChapter` at the point of model construction — the same pattern already used for `slug`.

```csharp
private static string? StripHtml(string? value) =>
    value?.EndsWith(".html", StringComparison.OrdinalIgnoreCase) == true ? value[..^5] : value;
```

**Rationale:** Front matter is author-controlled and has historically included `.html` suffixes on navigation slugs (likely written before the v3 slug-normalisation convention). Stripping at the model layer keeps templates free of conditional suffix logic and applies the fix to all existing content without requiring front matter edits.

**Scope:** `PreviousChapter` and `NextChapter` only. Other fields that carry slugs (e.g. `next_chapter` in chapter objects) follow the same pattern. Fields that carry full URLs or paths are not affected.

---

### 2. `Author` Property Added to `StudyResource`

**Chosen:** Add `Author { get; set; } = ""` with `[YamlMember(Alias = "author")]` to the `StudyResource` class in `CategoryPageModel.cs`.

**Root cause:** `StudyResource` was defined with only `Title`, `Description`, and `Url`. The front matter format and the template both assumed `author` existed, but the model did not declare it, so YamlDotNet (configured with `IgnoreUnmatchedProperties`) silently dropped it.

**Template rendering:** The existing template guard `{{ if resource.author }}` was already correct; only the model was missing the property.

---

### 3. xUnit Test Suite

**Chosen:** A dedicated `HappyFrog.Tests` xUnit project added to the solution, referencing the main project.

**Structure:**

```
HappyFrog.Tests/
├── PostProcessorTests.cs   — slug/nav normalisation, study resources, draft filtering, reading time
└── BookProcessorTests.cs   — progress calculation, totalChapters override, cover image passthrough
```

Tests use temporary directories created per test class and cleaned up in `Dispose()`. `PostProcessor.ProcessFile` and `BookProcessor.ProcessBooks` are exercised through their public APIs with real markdown content.

**csproj exclusion:** The main `HappyFrog.csproj` uses the SDK's implicit glob, which would otherwise compile all `.cs` files in subdirectories including `HappyFrog.Tests/`. An explicit `<Compile Remove="HappyFrog.Tests/**" />` exclusion was added to prevent this.

**Coverage (19 tests):**

| Area | Tests |
|---|---|
| Slug normalisation | `.html` stripped, bare slug unchanged, slug generated from title |
| Chapter navigation | `.html` stripped from `previous_chapter` / `next_chapter`, bare slugs unchanged, absent fields are null |
| Study resources | Author deserialised, multiple resources in order, author is optional |
| Draft filtering | Draft excluded when `includeDrafts = false`, included when `true` |
| Reading time | Always ≥ 1 min, scales with word count |
| Book progress | 100% when all written, `totalChapters` overrides denominator, zero when none written |
| Chapters available | Only written chapters counted |
| Cover image | Passed through from `BookConfig` to `BookIndexModel` |

---

## Consequences

**Positive:**
- The `.html.html` double-extension bug cannot silently regress — the slug and navigation normalisation paths are both tested.
- Missing model properties on `StudyResource` or similar classes will be caught at the model layer; template-only fixes are no longer the only safety net.
- Future contributors have a working test harness and can follow the `ProcessFile`-with-temp-file pattern to add coverage for new front matter fields.

**Negative / watch items:**
- Tests use real file I/O (temp directories) rather than an abstracted file system. This is intentional — `PostProcessor` reads files directly and the overhead is negligible — but it means tests are not purely in-memory.
- The `<Compile Remove>` exclusion in `HappyFrog.csproj` is necessary only because the test project is nested inside the main project directory. If the test project is ever moved to a sibling directory, this exclusion can be removed.
