using Silk.NET.Input;

namespace PlusUi.core;

public interface ITextInputControl
{
    void SetSelectionStatus(bool isSelected);
    void HandleInput(Key key);
    void HandleInput(char chr);
}
