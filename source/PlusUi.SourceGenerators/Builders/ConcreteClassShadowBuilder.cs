using Microsoft.CodeAnalysis;
using PlusUi.SourceGenerators.Models;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace PlusUi.SourceGenerators.Builders;

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
        var existingMethodNames = new HashSet<string>(); // Just method names for UiPropGen comparison
        foreach (var member in classSymbol.GetMembers())
        {
            if (member is IMethodSymbol methodSymbol &&
                methodSymbol.MethodKind == MethodKind.Ordinary)
            {
                var signature = $"{methodSymbol.Name}({string.Join(",", methodSymbol.Parameters.Select(p => p.Type.ToDisplayString()))})";
                existingMethods.Add(signature);
                existingMethodNames.Add(methodSymbol.Name);
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

                    // Skip generic methods - they cannot be shadowed with simple wrappers
                    if (methodSymbol.IsGenericMethod)
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

        // Also collect methods from UiPropGen attributes on base classes
        var uiPropGenMethods = CollectUiPropGenMethods(classSymbol, existingMethodNames);
        foreach (var method in uiPropGenMethods)
        {
            // Track with full signature to allow overloads
            var fullSig = $"{method.Name}|{method.Parameters}";
            if (!methods.Add(fullSig))
                continue;
            result.Add(method);
        }

        return result;
    }

    private static string CreateTypeOnlySignature(string methodName, string fullParameters)
    {
        // Convert "float value, Color? color" to "float,Color?"
        if (string.IsNullOrWhiteSpace(fullParameters))
            return $"{methodName}()";

        var types = new List<string>();
        var parts = fullParameters.Split(',');
        foreach (var part in parts)
        {
            var trimmed = part.Trim();
            if (string.IsNullOrWhiteSpace(trimmed))
                continue;

            // Extract the type (everything except the last word which is the parameter name)
            var lastSpace = trimmed.LastIndexOf(' ');
            if (lastSpace > 0)
            {
                var typePart = trimmed.Substring(0, lastSpace).Trim();
                // Remove "global::" prefix if present for consistency
                typePart = typePart.Replace("global::", "");
                // For complex types like Expression<Func<T>>, simplify
                types.Add(typePart);
            }
            else
            {
                types.Add(trimmed);
            }
        }

        return $"{methodName}({string.Join(",", types)})";
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

    private const string UiPropGenAttributePrefix = "UiPropGen";
    private const string AttributeSuffix = "Attribute";
    private const string TemplateFieldName = "Template";

    private static List<MethodInfo> CollectUiPropGenMethods(INamedTypeSymbol classSymbol, HashSet<string> existingMethodNames)
    {
        var result = new List<MethodInfo>();
        var methods = new HashSet<string>(); // Track by method name only

        // Get UiPropGen attributes on the class itself to exclude them
        var ownUiPropGenAttributes = new HashSet<string>();
        foreach (var attribute in classSymbol.GetAttributes())
        {
            var attrClass = attribute.AttributeClass;
            if (attrClass?.Name.StartsWith(UiPropGenAttributePrefix) == true)
            {
                ownUiPropGenAttributes.Add(attrClass.Name);
            }
        }

        // Walk up the inheritance chain looking for UiPropGen attributes
        var currentType = classSymbol.BaseType;
        while (currentType != null && currentType.SpecialType != SpecialType.System_Object)
        {
            foreach (var attribute in currentType.GetAttributes())
            {
                var attrClass = attribute.AttributeClass;
                if (attrClass == null)
                    continue;

                var attrName = attrClass.Name;
                if (!attrName.StartsWith(UiPropGenAttributePrefix))
                    continue;

                // Skip if the class itself has this same UiPropGen attribute
                // (it will generate its own methods via UiPropGenerator)
                if (ownUiPropGenAttributes.Contains(attrName))
                    continue;

                var template = GetTemplateFromAttribute(attrClass);
                if (string.IsNullOrEmpty(template))
                    continue;

                // Parse method signatures from the template
                var templateMethods = ParseMethodsFromTemplate(template);
                foreach (var method in templateMethods)
                {
                    // Check if this exact method name exists in the class
                    // (Skip only if the class explicitly defines this method)
                    if (existingMethodNames.Contains(method.Name))
                        continue;

                    // Use full signature for tracking (to allow overloads)
                    var fullSig = $"{method.Name}|{method.Parameters}";
                    if (!methods.Add(fullSig))
                        continue;

                    result.Add(method);
                }
            }

            currentType = currentType.BaseType;
        }

        return result;
    }

    private static string GetTemplateFromAttribute(INamedTypeSymbol attributeClass)
    {
        var templateField = attributeClass
            .GetMembers(TemplateFieldName)
            .OfType<IFieldSymbol>()
            .FirstOrDefault();

        if (templateField == null || !templateField.HasConstantValue)
            return "";

        return templateField.ConstantValue as string ?? "";
    }

    private static List<MethodInfo> ParseMethodsFromTemplate(string template)
    {
        var result = new List<MethodInfo>();

        // Match public methods that return {ClassName}
        // Pattern: public {ClassName} MethodName(parameters)
        var methodPattern = @"public\s+\{ClassName\}\s+(\w+)\s*\(([^)]*)\)";
        var matches = Regex.Matches(template, methodPattern);

        foreach (Match match in matches)
        {
            var methodName = match.Groups[1].Value;
            var rawParams = match.Groups[2].Value.Trim();

            // Skip Bind methods as they use Expression<Func<T>> which is complex
            // Actually we should include them, let's convert the parameter
            var parameters = ConvertTemplateParameters(rawParams);
            var arguments = ExtractArgumentNames(rawParams);

            result.Add(new MethodInfo(methodName, parameters, arguments));
        }

        return result;
    }

    private static string ConvertTemplateParameters(string rawParams)
    {
        if (string.IsNullOrWhiteSpace(rawParams))
            return "";

        // Convert simple types to fully qualified names for consistency
        // E.g., "float value" -> "float value"
        // E.g., "Expression<Func<float>> propertyExpression" -> full type
        var parts = rawParams.Split(',').Select(p => p.Trim());
        var converted = new List<string>();

        foreach (var part in parts)
        {
            if (string.IsNullOrWhiteSpace(part))
                continue;

            // Handle Expression<Func<T>> pattern
            if (part.Contains("Expression<Func<"))
            {
                // Extract the inner type and convert to fully qualified
                var exprMatch = Regex.Match(part, @"Expression<Func<([^>]+)>>\s+(\w+)");
                if (exprMatch.Success)
                {
                    var innerType = exprMatch.Groups[1].Value;
                    var paramName = exprMatch.Groups[2].Value;
                    converted.Add($"System.Linq.Expressions.Expression<System.Func<{ConvertSimpleType(innerType)}>> {paramName}");
                }
                else
                {
                    converted.Add(part);
                }
            }
            else
            {
                // Simple parameter like "float value" or "Color? value"
                var simpleMatch = Regex.Match(part, @"^(\S+)\s+(\w+)$");
                if (simpleMatch.Success)
                {
                    var typeName = simpleMatch.Groups[1].Value;
                    var paramName = simpleMatch.Groups[2].Value;
                    converted.Add($"{ConvertSimpleType(typeName)} {paramName}");
                }
                else
                {
                    converted.Add(part);
                }
            }
        }

        return string.Join(", ", converted);
    }

    private static string ConvertSimpleType(string typeName)
    {
        // Convert common type names to fully qualified where needed
        // Most types used in templates are simple or from known namespaces
        return typeName switch
        {
            "float" => "float",
            "float?" => "float?",
            "int" => "int",
            "int?" => "int?",
            "bool" => "bool",
            "bool?" => "bool?",
            "string" => "string",
            "string?" => "string?",
            "double" => "double",
            "double?" => "double?",
            _ => typeName // Keep as-is for complex types like Color, Margin, etc.
        };
    }

    private static string ExtractArgumentNames(string rawParams)
    {
        if (string.IsNullOrWhiteSpace(rawParams))
            return "";

        var parts = rawParams.Split(',').Select(p => p.Trim());
        var names = new List<string>();

        foreach (var part in parts)
        {
            if (string.IsNullOrWhiteSpace(part))
                continue;

            // Extract the last word as the parameter name
            var match = Regex.Match(part, @"(\w+)\s*$");
            if (match.Success)
            {
                names.Add(match.Groups[1].Value);
            }
        }

        return string.Join(", ", names);
    }
}
