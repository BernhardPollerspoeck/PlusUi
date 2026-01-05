# PlusUi Development Guide

## Build & Test Commands
```
# Build entire solution
dotnet build PlusUi.sln

# Build with minimal output
dotnet build PlusUi.sln -v q

# Run all tests
dotnet test PlusUi.sln

# Run a specific test
dotnet test PlusUi.sln --filter "FullyQualifiedName=PlusUi.core.Tests.LabelTests.TestLabelMeasureAndArrange_NoMargin_LeftTopAligned"

# Run tests in a specific class
dotnet test PlusUi.sln --filter "FullyQualifiedName~PlusUi.core.Tests.LabelTests"
```

## Behavior
- You are not allowed to remove or mark tasks in ToDo files as completed unless the user specifically tells you to.

## Code Style Guidelines
- **Naming**: PascalCase for types, methods, and properties; camelCase for local variables and parameters
- **Nullable**: Enable nullable reference types (`<Nullable>enable</Nullable>`)
- **Language**: Use latest C# features (`<LangVersion>latest</LangVersion>` or `<LangVersion>preview</LangVersion>`)
- **Framework**: Target .NET 10.0
- **Primary Constructors**: Use primary constructors for DI and initialization. Parameters are directly accessible in the class body - no extra fields required
- **Styles**: Fluent API style with builder pattern (method chaining)
- **UI Components**: Follow the pattern of exposing properties with internal setters and public fluent Set* methods
- **Set/Bind Rule**: EVERY public `Set*` method MUST have a corresponding `Bind*` method for data binding
- **Tests**: Use MSTest with AAA pattern (Arrange-Act-Assert)
- **Architecture**: Controls inherit from base elements (UiElement, UiTextElement)
- **Error Handling**: Avoid exceptions for control flow; use nullable types appropriately
- **Component Structure**: Components should have predictable initialization through constructor and fluent methods
- **Comments**: Do NOT add unnecessary comments to code. Code should be self-explanatory
- **Registry Disposal**: In Dispose methods, set registry-managed resources (Paint, Font) to null without calling Release(). Registries handle cleanup themselves
- **Prefer Built-in Controls**: ALWAYS check if a built-in control exists (TabControl, Grid, VStack, etc.) before creating custom controls. Use existing controls whenever possible
- **Fix Bugs in Controls, Not Workarounds**: When a bug is found in a core control (PlusUi.core), fix it in the control itself rather than creating workarounds. ALWAYS get user confirmation before modifying core controls

## UiElement Control Rules
- **Partial Class**: ALL controls inheriting from UiElement MUST be declared as `partial class`
- **Source Generator**: ALL controls MUST use the `[GenerateShadowMethods]` attribute from `PlusUi.core.Attributes`
- **No Property Duplication**: NEVER redefine properties that exist in base class (e.g., `Background`, `CornerRadius`). Use inherited properties and initialize in constructor if needed
- **Abstract Members**: Controls must implement `IsFocusable` and `AccessibilityRole`

Example:
```csharp
using PlusUi.core.Attributes;

[GenerateShadowMethods]
public partial class MyControl : UiElement
{
    protected internal override bool IsFocusable => false;
    public override AccessibilityRole AccessibilityRole => AccessibilityRole.None;

    public MyControl()
    {
        // Use inherited SetBackground instead of own BackgroundColor property
        SetBackground(new SKColor(45, 45, 45));
    }
}
```