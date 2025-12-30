---
title: Toolbar
layout: default
parent: Controls
nav_order: 240
---

# Toolbar

A toolbar control with support for left icons, right icons, title, icon groups, and overflow handling. Follows Material Design patterns with automatic responsive overflow menus.

---

## Basic Usage

```csharp
// Simple toolbar with title
new Toolbar()
    .SetTitle("My App")
    .SetDesiredHeight(56)

// Toolbar with icons
new Toolbar()
    .SetTitle("My App")
    .AddLeft(new Button().SetIcon("menu"))
    .AddRight(new Button().SetIcon("search"))
    .AddRight(new Button().SetIcon("more_vert"))

// Responsive overflow handling
new Toolbar()
    .SetTitle("Editor")
    .AddRight(new Button().SetText("Save"))
    .AddRight(new Button().SetText("Export"))
    .AddRight(new Button().SetText("Share"))
    .SetOverflowBehavior(OverflowBehavior.CollapseToMenu)
```

---

## Toolbar-Specific Methods

### Title Methods

| Method | Description |
|:-------|:------------|
| `SetTitle(string)` | Sets toolbar title |
| `BindTitle(name, getter)` | Binds title |
| `SetTitleFontSize(float)` | Sets title font size (default: 20) |
| `BindTitleFontSize(name, getter)` | Binds title font size |
| `SetTitleColor(Color)` | Sets title color (default: black) |
| `BindTitleColor(name, getter)` | Binds title color |
| `SetTitleAlignment(TitleAlignment)` | Sets title alignment (default: Center) |
| `BindTitleAlignment(name, getter)` | Binds title alignment |

### TitleAlignment Values

| Value | Description |
|:------|:------------|
| `TitleAlignment.Center` | Title centered (default) |
| `TitleAlignment.Left` | Title aligned left |

### Content Methods

| Method | Description |
|:-------|:------------|
| `AddLeft(UiElement)` | Adds element to left section |
| `AddLeftGroup(ToolbarIconGroup)` | Adds icon group to left section |
| `AddRight(UiElement)` | Adds element to right section |
| `AddRightGroup(ToolbarIconGroup)` | Adds icon group to right section |
| `SetCenterContent(UiElement)` | Sets center content (replaces title) |
| `BindCenterContent(name, getter)` | Binds center content |

### Layout Methods

| Method | Description |
|:-------|:------------|
| `SetItemSpacing(float)` | Sets spacing between items (default: 8) |
| `BindItemSpacing(name, getter)` | Binds item spacing |
| `SetContentPadding(Margin)` | Sets content padding (default: 16, 0, 16, 0) |
| `BindContentPadding(name, getter)` | Binds content padding |

### Overflow Methods

| Method | Description |
|:-------|:------------|
| `SetOverflowBehavior(OverflowBehavior)` | Sets overflow behavior (default: None) |
| `BindOverflowBehavior(name, getter)` | Binds overflow behavior |
| `SetOverflowThreshold(float)` | Sets width at which overflow starts (default: 600) |
| `BindOverflowThreshold(name, getter)` | Binds overflow threshold |
| `SetOverflowIcon(string)` | Sets overflow menu icon (default: "more_vert") |
| `BindOverflowIcon(name, getter)` | Binds overflow icon |

### OverflowBehavior Values

| Value | Description |
|:------|:------------|
| `OverflowBehavior.None` | No overflow handling (default) |
| `OverflowBehavior.CollapseToMenu` | Collapse excess items to overflow menu |

### Overflow Menu Styling

| Method | Description |
|:-------|:------------|
| `SetOverflowMenuBackground(Color)` | Sets overflow menu background |
| `BindOverflowMenuBackground(name, getter)` | Binds menu background |
| `SetOverflowMenuItemBackground(Color)` | Sets menu item background |
| `BindOverflowMenuItemBackground(name, getter)` | Binds item background |
| `SetOverflowMenuItemHoverBackground(Color)` | Sets menu item hover background |
| `BindOverflowMenuItemHoverBackground(name, getter)` | Binds item hover background |
| `SetOverflowMenuItemTextColor(Color)` | Sets menu item text color |
| `BindOverflowMenuItemTextColor(name, getter)` | Binds item text color |

{: .note }
> Toolbar has `AccessibilityRole.Toolbar` by default. When overflow behavior is enabled, items that don't fit are moved to a dropdown menu.

---

## ToolbarIconGroup

Groups related toolbar icons together with optional separator.

| Method | Description |
|:-------|:------------|
| `AddIcon(Button)` | Adds an icon button to the group |
| `SetSeparator(bool)` | Adds visual separator after group |
| `SetPriority(int)` | Sets priority for overflow (higher = kept visible) |

---

## Layout Methods

Methods inherited from `UiElement`:

| Method | Description |
|:-------|:------------|
| `SetMargin(Margin)` | Sets outer margin |
| `BindMargin(name, getter)` | Binds margin |
| `SetHorizontalAlignment(HorizontalAlignment)` | Sets horizontal alignment |
| `BindHorizontalAlignment(name, getter)` | Binds horizontal alignment |
| `SetVerticalAlignment(VerticalAlignment)` | Sets vertical alignment |
| `BindVerticalAlignment(name, getter)` | Binds vertical alignment |
| `SetDesiredSize(Size)` | Sets explicit size |
| `BindDesiredSize(name, getter)` | Binds size |
| `SetDesiredWidth(float)` | Sets explicit width |
| `BindDesiredWidth(name, getter)` | Binds width |
| `SetDesiredHeight(float)` | Sets explicit height |
| `BindDesiredHeight(name, getter)` | Binds height |

---

## Appearance Methods

Methods inherited from `UiElement`:

| Method | Description |
|:-------|:------------|
| `SetIsVisible(bool)` | Shows/hides element |
| `BindIsVisible(name, getter)` | Binds visibility |
| `SetOpacity(float)` | Sets opacity 0.0-1.0 (default: 1.0) |
| `BindOpacity(name, getter)` | Binds opacity |
| `SetBackground(IBackground)` | Sets background (gradient, solid, etc.) |
| `SetBackground(Color)` | Sets solid color background |
| `BindBackground(name, getter)` | Binds background |
| `SetCornerRadius(float)` | Sets corner radius |
| `BindCornerRadius(name, getter)` | Binds corner radius |

---

## Examples

### Material Design AppBar

```csharp
new Toolbar()
    .SetDesiredHeight(56)
    .SetBackground(new Color(98, 0, 238))  // Purple
    .SetTitle("My App")
    .SetTitleColor(Colors.White)
    .AddLeft(
        new Button()
            .SetIcon("menu")
            .SetTextColor(Colors.White)
            .SetBackground(Colors.Transparent)
    )
    .AddRight(
        new Button()
            .SetIcon("search")
            .SetTextColor(Colors.White)
            .SetBackground(Colors.Transparent)
    )
```

### Editor Toolbar with Groups

```csharp
new Toolbar()
    .SetDesiredHeight(48)
    .SetBackground(new Color(45, 45, 45))
    .AddLeftGroup(new ToolbarIconGroup()
        .AddIcon(new Button().SetIcon("undo").SetCommand(vm.UndoCommand))
        .AddIcon(new Button().SetIcon("redo").SetCommand(vm.RedoCommand))
        .SetSeparator(true)
        .SetPriority(10))
    .AddLeftGroup(new ToolbarIconGroup()
        .AddIcon(new Button().SetIcon("format_bold").SetCommand(vm.BoldCommand))
        .AddIcon(new Button().SetIcon("format_italic").SetCommand(vm.ItalicCommand))
        .AddIcon(new Button().SetIcon("format_underline").SetCommand(vm.UnderlineCommand))
        .SetSeparator(true))
    .SetOverflowBehavior(OverflowBehavior.CollapseToMenu)
```

### Responsive Toolbar

```csharp
new Toolbar()
    .SetTitle("Document Editor")
    .AddRight(new Button().SetText("Save").SetCommand(vm.SaveCommand))
    .AddRight(new Button().SetText("Export").SetCommand(vm.ExportCommand))
    .AddRight(new Button().SetText("Share").SetCommand(vm.ShareCommand))
    .AddRight(new Button().SetText("Print").SetCommand(vm.PrintCommand))
    .SetOverflowBehavior(OverflowBehavior.CollapseToMenu)
    .SetOverflowThreshold(500)  // Collapse at 500px width
```

### Custom Center Content

```csharp
new Toolbar()
    .SetDesiredHeight(56)
    .AddLeft(new Button().SetIcon("arrow_back"))
    .SetCenterContent(
        new Entry()
            .SetPlaceholder("Search...")
            .SetDesiredWidth(300)
            .SetBackground(new Color(60, 60, 60))
            .SetCornerRadius(20)
    )
    .AddRight(new Button().SetIcon("filter"))
```

### Styled Overflow Menu

```csharp
new Toolbar()
    .SetTitle("Actions")
    .AddRight(new Button().SetText("Action 1"))
    .AddRight(new Button().SetText("Action 2"))
    .AddRight(new Button().SetText("Action 3"))
    .SetOverflowBehavior(OverflowBehavior.CollapseToMenu)
    .SetOverflowMenuBackground(new Color(35, 35, 35))
    .SetOverflowMenuItemBackground(new Color(45, 45, 45))
    .SetOverflowMenuItemHoverBackground(new Color(65, 65, 65))
    .SetOverflowMenuItemTextColor(Colors.White)
```

### Left-Aligned Title

```csharp
new Toolbar()
    .SetDesiredHeight(56)
    .SetTitle("Settings")
    .SetTitleAlignment(TitleAlignment.Left)
    .AddLeft(new Button().SetIcon("arrow_back").SetCommand(vm.BackCommand))
    .AddRight(new Button().SetIcon("check").SetCommand(vm.SaveCommand))
```
