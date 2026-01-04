namespace PlusUi.core.Services.DebugBridge;

/// <summary>
/// Internal interface for debug tree traversal.
/// Controls implement this to expose their children for debugging.
/// </summary>
internal interface IDebugInspectable
{
    /// <summary>
    /// Returns all children for debug inspection.
    /// For layout elements, this returns the Children collection.
    /// For pages, this returns the ContentTree.
    /// For leaf controls, this returns an empty collection.
    /// </summary>
    IEnumerable<UiElement> GetDebugChildren();
}
