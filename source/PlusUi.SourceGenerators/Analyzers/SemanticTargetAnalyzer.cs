using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using PlusUi.SourceGenerators.Models;

namespace PlusUi.SourceGenerators.Analyzers;

internal static class SemanticTargetAnalyzer
{
    internal static GeneratorContext GetSemanticTargetForGeneration(GeneratorSyntaxContext context)
    {
        var classDeclarationSyntax = (ClassDeclarationSyntax)context.Node;

        // Check if class has GenerateGenericWrapper or GenerateShadowMethods attribute
        foreach (var attributeListSyntax in classDeclarationSyntax.AttributeLists)
        {
            foreach (var attributeSyntax in attributeListSyntax.Attributes)
            {
                var attributeName = attributeSyntax.Name.ToString();
                if (attributeName == "GenerateGenericWrapper" ||
                    attributeName == "GenerateGenericWrapperAttribute" ||
                    attributeName == "GenerateShadowMethods" ||
                    attributeName == "GenerateShadowMethodsAttribute" ||
                    attributeName.EndsWith(".GenerateGenericWrapper") ||
                    attributeName.EndsWith(".GenerateGenericWrapperAttribute") ||
                    attributeName.EndsWith(".GenerateShadowMethods") ||
                    attributeName.EndsWith(".GenerateShadowMethodsAttribute"))
                {
                    return new GeneratorContext(classDeclarationSyntax, context.SemanticModel);
                }

                // Also try semantic analysis if available
                try
                {
                    var symbolInfo = context.SemanticModel.GetSymbolInfo(attributeSyntax);
                    if (symbolInfo.Symbol is IMethodSymbol attributeSymbol)
                    {
                        var attributeContainingTypeSymbol = attributeSymbol.ContainingType;
                        var fullName = attributeContainingTypeSymbol.ToDisplayString();

                        if (fullName == "PlusUi.core.Attributes.GenerateGenericWrapperAttribute" ||
                            fullName == "PlusUi.core.Attributes.GenerateShadowMethodsAttribute")
                        {
                            return new GeneratorContext(classDeclarationSyntax, context.SemanticModel);
                        }
                    }
                }
                catch
                {
                    // Ignore semantic analysis errors
                }
            }
        }

        return null;
    }
}
