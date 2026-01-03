namespace PlusUi.core.Services.Rendering;

/// <summary>
/// Interface for components that require continuous rendering (animations, transitions, etc.).
/// Components implementing this interface can register with InvalidationTracker to control
/// when the render loop should be active.
/// </summary>
public interface IInvalidator
{
    /// <summary>
    /// Indicates whether this component currently needs continuous rendering.
    /// When true, the platform render loop will continue running.
    /// When false, the component is idle and doesn't require rendering.
    /// </summary>
    bool NeedsRendering { get; }

    /// <summary>
    /// Event that fires when NeedsRendering state changes.
    /// This allows InvalidationTracker to react immediately to state changes
    /// without polling every frame.
    /// </summary>
    event EventHandler? InvalidationChanged;
}
