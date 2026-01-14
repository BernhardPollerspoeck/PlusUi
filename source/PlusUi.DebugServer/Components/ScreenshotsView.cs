using NativeFileDialogSharp;
using PlusUi.core;
using PlusUi.DebugServer.Pages;
using TextCopy;

namespace PlusUi.DebugServer.Components;

/// <summary>
/// Displays captured screenshots with save/copy/delete actions.
/// </summary>
internal class ScreenshotsView : UserControl
{
    private readonly MainViewModel _viewModel;

    public ScreenshotsView(MainViewModel viewModel)
    {
        _viewModel = viewModel;
    }

    protected override UiElement Build()
    {
        return new VStack()
            .AddChild(
                // Header
                new HStack(
                    new Label()
                        .SetText("Screenshots")
                        .SetTextColor(Colors.White)
                        .SetTextSize(16)
                        .SetFontWeight(FontWeight.SemiBold),
                    new Label()
                        .BindText(() => $"({_viewModel.ScreenshotsCount})")
                        .SetTextColor(Colors.Gray)
                        .SetTextSize(14))
                .SetSpacing(8)
                .SetMargin(new Margin(12, 8)))
            .AddChild(
                // Screenshots list
                new ItemsList<ScreenshotItem>()
                    .BindItemsSource(() => _viewModel.Screenshots)
                    .SetItemTemplate((item, index) => CreateScreenshotItem(item))
                    .SetMargin(new Margin(8)));
    }

    private UiElement CreateScreenshotItem(ScreenshotItem item)
    {
        return new Border()
            .SetBackground(new Color(45, 45, 45))
            .SetCornerRadius(4)
            .SetMargin(new Margin(4))
            .SetDesiredHeight(116)
            .SetHorizontalAlignment(HorizontalAlignment.Stretch)
            .AddChild(
                new Grid()
                    .AddColumn(Column.Star, 1)
                    .AddColumn(Column.Star, 1)
                    .AddColumn(Column.Auto)
                    .SetMargin(new Margin(8))
                    .AddChild(
                        // Thumbnail preview with border - stretches vertically, left aligned
                        new Border()
                            .SetStrokeColor(new Color(80, 80, 80))
                            .SetStrokeThickness(1)
                            .SetCornerRadius(2)
                            .SetVerticalAlignment(VerticalAlignment.Stretch)
                            .SetHorizontalAlignment(HorizontalAlignment.Left)
                            .AddChild(
                                new ScreenshotPreview()
                                    .SetImageData(item.ImageData)
                                    .SetVerticalAlignment(VerticalAlignment.Stretch)
                                    .SetHorizontalAlignment(HorizontalAlignment.Left)),
                        row: 0, column: 0)
                    .AddChild(
                        // Info: Name, Size, Timestamp
                        new VStack()
                            .AddChild(new Label()
                                .SetText(item.DisplayName)
                                .SetTextColor(Colors.White)
                                .SetTextSize(13)
                                .SetFontWeight(FontWeight.Medium))
                            .AddChild(new Label()
                                .SetText(item.SizeDisplay)
                                .SetTextColor(Colors.Gray)
                                .SetTextSize(11))
                            .AddChild(new Label()
                                .SetText(item.TimestampDisplay)
                                .SetTextColor(Colors.Gray)
                                .SetTextSize(11))
                            .SetSpacing(2)
                            .SetVerticalAlignment(VerticalAlignment.Center)
                            .SetMargin(new Margin(12, 0, 0, 0)),
                        row: 0, column: 1)
                    .AddChild(
                        // Action buttons: Copy, Save, Delete
                        new HStack()
                            .AddChild(new Button()
                                .SetIcon("clipboard.svg")
                                .SetIconTintColor(Colors.White)
                                .SetBackground(new Color(60, 60, 60))
                                .SetCornerRadius(4)
                                .SetDesiredWidth(32)
                                .SetDesiredHeight(32)
                                .SetOnClick(() => CopyToClipboard(item)))
                            .AddChild(new Button()
                                .SetIcon("save.svg")
                                .SetIconTintColor(Colors.White)
                                .SetBackground(new Color(60, 60, 60))
                                .SetCornerRadius(4)
                                .SetDesiredWidth(32)
                                .SetDesiredHeight(32)
                                .SetOnClick(() => SaveScreenshot(item)))
                            .AddChild(new Button()
                                .SetIcon("trash.svg")
                                .SetIconTintColor(new Color(255, 100, 100))
                                .SetBackground(new Color(60, 60, 60))
                                .SetCornerRadius(4)
                                .SetDesiredWidth(32)
                                .SetDesiredHeight(32)
                                .SetCommand(_viewModel.DeleteScreenshotCommand)
                                .SetCommandParameter(item))
                            .SetSpacing(4)
                            .SetVerticalAlignment(VerticalAlignment.Center),
                        row: 0, column: 2));
    }

    private void SaveScreenshot(ScreenshotItem item)
    {
        var timestamp = item.Timestamp.ToString("yyyyMMdd_HHmmss");
        var elementName = string.IsNullOrEmpty(item.ElementId) ? "FullPage" : item.ElementId.Replace(".", "_");
        var defaultFileName = $"Screenshot_{elementName}_{timestamp}.png";

        try
        {
            var result = Dialog.FileSave("png", defaultFileName);
            if (result.IsOk && !string.IsNullOrEmpty(result.Path))
            {
                var filePath = result.Path;
                if (!filePath.EndsWith(".png", StringComparison.OrdinalIgnoreCase))
                    filePath += ".png";

                File.WriteAllBytes(filePath, item.ImageData);
                if (_viewModel.SelectedApp != null)
                    _viewModel.SelectedApp.StatusText = $"Saved: {filePath}";
            }
        }
        catch (Exception ex)
        {
            if (_viewModel.SelectedApp != null)
                _viewModel.SelectedApp.StatusText = $"Save failed: {ex.Message}";
        }
    }

    private async void CopyToClipboard(ScreenshotItem item)
    {
        try
        {
            // Convert PNG bytes to base64 data URL for clipboard
            var base64 = Convert.ToBase64String(item.ImageData);
            var dataUrl = $"data:image/png;base64,{base64}";
            await ClipboardService.SetTextAsync(dataUrl);

            if (_viewModel.SelectedApp != null)
                _viewModel.SelectedApp.StatusText = "Screenshot copied to clipboard (as data URL)";
        }
        catch (Exception ex)
        {
            if (_viewModel.SelectedApp != null)
                _viewModel.SelectedApp.StatusText = $"Copy failed: {ex.Message}";
        }
    }
}
