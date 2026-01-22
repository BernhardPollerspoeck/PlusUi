using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace PlusUi.SourceGenerators.UiPropGen;

internal static class UiPropSyntaxAnalyzer
{
    internal static bool IsSyntaxTargetForGeneration(SyntaxNode node)
    {
        // We're looking for classes with attributes that start with "UiPropGen"
        if (node is not ClassDeclarationSyntax classDeclaration)
            return false;

        if (classDeclaration.AttributeLists.Count == 0)
            return false;

        // Quick check: does any attribute name contain "UiPropGen"?
        foreach (var attributeList in classDeclaration.AttributeLists)
        {
            foreach (var attribute in attributeList.Attributes)
            {
                var name = attribute.Name.ToString();
                if (name.Contains("UiPropGen"))
                    return true;
            }
        }

        return false;
    }
}
