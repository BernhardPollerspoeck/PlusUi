namespace PlusUi.core;

/// <summary>
/// Defines accessibility landmarks for skip navigation.
/// Landmarks help screen reader users quickly navigate to main content areas.
/// </summary>
public enum AccessibilityLandmark
{
    /// <summary>
    /// No landmark - default for most elements.
    /// </summary>
    None,

    /// <summary>
    /// Main content area of the page.
    /// </summary>
    Main,

    /// <summary>
    /// Primary navigation area.
    /// </summary>
    Navigation,

    /// <summary>
    /// Search functionality.
    /// </summary>
    Search,

    /// <summary>
    /// Banner/header area, typically contains site title and primary navigation.
    /// </summary>
    Banner,

    /// <summary>
    /// Footer/content info area.
    /// </summary>
    ContentInfo,

    /// <summary>
    /// Complementary content, like a sidebar.
    /// </summary>
    Complementary,

    /// <summary>
    /// Form region.
    /// </summary>
    Form,

    /// <summary>
    /// Generic region with a label.
    /// </summary>
    Region
}
