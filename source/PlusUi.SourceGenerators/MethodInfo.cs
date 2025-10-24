namespace PlusUi.SourceGenerators;

internal class MethodInfo(string name, string parameters, string arguments)
{
    public string Name { get; } = name;
    public string Parameters { get; } = parameters;
    public string Arguments { get; } = arguments;
}