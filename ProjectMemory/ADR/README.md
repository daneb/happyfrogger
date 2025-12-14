# Architecture Decision Records (ADRs)

This directory contains Architecture Decision Records for the HappyFrogger static site generator.

## Index

| ADR | Title | Status | Date | Version |
|-----|-------|--------|------|---------|
| [001](001-rss-feed-implementation.md) | RSS Feed Implementation | Accepted | 2025-12-14 | 2.1.0 |
| [002](002-reading-time-calculation.md) | Reading Time Calculation | Accepted | 2025-12-14 | 2.2.0 |
| [003](003-social-media-meta-tags.md) | Social Media Meta Tags | Accepted | 2025-12-14 | 2.3.0 |
| [004](004-sitemap-generation.md) | Sitemap.xml Generation | Accepted | 2025-12-14 | 2.4.0 |
| [005](005-table-of-contents-generation.md) | Table of Contents Auto-Generation | Accepted | 2025-12-14 | 2.5.0 |

## What is an ADR?

An Architecture Decision Record (ADR) is a document that captures an important architectural decision made along with its context and consequences.

## ADR Format

Each ADR follows this structure:

- **Status**: Accepted, Proposed, Deprecated, or Superseded
- **Date**: When the decision was made
- **Deciders**: Who was involved
- **Technical Story**: Brief context
- **Context and Problem Statement**: What problem are we solving?
- **Decision Drivers**: What factors influenced the decision?
- **Considered Options**: What alternatives were evaluated?
- **Decision Outcome**: What was chosen and why?
- **Consequences**: Positive, negative, and neutral outcomes

## Creating a New ADR

1. Copy the template from an existing ADR
2. Number it sequentially (e.g., 005-feature-name.md)
3. Fill in all sections
4. Update this index
5. Commit with the feature implementation

## References

- [ADR GitHub Organization](https://adr.github.io/)
- [Architectural Decision Records](https://cognitect.com/blog/2011/11/15/documenting-architecture-decisions)
