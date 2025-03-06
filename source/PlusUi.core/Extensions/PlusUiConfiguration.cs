using Silk.NET.Windowing;

namespace PlusUi.core;

public class PlusUiConfiguration
{
    public SizeI Size { get; set; } = new SizeI(800, 600);
    public SizeI Position { get; set; } = new SizeI(100, 100);
    public string Title { get; set; } = "Plus Ui Application";
    public WindowState WindowState { get; set; } = WindowState.Normal;
    public WindowBorder WindowBorder { get; set; } = WindowBorder.Resizable;
    public bool IsWindowTopMost { get; set; } = false;
    public bool IsWindowTransparent { get; set; } = false;
}
