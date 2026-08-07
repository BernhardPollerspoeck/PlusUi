namespace PlusUi.core;

/// <summary>
/// Where text sits inside the vertical space its element was given.
/// </summary>
public enum VerticalTextAlignment
{
    /// <summary>
    /// Against the top edge. The default, because it is what text elements did before this
    /// existed and changing that silently would move text in every application.
    /// </summary>
    Top = 0,

    /// <summary>Centred in the available height.</summary>
    Center = 1,

    /// <summary>Against the bottom edge.</summary>
    Bottom = 2
}
