#nullable enable
using System.Collections.Generic;

namespace PlusUi.SourceGenerators.UiPropGen;

internal class UiPropGenContext
{
    public string Namespace { get; set; } = "";
    public string ClassName { get; set; } = "";
    public string FullClassName => string.IsNullOrEmpty(Namespace) ? ClassName : $"{Namespace}.{ClassName}";
    public List<PropertyTemplate> Properties { get; set; } = [];
}

internal class PropertyTemplate
{
    public string PropertyName { get; set; } = "";
    public string Template { get; set; } = "";
}
