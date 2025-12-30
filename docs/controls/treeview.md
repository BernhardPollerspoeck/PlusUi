---
title: TreeView
layout: default
parent: Controls
nav_order: 260
---

# TreeView

A hierarchical tree view control for displaying nested data structures. Supports lazy loading of children, expand/collapse animations, custom item templates, and keyboard navigation. Efficiently handles large trees with visible-node-only rendering.

---

## Basic Usage

```csharp
// Simple file system tree
new TreeView()
    .SetItemsSource(folders)
    .SetChildrenSelector<Folder>(f => f.SubFolders)
    .SetItemTemplate((item, depth) =>
        new Label().SetText(((Folder)item).Name))

// With selection
new TreeView()
    .SetItemsSource(categories)
    .SetChildrenSelector<Category>(c => c.SubCategories)
    .SetItemTemplate((item, depth) =>
        new Label().SetText(((Category)item).Name))
    .BindSelectedItem(nameof(vm.SelectedCategory), () => vm.SelectedCategory, c => vm.SelectedCategory = c)
```

---

## TreeView-Specific Methods

### Data Methods

| Method | Description |
|:-------|:------------|
| `SetItemsSource(IEnumerable<object>)` | Sets root items |
| `BindItemsSource(name, getter)` | Binds root items |
| `SetChildrenSelector<TItem>(Func<TItem, IEnumerable<object>>)` | Registers children selector for type |
| `SetItemTemplate(Func<object, int, UiElement>)` | Sets item template (receives item and depth) |
| `BindItemTemplate(name, getter)` | Binds item template |

### Selection Methods

| Method | Description |
|:-------|:------------|
| `SetSelectedItem(object?)` | Sets selected item |
| `BindSelectedItem(name, getter, setter)` | Two-way binds selected item |

### Layout Methods

| Method | Description |
|:-------|:------------|
| `SetIndentation(float)` | Sets horizontal indent per level (default: 20) |
| `BindIndentation(name, getter)` | Binds indentation |
| `SetItemHeight(float)` | Sets row height (default: 32) |
| `BindItemHeight(name, getter)` | Binds item height |
| `SetExpanderSize(float)` | Sets expand/collapse icon size (default: 16) |
| `BindExpanderSize(name, getter)` | Binds expander size |

### Tree Lines

| Method | Description |
|:-------|:------------|
| `SetShowLines(bool)` | Shows/hides tree connection lines (default: false) |
| `BindShowLines(name, getter)` | Binds show lines |
| `SetLineColor(SKColor)` | Sets line color |
| `BindLineColor(name, getter)` | Binds line color |
| `SetLineThickness(float)` | Sets line thickness (default: 1) |
| `BindLineThickness(name, getter)` | Binds line thickness |

### Node Methods

| Method | Description |
|:-------|:------------|
| `ExpandNode(object)` | Expands a node (lazy loads children) |
| `CollapseNode(object)` | Collapses a node |
| `ToggleNode(object)` | Toggles expand/collapse state |
| `IsNodeExpanded(object)` | Checks if node is expanded |
| `GetNodeDepth(object)` | Gets depth of a node |

{: .note }
> TreeView has `AccessibilityRole.Tree` by default. Children are loaded lazily when a node is first expanded, making it efficient for large hierarchies.

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

---

## Appearance Methods

Methods inherited from `UiElement`:

| Method | Description |
|:-------|:------------|
| `SetIsVisible(bool)` | Shows/hides element |
| `BindIsVisible(name, getter)` | Binds visibility |
| `SetOpacity(float)` | Sets opacity 0.0-1.0 (default: 1.0) |
| `BindOpacity(name, getter)` | Binds opacity |
| `SetBackground(IBackground)` | Sets background |
| `SetBackground(Color)` | Sets solid color background |
| `BindBackground(name, getter)` | Binds background |
| `SetCornerRadius(float)` | Sets corner radius (content is clipped) |
| `BindCornerRadius(name, getter)` | Binds corner radius |

---

## Examples

### File Explorer

```csharp
new TreeView()
    .SetItemsSource(fileSystem.RootFolders)
    .SetChildrenSelector<Folder>(folder =>
        folder.SubFolders.Cast<object>().Concat(folder.Files.Cast<object>()))
    .SetItemTemplate((item, depth) =>
    {
        if (item is Folder folder)
            return new HStack()
                .AddChildren(
                    new Image().SetImageSource("folder_icon.png").SetDesiredSize(new Size(16, 16)),
                    new Label().SetText(folder.Name).SetMargin(new Margin(4, 0, 0, 0))
                );
        else if (item is File file)
            return new HStack()
                .AddChildren(
                    new Image().SetImageSource("file_icon.png").SetDesiredSize(new Size(16, 16)),
                    new Label().SetText(file.Name).SetMargin(new Margin(4, 0, 0, 0))
                );
        return new Label().SetText(item.ToString());
    })
    .SetShowLines(true)
    .SetLineColor(new SKColor(100, 100, 100))
    .BindSelectedItem(nameof(vm.SelectedItem), () => vm.SelectedItem, i => vm.SelectedItem = i)
```

### Category Hierarchy

```csharp
new TreeView()
    .SetItemsSource(categories.Where(c => c.ParentId == null))
    .SetChildrenSelector<Category>(c =>
        categories.Where(sub => sub.ParentId == c.Id))
    .SetItemTemplate((item, depth) =>
        new HStack()
            .AddChildren(
                new Label()
                    .SetText(((Category)item).Name)
                    .SetFontWeight(depth == 0 ? FontWeight.Bold : FontWeight.Normal)
            ))
    .SetIndentation(24)
    .SetItemHeight(36)
```

### Organization Chart

```csharp
new TreeView()
    .SetItemsSource(new[] { ceo })
    .SetChildrenSelector<Employee>(e => e.DirectReports)
    .SetItemTemplate((item, depth) =>
    {
        var employee = (Employee)item;
        return new HStack()
            .AddChildren(
                new Image()
                    .SetImageSource(employee.PhotoUrl)
                    .SetDesiredSize(new Size(24, 24))
                    .SetCornerRadius(12),
                new VStack()
                    .SetMargin(new Margin(8, 0, 0, 0))
                    .AddChildren(
                        new Label().SetText(employee.Name).SetFontWeight(FontWeight.Bold),
                        new Label().SetText(employee.Title).SetTextSize(12).SetTextColor(Colors.Gray)
                    )
            );
    })
    .SetItemHeight(40)
    .SetShowLines(true)
```

### Menu Structure

```csharp
new TreeView()
    .SetItemsSource(menuItems)
    .SetChildrenSelector<MenuItem>(m => m.SubItems)
    .SetItemTemplate((item, depth) =>
    {
        var menuItem = (MenuItem)item;
        return new HStack()
            .AddChildren(
                new Image().SetImageSource(menuItem.Icon).SetDesiredSize(new Size(16, 16)),
                new Label().SetText(menuItem.Label).SetMargin(new Margin(8, 0, 0, 0))
            );
    })
    .SetExpanderSize(12)
    .SetBackground(new Color(35, 35, 35))
```

### Heterogeneous Types

```csharp
// Different types at different levels
new TreeView()
    .SetItemsSource(projects)
    .SetChildrenSelector<Project>(p => p.Tasks.Cast<object>())
    .SetChildrenSelector<ProjectTask>(t => t.SubTasks.Cast<object>())
    .SetItemTemplate((item, depth) => item switch
    {
        Project p => new Label().SetText(p.Name).SetFontWeight(FontWeight.Bold),
        ProjectTask t => new HStack()
            .AddChildren(
                new Checkbox().BindIsChecked(nameof(t.IsComplete), () => t.IsComplete, v => t.IsComplete = v),
                new Label().SetText(t.Title)
            ),
        _ => new Label().SetText(item.ToString())
    })
```

### Styled Tree

```csharp
new TreeView()
    .SetItemsSource(nodes)
    .SetChildrenSelector<TreeNode>(n => n.Children)
    .SetItemTemplate((item, depth) =>
        new Label().SetText(((TreeNode)item).Name))
    .SetBackground(new Color(30, 30, 30))
    .SetCornerRadius(8)
    .SetShowLines(true)
    .SetLineColor(new SKColor(60, 60, 60))
    .SetLineThickness(1.5f)
    .SetIndentation(28)
    .SetItemHeight(32)
    .SetExpanderSize(14)
```
