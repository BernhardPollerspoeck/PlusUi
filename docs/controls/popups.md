---
title: Popups
layout: default
parent: Controls
nav_order: 11
---

# Popups

Modal dialogs that overlay the main content.

---

## Overview

Popups are modal dialogs that appear on top of your application content. They are useful for:
- Confirmation dialogs
- Detail views
- Forms that require focused input
- Alert messages

---

## Creating a Popup

Inherit from `UiPopupElement<TArgument>` where `TArgument` is the type of data you want to pass to the popup.

### Basic Popup

```csharp
public partial class ConfirmPopupViewModel : ObservableObject
{
    [ObservableProperty]
    private string _message = "";
}

public class ConfirmPopup(ConfirmPopupViewModel vm) : UiPopupElement<string>(vm)
{
    protected override UiElement Build() =>
        new Border()
            .SetBackground(new SolidColorBackground(new Color(45, 45, 45)))
            .SetCornerRadius(16)
            .SetPadding(new Margin(24))
            .SetDesiredWidth(300)
            .AddChild(
                new VStack(
                    new Label()
                        .SetText("Confirm")
                        .SetTextSize(20)
                        .SetFontWeight(FontWeight.Bold)
                        .SetTextColor(Colors.White),
                    new Label()
                        .BindText(nameof(vm.Message), () => vm.Message)
                        .SetTextColor(Colors.White)
                        .SetMargin(new Margin(0, 16, 0, 24)),
                    new HStack(
                        new Button()
                            .SetText("Cancel")
                            .SetCommand(new RelayCommand(() => Close(false))),
                        new Button()
                            .SetText("OK")
                            .SetCommand(new RelayCommand(() => Close(true)))
                            .SetMargin(new Margin(12, 0, 0, 0))
                    ).SetHorizontalAlignment(HorizontalAlignment.Right)
                )
            );

    public override void Close(bool success)
    {
        var popupService = ServiceProviderService.ServiceProvider?
            .GetRequiredService<IPopupService>();
        popupService?.ClosePopup(success);
    }
}
```

---

## Showing Popups

### Inject IPopupService

```csharp
public partial class MainViewModel : ObservableObject
{
    private readonly IPopupService _popupService;

    public MainViewModel(IPopupService popupService)
    {
        _popupService = popupService;
    }
}
```

### Show Popup

```csharp
[RelayCommand]
private void ShowConfirm()
{
    _popupService.ShowPopup<ConfirmPopup, string>(
        arg: "Are you sure you want to delete this item?",
        onClosed: () =>
        {
            // Called when popup closes with success=true
            DeleteItem();
        });
}
```

### With Configuration

```csharp
_popupService.ShowPopup<ConfirmPopup, string>(
    arg: "Delete item?",
    onClosed: OnConfirmed,
    configure: config =>
    {
        config.CloseOnBackgroundClick = true;  // Click outside to close
        config.CloseOnEscape = true;           // Press Escape to close
        config.BackgroundColor = new Color(0, 0, 0, 128);  // Semi-transparent
    });
```

---

## Registering Popups

Register popups in your app configuration:

```csharp
public class App : IAppConfiguration
{
    public void ConfigureServices(IServiceCollection services)
    {
        // Register popup and its ViewModel
        services.AddTransient<ConfirmPopup>();
        services.AddTransient<ConfirmPopupViewModel>();

        services.AddTransient<EditItemPopup>();
        services.AddTransient<EditItemPopupViewModel>();
    }
}
```

---

## Popup Lifecycle

### Appearing

Called when the popup is shown:

```csharp
public override void Appearing()
{
    // Initialize data, start animations, etc.
    ((MyPopupViewModel)ViewModel).LoadData();
}
```

### Disappearing

Called when the popup is about to close:

```csharp
public override void Disappearing()
{
    // Cleanup, save state, etc.
}
```

---

## Examples

### Edit Form Popup

```csharp
public class PersonEditPopup(PersonEditPopupViewModel vm)
    : UiPopupElement<Person>(vm)
{
    protected override UiElement Build() =>
        new Border()
            .SetBackground(new SolidColorBackground(new Color(40, 40, 40)))
            .SetCornerRadius(12)
            .SetPadding(new Margin(24))
            .SetDesiredWidth(400)
            .AddChild(
                new VStack(
                    new Label()
                        .SetText("Edit Person")
                        .SetTextSize(24)
                        .SetFontWeight(FontWeight.Bold)
                        .SetTextColor(Colors.White),

                    new Label().SetText("Name").SetMargin(new Margin(0, 16, 0, 4)),
                    new Entry()
                        .BindText(nameof(vm.Name), () => vm.Name, v => vm.Name = v)
                        .SetPlaceholder("Enter name"),

                    new Label().SetText("Email").SetMargin(new Margin(0, 16, 0, 4)),
                    new Entry()
                        .BindText(nameof(vm.Email), () => vm.Email, v => vm.Email = v)
                        .SetPlaceholder("Enter email"),

                    new HStack(
                        new Button()
                            .SetText("Cancel")
                            .SetCommand(new RelayCommand(() => Close(false))),
                        new Button()
                            .SetText("Save")
                            .SetCommand(new RelayCommand(() => Close(true)))
                    )
                    .SetHorizontalAlignment(HorizontalAlignment.Right)
                    .SetMargin(new Margin(0, 24, 0, 0))
                )
            );

    public override void Close(bool success)
    {
        if (success)
        {
            OnClosed?.Invoke();
        }
        var popupService = ServiceProviderService.ServiceProvider?
            .GetRequiredService<IPopupService>();
        popupService?.ClosePopup(success);
    }
}
```

### Alert Popup

```csharp
public class AlertPopup(AlertPopupViewModel vm) : UiPopupElement<string>(vm)
{
    protected override UiElement Build() =>
        new Border()
            .SetBackground(new SolidColorBackground(new Color(50, 50, 50)))
            .SetCornerRadius(16)
            .SetPadding(new Margin(20))
            .SetDesiredWidth(280)
            .AddChild(
                new VStack(
                    new Label()
                        .BindText(nameof(vm.Message), () => vm.Message)
                        .SetTextColor(Colors.White)
                        .SetHorizontalTextAlignment(HorizontalTextAlignment.Center),
                    new Button()
                        .SetText("OK")
                        .SetHorizontalAlignment(HorizontalAlignment.Stretch)
                        .SetCommand(new RelayCommand(() => Close(true)))
                        .SetMargin(new Margin(0, 20, 0, 0))
                )
            );

    public override void Close(bool success)
    {
        var popupService = ServiceProviderService.ServiceProvider?
            .GetRequiredService<IPopupService>();
        popupService?.ClosePopup(success);
    }
}

// Usage
_popupService.ShowPopup<AlertPopup, string>("Operation completed successfully!");
```

---

## IPopupConfiguration

| Property | Type | Description |
|:---------|:-----|:------------|
| `CloseOnBackgroundClick` | `bool` | Close when clicking outside the popup |
| `CloseOnEscape` | `bool` | Close when pressing Escape key |
| `BackgroundColor` | `Color` | Overlay background color |
