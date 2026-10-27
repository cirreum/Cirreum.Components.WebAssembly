# Cirreum.Components.WebAssembly 2.0.0 — the interop contract says what the JavaScript does

## Why this release exists

The component library talks to the browser through one ES module, `cirreum-interop.js`, and one
C# consumer of it in the runtime. The two sides had drifted apart in ways that nothing checked:
a `string` id on one side and an `int` on the other, a rest parameter receiving a whole array as
one token, a rect declared non-null that was `null` whenever the element was missing. None of
it threw. Each mismatch produced a quiet no-op, a wrong class name, or a position a few pixels
off, and every one was gated on an input the consuming applications happen not to produce.

The review that found them also found what they hide. A context menu opened from inside a
scrolled, focused container landed below its row. A listener for the right mouse button
suppressed the default action of every left click inside its element. A tap on a touch device
raised a left-button callback twice. A fresh browser never followed the system theme until the
user chose a mode by hand. And two older regressions that had been living behind the same
silence: a keyboard-activated menu opening at the page origin, and the DataGrid column chooser
whose toggle had been dead since a clean-up in November 2025.

The 2.0 break makes the contract say what the JavaScript does, and ships the fixes behind it.

## What changed

**Breaking, in the interop contract:**

- `IThemeModeChangedRef.ModeChanged(bool isDarkMode)` drops the `storedMode` parameter. The
  JavaScript reports the system's preference only; whether the application follows it is a
  decision for the side that holds the user's choice.
- `IJSAppModule.ForceUpdatePopperJS(string id)` takes the id `ShowPopperJS` returns.
- `GetBoundingClientRect` and `GetAccurateBoundingClientRect` return `DomRect?`.
- For direct importers of the module: `addElementClassIfScrollbar` and `removeElementClasses`
  take `string[]`; `monitorSystemThemeMode` invokes `OnModeChanged(isDark)`;
  `getFormattedSamples` is removed.

**Fixed, in the JavaScript:**

- The mouse button listener consumes only its own button's events, raises once per tap on
  touch, and treats a long press as a single context-menu event.
- `getScrollPosition` returns the document scroll. A viewport-relative rect already reflects
  inner scroll containers; adding them again is what displaced the menus.
- `getAccurateBoundingClientRect` no longer adds a CSS transform that the rect already includes.
- Breakpoint re-registration replaces the listener instead of stacking one; stylesheet
  lookups escape the href; .NET callback failures reach the console instead of dying as
  unhandled rejections; scrollbar detection no longer injects a `<style>` element.

**Fixed, in the components:**

- `Menu` opened from the keyboard, where the click carries zero coordinates, opens below its
  trigger.
- The DataGrid column chooser toggles, shows as active while open, and closes cleanly. The
  chooser tracks its own open state rather than subscribing to the popover, because that
  subscription re-rendered the popover from its own after-render while Popper was still
  placing it. `Popover.Close` now removes its own markup, so a caller outside the popover's
  render tree does not leave an un-positioned box behind.

## What didn't change

Applications that use the components through `Cirreum.Runtime.Wasm` and never touch
`IJSAppModule`, `IThemeModeChangedRef` or the module directly compile unchanged; the runtime
implements the new contract. Every component's public surface is as it was. The `'unsafe-inline'`
style baseline is untouched; removing the injected `<style>` element only clears the way for a
later `style-src-attr` split.

## Upgrading

Implementations of `IThemeModeChangedRef` drop the second parameter. Callers of the rect
accessors handle `null`, which they were already receiving. Callers of `ForceUpdatePopperJS`
pass the string id. See [MIGRATION-v2.md](MIGRATION-v2.md) for the before/after of each.

Four behaviour changes are worth a glance even where nothing fails to compile: left clicks
inside a right-button listener's element regain their default action; touch raises once;
`GetScrollPosition` no longer includes inner containers; the "accurate" rect no longer
double-counts transforms. An application that compensated for any of these should remove the
compensation.

## See also

- `Cirreum.Runtime.Wasm` 4.1.0 — implements this contract, fixes the consumer-side defects
  (footer height as `double`, theme follow on a fresh browser), and adds the contract test that
  checks every invoked identifier against this module's exports.
