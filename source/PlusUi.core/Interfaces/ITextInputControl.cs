
namespace PlusUi.core;

public interface ITextInputControl : IInteractiveControl
{
    void SetSelectionStatus(bool isSelected);
    void HandleInput(PlusKey key);
    void HandleInput(char chr);
}
