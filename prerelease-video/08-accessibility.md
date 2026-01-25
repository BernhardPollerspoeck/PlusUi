# 8. Accessibility

## Features

- Screen reader support
- High contrast mode
- Focus navigation
- Focus ring customization
- TabIndex support

**Narration:**

> "Accessibility. It's one of those things that's easy to skip when you're building a framework. 'We'll add it later', right? And then later never comes.
>
> With PlusUi, it's built in from the start.
>
> Every control has an accessibility role. Button, checkbox, text field, slider - screen readers know exactly what they're dealing with. Not just 'some element on screen', but 'this is a button, and here's what it does'.
>
> You can add labels, hints, and values for elements that need more context. A button that just shows an icon? Give it a screen reader label. A slider? Announce its current value. It's all there, ready to use.
>
> Keyboard navigation works out of the box. Tab through your UI, use arrow keys within groups, press Enter or Space to activate. Focus moves logically through your controls. And when the default order doesn't make sense for your layout, you can customize the tab index.
>
> The focus ring - that visual indicator showing where keyboard focus currently is - is fully customizable. Color, width, offset from the control. Make it subtle to match your design, or make it bold and obvious for users who rely on it.
>
> High contrast mode? Supported. When the operating system tells us the user has enabled high contrast settings, your app can respond with alternative colors. You define what high contrast looks like for your design.
>
> Building accessible apps shouldn't require a separate effort. It shouldn't be a checklist you go through at the end. It's part of how you build. And PlusUi makes that easy."

## Visuals

- Dunkler Hintergrund (#1E1E1E)
- Code-Block groß und lesbar (Syntax Highlighting)
- Sprecher in Ecke

```csharp
// Screen reader support
new Button()
    .SetIcon("delete.svg")
    .SetAccessibilityLabel("Delete item")
    .SetAccessibilityHint("Removes this item from your cart")

// Keyboard navigation
new Entry()
    .SetPlaceholder("First name")
    .SetTabIndex(1)

new Entry()
    .SetPlaceholder("Last name")
    .SetTabIndex(2)

// Focus ring customization
new Button()
    .SetFocusRingColor(Colors.Blue)
    .SetFocusRingWidth(3)
    .SetFocusRingOffset(2)

// High contrast mode
new Button()
    .SetText("Submit")
    .SetHighContrastBackground(new SolidColorBackground(Colors.Black))
    .SetHighContrastForeground(Colors.White)
```

**Übergang zu Section 9:** Slide (Page Transition)
