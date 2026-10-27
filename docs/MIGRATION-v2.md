# Migrating to Cirreum.Components.WebAssembly 2.0

The 2.0 break is confined to the JavaScript interop contract. Applications that use the
components and never touch `IJSAppModule`, `IThemeModeChangedRef` or the
`cirreum-interop.js` module directly compile unchanged.

## Does this affect me?

| Situation | Action |
| --- | --- |
| App uses components only, through `Cirreum.Runtime.Wasm` | None. The runtime implements the new contract. |
| App implements `IThemeModeChangedRef` | Drop the second parameter (below). |
| App calls `GetBoundingClientRect` / `GetAccurateBoundingClientRect` | Handle `null` (below). |
| App calls `ForceUpdatePopperJS` | Remove the call (below). |
| App imports `cirreum-interop.js` from its own JavaScript | Review the module signatures (below). |

## `IThemeModeChangedRef.ModeChanged`

```csharp
// 1.x
Task ModeChanged(bool isDarkMode, string storedMode);

// 2.0
Task ModeChanged(bool isDarkMode);
```

The monitor reports the system's preference only. Decide whether to follow it from the
application's own theme state (`IThemeState.Mode`), which is where the user's choice already
lives. The old `storedMode` was read from `localStorage` on the JavaScript side and was `null`
on any browser that had never stored a preference, which silently disabled the callback.

## Bounding rects are nullable

```csharp
// 1.x: declared non-null, returned null at runtime for a missing element
DomRect rect = js.GetBoundingClientRect("#menu");

// 2.0
DomRect? rect = js.GetBoundingClientRect("#menu");
if (rect is null) {
    return;
}
```

Behaviour is unchanged; the annotation now matches it.

## `ForceUpdatePopperJS`

Removed, together with the module's `updatePopper` export. It took an `int` where the popper
ids are strings, so no call ever reached a Popper instance; removing the call changes nothing
at runtime. A popover that needs a forced reposition after its content changes size should ask
for the member to return with that use case, rather than keep a corrected signature nothing
exercises.

## Module signatures (direct JavaScript consumers only)

| Export | 1.x | 2.0 |
| --- | --- | --- |
| `addElementClassIfScrollbar(selector, vertical, ...tokens)` | rest parameter | `tokens: string[]` |
| `removeElementClasses(element, ...tokens)` | rest parameter | `tokens: string[]` |
| `monitorSystemThemeMode` callback | `OnModeChanged(isDark, storedMode)` | `OnModeChanged(isDark)` |
| `getFormattedSamples()` | exported | removed |
| `updatePopper(id)` | exported | removed |

## Behaviour changes to be aware of

These are fixes, not contract changes, but they alter what a working path does:

- A `MouseButtonListener` for one button no longer calls `preventDefault` on clicks of other
  buttons. A right-button context menu on a container stops suppressing left-click defaults
  inside it.
- On touch devices a left-button listener raises once per tap, not twice.
- `GetScrollPosition` returns `window.scrollX`/`scrollY` only. If an application added inner
  container offsets itself to compensate, remove that.
- `GetAccurateBoundingClientRect` no longer adds a transform's translation on top of a rect
  that already includes it.
