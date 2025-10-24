using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace PlusUi.SourceGenerators;

internal static class NamespaceExtractor
{
    internal static string GetNamespace(ClassDeclarationSyntax classDeclarationSyntax)
    {
        // Find the namespace
        var namespaceSyntax = classDeclarationSyntax.FirstAncestorOrSelf<BaseNamespaceDeclarationSyntax>();
        return namespaceSyntax?.Name.ToString() ?? "";
    }
}
