# cirreum-bootstrap Styles

`cirreum-bootstrap` provides a fully customized **Bootstrap 5.3+** theme system designed for Blazor applications.  
It includes multiple **pre-compiled theme variants**, deep integration with Bootstrap's CSS variables, and automatic **light/dark mode** support through `data-bs-theme`.

All Cirreum themes:
- Maintain full Bootstrap component structure  
- Override variables (not raw CSS overrides)  
- Support both light & dark modes  
- Integrate seamlessly with Cirreum.Blazor.Components  
- Serve as drop-in replacements for `bootstrap.min.css`  

---

## 🎨 Available Themes

Each theme offers a unique color palette while preserving consistent component behavior.

| Theme | Primary | Secondary | Best For |
|-------|---------|-----------|----------|
| **Aspire** | `#512bd4` | `#2d3748` | Modern .NET apps, dashboards, tooling |
| **Default** | `#2c3e50` | `#95a5a6` | Clean professional look (Flatly-inspired) |
| **Excel** | `#217346` | `#605e5c` | Data-heavy apps, spreadsheet-like UIs |
| **Office** | `#d83b01` | `#605e5c` | Office 365-style branding |
| **Outlook** | `#0f6cbd` | `#605e5c` | Email & communication interfaces |
| **Windows** | `#0078d4` | `#605e5c` | Microsoft-styled enterprise UIs |
| **Aqua** | `#007AFF` | `#8E8E93` | macOS-style interfaces, Apple ecosystem apps |

---

## 📦 Usage

Include **one** theme CSS file in your Blazor application:

    <!-- Default -->
    <link href="_content/YourLibrary/css/cirreum-bootstrap-default.css" rel="stylesheet" />

    <!-- Microsoft-inspired themes -->
    <link href="_content/YourLibrary/css/cirreum-bootstrap-aspire.css" rel="stylesheet" />
    <link href="_content/YourLibrary/css/cirreum-bootstrap-excel.css" rel="stylesheet" />
    <link href="_content/YourLibrary/css/cirreum-bootstrap-office.css" rel="stylesheet" />
    <link href="_content/YourLibrary/css/cirreum-bootstrap-outlook.css" rel="stylesheet" />
    <link href="_content/YourLibrary/css/cirreum-bootstrap-windows.css" rel="stylesheet" />

    <!-- Apple-inspired themes -->
    <link href="_content/YourLibrary/css/cirreum-bootstrap-aqua.css" rel="stylesheet" />

Only load *one* theme per application to prevent conflicting variable scopes.

---

## 🌗 Dark Mode Support

All themes fully support Bootstrap 5.3's built-in light/dark mode.

    <!-- Light -->
    <html data-bs-theme="light">

    <!-- Dark -->
    <html data-bs-theme="dark">

You can toggle this at runtime using JavaScript or Blazor interop.

**Note:** The **Aqua** theme uses macOS's native dark mode color palette, with brighter, more vibrant colors for improved contrast on dark backgrounds—matching the experience of native macOS apps.

---

## 📁 Project Structure

    Styles/
    ├── bootstrap/
    │   ├── themes/
    │   │   ├── _theme-default.scss
    │   │   ├── _theme-aspire.scss
    │   │   ├── _theme-excel.scss
    │   │   ├── _theme-office.scss
    │   │   ├── _theme-outlook.scss
    │   │   ├── _theme-windows.scss
    │   │   ├── _theme-aqua.scss
    │   ├── _cirreum-variables.scss
    │   ├── _cirreum-bootstrap.scss
    │   ├── variables.scss
    │   └── variables-dark.scss
    ├── _cirreum-bootstrap-base.scss
    ├── cirreum-bootstrap-aspire.scss
    ├── cirreum-bootstrap-default.scss
    ├── cirreum-bootstrap-excel.scss
    ├── cirreum-bootstrap-office.scss
    ├── cirreum-bootstrap-outlook.scss
    ├── cirreum-bootstrap-windows.scss
    ├── cirreum-bootstrap-aqua.scss
    └── cirreum-spinners.scss

### Key Files

- **_cirreum-bootstrap-base.scss**  
  Shared imports, Bootstrap core, variable overrides, theme scaffolding.

- **cirreum-bootstrap-*.scss**  
  One entry file per theme. Compiles into a standalone CSS theme.

- **variables.scss / variables-dark.scss**  
  Bootstrap variable palette for both modes.

- **_theme-aqua.scss**  
  macOS Big Sur-inspired color palette with authentic system colors for light and dark modes.

---

## ⚙️ Building Themes

Themes are compiled automatically using **AspNetCore.SassCompiler** according to the configuration in:

    sasscompiler.json

Each theme SCSS entry builds into:

    cirreum-bootstrap-{theme}.css
    cirreum-bootstrap-{theme}.min.css

on build or publish.

---

## 🎨 Theme Design Philosophy

### Microsoft-Inspired Themes
The Aspire, Excel, Office, Outlook, and Windows themes draw from Microsoft's Fluent Design System and Office product palettes, providing familiar experiences for enterprise and productivity applications.

### Apple-Inspired Themes
The Aqua theme replicates macOS Big Sur's native system colors, including:
- **iOS/macOS blue** (`#007AFF` / `#0A84FF`) for primary actions
- **System green, orange, and red** for semantic colors
- **Subtle backgrounds** with low-opacity overlays
- **Vibrant dark mode colors** matching native macOS apps

---

## ✔ Summary

`cirreum-bootstrap` delivers:
- Clean, professional Bootstrap-based themes
- Full dark mode support
- Drop-in integration with Cirreum components
- Multiple Microsoft and Apple-inspired palettes
- Cross-platform design language support
- Easy theming and future customization

---