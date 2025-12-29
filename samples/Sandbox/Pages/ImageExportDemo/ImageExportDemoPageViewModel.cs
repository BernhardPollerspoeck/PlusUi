using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using PlusUi.core;

namespace Sandbox.Pages.ImageExportDemo;

public partial class ImageExportDemoPageViewModel(IImageExportService imageExportService) : ObservableObject
{
    [ObservableProperty]
    private string _statusText = "Click a button to export an element";

    [ObservableProperty]
    private UiElement? _elementToExport;

    [RelayCommand]
    private void ExportPng()
    {
        if (ElementToExport is null) return;

        var path = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Desktop), "export.png");
        imageExportService.ExportToFile(ElementToExport, path, ImageExportFormat.Png);
        StatusText = $"Exported to: {path}";
    }

    [RelayCommand]
    private void ExportJpeg()
    {
        if (ElementToExport is null) return;

        var path = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Desktop), "export.jpg");
        imageExportService.ExportToFile(ElementToExport, path, ImageExportFormat.Jpeg, 90);
        StatusText = $"Exported to: {path}";
    }

    [RelayCommand]
    private void ExportWebp()
    {
        if (ElementToExport is null) return;

        var path = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Desktop), "export.webp");
        imageExportService.ExportToFile(ElementToExport, path, ImageExportFormat.Webp, 85);
        StatusText = $"Exported to: {path}";
    }
}
