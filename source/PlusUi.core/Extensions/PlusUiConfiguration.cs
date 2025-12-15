using PlusUi.core.Animations;
using Silk.NET.Windowing;

namespace PlusUi.core;

public class PlusUiConfiguration
{
    // Window Configuration
    public SizeI Size { get; set; } = new SizeI(800, 600);
    public SizeI Position { get; set; } = new SizeI(100, 100);
    public string Title { get; set; } = "Plus Ui Application";
    public WindowState WindowState { get; set; } = WindowState.Normal;
    public WindowBorder WindowBorder { get; set; } = WindowBorder.Resizable;
    public bool IsWindowTopMost { get; set; } = false;
    public bool IsWindowTransparent { get; set; } = false;
    public bool LoadImagesSynchronously { get; set; } = false;

    // Navigation Configuration

    /// <summary>
    /// Gets or sets a value indicating whether the navigation stack is enabled.
    /// When disabled, navigation behaves like page replacement (default behavior).
    /// When enabled, pages are pushed onto a stack and can be navigated back.
    /// Default is false for backward compatibility.
    /// </summary>
    public bool EnableNavigationStack { get; set; } = false;

    /// <summary>
    /// Gets or sets a value indicating whether page state is preserved when navigating.
    /// When true, pages remain in memory when navigating away and are reused when navigating back.
    /// When false, pages are disposed when navigating away and recreated when needed.
    /// Only applicable when EnableNavigationStack is true.
    /// Default is true.
    /// </summary>
    public bool PreservePageState { get; set; } = true;

    /// <summary>
    /// Gets or sets the maximum depth of the navigation stack.
    /// This prevents infinite stack growth in applications with deep navigation hierarchies.
    /// When the maximum depth is reached, NavigateTo will throw an InvalidOperationException.
    /// Default is 50.
    /// </summary>
    public int MaxStackDepth { get; set; } = 50;

    // Transition Configuration

    /// <summary>
    /// Gets or sets the default page transition used when navigating between pages.
    /// Individual pages can override this by setting their Transition property.
    /// Default is SlideTransition with Direction = Left.
    /// Set to NoneTransition to disable transitions globally.
    /// </summary>
    public IPageTransition DefaultTransition { get; set; } = new SlideTransition();
}
