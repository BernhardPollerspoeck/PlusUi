using Silk.NET.Input;

namespace PlusUi.desktop.Tests;

#pragma warning disable CS0067
internal class FakeKeyboard : IKeyboard
{
    public string ClipboardText { get; set; } = string.Empty;
    public IReadOnlyList<Key> SupportedKeys => [];
    public string Name => "FakeKeyboard";
    public int Index => 0;
    public bool IsConnected => true;

    public event Action<IKeyboard, Key, int>? KeyDown;
    public event Action<IKeyboard, Key, int>? KeyUp;
    public event Action<IKeyboard, char>? KeyChar;

    public bool IsKeyPressed(Key key) => false;
    public bool IsScancodePressed(int scancode) => false;
    public void BeginInput() { }
    public void EndInput() { }
}
#pragma warning restore CS0067
