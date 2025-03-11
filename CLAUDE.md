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
dotnet test PlusUi.sln --filter "FullyQualifiedName=UiPlus.core.Tests.LabelTests.TestLabelMeasureAndArrange_NoMargin_LeftTopAligned"

# Run tests in a specific class
dotnet test PlusUi.sln --filter "FullyQualifiedName~UiPlus.core.Tests.LabelTests"
```

## Code Style Guidelines
- **Naming**: PascalCase for types, methods, and properties; camelCase for local variables and parameters
- **Nullable**: Enable nullable reference types (`<Nullable>enable</Nullable>`)
- **Language**: Use latest C# features (`<LangVersion>latest</LangVersion>` or `<LangVersion>preview</LangVersion>`)
- **Framework**: Target .NET 9.0
- **Styles**: Fluent API style with builder pattern (method chaining)
- **UI Components**: Follow the pattern of exposing properties with internal setters and public fluent Set* methods
- **Tests**: Use MSTest with AAA pattern (Arrange-Act-Assert)
- **Architecture**: Controls inherit from base elements (UiElement, UiTextElement)
- **Error Handling**: Avoid exceptions for control flow; use nullable types appropriately
- **Component Structure**: Components should have predictable initialization through constructor and fluent methods