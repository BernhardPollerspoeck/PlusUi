namespace PlusUi.core.Services;

/// <summary>
/// Service for managing RadioButton group state.
/// Tracks all RadioButtons and ensures only one per group is selected.
/// </summary>
public interface IRadioButtonManager
{
    /// <summary>
    /// Registers a RadioButton with the manager.
    /// </summary>
    void Register(RadioButton radioButton);

    /// <summary>
    /// Unregisters a RadioButton from the manager.
    /// </summary>
    void Unregister(RadioButton radioButton);

    /// <summary>
    /// Notifies the manager that a RadioButton was selected.
    /// Deselects all other RadioButtons in the same group.
    /// </summary>
    void NotifySelected(RadioButton radioButton);
}
