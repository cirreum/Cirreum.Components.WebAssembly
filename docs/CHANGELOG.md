# Cirreum.Components.WebAssembly Changelog

All notable changes to **Cirreum.Components.WebAssembly** are documented in this file.

The format is based on [Keep a Changelog](https://keepachangelog.com/en/1.1.0/),
and this project adheres to [Semantic Versioning](https://semver.org/spec/v2.0.0.html).

## [Unreleased]

### Breaking

- **`IThemeModeChangedRef.ModeChanged` takes only `isDarkMode`.** The JavaScript monitor no
  longer reads the stored mode preference and passes it along; it reports the system change
  and the .NET side, which already holds the user's preference, decides whether to follow it.
  On a fresh browser nothing is stored, so the old callback compared `null` to `"auto"` and
  ignored every system change until the user picked a mode explicitly. Implementations drop the
  second parameter. See `docs/MIGRATION-v2.md`.
- **`IJSAppModule.ForceUpdatePopperJS` is removed**, with the module's `updatePopper` export.
  `ShowPopperJS` returns a string id and `ClosePopperJS` takes one; the force-update member took
  an `int`, so the JavaScript map lookup could never match and the call was a silent no-op.
  Nothing in the framework or its consumers called it. A member nothing exercises is removed
  rather than corrected; a popover that needs a forced reposition brings it back with its
  consumer.
- **The four bounding-rect accessors return `DomRect?`.** The JavaScript returned `null` for a
  missing element throughout; the C# signatures said otherwise. `Menu` already null-checked.
- **Module-level JavaScript signatures.** For anyone importing `cirreum-interop.js` directly:
  `addElementClassIfScrollbar` and `removeElementClasses` take a `string[]` instead of a rest
  parameter (the .NET side passes an array as one argument, so a rest parameter received the
  array as a single token and two classes became `a,b`); `monitorSystemThemeMode` invokes
  `OnModeChanged(isDark)` with one argument; `getFormattedSamples` is removed, since it was
  reachable only by importing the module (the demo defines its own global of that name).

### Fixed

- **A link inside a DataGrid cell navigates** ([#3](https://github.com/cirreum/Cirreum.Components.WebAssembly/issues/3)).
  Every data row carried `@onclick:preventDefault`, so a click on an `<a>` in a cell bubbled to
  the row, had its default action cancelled, and Blazor's link interception never ran; the same
  applied to a checkbox or label a consumer placed in a cell. A `<tr>` has no default action of
  its own, and nothing the grid renders inside a row depends on the cancellation, so it is
  removed. The row still stops propagation and still handles the click, so `OnRowClicked` and
  row selection fire for a link click as for any other; the row cannot see the click's target
  from C#, and stopping propagation in JavaScript for anchors would also stop the click
  reaching Blazor's own navigation listener. Consumers who added `@onclick:stopPropagation` to
  their anchors as a workaround can remove it.
- **A keyboard-activated `Menu` opens at its trigger.** Enter or Space on the trigger raises a
  click with zero coordinates. `ShowAsync` already fetched the trigger's rect for that case but
  only used it when `Anchored` was set, so an unanchored menu opened at the page origin. It now
  opens below the trigger, left-aligned, whenever the pointer position is unusable.
- **The DataGrid column-chooser button toggles again.** A 2025-11-23 clean-up removed the
  popover `OnOpenChanged` handler that set `IsPopoverOpen`, so the flag was never true: a second
  click reopened instead of closing and the button never showed as active. The chooser now
  tracks the state itself in `ShowChooser`/`HideChooser`, without subscribing to the popover:
  the old callback fired from the popover's after-render, re-rendered the chooser, whose
  after-render re-rendered the popover while Popper was still applying its initial position.
  That after-render update of the popover is removed with it. The same clean-up had dropped
  the re-render from `Popover.Close()`, so a popover closed by a caller outside its render tree
  (the chooser renders its popover inside the grid) kept its un-positioned markup until
  something else re-rendered; `Close()` re-renders the popover again.
- **Mouse button listener no longer consumes clicks it is not for.** `handleClick` called
  `preventDefault` before checking the button, so a right-button listener on a container
  suppressed the default action of every left click inside it (links, checkboxes, labels).
  Only the trigger button's events are prevented and stopped now. On touch, a left-button
  trigger raised the callback on `touchstart` and again on the browser's synthesized `click`;
  it now relies on the click alone. A right-button long press suppresses the native
  `contextmenu`/`click` the same touch produces, so it raises once, and `touchmove` cancels it.
- **`getScrollPosition` returns the document scroll only.** It added the scroll offsets of
  every scrollable ancestor of the focused element, which `Menu` then added to a
  viewport-relative rect that already reflects inner scrolling. A trigger inside a scrolled,
  focused container positioned its menu off by that container's scroll.
- **`getAccurateBoundingClientRect` no longer double-counts transforms.** It added the CSS
  transform's translation to a rect that already included it, and shifted `Left`/`Top` while
  leaving `X`/`Y` raw. Position now comes from the rendered rect as is; size from the layout
  box, which is the distinction the function exists for.
- **`monitorBreakpointChanges` replaces an existing listener for the same breakpoint** instead
  of stacking a second one and dropping the first's abort controller.
- **Stylesheet lookups escape the href** (`CSS.escape`) before building an attribute selector.
- **.NET callback failures are reported to the console** rather than surfacing as unhandled
  promise rejections attributed to no caller.
- `MouseButton` and `MouseButtonEventType` use `JsonStringEnumMemberName`; the
  `JsonPropertyName` attributes they carried are not honoured on enum members and the names
  were matching by case-insensitive fallback.

### Changed

- **Scrollbar detection no longer injects a `<style>` element.** The probe changed nothing the
  subsequent read depended on, and an injected style element is blocked under a policy that
  separates `style-src-elem` from `style-src-attr`. This removes the framework's only
  obstacle to that hardening.

## [1.0.51] - 2026-10-25

### Updated

- Updated NuGet packages.

## [1.0.50] - 2026-08-23

### Updated

- Updated NuGet packages.

## [1.0.49] - 2026-08-20

### Updated

- Updated NuGet packages.

## [1.0.48] - 2026-08-18

### Updated

- Updated NuGet packages.

## [1.0.47] - 2026-08-16

### Updated

- Updated NuGet packages.

## [1.0.46] - 2026-08-03

### Updated

- Updated NuGet packages.

## [1.0.45] - 2026-07-29

### Updated

- Updated NuGet packages.

## [1.0.44] - 2026-07-20

### Updated

- Updated NuGet packages.
- Updated TypeScript to use latest version `7.0.0` and ES2025

## [1.0.43] - 2026-07-18

### Updated

- Updated NuGet packages.

## [1.0.40] - 2026-07-04

### Updated

- Updated NuGet packages.

## [1.0.39] - 2026-07-04

### Updated

- Updated NuGet packages.

## [1.0.38] - 2026-06-05

### Fixed

- Updated dependencies to their latest versions to pick up upstream fixes:
  - `Cirreum.Startup` `1.0.116` → `1.0.117`
  - `AspNetCore.SassCompiler` `1.99.0` → `1.100.0`
  - `Microsoft.AspNetCore.Components.WebAssembly.Authentication` `10.0.7` → `10.0.8`

## [1.0.37]

Baseline entry. Prior history predates this changelog.
