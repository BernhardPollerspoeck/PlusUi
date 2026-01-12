---
title: Menu
layout: default
parent: Controls
nav_order: 180
---

# Menu

A horizontal menu bar control for application menus. Displays top-level menu items that open dropdown menus when clicked.

---

## Basic Usage

```csharp
new Menu()
    .AddItem(new MenuItem()
        .SetText("File")
        .AddItem(new MenuItem().SetText("New").SetShortcut("Ctrl+N").SetCommand(vm.NewCommand))
        .AddItem(new MenuItem().SetText("Open").SetShortcut("Ctrl+O").SetCommand(vm.OpenCommand))
        .AddSeparator()
        .AddItem(new MenuItem().SetText("Exit").SetCommand(vm.ExitCommand)))
    .AddItem(new MenuItem()
        .SetText("Edit")
        .AddItem(new MenuItem().SetText("Cut").SetShortcut("Ctrl+X"))
        .AddItem(new MenuItem().SetText("Copy").SetShortcut("Ctrl+C"))
        .AddItem(new MenuItem().SetText("Paste").SetShortcut("Ctrl+V")))
```

---

## Menu Methods

| Method | Description |
|:-------|:------------|
| `AddItem(MenuItem)` | Adds a top-level menu item |
| `AddSeparator()` | Adds a visual separator |
| `SetHoverBackgroundColor(Color)` | Sets hover background color |
| `BindHoverBackgroundColor(getter)` | Binds hover color |
| `SetActiveBackgroundColor(Color)` | Sets active/open item background |
| `BindActiveBackgroundColor(getter)` | Binds active color |
| `SetTextColor(Color)` | Sets menu text color |
| `BindTextColor(getter)` | Binds text color |
| `SetTextSize(float)` | Sets menu text size |
| `BindTextSize(getter)` | Binds text size |

---

## MenuItem Methods

MenuItem represents a single entry in a menu.

| Method | Description |
|:-------|:------------|
| `SetText(string)` | Sets the display text |
| `BindText(getter)` | Binds text |
| `SetIcon(string)` | Sets icon image path |
| `BindIcon(getter)` | Binds icon |
| `SetShortcut(string)` | Sets keyboard shortcut text (e.g., "Ctrl+S") |
| `BindShortcut(getter)` | Binds shortcut |
| `SetCommand(ICommand)` | Sets command to execute on click |
| `BindCommand(getter)` | Binds command |
| `SetCommandParameter(object)` | Sets command parameter |
| `BindCommandParameter(getter)` | Binds command parameter |
| `SetIsEnabled(bool)` | Enables/disables the item |
| `BindIsEnabled(getter)` | Binds enabled state |
| `AddItem(MenuItem)` | Adds a submenu item |
| `AddSeparator()` | Adds a separator in the submenu |

---

## Examples

### Application Menu Bar

```csharp
new VStack()
    .AddChildren(
        new Menu()
            .SetHoverBackgroundColor(new Color(60, 60, 60))
            .AddItem(new MenuItem()
                .SetText("File")
                .AddItem(new MenuItem().SetText("New Project").SetShortcut("Ctrl+Shift+N"))
                .AddItem(new MenuItem().SetText("Open Project").SetShortcut("Ctrl+Shift+O"))
                .AddSeparator()
                .AddItem(new MenuItem().SetText("Save All").SetShortcut("Ctrl+Shift+S"))
                .AddSeparator()
                .AddItem(new MenuItem().SetText("Exit").SetCommand(vm.ExitCommand)))
            .AddItem(new MenuItem()
                .SetText("View")
                .AddItem(new MenuItem().SetText("Zoom In").SetShortcut("Ctrl++"))
                .AddItem(new MenuItem().SetText("Zoom Out").SetShortcut("Ctrl+-"))),
        // ... rest of app content
    )
```

### Nested Submenus

```csharp
new MenuItem()
    .SetText("Recent Files")
    .AddItem(new MenuItem().SetText("Document1.txt").SetCommand(vm.OpenRecentCommand).SetCommandParameter("doc1.txt"))
    .AddItem(new MenuItem().SetText("Document2.txt").SetCommand(vm.OpenRecentCommand).SetCommandParameter("doc2.txt"))
    .AddSeparator()
    .AddItem(new MenuItem().SetText("Clear Recent"))
```

### Data-Bound Menu

```csharp
new MenuItem()
    .SetText("Window")
    .BindIsEnabled(() => vm.HasOpenWindows)
    .AddItem(new MenuItem().SetText("Close All").SetCommand(vm.CloseAllCommand))
```

{: .note }
> Menu has `AccessibilityRole.Menu` by default. Menu items are keyboard navigable with arrow keys.
