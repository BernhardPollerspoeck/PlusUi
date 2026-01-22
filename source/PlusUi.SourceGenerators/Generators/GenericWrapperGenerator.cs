using Microsoft.CodeAnalysis;
using System.Linq;

namespace PlusUi.SourceGenerators.Generators;

[Generator]
public class GenericWrapperGenerator : IIncrementalGenerator
{
    public void Initialize(IncrementalGeneratorInitializationContext context)
    {
        // Find classes with GenerateGenericWrapper attribute
        var classDeclarations = context.SyntaxProvider
            .CreateSyntaxProvider(
                predicate: static (s, _) => Analyzers.SyntaxTargetAnalyzer.IsSyntaxTargetForGeneration(s),
                transform: static (ctx, _) => Analyzers.SemanticTargetAnalyzer.GetSemanticTargetForGeneration(ctx))
            .Where(static m => m is not null);

        // Generate source for each class
        context.RegisterSourceOutput(classDeclarations,
            static (spc, source) => SourceGenerator.Execute(source, spc));
    }
}
