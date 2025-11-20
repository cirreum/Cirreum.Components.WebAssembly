# Cirreum.Components.WebAssembly

[![NuGet Version](https://img.shields.io/nuget/v/Cirreum.Components.WebAssembly.svg?style=flat-square&labelColor=1F1F1F&color=003D8F)](https://www.nuget.org/packages/Cirreum.Components.WebAssembly/)
[![NuGet Downloads](https://img.shields.io/nuget/dt/Cirreum.Components.WebAssembly.svg?style=flat-square&labelColor=1F1F1F&color=003D8F)](https://www.nuget.org/packages/Cirreum.Components.WebAssembly/)
[![GitHub Release](https://img.shields.io/github/v/release/cirreum/Cirreum.Components.WebAssembly?style=flat-square&labelColor=1F1F1F&color=FF3B2E)](https://github.com/cirreum/Cirreum.Components.WebAssembly/releases)
[![License](https://img.shields.io/github/license/cirreum/Cirreum.Components.WebAssembly?style=flat-square&labelColor=1F1F1F&color=F2F2F2)](https://github.com/cirreum/Cirreum.Components.WebAssembly/blob/main/LICENSE)
[![.NET](https://img.shields.io/badge/.NET-10.0-003D8F?style=flat-square&labelColor=1F1F1F)](https://dotnet.microsoft.com/)

A complete, production-ready UI component library for **Blazor WebAssembly**, built as part of the Cirreum application framework.

## Overview

**Cirreum.Components.WebAssembly** provides a comprehensive suite of reusable, theme-aware UI components used across real-world enterprise applications.  
It includes layout primitives, dialogs, selectors, behaviors, authentication helpers, utilities, and a full Bootstrap-based theming system featuring **light/dark mode** and **color schemes**.

---

## Features

### Complete Component Suite

Over **25+ professional UI components**, including:

- Accordion  
- BreakpointMonitor  
- Checkbox  
- ClickDetector  
- Collapse  
- ContentLoader  
- CopyToClipboard  
- CounterBadge  
- DataGrid  
- Dialog  
- Dropdown  
- FileSelection  
- FocusTrap  
- Menu  
- MultipleSelector  
- NavBar  
- Popover  
- Progress  
- Prompts  
- Radio  
- ScrollbarBehavior  
- ScrollToTop  
- Spinners  
- Switch  
- Tabs  

### Authentication & Utility Components

> **Note:** Authentication components are designed specifically for Blazor WebAssembly authentication flows.

- **LoginButton** - Quick authentication trigger  
- **RedirectToLogin** / **RedirectToUnauthorized** - Navigation helpers  
- **ClientAuthenticationView** - Remote authentication component that automatically captures user identity and tenant information on login/logout
- **ITelemetryUserContext** - Abstraction for propagating authenticated user context to any telemetry provider (Application Insights, OpenTelemetry, etc.)
- Auth utility helpers  

### Bootstrap-Based Theme System

Cirreum includes a **production-grade SCSS theming pipeline** with multiple pre-compiled color schemes:

- `cirreum-bootstrap-base.scss`  
- `cirreum-bootstrap-default.scss`  
- `cirreum-bootstrap-aqua.scss`  
- `cirreum-bootstrap-aspire.scss`  
- `cirreum-bootstrap-excel.scss`  
- `cirreum-bootstrap-office.scss`  
- `cirreum-bootstrap-outlook.scss`  
- `cirreum-bootstrap-windows.scss`  
- `cirreum-spinners.scss`

All themes support:

- Light mode  
- Dark mode  
- Auto (system theme)  
- CSS variable overrides  
- Extensible SCSS customization  

### Modern UX Behaviors

- Click-outside detection  
- Focus trapping  
- Responsive breakpoint monitoring  
- Scroll position & visibility behaviors  
- Smooth navigation utilities  
- Element metrics and interop helpers  

---

## Customizing Dialog Scrollable Margins

The `<Dialog>` component provides CSS custom properties for vertical spacing when content becomes scrollable.

| Variable                             | Description                      | Default |
|-------------------------------------|----------------------------------|---------|
| `--dialog-scrollable-margin-top`    | Top margin in scrollable mode    | `64px`  |
| `--dialog-scrollable-margin-bottom` | Bottom margin in scrollable mode | `64px`  |

### Example

```css
.my-custom-dialog {
  @extend .dialog;
  --dialog-scrollable-margin-top: 80px;
  --dialog-scrollable-margin-bottom: 40px;
}
```

Usage:

```razor
<Dialog class="my-custom-dialog">
    <!-- dialog content -->
</Dialog>
```

---

## Installation

```bash
dotnet add package Cirreum.Components.WebAssembly
```

---

## Getting Started

Import the namespace:

```razor
@using Cirreum.Components.WebAssembly
```

Reference a color theme stylesheet:

```html
<link href="css/cirreum-bootstrap-default.css" rel="stylesheet" />
```

Use any component:

```razor
<Dialog Title="Welcome">
    <p>Hello from Cirreum.</p>
</Dialog>
```

---

## Component Groups

### Layout & Navigation

- Accordion  
- Collapse  
- Tabs  
- NavBar  
- Menu  

### Inputs & Selectors

- Checkbox  
- Radio  
- Switch  
- MultipleSelector  
- FileSelection  

### Feedback & Overlays

- Dialog  
- Popover  
- Progress  
- Prompts  
- Spinners  

### Utilities & Behavior

- BreakpointMonitor  
- FocusTrap  
- ScrollToTop  
- ScrollbarBehavior  
- ContentLoader  
- CopyToClipboard  
- ClickDetector  

### Data Display

- DataGrid  
- CounterBadge  

### Authentication Helpers

- LoginButton  
- RedirectToLogin  
- RedirectToUnauthorized  
- ClientAuthenticationView  

---

## Roadmap

- Full documentation website  
- Interactive live demo gallery  
- Cirreum icon pack  
- Advanced DataGrid extensions  
- Theme builder UI  
- A11y enhancements across all components  

---

## About Cirreum

Cirreum.Components.WebAssembly is part of the larger **Cirreum application framework**, a modern .NET ecosystem focused on simplicity, layered design, and production-ready patterns across:

- Authorization  
- Messaging  
- Conductor (dispatcher)  
- Hosting  
- Configuration  
- Diagnostics  
- Client behavior  

---

**Cirreum Foundation Framework**  
*Layered simplicity for modern .NET*