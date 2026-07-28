# Backlog

Deferred work for **Cirreum.Components.WebAssembly**. Items here are tracked but not yet ready
to ship — either because the cost outweighs the benefit in isolation, or
because they're waiting on a forcing function (a related change, a consumer
upgrade, a coordinated multi-repo rollout).

## How this file works

- Each item is a `###` heading so it can be linked to and parsed.
- Each item declares **`SemVer:`** (`Patch` | `Minor` | `Major` | `Unspecified`),
  **`Trigger:`** (the human-readable condition that will make it ready), and
  **`Noted:`** (the date the item was added). Write them one per line, with or
  without a leading `- ` bullet — both parse:

  ```markdown
  ### A short imperative title

  **SemVer:** Patch
  **Trigger:** What will make this ready.
  **Noted:** 2026-01-01

  What the work is and why it was deferred.
  ```
- The Cirreum DevOps release scripts (`PatchRelease`, `MinorRelease`,
  `MajorRelease`) surface items at-or-below the requested bump level so the
  operator can decide whether to fold them in before tagging.
- Items that ship: move from this file to `docs/CHANGELOG.md` under
  `[Unreleased]`. Items that grow into design discussions: promote to an ADR.

- **File cross-repo work in the repo where it *starts*** — the lowest layer in the
  chain — not only where the symptom is felt. Releases climb bottom-up, so an item
  filed only at the top surfaces after every rung it depends on has already shipped.

## Queued
