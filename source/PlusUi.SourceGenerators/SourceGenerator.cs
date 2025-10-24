using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Text;
using System.Text;

namespace PlusUi.SourceGenerators;

internal static class SourceGenerator
{
    internal static void Execute(GeneratorContext generatorContext, SourceProductionContext context)
    {
        if (generatorContext is null)
            return;

        var classDeclarationSyntax = generatorContext.ClassDeclaration;
        var semanticModel = generatorContext.SemanticModel;

        var className = classDeclarationSyntax.Identifier.ValueText;
        var namespaceName = NamespaceExtractor.GetNamespace(classDeclarationSyntax);

        // Get the class symbol
        var classSymbol = semanticModel.GetDeclaredSymbol(classDeclarationSyntax) as INamedTypeSymbol;
        if (classSymbol == null)
            return;

        // Generate the generic wrapper class
        var sourceText = GenericWrapperClassBuilder.GenerateGenericWrapperClass(namespaceName, className, classSymbol);
        context.AddSource($"{className}Generic.g.cs", SourceText.From(sourceText, Encoding.UTF8));
    }
}
