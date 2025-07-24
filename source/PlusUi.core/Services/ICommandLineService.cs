namespace PlusUi.core.Services;
public interface ICommandLineService
{
    string[] Args { get; }
    bool HasFlag(string flag);
    string? GetOptionValue(string option, string? defaultValue = null);
}

