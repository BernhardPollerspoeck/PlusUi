using Microsoft.CodeAnalysis;
using System.Linq;
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

        // Determine the base class for the generic wrapper
        var baseClass = GetGenericBaseClass(classSymbol);

        sb.AppendLine($"// Auto-generated generic wrapper for {className}");
        sb.AppendLine($"public abstract class {className}<T> : {baseClass} where T : {className}<T>");
        sb.AppendLine("{");

        // Generate wrapper methods ONLY for methods defined directly in THIS class
        // Not inherited methods - those are already wrapped in the base generic class
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

    private static string GetGenericBaseClass(INamedTypeSymbol classSymbol)
    {
        // Check if the base type has the GenerateGenericWrapper attribute
        var baseType = classSymbol.BaseType;

        if (baseType != null &&
            baseType.GetAttributes().Any(attr =>
                attr.AttributeClass?.Name == "GenerateGenericWrapperAttribute" ||
                attr.AttributeClass?.Name == "GenerateGenericWrapper"))
        {
            // Use the generic version of the base class
            return $"{baseType.Name}<T>";
        }

        // Fall back to the non-generic base class
        return classSymbol.Name;
    }
}
