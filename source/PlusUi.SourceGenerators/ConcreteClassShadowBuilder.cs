using Microsoft.CodeAnalysis;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PlusUi.SourceGenerators;

internal static class ConcreteClassShadowBuilder
{
    internal static string GenerateConcreteClassShadows(string namespaceName, string className, INamedTypeSymbol classSymbol)
    {
        var sb = new StringBuilder();

        if (!string.IsNullOrEmpty(namespaceName))
        {
            sb.AppendLine($"namespace {namespaceName};");
            sb.AppendLine();
        }

        sb.AppendLine($"// Auto-generated shadow methods for {className}");
        sb.AppendLine($"public partial class {className}");
        sb.AppendLine("{");

        // Collect all methods from base classes that return a base type
        var shadowMethods = CollectShadowMethods(classSymbol);

        foreach (var method in shadowMethods)
        {
            sb.AppendLine($"    public new {className} {method.Name}({method.Parameters})");
            sb.AppendLine("    {");
            sb.AppendLine($"        base.{method.Name}({method.Arguments});");
            sb.AppendLine("        return this;");
            sb.AppendLine("    }");
            sb.AppendLine();
        }

        sb.AppendLine("}");

        return sb.ToString();
    }

    private static List<MethodInfo> CollectShadowMethods(INamedTypeSymbol classSymbol)
    {
        var methods = new HashSet<string>(); // To track method signatures
        var result = new List<MethodInfo>();

        // Get methods already defined in the class itself (to skip generating duplicates)
        var existingMethods = new HashSet<string>();
        foreach (var member in classSymbol.GetMembers())
        {
            if (member is IMethodSymbol methodSymbol &&
                methodSymbol.MethodKind == MethodKind.Ordinary)
            {
                var signature = $"{methodSymbol.Name}({string.Join(",", methodSymbol.Parameters.Select(p => p.Type.ToDisplayString()))})";
                existingMethods.Add(signature);
            }
        }

        // Walk up the inheritance chain
        var currentType = classSymbol.BaseType;
        while (currentType != null && currentType.SpecialType != SpecialType.System_Object)
        {
            // Get all public instance methods that return the current base type
            foreach (var member in currentType.GetMembers())
            {
                if (member is IMethodSymbol methodSymbol &&
                    methodSymbol.MethodKind == MethodKind.Ordinary &&
                    methodSymbol.DeclaredAccessibility == Accessibility.Public &&
                    !methodSymbol.IsStatic)
                {
                    // Skip methods marked with [Obsolete]
                    if (IsObsolete(methodSymbol))
                        continue;

                    // Check if return type matches one of the base types in the chain
                    if (IsReturningBaseType(methodSymbol, classSymbol) &&
                        methodSymbol.ReturnType.NullableAnnotation != NullableAnnotation.Annotated)
                    {
                        var methodSignature = $"{methodSymbol.Name}({string.Join(",", methodSymbol.Parameters.Select(p => p.Type.ToDisplayString()))})";

                        // Skip if method is already defined in the class or already added
                        if (existingMethods.Contains(methodSignature) || !methods.Add(methodSignature))
                        {
                            continue;
                        }

                        var methodInfo = CreateMethodInfo(methodSymbol);
                        if (methodInfo != null)
                        {
                            result.Add(methodInfo);
                        }
                    }
                }
            }

            currentType = currentType.BaseType;
        }

        return result;
    }

    private static bool IsObsolete(IMethodSymbol methodSymbol)
    {
        return methodSymbol.GetAttributes().Any(attr =>
            attr.AttributeClass?.ToDisplayString() == "System.ObsoleteAttribute");
    }

    private static bool IsReturningBaseType(IMethodSymbol methodSymbol, INamedTypeSymbol classSymbol)
    {
        // Check if the method returns any type in the inheritance chain
        var currentType = classSymbol.BaseType;
        while (currentType != null && currentType.SpecialType != SpecialType.System_Object)
        {
            if (SymbolEqualityComparer.Default.Equals(methodSymbol.ReturnType, currentType))
            {
                return true;
            }
            currentType = currentType.BaseType;
        }
        return false;
    }

    private static MethodInfo CreateMethodInfo(IMethodSymbol methodSymbol)
    {
        var name = methodSymbol.Name;
        var parameters = string.Join(", ", methodSymbol.Parameters.Select(p =>
        {
            var paramType = p.Type.ToDisplayString(SymbolDisplayFormat.FullyQualifiedFormat);
            var paramName = p.Name;

            if (p.HasExplicitDefaultValue)
            {
                var defaultValue = FormatDefaultValue(p);
                return $"{paramType} {paramName} = {defaultValue}";
            }

            return $"{paramType} {paramName}";
        }));

        var arguments = string.Join(", ", methodSymbol.Parameters.Select(p => p.Name));

        return new MethodInfo(name, parameters, arguments);
    }

    private static string FormatDefaultValue(IParameterSymbol parameter)
    {
        if (parameter.ExplicitDefaultValue == null)
        {
            return parameter.Type.IsValueType ? "default" : "null";
        }

        var value = parameter.ExplicitDefaultValue;

        return value switch
        {
            string stringValue => $"\"{stringValue}\"",
            bool boolValue => boolValue ? "true" : "false",
            _ when parameter.Type.TypeKind == TypeKind.Enum => $"{parameter.Type.ToDisplayString(SymbolDisplayFormat.FullyQualifiedFormat)}.{value}",
            _ => value.ToString()
        };
    }
}
