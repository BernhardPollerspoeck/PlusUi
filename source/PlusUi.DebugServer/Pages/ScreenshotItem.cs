using CommunityToolkit.Mvvm.ComponentModel;

namespace PlusUi.DebugServer.Pages;

internal partial class ScreenshotItem : ObservableObject
{
    public required string Id { get; init; }

    [ObservableProperty]
    private string? _elementId;

    [ObservableProperty]
    private byte[] _imageData = [];

    [ObservableProperty]
    private int _width;

    [ObservableProperty]
    private int _height;

    [ObservableProperty]
    private DateTimeOffset _timestamp;

    public string DisplayName => ElementId ?? "Full Page";

    public string TimestampDisplay => Timestamp.ToString("HH:mm:ss");

    public string SizeDisplay => $"{Width}x{Height}";
}
