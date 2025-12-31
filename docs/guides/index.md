---
title: Guides
layout: default
nav_order: 3
has_children: true
---

# Guides

In-depth guides for building great PlusUi applications.

## Available Guides

| Guide | Description |
|:------|:------------|
| [Project Setup](project-setup.html) | App configuration, DI, pages, navigation |
| [Theming](theming.html) | Styles, colors, fonts, custom themes |
| [Best Practices](best-practices.html) | Patterns, performance, do's and don'ts |
| [Headless Mode](headless.html) | Automated testing, screenshots, CI/CD |

## Quick Tips

{: .tip }
> **Use Primary Constructors** - PlusUi pages and ViewModels work great with C# primary constructors for dependency injection.

{: .tip }
> **Fluent API** - Chain methods for cleaner code: `.SetText("Hi").SetTextSize(20).SetMargin(new Margin(10))`

{: .warning }
> **Avoid Blocking the UI Thread** - Use `async/await` for long operations to keep your app responsive.
