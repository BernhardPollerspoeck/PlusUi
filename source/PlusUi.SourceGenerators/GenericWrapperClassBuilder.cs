using Microsoft.CodeAnalysis;
using System.Text;

namespace PlusUi.SourceGenerators;

internal static class GenericWrapperClassBuilder
{
    internal static string GenerateGenericWrapperClass(string namespaceName, string className, INamedTypeSymbol classSymbol)
    {
        var sb = new StringBuilder();

        if (!string.IsNullOrEmpty(namespaceName))
        {
            sb.AppendLine($"namespace {namespaceName};");
            sb.AppendLine();
        }

        sb.AppendLine($"// Auto-generated generic wrapper for {className}");
        sb.AppendLine($"public abstract class {className}<T> : {className} where T : {className}<T>");
        sb.AppendLine("{");

        // Generate all the wrapper methods
        var methods = UiElementMethodProvider.GetMethodsToWrap(classSymbol);
        foreach (var method in methods)
        {
            sb.AppendLine($"    public new T {method.Name}({method.Parameters})");
            sb.AppendLine("    {");
            sb.AppendLine($"        base.{method.Name}({method.Arguments});");
            sb.AppendLine("        return (T)this;");
            sb.AppendLine("    }");
            sb.AppendLine();
        }

        sb.AppendLine("}");

        return sb.ToString();
    }
}
