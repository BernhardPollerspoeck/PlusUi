
namespace PlusUi.core;

public interface ITextInputControl
{
    void SetSelectionStatus(bool isSelected);
    void HandleInput(PlusKey key);
    void HandleInput(char chr);
}
