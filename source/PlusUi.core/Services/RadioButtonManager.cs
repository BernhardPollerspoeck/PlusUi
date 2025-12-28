namespace PlusUi.core.Services;

/// <summary>
/// Manages RadioButton group state, ensuring mutual exclusivity within groups.
/// </summary>
public class RadioButtonManager : IRadioButtonManager
{
    private readonly List<WeakReference<RadioButton>> _radioButtons = [];

    public void Register(RadioButton radioButton)
    {
        if (!IsAlreadyRegistered(radioButton))
        {
            _radioButtons.Add(new WeakReference<RadioButton>(radioButton));
        }
    }

    public void Unregister(RadioButton radioButton)
    {
        _radioButtons.RemoveAll(wr => !wr.TryGetTarget(out var target) || target == radioButton);
    }

    public void NotifySelected(RadioButton selectedButton)
    {
        if (selectedButton.Group == null)
        {
            return;
        }

        foreach (var weakRef in _radioButtons)
        {
            if (weakRef.TryGetTarget(out var radioButton) &&
                radioButton != selectedButton &&
                radioButton.Group != null &&
                radioButton.Group.Equals(selectedButton.Group))
            {
                radioButton.DeselectInternal();
            }
        }

        // Cleanup dead references periodically
        CleanupDeadReferences();
    }

    private bool IsAlreadyRegistered(RadioButton radioButton)
    {
        // Clean up dead references and check if already registered
        var alreadyRegistered = false;
        for (int i = _radioButtons.Count - 1; i >= 0; i--)
        {
            if (!_radioButtons[i].TryGetTarget(out var target))
            {
                _radioButtons.RemoveAt(i); // Remove dead reference
            }
            else if (target == radioButton)
            {
                alreadyRegistered = true;
            }
        }
        return alreadyRegistered;
    }

    private void CleanupDeadReferences()
    {
        _radioButtons.RemoveAll(wr => !wr.TryGetTarget(out _));
    }
}
