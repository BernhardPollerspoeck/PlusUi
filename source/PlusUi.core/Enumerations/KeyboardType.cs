namespace PlusUi.core;

/// <summary>
/// Specifies the type of keyboard to display for text input on mobile platforms.
/// </summary>
public enum KeyboardType
{
    /// <summary>
    /// Default keyboard with standard alphanumeric keys.
    /// </summary>
    Default = 0,

    /// <summary>
    /// Standard text keyboard optimized for general text entry.
    /// </summary>
    Text = 1,

    /// <summary>
    /// Numeric keyboard for entering numbers.
    /// </summary>
    Numeric = 2,

    /// <summary>
    /// Email keyboard with @ and . symbols easily accessible.
    /// </summary>
    Email = 3,

    /// <summary>
    /// Telephone keyboard optimized for entering phone numbers.
    /// </summary>
    Telephone = 4,

    /// <summary>
    /// URL keyboard with / and .com easily accessible.
    /// </summary>
    Url = 5,
}
