#nullable enable
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Text;
using System.Collections.Immutable;
using System.Linq;
using System.Text;

namespace PlusUi.SourceGenerators.UiPropGen;

[Generator]
public class UiPropGenerator : IIncrementalGenerator
{
    public void Initialize(IncrementalGeneratorInitializationContext context)
    {
        var classDeclarations = context.SyntaxProvider
            .CreateSyntaxProvider(
                predicate: static (s, _) => UiPropSyntaxAnalyzer.IsSyntaxTargetForGeneration(s),
                transform: static (ctx, _) => UiPropSemanticAnalyzer.GetSemanticTargetForGeneration(ctx))
            .Where(static m => m is not null)
            .Collect();

        context.RegisterSourceOutput(classDeclarations,
            static (spc, source) => Execute(source!, spc));
    }

    private static void Execute(ImmutableArray<UiPropGenContext?> contexts, SourceProductionContext context)
    {
        var byClass = contexts
            .Where(c => c is not null)
            .GroupBy(c => c!.FullClassName)
            .ToList();

        foreach (var classGroup in byClass)
        {
            var firstContext = classGroup.First()!;
            var properties = classGroup.SelectMany(c => c!.Properties).ToList();

            foreach (var prop in properties)
            {
                var sourceText = UiPropCodeBuilder.GeneratePropertyFile(
                    firstContext.Namespace,
                    firstContext.ClassName,
                    prop.Template);

                context.AddSource(
                    $"{firstContext.ClassName}/{firstContext.ClassName}_{prop.PropertyName}.g.cs",
                    SourceText.From(sourceText, Encoding.UTF8));
            }
        }
    }
}
