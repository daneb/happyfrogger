# ADR-0004: Build Script and Content-Directory Resolution

**Date:** 2026-07-18
**Status:** Accepted
**Deciders:** Dane Balia

---

## Context

After the content/engine separation (ADR-0002), the engine must be run from the blog directory so that `LoadConfiguration` resolves `happyfrog.config.json` and all relative paths against the content repo. Running `dotnet run` from the engine directory picks up a stale local config with wrong relative paths and fails at startup.

Two additional build concerns arose:
1. **Tailwind CSS** — the engine no longer manages a Node toolchain, but `tailwindcss` CLI must still run before `dotnet run` to regenerate `output.css` when templates change.
2. **Discoverability** — contributors and CI need a single entry point that handles directory switching and optional Tailwind without reading internal docs.

---

## Decision

### `build.sh` as the canonical build entry point

`build.sh` in the engine root is the single command to build the site:

```bash
./build.sh                          # uses ../blog as content dir (default)
./build.sh --blog /path/to/blog     # explicit content directory
./build.sh -- --serve               # pass-through flags to dotnet
```

**Directory resolution algorithm:**

1. `SCRIPT_DIR` — absolute path of `build.sh` itself (always the engine directory).
2. `BLOG_DIR` — `--blog <path>` argument if supplied, otherwise `$(cd "$SCRIPT_DIR/../blog" && pwd)`.
3. Fail fast with a clear message if `BLOG_DIR` does not exist.
4. Run Tailwind (skipped with a warning if the binary is absent).
5. `cd "$BLOG_DIR" && dotnet run --project "$SCRIPT_DIR/HappyFrog.csproj" -- "${PASS_ARGS[@]}"`.

The `cd` in step 5 is the key mechanism: once the working directory is the blog repo, `Program.cs → LoadConfiguration` finds `happyfrog.config.json` and every path in it resolves correctly relative to the content directory.

### Tailwind binary in `tools/`

`tools/tailwindcss` (gitignored standalone binary) is run by the script when present. Its absence is non-fatal — the script warns and continues, allowing the site to build with an existing `output.css`. This keeps the engine free of Node and npm.

### `dotnet run --project` remains valid

Running `cd ../my-blog && dotnet run --project ../happyfrogger` directly still works and is documented. `build.sh` is a convenience wrapper, not a gate.

---

## Consequences

**Positive:**
- Single command for the common case; no need to remember to `cd` first.
- `--blog` flag enables CI and non-sibling directory layouts without editing the script.
- Pass-through `-- <args>` allows `--serve`, `--drafts`, `new`, etc. without a separate invocation pattern.
- Tailwind is run in the right order (before dotnet) without a separate shell step.

**Negative / watch items:**
- The script assumes a Unix shell (`#!/usr/bin/env bash`). Windows users must run via WSL or use the `dotnet run --project` form directly.
- If the blog directory is not a sibling of the engine (`../blog`), the `--blog` flag or `BLOG_DIR` environment variable must be used.
- `build.sh` is not on `PATH` by default; the caller must be in the engine directory or use an absolute path.
