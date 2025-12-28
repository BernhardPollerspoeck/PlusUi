using PlusUi.core.Animations;

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

    // Accessibility Configuration

    /// <summary>
    /// Gets or sets a value indicating whether minimum touch target sizes (44x44 pts) are enforced on interactive controls.
    /// When true, buttons, checkboxes, radio buttons, and other interactive elements will have a minimum size
    /// to ensure they are easily tappable on touch devices, following WCAG 2.1 accessibility guidelines.
    /// Individual controls can override this via SetEnforceMinimumTouchTarget().
    /// Default is false (opt-in for accessibility).
    /// </summary>
    public bool EnforceMinimumTouchTargets { get; set; } = false;

    /// <summary>
    /// Gets or sets a value indicating whether high contrast mode support is enabled.
    /// When true, controls will use HighContrastBackground and HighContrastForeground colors
    /// when the system reports high contrast mode is active.
    /// Default is false (opt-in).
    /// </summary>
    public bool EnableHighContrastSupport { get; set; } = false;

    /// <summary>
    /// Gets or sets a value indicating whether to force high contrast mode regardless of system settings.
    /// Useful for testing high contrast appearance without enabling system-wide high contrast.
    /// Default is false.
    /// </summary>
    public bool ForceHighContrast { get; set; } = false;

    /// <summary>
    /// Gets or sets a value indicating whether system font scaling is respected.
    /// When true, text elements will scale according to system accessibility font size settings.
    /// Default is false (opt-in).
    /// </summary>
    public bool EnableFontScaling { get; set; } = false;

    /// <summary>
    /// Gets or sets a value indicating whether reduced motion preferences are respected.
    /// When true, page transitions and animations will be skipped when the system reports
    /// reduced motion preference is active.
    /// Default is false (opt-in).
    /// </summary>
    public bool RespectReducedMotion { get; set; } = false;
}
