using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.Text;
using PlusUi.SourceGenerators;

namespace UiPlus.core.Tests.Generators
{
    [TestClass]
    public class GenericWrapperGeneratorTests
    {
        [TestMethod]
        public void Generator_RecognizesClassWithAttribute()
        {
            // Arrange
            var source = @"
using System;
using PlusUi.SourceGenerators;

namespace TestNamespace
{
    [GenerateGenericWrapper]
    public abstract class UiElement
    {
        public UiElement SetMargin(int margin) => this;
    }
}";

            // Act
            var result = RunGenerator(source);

            // Assert
            Assert.IsTrue(result.Results.Length > 0, "Generator should process input");
            Assert.IsTrue(result.Results[0].GeneratedSources.Length > 0, "Generator should produce output for class with attribute");
        }

        [TestMethod]
        public void Generator_IgnoresClassWithoutAttribute()
        {
            // Arrange
            var source = @"
namespace TestNamespace
{
    public abstract class SomeElement
    {
        public SomeElement SetValue(int value) => this;
    }
}";

            // Act
            var result = RunGenerator(source);

            // Assert
            Assert.AreEqual(0, result.Results[0].GeneratedSources.Length, "Generator should not produce output for class without attribute");
        }

        [TestMethod]
        public void GeneratedUiElement_ContainsAllRequiredMethods()
        {
            // Arrange - Use UiElement to trigger generation
            var source = @"
using System;
using PlusUi.SourceGenerators;

namespace PlusUi.core
{
    [GenerateGenericWrapper]
    public abstract class UiElement
    {
        public UiElement SetMargin(object margin) => this;
        public UiElement SetBackgroundColor(object color) => this;
    }
}";

            // Act
            var result = RunGenerator(source);

            // Assert
            Assert.IsTrue(result.Results.Length > 0);
            Assert.IsTrue(result.Results[0].GeneratedSources.Length > 0);
            
            var generatedCode = result.Results[0].GeneratedSources[0].SourceText.ToString();
            
            // Verify all major method categories are present
            var requiredMethods = new[]
            {
                "SetBackgroundColor", "BindBackgroundColor",
                "SetMargin", "BindMargin", 
                "SetVisualOffset", "BindVisualOffset",
                "SetIsVisible", "BindIsVisible",
                "SetHorizontalAlignment", "BindHorizontalAlignment",
                "SetVerticalAlignment", "BindVerticalAlignment", 
                "SetCornerRadius", "BindCornerRadius",
                "SetDesiredSize", "BindDesiredSize",
                "SetDesiredWidth", "SetDesiredHeight",
                "BindDesiredWidth", "BindDesiredHeight",
                "IgnoreStyling", "SetDebug"
            };

            foreach (var method in requiredMethods)
            {
                Assert.IsTrue(generatedCode.Contains($"public new T {method}"), 
                    $"Generated class should have {method} method");
            }
        }

        [TestMethod]
        public void GeneratedMethods_HaveCorrectStructure()
        {
            // Arrange
            var source = CreateUiElementSource();

            // Act
            var result = RunGenerator(source);

            // Assert
            var generatedCode = result.Results[0].GeneratedSources[0].SourceText.ToString();
            
            // Check overall class structure
            Assert.IsTrue(generatedCode.Contains("public abstract class UiElement<T> : UiElement where T : UiElement<T>"), 
                "Generated class should have correct inheritance and constraints");

            // Check method structure
            Assert.IsTrue(generatedCode.Contains("public new T SetMargin"), "Methods should return T");
            Assert.IsTrue(generatedCode.Contains("base.SetMargin"), "Methods should call base implementation");
            Assert.IsTrue(generatedCode.Contains("return (T)this;"), "Methods should cast and return T");
        }

        [TestMethod]
        public void GeneratedCode_SupportsMethodChaining()
        {
            // This test verifies the generated code structure supports the fluent interface pattern
            // used extensively in PlusUi (as seen in MainPage.cs and other examples)
            
            var source = CreateUiElementSource();
            var result = RunGenerator(source);
            var generatedCode = result.Results[0].GeneratedSources[0].SourceText.ToString();

            // Verify that each method returns T to enable chaining
            var chainableMethods = new[]
            {
                "SetBackgroundColor", "SetMargin", "SetIsVisible", "SetVisualOffset",
                "SetDesiredSize", "SetHorizontalAlignment", "SetVerticalAlignment"
            };

            foreach (var method in chainableMethods)
            {
                var methodPattern = $"public new T {method}";
                Assert.IsTrue(generatedCode.Contains(methodPattern),
                    $"Method {method} should return T for chaining");
                
                var returnPattern = "return (T)this;";
                // Find the method and ensure it has the return statement
                var methodIndex = generatedCode.IndexOf(methodPattern);
                Assert.IsTrue(methodIndex >= 0);
                
                var nextMethodIndex = generatedCode.IndexOf("public new T", methodIndex + 1);
                var methodBody = nextMethodIndex > 0 
                    ? generatedCode.Substring(methodIndex, nextMethodIndex - methodIndex)
                    : generatedCode.Substring(methodIndex);
                    
                Assert.IsTrue(methodBody.Contains(returnPattern),
                    $"Method {method} should return (T)this for chaining");
            }
        }

        private string CreateUiElementSource()
        {
            return @"
using System;
using PlusUi.SourceGenerators;

namespace PlusUi.core
{
    [GenerateGenericWrapper]
    public abstract class UiElement
    {
        public UiElement SetMargin(object margin) => this;
        public UiElement SetBackgroundColor(object color) => this;
    }
}";
        }

        private GeneratorDriverRunResult RunGenerator(string source)
        {
            var compilation = CreateCompilation(source);
            
            var generator = new GenericWrapperGenerator();
            GeneratorDriver driver = CSharpGeneratorDriver.Create(generator);

            driver = driver.RunGeneratorsAndUpdateCompilation(compilation, out var outputCompilation, out var diagnostics);

            return driver.GetRunResult();
        }

        private static Compilation CreateCompilation(string source)
            => CSharpCompilation.Create("compilation",
                new[] { CSharpSyntaxTree.ParseText(source) },
                new[] { MetadataReference.CreateFromFile(typeof(Binder).GetType().Assembly.Location) },
                new CSharpCompilationOptions(OutputKind.ConsoleApplication));
    }
}