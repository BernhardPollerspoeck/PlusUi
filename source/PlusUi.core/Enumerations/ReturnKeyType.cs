namespace PlusUi.core;

/// <summary>
/// Specifies the label and behavior of the return key on mobile keyboards.
/// </summary>
public enum ReturnKeyType
{
    /// <summary>
    /// Default return key label (typically "Return" or "Enter").
    /// </summary>
    Default = 0,

    /// <summary>
    /// "Go" label, typically used for navigation or URL entry.
    /// </summary>
    Go = 1,

    /// <summary>
    /// "Send" label, typically used for sending messages or forms.
    /// </summary>
    Send = 2,

    /// <summary>
    /// "Search" label, typically used for search fields.
    /// </summary>
    Search = 3,

    /// <summary>
    /// "Next" label, moves to the next input field.
    /// </summary>
    Next = 4,

    /// <summary>
    /// "Done" label, dismisses the keyboard.
    /// </summary>
    Done = 5,
}
