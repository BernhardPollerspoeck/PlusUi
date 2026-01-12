---
title: ContextMenu
layout: default
parent: Controls
nav_order: 185
---

# ContextMenu

A popup context menu that can be attached to any UI element. Opens on right-click (desktop) or long-press (touch devices).

---

## Basic Usage

```csharp
// Attach to any element
myElement.SetContextMenu(new ContextMenu()
    .AddItem(new MenuItem().SetText("Cut").SetShortcut("Ctrl+X").SetCommand(vm.CutCommand))
    .AddItem(new MenuItem().SetText("Copy").SetShortcut("Ctrl+C").SetCommand(vm.CopyCommand))
    .AddItem(new MenuItem().SetText("Paste").SetShortcut("Ctrl+V").SetCommand(vm.PasteCommand))
    .AddSeparator()
    .AddItem(new MenuItem().SetText("Delete").SetCommand(vm.DeleteCommand)))

// Using builder pattern
myElement.SetContextMenu(menu => menu
    .AddItem(new MenuItem().SetText("Refresh").SetCommand(vm.RefreshCommand))
    .AddItem(new MenuItem().SetText("Properties").SetCommand(vm.PropertiesCommand)))
```

---

## ContextMenu Methods

| Method | Description |
|:-------|:------------|
| `AddItem(MenuItem)` | Adds a menu item |
| `AddItem(MenuSeparator)` | Adds a separator |
| `AddSeparator()` | Adds a visual separator |
| `SetHoverBackgroundColor(Color)` | Sets item hover background |
| `BindHoverBackgroundColor(getter)` | Binds hover color |
| `SetTextColor(Color)` | Sets menu text color |
| `BindTextColor(getter)` | Binds text color |
| `SetCornerRadius(float)` | Sets menu corner radius |
| `BindCornerRadius(getter)` | Binds corner radius |
| `SetBorderColor(Color)` | Sets menu border color |
| `BindBorderColor(getter)` | Binds border color |
| `SetBorderWidth(float)` | Sets border width |
| `BindBorderWidth(getter)` | Binds border width |

---

## Attaching to Elements

Any `UiElement` can have a context menu via extension methods:

| Method | Description |
|:-------|:------------|
| `SetContextMenu(ContextMenu)` | Sets context menu instance |
| `SetContextMenu(Action<ContextMenu>)` | Configures via builder callback |

---

## Examples

### File Explorer Context Menu

```csharp
fileListItem.SetContextMenu(new ContextMenu()
    .AddItem(new MenuItem().SetText("Open").SetCommand(vm.OpenCommand))
    .AddItem(new MenuItem().SetText("Open With...")
        .AddItem(new MenuItem().SetText("Notepad"))
        .AddItem(new MenuItem().SetText("VS Code")))
    .AddSeparator()
    .AddItem(new MenuItem().SetText("Cut").SetShortcut("Ctrl+X"))
    .AddItem(new MenuItem().SetText("Copy").SetShortcut("Ctrl+C"))
    .AddSeparator()
    .AddItem(new MenuItem().SetText("Delete").SetIcon("delete.png"))
    .AddItem(new MenuItem().SetText("Rename").SetShortcut("F2"))
    .AddSeparator()
    .AddItem(new MenuItem().SetText("Properties").SetShortcut("Alt+Enter")))
```

### Styled Context Menu

```csharp
element.SetContextMenu(new ContextMenu()
    .SetBackground(new Color(30, 30, 30))
    .SetHoverBackgroundColor(new Color(50, 80, 120))
    .SetTextColor(Colors.White)
    .SetCornerRadius(4)
    .SetBorderColor(new Color(60, 60, 60))
    .SetBorderWidth(1)
    .AddItem(new MenuItem().SetText("Action 1"))
    .AddItem(new MenuItem().SetText("Action 2")))
```

### Dynamic Context Menu

```csharp
dataGridRow.SetContextMenu(menu =>
{
    menu.AddItem(new MenuItem()
        .SetText("Edit")
        .BindIsEnabled(() => vm.CanEdit)
        .SetCommand(vm.EditCommand));

    if (vm.IsAdmin)
    {
        menu.AddSeparator();
        menu.AddItem(new MenuItem().SetText("Admin Options")
            .AddItem(new MenuItem().SetText("Force Delete")));
    }
})
```

{: .note }
> ContextMenu has `AccessibilityRole.Menu`. It automatically closes when clicking outside or pressing Escape.
