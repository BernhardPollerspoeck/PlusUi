# PlusUi

PlusUi is a fully cross-platform capable UI Framework. It runs on iOS, Android, Windows, Mac, and Linux. The project is in its early stages and welcomes contributions. However, we ask that you open issues to discuss planned changes before submitting pull requests.

## Features

- Cross-platform support: iOS (TODO), Android (TODO), Windows, Mac (TODO) and Linux (TODO)
- Easy to use and extend
- Mvvm Data Binding
- Modern UI components (TODO)
- Customizable themes (TODO)

## Example Ui Definition
Here just as a quick introduction is a Sample Ui that i use to visualize my current developments.
```csharp
public class MainPage(MainViewModel vm) : UiPageElement(vm)
{
    protected override UiElement Build()
    {
        return new VStack(
            new Solid(height: 10, color: SKColors.Green)
                .BindDesiredWidth(nameof(vm.Count), () => vm.Count * 3)
                .SetMargin(new(0, 10, 0, 0)),
            new Solid(50, 10, SKColors.Red),
            new VStack(
                new Solid(10, 10, SKColors.Blue),
                new Solid(10, 10, SKColors.Purple))
                .SetHorizontalAlignment(HorizontalAlignment.Right)
                .SetMargin(new(10, 0, 0, 0)),
            new Label()
                .SetText("Hello World !"),
            new Button()
                .SetCommand(new SyncCommand(() => vm.Count = 0))
                .SetText("Click me")
                .SetMargin(new(10, 0, 0, 0)),
            new Label()
                .BindText(nameof(vm.Count), () => vm.Count.ToString())
                .SetTextColor(SKColors.Black)
                .SetBackgroundColor(SKColors.Aqua))
            .SetMargin(new(10, 0, 0, 0));
    }
}
```


## Contributing

We welcome contributions! Please open an issue to discuss any planned changes before submitting a pull request.

## License

This project is licensed under the MIT License - see the [LICENSE](LICENSE) file for details.
