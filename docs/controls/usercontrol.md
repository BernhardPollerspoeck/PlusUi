---
title: UserControl
layout: default
parent: Controls
nav_order: 9
---

# UserControl

Create reusable custom controls by composing other UI elements.

---

## Overview

`UserControl` allows you to create complex, reusable components from simpler elements. Define your control's UI by implementing the `Build` method.

---

## Basic Usage

```csharp
public class UserCard : UserControl
{
    public string Name { get; set; } = "";
    public string Email { get; set; } = "";

    protected override UiElement Build() =>
        new VStack(
            new Label()
                .SetText(Name)
                .SetFontWeight(FontWeight.Bold)
                .SetTextSize(18),
            new Label()
                .SetText(Email)
                .SetTextSize(14)
                .SetTextColor(Colors.Gray)
                .SetMargin(new Margin(0, 4, 0, 0))
        )
        .SetMargin(new Margin(12));
}
```

### Using Your Control

```csharp
new UserCard { Name = "John Doe", Email = "john@example.com" }
```

---

## With Styling

```csharp
public class StyledCard : UserControl
{
    public string Title { get; set; } = "";
    public string Description { get; set; } = "";

    protected override UiElement Build() =>
        new Border()
            .SetBackground(new SolidColorBackground(new Color(45, 45, 45)))
            .SetCornerRadius(12)
            .AddChild(
                new VStack(
                    new Label()
                        .SetText(Title)
                        .SetTextSize(20)
                        .SetFontWeight(FontWeight.Bold)
                        .SetTextColor(Colors.White),
                    new Label()
                        .SetText(Description)
                        .SetTextSize(14)
                        .SetTextColor(new Color(180, 180, 180))
                        .SetMargin(new Margin(0, 8, 0, 0))
                )
                .SetMargin(new Margin(16))
            );
}
```

---

## With Data Binding

```csharp
public class ProductCard : UserControl
{
    public Product? Product { get; set; }

    protected override UiElement Build()
    {
        if (Product == null)
            return new NullElement();

        return new Border()
            .SetBackground(new SolidColorBackground(new Color(40, 40, 40)))
            .SetCornerRadius(8)
            .AddChild(
                new VStack(
                    new Image()
                        .SetImageSource(Product.ImageUrl)
                        .SetDesiredHeight(150)
                        .SetAspect(Aspect.AspectFill),
                    new VStack(
                        new Label()
                            .SetText(Product.Name)
                            .SetFontWeight(FontWeight.Bold),
                        new Label()
                            .SetText($"${Product.Price:F2}")
                            .SetTextColor(Colors.Green)
                    ).SetMargin(new Margin(12))
                )
            );
    }
}
```

---

## With Events

```csharp
public class ClickableCard : UserControl
{
    public string Title { get; set; } = "";
    public ICommand? Command { get; set; }

    protected override UiElement Build() =>
        new TapGestureDetector(
            new Border()
                .SetBackground(new SolidColorBackground(new Color(50, 50, 50)))
                .SetCornerRadius(8)
                .AddChild(
                    new Label()
                        .SetText(Title)
                        .SetTextColor(Colors.White)
                        .SetMargin(new Margin(16))
                )
        ).SetCommand(Command!);
}
```

---

## Examples

### Avatar Component

```csharp
public class Avatar : UserControl
{
    public string ImageUrl { get; set; } = "";
    public float Size { get; set; } = 48;

    protected override UiElement Build() =>
        new Image()
            .SetImageSource(ImageUrl)
            .SetDesiredSize(new Size(Size, Size))
            .SetAspect(Aspect.AspectFill)
            .SetCornerRadius(Size / 2);
}

// Usage
new Avatar { ImageUrl = "user.jpg", Size = 64 }
```

### Status Badge

```csharp
public class StatusBadge : UserControl
{
    public string Text { get; set; } = "";
    public Color BackgroundColor { get; set; } = Colors.Gray;

    protected override UiElement Build() =>
        new Border()
            .SetBackground(new SolidColorBackground(BackgroundColor))
            .SetCornerRadius(12)
            .AddChild(
                new Label()
                    .SetText(Text)
                    .SetTextSize(12)
                    .SetFontWeight(FontWeight.Bold)
                    .SetTextColor(Colors.White)
                    .SetMargin(new Margin(12, 6))
            );
}

// Usage
new StatusBadge { Text = "Active", BackgroundColor = new Color(52, 199, 89) }
new StatusBadge { Text = "Pending", BackgroundColor = new Color(255, 149, 0) }
```

### List Item

```csharp
public class ListItem : UserControl
{
    public string Icon { get; set; } = "";
    public string Title { get; set; } = "";
    public string Subtitle { get; set; } = "";
    public ICommand? Command { get; set; }

    protected override UiElement Build() =>
        new TapGestureDetector(
            new HStack(
                new Image()
                    .SetImageSource(Icon)
                    .SetDesiredSize(new Size(40, 40))
                    .SetCornerRadius(20),
                new VStack(
                    new Label()
                        .SetText(Title)
                        .SetFontWeight(FontWeight.Bold)
                        .SetTextColor(Colors.White),
                    new Label()
                        .SetText(Subtitle)
                        .SetTextSize(14)
                        .SetTextColor(Colors.Gray)
                ).SetMargin(new Margin(12, 0, 0, 0))
            ).SetMargin(new Margin(16, 12))
        ).SetCommand(Command!);
}
```

---

## Best Practices

1. **Keep controls focused** - Each UserControl should do one thing well
2. **Use properties for configuration** - Make controls customizable via properties
3. **Handle null gracefully** - Return `NullElement` for missing data
4. **Composition over inheritance** - Compose smaller controls rather than deep inheritance
5. **Consider data binding** - For dynamic content, use binding-friendly patterns
