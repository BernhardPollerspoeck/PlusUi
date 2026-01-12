---
title: TabControl
layout: default
parent: Controls
nav_order: 230
---

# TabControl

A container control that organizes content into tabbed views. Users can switch between tabs to display different content. Supports top, bottom, left, and right tab positions with keyboard navigation.

---

## Basic Usage

```csharp
// Simple tab control
new TabControl()
    .AddTab(new TabItem()
        .SetHeader("General")
        .SetContent(new VStack().AddChildren(...)))
    .AddTab(new TabItem()
        .SetHeader("Advanced")
        .SetContent(new VStack().AddChildren(...)))
    .SetSelectedIndex(0)

// Tabs at bottom
new TabControl()
    .SetTabPosition(TabPosition.Bottom)
    .AddTab(new TabItem().SetHeader("Home").SetContent(homePage))
    .AddTab(new TabItem().SetHeader("Settings").SetContent(settingsPage))

// Vertical tabs
new TabControl()
    .SetTabPosition(TabPosition.Left)
    .AddTab(new TabItem().SetHeader("Profile").SetContent(profilePanel))
    .AddTab(new TabItem().SetHeader("Security").SetContent(securityPanel))
```

---

## TabControl-Specific Methods

### Tab Management

| Method | Description |
|:-------|:------------|
| `AddTab(TabItem)` | Adds a tab to the control |
| `RemoveTab(TabItem)` | Removes a tab |
| `ClearTabs()` | Removes all tabs |
| `SetTabs(IEnumerable<TabItem>)` | Sets all tabs at once |
| `BindTabs(name, getter)` | Binds tabs collection |

### Selection Methods

| Method | Description |
|:-------|:------------|
| `SetSelectedIndex(int)` | Sets selected tab by index |
| `BindSelectedIndex(name, getter, setter?)` | Binds selected index (optional two-way) |
| `SetSelectedTab(TabItem)` | Sets selected tab directly |
| `BindSelectedTab(name, getter)` | Binds selected tab |

### Position Methods

| Method | Description |
|:-------|:------------|
| `SetTabPosition(TabPosition)` | Sets tab header position (default: Top) |
| `BindTabPosition(name, getter)` | Binds tab position |

### TabPosition Values

| Value | Description |
|:------|:------------|
| `TabPosition.Top` | Tabs at top (default) |
| `TabPosition.Bottom` | Tabs at bottom |
| `TabPosition.Left` | Tabs on left side |
| `TabPosition.Right` | Tabs on right side |

### Event Methods

| Method | Description |
|:-------|:------------|
| `SetOnSelectedIndexChanged(Action<int>)` | Sets callback for tab changes |
| `BindOnSelectedIndexChanged(name, getter)` | Binds change callback |
| `SetSelectionChangedCommand(ICommand)` | Sets command for tab changes |
| `BindSelectionChangedCommand(name, getter)` | Binds selection command |

### Styling Methods

| Method | Description |
|:-------|:------------|
| `SetHeaderTextSize(float)` | Sets tab header text size (default: 14) |
| `BindHeaderTextSize(name, getter)` | Binds header text size |
| `SetHeaderTextColor(SKColor)` | Sets inactive tab text color |
| `BindHeaderTextColor(name, getter)` | Binds header text color |
| `SetActiveHeaderTextColor(SKColor)` | Sets active tab text color (default: green) |
| `BindActiveHeaderTextColor(name, getter)` | Binds active text color |
| `SetDisabledHeaderTextColor(SKColor)` | Sets disabled tab text color |
| `BindDisabledHeaderTextColor(name, getter)` | Binds disabled text color |
| `SetHeaderBackgroundColor(SKColor)` | Sets header bar background |
| `BindHeaderBackgroundColor(name, getter)` | Binds header background |
| `SetActiveTabBackgroundColor(SKColor)` | Sets active tab background |
| `BindActiveTabBackgroundColor(name, getter)` | Binds active tab background |
| `SetHoverTabBackgroundColor(SKColor)` | Sets hover tab background |
| `BindHoverTabBackgroundColor(name, getter)` | Binds hover background |
| `SetTabIndicatorColor(SKColor)` | Sets active tab indicator color |
| `BindTabIndicatorColor(name, getter)` | Binds indicator color |
| `SetTabIndicatorHeight(float)` | Sets indicator height (default: 3) |
| `BindTabIndicatorHeight(name, getter)` | Binds indicator height |
| `SetTabPadding(Margin)` | Sets tab header padding (default: 16, 10) |
| `BindTabPadding(name, getter)` | Binds tab padding |
| `SetTabSpacing(float)` | Sets spacing between tabs (default: 0) |
| `BindTabSpacing(name, getter)` | Binds tab spacing |

{: .note }
> Keyboard navigation: Arrow Left/Right for horizontal tabs, Arrow Up/Down for vertical tabs. Disabled tabs are skipped.

---

## TabItem

TabItem represents a single tab with header text and content.

| Method | Description |
|:-------|:------------|
| `SetHeader(string)` | Sets tab header text |
| `SetContent(UiElement)` | Sets tab content |
| `SetIcon(string)` | Sets optional icon for the tab header |
| `SetIsEnabled(bool)` | Enables/disables the tab (default: true) |
| `SetTag(object)` | Sets custom data associated with this tab |

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
| `SetVisualOffset(Point)` | Offsets visual position |
| `BindVisualOffset(name, getter)` | Binds visual offset |

---

## Accessibility Methods

Methods inherited from `UiElement`:

| Method | Description |
|:-------|:------------|
| `SetAccessibilityLabel(string?)` | Sets screen reader label |
| `BindAccessibilityLabel(name, getter)` | Binds accessibility label |
| `SetAccessibilityHint(string?)` | Sets additional context hint |
| `BindAccessibilityHint(name, getter)` | Binds accessibility hint |
| `SetTabIndex(int)` | Sets tab order |
| `BindTabIndex(name, getter)` | Binds tab index |
| `SetTabStop(bool)` | Enables/disables tab stop (default: true) |
| `BindTabStop(name, getter)` | Binds tab stop |

{: .note }
> TabControl has `AccessibilityRole.Tab` by default.

---

## Examples

### Settings Page

```csharp
new TabControl()
    .SetTabPosition(TabPosition.Top)
    .AddTab(new TabItem()
        .SetHeader("General")
        .SetContent(
            new VStack()
                .AddChildren(
                    new Label().SetText("Language"),
                    new ComboBox<string>().SetItemsSource(languages)
                )
        ))
    .AddTab(new TabItem()
        .SetHeader("Appearance")
        .SetContent(
            new VStack()
                .AddChildren(
                    new Label().SetText("Theme"),
                    new ComboBox<string>().SetItemsSource(themes)
                )
        ))
    .AddTab(new TabItem()
        .SetHeader("Privacy")
        .SetContent(privacySettings))
```

### Bottom Navigation (Mobile Style)

```csharp
new TabControl()
    .SetTabPosition(TabPosition.Bottom)
    .SetHeaderBackgroundColor(new SKColor(30, 30, 30))
    .SetTabIndicatorHeight(0)  // No indicator
    .AddTab(new TabItem().SetHeader("Home").SetContent(homePage))
    .AddTab(new TabItem().SetHeader("Search").SetContent(searchPage))
    .AddTab(new TabItem().SetHeader("Profile").SetContent(profilePage))
```

### Sidebar Navigation

```csharp
new TabControl()
    .SetTabPosition(TabPosition.Left)
    .SetDesiredWidth(800)
    .SetHeaderBackgroundColor(new SKColor(25, 25, 25))
    .SetTabPadding(new Margin(20, 12))
    .AddTab(new TabItem().SetHeader("Dashboard").SetContent(dashboard))
    .AddTab(new TabItem().SetHeader("Reports").SetContent(reports))
    .AddTab(new TabItem().SetHeader("Settings").SetContent(settings))
    .BindSelectedIndex(nameof(vm.ActiveSection), () => vm.ActiveSection, i => vm.ActiveSection = i)
```

### Styled Tabs

```csharp
new TabControl()
    .SetHeaderTextColor(SKColors.Gray)
    .SetActiveHeaderTextColor(SKColors.White)
    .SetTabIndicatorColor(new SKColor(0, 122, 255))  // Blue indicator
    .SetTabIndicatorHeight(4)
    .SetActiveTabBackgroundColor(new SKColor(45, 45, 45))
    .SetHoverTabBackgroundColor(new SKColor(55, 55, 55))
    .SetTabSpacing(8)
    .AddTab(new TabItem().SetHeader("Tab 1").SetContent(content1))
    .AddTab(new TabItem().SetHeader("Tab 2").SetContent(content2))
```

### Disabled Tab

```csharp
new TabControl()
    .AddTab(new TabItem().SetHeader("Available").SetContent(content))
    .AddTab(new TabItem().SetHeader("Premium").SetContent(premiumContent).SetIsEnabled(false))
    .AddTab(new TabItem().SetHeader("Help").SetContent(helpContent))
```

### Dynamic Tabs

```csharp
new TabControl()
    .BindTabs(nameof(vm.Tabs), () => vm.Tabs.Select(t =>
        new TabItem()
            .SetHeader(t.Title)
            .SetContent(t.CreateContent())
    ))
    .BindSelectedIndex(nameof(vm.ActiveTabIndex), () => vm.ActiveTabIndex, i => vm.ActiveTabIndex = i)
```

### Tab Changed Event

```csharp
new TabControl()
    .AddTab(new TabItem().SetHeader("Tab 1").SetContent(content1))
    .AddTab(new TabItem().SetHeader("Tab 2").SetContent(content2))
    .SetOnSelectedIndexChanged(index =>
        Console.WriteLine($"Switched to tab {index}"))
    .SetSelectionChangedCommand(vm.TabChangedCommand)
```
