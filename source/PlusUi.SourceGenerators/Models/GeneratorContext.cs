using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace PlusUi.SourceGenerators.Models;

internal class GeneratorContext(ClassDeclarationSyntax classDeclaration, SemanticModel semanticModel)
{
    public ClassDeclarationSyntax ClassDeclaration { get; } = classDeclaration;
    public SemanticModel SemanticModel { get; } = semanticModel;
}
