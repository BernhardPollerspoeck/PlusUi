using PlusUi.core.Attributes;
using SkiaSharp;

namespace PlusUi.core;

/// <summary>
/// Represents a single tab in a TabControl with a header and content.
/// </summary>
/// <example>
/// <code>
/// new TabItem()
///     .SetHeader("Settings")
///     .SetContent(new VStack().Children(...));
/// </code>
/// </example>
[GenerateShadowMethods]
public partial class TabItem
{
    #region Header
    /// <summary>
    /// Gets or sets the header text displayed on the tab.
    /// </summary>
    internal string Header { get; set; } = string.Empty;

    public TabItem SetHeader(string header)
    {
        Header = header;
        return this;
    }
    #endregion

    #region Content
    /// <summary>
    /// Gets or sets the content displayed when this tab is selected.
    /// </summary>
    internal UiElement? Content { get; set; }

    public TabItem SetContent(UiElement content)
    {
        Content = content;
        return this;
    }
    #endregion

    #region Icon
    /// <summary>
    /// Gets or sets an optional icon for the tab.
    /// </summary>
    internal string? Icon { get; set; }

    public TabItem SetIcon(string icon)
    {
        Icon = icon;
        return this;
    }
    #endregion

    #region IsEnabled
    /// <summary>
    /// Gets or sets whether this tab can be selected.
    /// </summary>
    internal bool IsEnabled { get; set; } = true;

    public TabItem SetIsEnabled(bool isEnabled)
    {
        IsEnabled = isEnabled;
        return this;
    }
    #endregion

    #region Tag
    /// <summary>
    /// Gets or sets custom data associated with this tab.
    /// </summary>
    internal object? Tag { get; set; }

    public TabItem SetTag(object tag)
    {
        Tag = tag;
        return this;
    }
    #endregion
}
