using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Text;
using PlusUi.SourceGenerators.Builders;
using PlusUi.SourceGenerators.Models;
using PlusUi.SourceGenerators.Utilities;
using System.Linq;
using System.Text;

namespace PlusUi.SourceGenerators.Generators;

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

        // Check which attribute is present
        var hasGenericWrapper = classSymbol.GetAttributes().Any(attr =>
            attr.AttributeClass?.Name == "GenerateGenericWrapperAttribute" ||
            attr.AttributeClass?.Name == "GenerateGenericWrapper");

        var hasShadowMethods = classSymbol.GetAttributes().Any(attr =>
            attr.AttributeClass?.Name == "GenerateShadowMethodsAttribute" ||
            attr.AttributeClass?.Name == "GenerateShadowMethods");

        if (hasGenericWrapper)
        {
            // Generate the generic wrapper class (for base classes like UiElement, UiTextElement)
            var sourceText = GenericWrapperClassBuilder.GenerateGenericWrapperClass(namespaceName, className, classSymbol);
            context.AddSource($"{className}/{className}_Generic.g.cs", SourceText.From(sourceText, Encoding.UTF8));
        }
        else if (hasShadowMethods)
        {
            // Generate shadow methods for concrete classes (for Button, Entry, Label, etc.)
            var sourceText = ConcreteClassShadowBuilder.GenerateConcreteClassShadows(namespaceName, className, classSymbol);
            context.AddSource($"{className}/{className}_GenShadowMethods.g.cs", SourceText.From(sourceText, Encoding.UTF8));
        }
    }
}
