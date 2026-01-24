namespace PlusUi.core;

public interface ITextInputControl : IInteractiveControl
{
    void SetSelectionStatus(bool isSelected);
    void HandleInput(PlusKey key, bool shift, bool ctrl);
    void HandleInput(char chr);
    void HandleClick(float localX, float localY);
}
