// This file demonstrates the method chaining capability that the generator provides
// Since .NET 9 is not available, this is a conceptual demonstration

using PlusUi.core;
using SkiaSharp;

namespace PlusUi.Examples
{
    // Example of how a concrete UI element would inherit from UiElement<T>
    public class Button : UiElement<Button>
    {
        // Button-specific methods would be here
        public Button SetText(string text) => this;
        public Button SetPadding(Margin padding) => this;
    }

    public class DemoUsage
    {
        public static void DemonstratMethodChaining()
        {
            // The generated UiElement<T> class enables fluent method chaining
            // All these methods return Button (not UiElement), allowing access to Button-specific methods
            
            var button = new Button()
                .SetBackgroundColor(SKColors.Blue)     // Generated method returns Button
                .SetMargin(new Margin(10))             // Generated method returns Button
                .SetIsVisible(true)                    // Generated method returns Button
                .SetDesiredSize(new Size(100, 50))     // Generated method returns Button
                .SetHorizontalAlignment(HorizontalAlignment.Center) // Generated method returns Button
                .SetVerticalAlignment(VerticalAlignment.Center)     // Generated method returns Button
                .SetCornerRadius(5.0f)                 // Generated method returns Button
                .SetText("Click Me!")                  // Button-specific method
                .SetPadding(new Margin(5));            // Button-specific method

            // Without the generator, the chain would break at the first UiElement method:
            // var buttonBroken = new Button()
            //     .SetBackgroundColor(SKColors.Blue)  // Returns UiElement, not Button
            //     .SetText("Click Me!");               // ERROR: UiElement doesn't have SetText!

            // The generator also works for binding methods:
            var dynamicButton = new Button()
                .BindBackgroundColor("Color", () => SKColors.Red)      // Generated method
                .BindMargin("ButtonMargin", () => new Margin(15))      // Generated method
                .BindIsVisible("IsButtonVisible", () => true)          // Generated method
                .SetText("Dynamic Button");                            // Button method still accessible
        }
    }
}

/* 
Generated UiElement<T> class will look like this:

namespace PlusUi.core;

// Auto-generated generic wrapper for UiElement
public abstract class UiElement<T> : UiElement where T : UiElement<T>
{
    public new T SetBackgroundColor(SkiaSharp.SKColor color)
    {
        base.SetBackgroundColor(color);
        return (T)this;
    }

    public new T BindBackgroundColor(string propertyName, System.Func<SkiaSharp.SKColor> propertyGetter)
    {
        base.BindBackgroundColor(propertyName, propertyGetter);
        return (T)this;
    }

    public new T SetMargin(PlusUi.core.Margin margin)
    {
        base.SetMargin(margin);
        return (T)this;
    }

    // ... all other methods follow the same pattern
    // This enables perfect method chaining while preserving type safety
}
*/