# 5. Fluent API

## Sample Code

```csharp
public class MainPage(MainPageViewModel vm) : UiPageElement(vm)
{
    protected override UiElement Build()
    {
        return new VStack(
            new Label()
                .SetText("Welcome to PlusUi")
                .SetTextSize(32)
                .SetTextColor(Colors.White)
                .SetHorizontalTextAlignment(HorizontalTextAlignment.Center),

            new Entry()
                .SetPlaceholder("Enter your name...")
                .BindText(() => vm.Name, v => vm.Name = v)
                .SetPadding(new Margin(12, 8))
                .SetCornerRadius(8),

            new Button()
                .SetText("Click Me!")
                .SetPadding(new Margin(20, 10))
                .SetCornerRadius(8)
                .SetCommand(vm.SubmitCommand)
        )
        .SetSpacing(16)
        .SetPadding(new Margin(24))
        .SetHorizontalAlignment(HorizontalAlignment.Center)
        .SetVerticalAlignment(VerticalAlignment.Center);
    }
}
```

## Key Points

- No XAML, no markup languages - just C#
- Method chaining for readable UI construction
- Every `Set*` method has a corresponding `Bind*` method
- IntelliSense support for discoverability
- Compile-time type safety
- Extract reusable UI patterns into methods

**Narration:**

> "Let's talk about how you actually write UI code in PlusUi.
>
> No XAML. No markup languages. No angle brackets. No new syntax to learn.
>
> If you've been doing .NET development, you've probably dealt with XAML at some point. The separate files. The designer that sometimes works. The binding syntax you have to look up every time. The build errors that point to line 1, column 1.
>
> PlusUi takes a different approach. It's just C#.
>
> You create a control, then chain methods. `.SetText()`. `.SetPadding()`. `.SetBackground()`. `.SetCornerRadius()`. Each method returns the control, so you keep going. Build up your UI piece by piece.
>
> Everything is compile-time safe. Typo in a method name? Red squiggle, immediately. Wrong parameter type? Compiler tells you. Pass a string where it expects a color? Error before you even run the app. No runtime surprises.
>
> And IntelliSense becomes your best friend. Type a dot after any control and see every option. What properties does a Button have? Just look. No switching to documentation. No guessing.
>
> The pattern is consistent across the entire framework: every `Set` method has a matching `Bind` method. `SetText`, `BindText`. `SetIsVisible`, `BindIsVisible`. `SetBackground`, `BindBackground`. Whether you're setting a static value or binding to your ViewModel - same readable, chainable style.
>
> And because it's just C#, you can do what you'd normally do with code. Have a button style you use everywhere? Extract it to a method. Call it wherever you need it. Need ten cards with the same layout but different data? Write a method, call it in a loop. No templates, no styles resources, no control templates - just methods. The refactoring tools you already use just work.
>
> As a .NET developer, there's no learning curve. No 'let me figure out how this markup language works'. No XML namespaces. No converter syntax. Just the C# you already know, building UIs the way you'd expect.
>
> You want a VStack with a Label, an Entry, and a Button? You write exactly that. And when you read it back a month later, you'll still understand it."

## Visuals

- Dunkler Hintergrund (#1E1E1E)
- Code-Block groß und lesbar (Syntax Highlighting)
- Sprecher in Ecke

```csharp
public class MainPage(MainPageViewModel vm) : UiPageElement(vm)
{
    protected override UiElement Build()
    {
        return new VStack(
            new Label()
                .SetText("Welcome to PlusUi")
                .SetTextSize(32)
                .SetTextColor(Colors.White)
                .SetHorizontalTextAlignment(HorizontalTextAlignment.Center),

            new Entry()
                .SetPlaceholder("Enter your name...")
                .BindText(() => vm.Name, v => vm.Name = v)
                .SetPadding(new Margin(12, 8))
                .SetCornerRadius(8),

            new Button()
                .SetText("Click Me!")
                .SetPadding(new Margin(20, 10))
                .SetCornerRadius(8)
                .SetCommand(vm.SubmitCommand)
        )
        .SetSpacing(16)
        .SetPadding(new Margin(24))
        .SetHorizontalAlignment(HorizontalAlignment.Center)
        .SetVerticalAlignment(VerticalAlignment.Center);
    }
}
```

**Übergang zu Section 6:** Slide (Page Transition)
