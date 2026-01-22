#nullable enable
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using System.Collections.Generic;
using System.Linq;

namespace PlusUi.SourceGenerators.UiPropGen;

internal static class UiPropSemanticAnalyzer
{
    private const string UiPropGenAttributePrefix = "UiPropGen";
    private const string AttributeSuffix = "Attribute";
    private const string TemplateFieldName = "Template";

    internal static UiPropGenContext? GetSemanticTargetForGeneration(GeneratorSyntaxContext context)
    {
        var classDeclaration = (ClassDeclarationSyntax)context.Node;
        var semanticModel = context.SemanticModel;

        var classSymbol = semanticModel.GetDeclaredSymbol(classDeclaration) as INamedTypeSymbol;
        if (classSymbol == null)
            return null;

        var properties = new List<PropertyTemplate>();

        foreach (var attribute in classSymbol.GetAttributes())
        {
            var attrClass = attribute.AttributeClass;
            if (attrClass == null)
                continue;

            var attrName = attrClass.Name;
            if (!attrName.StartsWith(UiPropGenAttributePrefix))
                continue;

            var template = GetTemplateFromAttribute(attrClass);
            if (string.IsNullOrEmpty(template))
                continue;

            var propertyName = ExtractPropertyName(attrName);
            properties.Add(new PropertyTemplate
            {
                PropertyName = propertyName,
                Template = template
            });
        }

        if (properties.Count == 0)
            return null;

        var namespaceName = classSymbol.ContainingNamespace.IsGlobalNamespace
            ? ""
            : classSymbol.ContainingNamespace.ToDisplayString();

        return new UiPropGenContext
        {
            Namespace = namespaceName,
            ClassName = classSymbol.Name,
            Properties = properties
        };
    }

    private static string ExtractPropertyName(string attributeName)
    {
        var name = attributeName;
        if (name.StartsWith(UiPropGenAttributePrefix))
            name = name.Substring(UiPropGenAttributePrefix.Length);
        if (name.EndsWith(AttributeSuffix))
            name = name.Substring(0, name.Length - AttributeSuffix.Length);
        return name;
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
}
