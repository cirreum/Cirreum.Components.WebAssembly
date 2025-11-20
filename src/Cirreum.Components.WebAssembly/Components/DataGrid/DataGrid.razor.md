# DataGrid

## Table Layout Strategy

### Layout Decision Logic

The DataGrid uses `table-layout: fixed` when either resizable columns OR responsive mode is enabled, otherwise uses `table-layout: auto`.

```csharp
private string TableLayoutStyle => 
    (this.ResizableColumns || this.Responsive)
        ? "table-layout: fixed;" 
        : "table-layout: auto;";
```

### Business Rules

| Resizable | Responsive | Layout | Why |
|-----------|------------|--------|-----|
| No | No | `auto` | Pure browser control |
| Yes | No | `fixed` | User needs resize control |
| No | Yes | `fixed` | Need controlled scrolling |
| Yes | Yes | `fixed` | User control + scrolling |

### Key Behaviors

#### Auto Layout (table-layout: auto)

- Browser calculates optimal column widths based on content
- Columns resize automatically when content changes
- CSS width values are treated as suggestions, not rules
- Table grows/shrinks based on available content

#### Fixed Layout (table-layout: fixed)

- Explicit column widths are honored precisely
- Enables text truncation with text-overflow: ellipsis
- Required for predictable column resizing behavior
- Required for controlled horizontal scrolling in responsive containers
- Prevents auto-redistribution of column space
- Double Click to auto-size the column 
- Ctrl+Double Click to auto-size all columns

### Notes

- Row details feature typically pairs with responsive tables, automatically getting fixed layout
- Fixed layout is essential for truncating content wider than column width
- Auto layout provides the most natural behavior when no user control is needed