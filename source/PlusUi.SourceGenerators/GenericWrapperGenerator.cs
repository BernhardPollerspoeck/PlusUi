using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Text;
using System.Collections.Immutable;
using System.Linq;
using System.Text;

namespace PlusUi.SourceGenerators
{
    [Generator]
    public class GenericWrapperGenerator : IIncrementalGenerator
    {
        public void Initialize(IncrementalGeneratorInitializationContext context)
        {
            // Find classes with GenerateGenericWrapper attribute
            var classDeclarations = context.SyntaxProvider
                .CreateSyntaxProvider(
                    predicate: static (s, _) => IsSyntaxTargetForGeneration(s),
                    transform: static (ctx, _) => GetSemanticTargetForGeneration(ctx))
                .Where(static m => m is not null);

            // Generate source for each class
            context.RegisterSourceOutput(classDeclarations,
                static (spc, source) => Execute(source, spc));
        }

        static bool IsSyntaxTargetForGeneration(SyntaxNode node)
            => node is ClassDeclarationSyntax { AttributeLists.Count: > 0 };

        static ClassDeclarationSyntax GetSemanticTargetForGeneration(GeneratorSyntaxContext context)
        {
            var classDeclarationSyntax = (ClassDeclarationSyntax)context.Node;

            // Check if class has the GenerateGenericWrapper attribute
            foreach (var attributeListSyntax in classDeclarationSyntax.AttributeLists)
            {
                foreach (var attributeSyntax in attributeListSyntax.Attributes)
                {
                    var attributeName = attributeSyntax.Name.ToString();
                    if (attributeName == "GenerateGenericWrapper" || 
                        attributeName == "GenerateGenericWrapperAttribute" ||
                        attributeName.EndsWith(".GenerateGenericWrapper") ||
                        attributeName.EndsWith(".GenerateGenericWrapperAttribute"))
                    {
                        return classDeclarationSyntax;
                    }

                    // Also try semantic analysis if available
                    try
                    {
                        var symbolInfo = context.SemanticModel.GetSymbolInfo(attributeSyntax);
                        if (symbolInfo.Symbol is IMethodSymbol attributeSymbol)
                        {
                            var attributeContainingTypeSymbol = attributeSymbol.ContainingType;
                            var fullName = attributeContainingTypeSymbol.ToDisplayString();

                            if (fullName == "PlusUi.SourceGenerators.GenerateGenericWrapperAttribute")
                            {
                                return classDeclarationSyntax;
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

        static void Execute(ClassDeclarationSyntax classDeclarationSyntax, SourceProductionContext context)
        {
            if (classDeclarationSyntax is null)
                return;

            var className = classDeclarationSyntax.Identifier.ValueText;
            var namespaceName = GetNamespace(classDeclarationSyntax);

            // For now, we'll focus on UiElement specifically
            if (className != "UiElement")
                return;

            // Generate the generic wrapper class
            var sourceText = GenerateGenericWrapperClass(namespaceName, className);
            context.AddSource($"{className}Generic.g.cs", SourceText.From(sourceText, Encoding.UTF8));
        }

        static string GetNamespace(ClassDeclarationSyntax classDeclarationSyntax)
        {
            // Find the namespace
            var namespaceSyntax = classDeclarationSyntax.FirstAncestorOrSelf<BaseNamespaceDeclarationSyntax>();
            return namespaceSyntax?.Name.ToString() ?? "";
        }

        static string GenerateGenericWrapperClass(string namespaceName, string className)
        {
            var sb = new StringBuilder();

            if (!string.IsNullOrEmpty(namespaceName))
            {
                sb.AppendLine($"namespace {namespaceName};");
                sb.AppendLine();
            }

            sb.AppendLine($"// Auto-generated generic wrapper for {className}");
            sb.AppendLine($"public abstract class {className}<T> : {className} where T : {className}<T>");
            sb.AppendLine("{");

            // Generate all the wrapper methods for UiElement
            var methods = GetUiElementMethods();
            foreach (var method in methods)
            {
                sb.AppendLine($"    public new T {method.Name}({method.Parameters})");
                sb.AppendLine("    {");
                sb.AppendLine($"        base.{method.Name}({method.Arguments});");
                sb.AppendLine("        return (T)this;");
                sb.AppendLine("    }");
                sb.AppendLine();
            }

            sb.AppendLine("}");

            return sb.ToString();
        }

        static MethodInfo[] GetUiElementMethods()
        {
            // Hardcoded for now - these are all the methods in UiElement that return UiElement
            return new[]
            {
                new MethodInfo("SetBackgroundColor", "SkiaSharp.SKColor color", "color"),
                new MethodInfo("BindBackgroundColor", "string propertyName, System.Func<SkiaSharp.SKColor> propertyGetter", "propertyName, propertyGetter"),
                new MethodInfo("SetMargin", "PlusUi.core.Margin margin", "margin"),
                new MethodInfo("BindMargin", "string propertyName, System.Func<PlusUi.core.Margin> propertyGetter", "propertyName, propertyGetter"),
                new MethodInfo("SetVisualOffset", "PlusUi.core.Point offset", "offset"),
                new MethodInfo("BindVisualOffset", "string propertyName, System.Func<PlusUi.core.Point> propertyGetter", "propertyName, propertyGetter"),
                new MethodInfo("SetIsVisible", "bool isVisible", "isVisible"),
                new MethodInfo("BindIsVisible", "string propertyName, System.Func<bool> propertyGetter", "propertyName, propertyGetter"),
                new MethodInfo("SetHorizontalAlignment", "PlusUi.core.HorizontalAlignment alignment", "alignment"),
                new MethodInfo("BindHorizontalAlignment", "string propertyName, System.Func<PlusUi.core.HorizontalAlignment> propertyGetter", "propertyName, propertyGetter"),
                new MethodInfo("SetVerticalAlignment", "PlusUi.core.VerticalAlignment alignment", "alignment"),
                new MethodInfo("BindVerticalAlignment", "string propertyName, System.Func<PlusUi.core.VerticalAlignment> propertyGetter", "propertyName, propertyGetter"),
                new MethodInfo("SetCornerRadius", "float radius", "radius"),
                new MethodInfo("BindCornerRadius", "string propertyName, System.Func<float> propertyGetter", "propertyName, propertyGetter"),
                new MethodInfo("SetDesiredSize", "PlusUi.core.Size size", "size"),
                new MethodInfo("BindDesiredSize", "string propertyName, System.Func<PlusUi.core.Size> propertyGetter", "propertyName, propertyGetter"),
                new MethodInfo("SetDesiredWidth", "float width", "width"),
                new MethodInfo("SetDesiredHeight", "float height", "height"),
                new MethodInfo("BindDesiredWidth", "string propertyName, System.Func<float> propertyGetter", "propertyName, propertyGetter"),
                new MethodInfo("BindDesiredHeight", "string propertyName, System.Func<float> propertyGetter", "propertyName, propertyGetter"),
                new MethodInfo("IgnoreStyling", "", ""),
                new MethodInfo("SetDebug", "bool debug = true", "debug")
            };
        }

        class MethodInfo
        {
            public string Name { get; set; }
            public string Parameters { get; set; }
            public string Arguments { get; set; }

            public MethodInfo(string name, string parameters, string arguments)
            {
                Name = name;
                Parameters = parameters;
                Arguments = arguments;
            }
        }
    }
}