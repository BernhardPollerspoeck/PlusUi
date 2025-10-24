using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace PlusUi.SourceGenerators;

internal class GeneratorContext(ClassDeclarationSyntax classDeclaration, SemanticModel semanticModel)
{
    public ClassDeclarationSyntax ClassDeclaration { get; } = classDeclaration;
    public SemanticModel SemanticModel { get; } = semanticModel;
}
