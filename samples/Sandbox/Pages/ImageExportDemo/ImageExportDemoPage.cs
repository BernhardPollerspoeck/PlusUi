using PlusUi.core;

namespace Sandbox.Pages.ImageExportDemo;

public class ImageExportDemoPage(ImageExportDemoPageViewModel vm) : UiPageElement(vm)
{
    private UiElement? _exportTarget;

    protected override UiElement Build()
    {
        return new ScrollView(
            new VStack(
                new Label()
                    .SetText("Image Export Demo")
                    .SetTextSize(32)
                    .SetTextColor(Colors.White)
                    .SetHorizontalTextAlignment(HorizontalTextAlignment.Center)
                    .SetMargin(new Margin(0, 20, 0, 30)),

                new Label()
                    .SetText("This demo shows how to export UI elements as images.")
                    .SetTextColor(Colors.LightGray)
                    .SetHorizontalTextAlignment(HorizontalTextAlignment.Center)
                    .SetMargin(new Margin(0, 0, 0, 20)),

                CreateExportTarget(),

                new HStack(
                    new Button()
                        .SetText("Export as PNG")
                        .SetPadding(new Margin(15, 10))
                        .SetBackground(new SolidColorBackground(Colors.ForestGreen))
                        .SetCornerRadius(8)
                        .SetCommand(vm.ExportPngCommand)
                        .SetMargin(new Margin(5)),
                    new Button()
                        .SetText("Export as JPEG")
                        .SetPadding(new Margin(15, 10))
                        .SetBackground(new SolidColorBackground(Colors.DodgerBlue))
                        .SetCornerRadius(8)
                        .SetCommand(vm.ExportJpegCommand)
                        .SetMargin(new Margin(5)),
                    new Button()
                        .SetText("Export as WebP")
                        .SetPadding(new Margin(15, 10))
                        .SetBackground(new SolidColorBackground(Colors.Purple))
                        .SetCornerRadius(8)
                        .SetCommand(vm.ExportWebpCommand)
                        .SetMargin(new Margin(5))
                ).SetHorizontalAlignment(HorizontalAlignment.Center)
                 .SetMargin(new Margin(0, 20)),

                new Label()
                    .BindText(() => vm.StatusText)
                    .SetTextColor(Colors.Yellow)
                    .SetHorizontalTextAlignment(HorizontalTextAlignment.Center)
                    .SetMargin(new Margin(0, 10))
            )
        );
    }

    private UiElement CreateExportTarget()
    {
        _exportTarget = new Border()
            .AddChild(
                new VStack(
                    new Label()
                        .SetText("Export Target")
                        .SetTextSize(24)
                        .SetTextColor(Colors.White)
                        .SetFontWeight(FontWeight.Bold)
                        .SetHorizontalTextAlignment(HorizontalTextAlignment.Center),
                    new HStack(
                        new Solid()
                            .SetBackground(new SolidColorBackground(Colors.Red))
                            .SetDesiredSize(new Size(50, 50))
                            .SetCornerRadius(8),
                        new Solid()
                            .SetBackground(new SolidColorBackground(Colors.Green))
                            .SetDesiredSize(new Size(50, 50))
                            .SetCornerRadius(8),
                        new Solid()
                            .SetBackground(new SolidColorBackground(Colors.Blue))
                            .SetDesiredSize(new Size(50, 50))
                            .SetCornerRadius(8)
                    ).SetHorizontalAlignment(HorizontalAlignment.Center)
                     .SetMargin(new Margin(0, 10)),
                    new Label()
                        .SetText("This element will be exported as an image")
                        .SetTextColor(Colors.LightGray)
                        .SetHorizontalTextAlignment(HorizontalTextAlignment.Center)
                ).SetMargin(new Margin(20))
            )
            .SetBackground(new LinearGradient(new Color(60, 60, 80), new Color(30, 30, 50), 135))
            .SetCornerRadius(16)
            .SetStrokeColor(Colors.White)
            .SetStrokeThickness(2)
            .SetHorizontalAlignment(HorizontalAlignment.Center)
            .SetMargin(new Margin(20));

        vm.ElementToExport = _exportTarget;

        return _exportTarget;
    }
}
