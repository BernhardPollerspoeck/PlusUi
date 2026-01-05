using PlusUi.core.CoreElements;

namespace PlusUi.core;

/// <summary>
/// Provides services for displaying and managing modal popups.
/// </summary>
public interface IPopupService
{
    /// <summary>
    /// Shows a modal popup of the specified type with optional arguments and configuration.
    /// </summary>
    /// <typeparam name="TPopup">The type of popup to show.</typeparam>
    /// <typeparam name="TArg">The type of argument to pass to the popup.</typeparam>
    /// <param name="arg">Optional argument to pass to the popup.</param>
    /// <param name="onClosed">Optional callback invoked when the popup is closed successfully.</param>
    /// <param name="configure">Optional configuration action for popup behavior (close on background click, etc.).</param>
    /// <remarks>
    /// The popup type must be registered via dependency injection. Only one popup can be displayed at a time.
    /// If a popup is already open, it will be closed before showing the new one.
    /// </remarks>
    /// <exception cref="InvalidOperationException">Thrown if the popup is not registered in the service collection.</exception>
    void ShowPopup<TPopup, TArg>(
        TArg? arg = default,
        Action? onClosed = null,
        Action<IPopupConfiguration>? configure = null)
        where TPopup : UiPopupElement<TArg>;

    /// <summary>
    /// Shows a modal popup with typed result support.
    /// </summary>
    /// <typeparam name="TPopup">The type of popup to show.</typeparam>
    /// <typeparam name="TArg">The type of argument to pass to the popup.</typeparam>
    /// <typeparam name="TResult">The type of result returned by the popup.</typeparam>
    /// <param name="arg">Optional argument to pass to the popup.</param>
    /// <param name="onClosed">Optional async callback invoked when the popup is closed successfully, receiving the result.</param>
    /// <param name="configure">Optional configuration action for popup behavior (close on background click, etc.).</param>
    /// <remarks>
    /// The popup type must be registered via dependency injection. Only one popup can be displayed at a time.
    /// If a popup is already open, it will be closed before showing the new one.
    /// </remarks>
    /// <exception cref="InvalidOperationException">Thrown if the popup is not registered in the service collection.</exception>
    void ShowPopup<TPopup, TArg, TResult>(
        TArg? arg = default,
        Func<TResult?, Task>? onClosed = null,
        Action<IPopupConfiguration>? configure = null)
        where TPopup : UiPopupElement<TArg, TResult>;

    /// <summary>
    /// Closes the currently displayed popup.
    /// </summary>
    /// <param name="success">Indicates whether the popup was closed successfully (true) or cancelled (false). Affects whether the onClosed callback is invoked.</param>
    void ClosePopup(bool success = true);
}
