using Microsoft.CodeAnalysis;
using System.Collections.Generic;
using System.Linq;

namespace PlusUi.SourceGenerators;

internal static class UiElementMethodProvider
{
    internal static MethodInfo[] GetMethodsToWrap(INamedTypeSymbol classSymbol)
    {
        var methods = new List<MethodInfo>();

        // Get all public instance methods that return the class type
        var allMembers = classSymbol.GetMembers();
        
        foreach (var member in allMembers)
        {
            if (member is IMethodSymbol methodSymbol && 
                methodSymbol.MethodKind == MethodKind.Ordinary &&
                methodSymbol.DeclaredAccessibility == Accessibility.Public &&
                !methodSymbol.IsStatic)
            {
                // Skip methods marked with [Obsolete]
                if (IsObsolete(methodSymbol))
                    continue;

                // Check if return type matches the class type (not nullable, not derived types)
                // Only exact match and not optional/nullable
                if (SymbolEqualityComparer.Default.Equals(methodSymbol.ReturnType, classSymbol) &&
                    methodSymbol.ReturnType.NullableAnnotation != NullableAnnotation.Annotated)
                {
                    var methodInfo = CreateMethodInfo(methodSymbol);
                    if (methodInfo != null)
                    {
                        methods.Add(methodInfo);
                    }
                }
            }
        }

        return [.. methods];
    }

    private static bool IsObsolete(IMethodSymbol methodSymbol)
    {
        return methodSymbol.GetAttributes().Any(attr =>
            attr.AttributeClass?.ToDisplayString() == "System.ObsoleteAttribute");
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
            bool boolValue => boolValue ? "true" : "false",
            string stringValue => $"\"{stringValue}\"",
            _ => parameter.Type.TypeKind == TypeKind.Enum
                ? $"{parameter.Type.ToDisplayString(SymbolDisplayFormat.FullyQualifiedFormat)}.{value}"
                : value.ToString()
        };
    }
}
