namespace PlusUi.core.Services;

public class CommandLineService(string[] args) : ICommandLineService
{
    public string[] Args { get; } = args;

    public bool HasFlag(string flag) => Args.Contains(flag);

    public string? GetOptionValue(string option, string? defaultValue = null)
    {
        for (var i = 0; i < Args.Length - 1; i++)
        {
            if (Args[i] == option)
                return Args[i + 1];
        }
        return defaultValue;
    }
}

