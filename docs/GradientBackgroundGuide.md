# Gradient Background Support - Usage Examples

## Overview

The PlusUI framework now supports extensible backgrounds through the `IBackground` interface. This enables:
- ✅ Solid colors
- ✅ Linear gradients  
- ✅ Radial gradients
- ✅ Multi-stop gradients
- 📝 Future: Tileable images, patterns, custom backgrounds

All backgrounds are **immutable and thread-safe**.

## Basic Usage

### Solid Color Background

```csharp
// Using the new Background system
new VStack()
    .SetBackground(new SolidColorBackground(SKColors.Blue))

// Object initializer syntax
new VStack()
    .SetBackground(new SolidColorBackground { Color = SKColors.Blue })

// Implicit conversion from SKColor (when type is known)
SolidColorBackground bg = SKColors.Blue;
```

### Linear Gradient

```csharp
// Constructor
new Border()
    .SetBackground(new LinearGradient(SKColors.Blue, SKColors.Purple, angle: 45))

// Object initializer
new Border()
    .SetBackground(new LinearGradient 
    {
        StartColor = SKColors.Blue,
        EndColor = SKColors.Purple,
        Angle = 45
    })

// Angles: 0 = left to right, 90 = top to bottom, 180 = right to left, 270 = bottom to top
```

### Radial Gradient

```csharp
// Default center (0.5, 0.5)
new Border()
    .SetBackground(new RadialGradient(SKColors.White, SKColors.Gray))

// Custom center point (relative coordinates 0-1)
new Border()
    .SetBackground(new RadialGradient(
        SKColors.White, 
        SKColors.Gray, 
        new Point(0.3f, 0.7f)))

// Object initializer
new Border()
    .SetBackground(new RadialGradient
    {
        CenterColor = SKColors.White,
        EdgeColor = SKColors.Gray,
        Center = new Point(0.5f, 0.5f)
    })
```

### Multi-Stop Gradient

```csharp
// Rainbow gradient
new Border()
    .SetBackground(new MultiStopGradient(
        angle: 90,
        new MultiStopGradient.GradientStop(SKColors.Red, 0),
        new MultiStopGradient.GradientStop(SKColors.Yellow, 0.5f),
        new MultiStopGradient.GradientStop(SKColors.Green, 1)))

// From collection
var stops = new[]
{
    new MultiStopGradient.GradientStop(SKColors.Red, 0),
    new MultiStopGradient.GradientStop(SKColors.Orange, 0.25f),
    new MultiStopGradient.GradientStop(SKColors.Yellow, 0.5f),
    new MultiStopGradient.GradientStop(SKColors.Green, 0.75f),
    new MultiStopGradient.GradientStop(SKColors.Blue, 1)
};
new Border()
    .SetBackground(new MultiStopGradient(45, stops))
```

## Data Binding

### Simple Binding

```csharp
// In ViewModel
public SKColor ButtonColor { get; set; } = SKColors.Blue;

// In View
new Button()
    .BindBackground(nameof(vm.ButtonColor), () => new SolidColorBackground(vm.ButtonColor))
```

### Complex Binding with Different Background Types

```csharp
// In ViewModel
public string Theme { get; set; } = "Dark";
public bool IsSpecial { get; set; }

// In View - Switch between gradients based on theme
new VStack()
    .BindBackground(nameof(vm.Theme), () => 
        vm.Theme switch
        {
            "Dark" => new LinearGradient(SKColors.Black, SKColors.DarkGray, 180),
            "Light" => new LinearGradient(SKColors.White, SKColors.LightGray, 180),
            "Gradient" => new RadialGradient(SKColors.Blue, SKColors.Purple),
            _ => new SolidColorBackground(SKColors.Gray)
        })

// Conditional gradient
new Border()
    .BindBackground(nameof(vm.IsSpecial), () =>
        vm.IsSpecial 
            ? (IBackground)new RadialGradient(SKColors.Gold, SKColors.Orange)
            : new SolidColorBackground(SKColors.White))
```

## Corner Radius Support

All backgrounds automatically respect corner radius:

```csharp
new Border()
    .SetBackground(new LinearGradient(SKColors.Blue, SKColors.Purple, 45))
    .SetCornerRadius(20)  // Gradient will be clipped to rounded corners
```

## Backward Compatibility

Old code using `BackgroundColor` still works (with deprecation warnings):

```csharp
// Old API (deprecated but still functional)
new VStack()
    .SetBackgroundColor(SKColors.Blue)  // Warning: Use SetBackground() instead

// New API (recommended)
new VStack()
    .SetBackground(new SolidColorBackground(SKColors.Blue))
```

## Real-World Examples

### Card with Gradient Background

```csharp
new Border()
    .SetBackground(new LinearGradient(
        SKColors.LightBlue,
        SKColors.White,
        angle: 180))
    .SetCornerRadius(15)
    .SetStrokeColor(SKColors.LightGray)
    .SetStrokeThickness(1)
    .AddChild(new VStack(
        new Label().SetText("Card Title").SetTextSize(24),
        new Label().SetText("Card content goes here...")
    ).SetPadding(new(20)))
```

### Button with Gradient Hover Effect

```csharp
// In ViewModel
public bool IsHovered { get; set; }

// In View
new Button()
    .SetText("Hover Me")
    .BindBackground(nameof(vm.IsHovered), () =>
        vm.IsHovered
            ? (IBackground)new LinearGradient(SKColors.DarkBlue, SKColors.Blue, 90)
            : new SolidColorBackground(SKColors.Blue))
```

### Page with Gradient Background

```csharp
public class MyPage : UiPageElement
{
    protected override UiElement Build()
    {
        // Page-level gradient background
        this.SetBackground(new LinearGradient(
            SKColors.LightBlue,
            SKColors.White,
            180));

        return new VStack(
            // Content here
        );
    }
}
```

### Multi-Color Progress Indicator

```csharp
new Border()
    .SetBackground(new MultiStopGradient(
        0,  // Horizontal
        new MultiStopGradient.GradientStop(SKColors.Green, 0),
        new MultiStopGradient.GradientStop(SKColors.Yellow, 0.5f),
        new MultiStopGradient.GradientStop(SKColors.Red, 1)))
    .SetDesiredHeight(10)
    .SetCornerRadius(5)
```

## Performance Notes

- Backgrounds are rendered using SkiaSharp's native GPU-accelerated shaders
- Gradients use `SKShader.CreateLinearGradient()` and `SKShader.CreateRadialGradient()`
- All background classes are immutable, making them safe to cache and reuse
- Consider storing frequently used gradients as static fields to avoid recreation

## Future Extensions

The `IBackground` interface enables future enhancements:

```csharp
// Future: Tileable pattern background
new Border()
    .SetBackground(new PatternBackground
    {
        PatternImageSource = "texture.png",
        TileMode = TileMode.Repeat
    })

// Future: Image background
new Border()
    .SetBackground(new ImageBackground
    {
        ImageSource = "photo.jpg",
        Aspect = Aspect.AspectFill
    })

// Future: Custom user-defined backgrounds
public class MyCustomBackground : IBackground
{
    public void Render(SKCanvas canvas, SKRect bounds, float cornerRadius)
    {
        // Custom rendering logic
    }
}
```

## API Reference

### IBackground Interface

```csharp
public interface IBackground
{
    void Render(SKCanvas canvas, SKRect bounds, float cornerRadius);
}
```

### UiElement Background Methods

```csharp
public UiElement SetBackground(IBackground? background)
public UiElement BindBackground(string propertyName, Func<IBackground?> propertyGetter)
```

### Background Classes

All background classes are in the `PlusUi.core` namespace:

- `SolidColorBackground` - Single solid color
- `LinearGradient` - Two-color linear gradient with angle
- `RadialGradient` - Two-color radial gradient with custom center
- `MultiStopGradient` - Multi-color gradient with multiple stops
