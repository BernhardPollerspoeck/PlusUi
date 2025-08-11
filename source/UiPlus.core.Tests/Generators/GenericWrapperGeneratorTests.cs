using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.Text;
using PlusUi.SourceGenerators;

namespace UiPlus.core.Tests.Generators;

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
        Assert.IsTrue(result.GeneratedSources.Length > 0, "Generator should produce output for class with attribute");
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
        Assert.AreEqual(0, result.GeneratedSources.Length, "Generator should not produce output for class without attribute");
    }

    [TestMethod]
    public void GeneratedUiElement_HasSetMarginMethod()
    {
        // Arrange
        var source = @"
using System;
using PlusUi.SourceGenerators;

namespace PlusUi.core
{
    [GenerateGenericWrapper]
    public abstract class UiElement
    {
        public UiElement SetMargin(Margin margin) => this;
    }

    public struct Margin { }
}";

        // Act
        var result = RunGenerator(source);

        // Assert
        Assert.IsTrue(result.GeneratedSources.Length > 0);
        var generatedCode = result.GeneratedSources[0].SourceText.ToString();
        Assert.IsTrue(generatedCode.Contains("public new T SetMargin"), "Generated class should have SetMargin method");
    }

    [TestMethod]
    public void GeneratedUiElement_HasSetBackgroundColorMethod()
    {
        // Arrange
        var source = @"
using System;
using PlusUi.SourceGenerators;

namespace PlusUi.core
{
    [GenerateGenericWrapper]
    public abstract class UiElement
    {
        public UiElement SetBackgroundColor(object color) => this;
    }
}";

        // Act
        var result = RunGenerator(source);

        // Assert  
        Assert.IsTrue(result.GeneratedSources.Length > 0);
        var generatedCode = result.GeneratedSources[0].SourceText.ToString();
        Assert.IsTrue(generatedCode.Contains("public new T SetBackgroundColor"), "Generated class should have SetBackgroundColor method");
    }

    [TestMethod]
    public void GeneratedUiElement_HasSetIsVisibleMethod()
    {
        // Act
        var result = RunGeneratorForUiElement();

        // Assert
        var generatedCode = result.GeneratedSources[0].SourceText.ToString();
        Assert.IsTrue(generatedCode.Contains("public new T SetIsVisible"), "Generated class should have SetIsVisible method");
    }

    [TestMethod] 
    public void GeneratedUiElement_HasSetVisualOffsetMethod()
    {
        // Act
        var result = RunGeneratorForUiElement();

        // Assert
        var generatedCode = result.GeneratedSources[0].SourceText.ToString();
        Assert.IsTrue(generatedCode.Contains("public new T SetVisualOffset"), "Generated class should have SetVisualOffset method");
    }

    [TestMethod]
    public void GeneratedUiElement_HasSetDesiredSizeMethod()
    {
        // Act
        var result = RunGeneratorForUiElement();

        // Assert
        var generatedCode = result.GeneratedSources[0].SourceText.ToString();
        Assert.IsTrue(generatedCode.Contains("public new T SetDesiredSize"), "Generated class should have SetDesiredSize method");
    }

    [TestMethod]
    public void GeneratedUiElement_HasSetDesiredWidthMethod()
    {
        // Act
        var result = RunGeneratorForUiElement();

        // Assert
        var generatedCode = result.GeneratedSources[0].SourceText.ToString();
        Assert.IsTrue(generatedCode.Contains("public new T SetDesiredWidth"), "Generated class should have SetDesiredWidth method");
    }

    [TestMethod]
    public void GeneratedUiElement_HasSetDesiredHeightMethod()
    {
        // Act
        var result = RunGeneratorForUiElement();

        // Assert
        var generatedCode = result.GeneratedSources[0].SourceText.ToString();
        Assert.IsTrue(generatedCode.Contains("public new T SetDesiredHeight"), "Generated class should have SetDesiredHeight method");
    }

    [TestMethod]
    public void GeneratedUiElement_HasSetHorizontalAlignmentMethod()
    {
        // Act
        var result = RunGeneratorForUiElement();

        // Assert
        var generatedCode = result.GeneratedSources[0].SourceText.ToString();
        Assert.IsTrue(generatedCode.Contains("public new T SetHorizontalAlignment"), "Generated class should have SetHorizontalAlignment method");
    }

    [TestMethod]
    public void GeneratedUiElement_HasSetVerticalAlignmentMethod()
    {
        // Act
        var result = RunGeneratorForUiElement();

        // Assert
        var generatedCode = result.GeneratedSources[0].SourceText.ToString();
        Assert.IsTrue(generatedCode.Contains("public new T SetVerticalAlignment"), "Generated class should have SetVerticalAlignment method");
    }

    [TestMethod]
    public void GeneratedUiElement_HasSetCornerRadiusMethod()
    {
        // Act
        var result = RunGeneratorForUiElement();

        // Assert
        var generatedCode = result.GeneratedSources[0].SourceText.ToString();
        Assert.IsTrue(generatedCode.Contains("public new T SetCornerRadius"), "Generated class should have SetCornerRadius method");
    }

    [TestMethod]
    public void GeneratedUiElement_HasAllBindMethods()
    {
        // Act
        var result = RunGeneratorForUiElement();

        // Assert
        var generatedCode = result.GeneratedSources[0].SourceText.ToString();
        Assert.IsTrue(generatedCode.Contains("public new T BindBackgroundColor"), "Generated class should have BindBackgroundColor method");
        Assert.IsTrue(generatedCode.Contains("public new T BindMargin"), "Generated class should have BindMargin method");
        Assert.IsTrue(generatedCode.Contains("public new T BindVisualOffset"), "Generated class should have BindVisualOffset method");
        Assert.IsTrue(generatedCode.Contains("public new T BindIsVisible"), "Generated class should have BindIsVisible method");
        Assert.IsTrue(generatedCode.Contains("public new T BindHorizontalAlignment"), "Generated class should have BindHorizontalAlignment method");
        Assert.IsTrue(generatedCode.Contains("public new T BindVerticalAlignment"), "Generated class should have BindVerticalAlignment method");
        Assert.IsTrue(generatedCode.Contains("public new T BindCornerRadius"), "Generated class should have BindCornerRadius method");
        Assert.IsTrue(generatedCode.Contains("public new T BindDesiredSize"), "Generated class should have BindDesiredSize method");
        Assert.IsTrue(generatedCode.Contains("public new T BindDesiredWidth"), "Generated class should have BindDesiredWidth method");
        Assert.IsTrue(generatedCode.Contains("public new T BindDesiredHeight"), "Generated class should have BindDesiredHeight method");
    }

    [TestMethod]
    public void GeneratedMethods_HaveCorrectReturnType()
    {
        // Act
        var result = RunGeneratorForUiElement();

        // Assert
        var generatedCode = result.GeneratedSources[0].SourceText.ToString();
        
        // Check that methods return T not the original class type
        Assert.IsTrue(generatedCode.Contains("public new T SetMargin"), "Methods should return T");
        Assert.IsTrue(generatedCode.Contains("return (T)this;"), "Methods should cast and return T");
    }

    [TestMethod]
    public void GeneratedMethods_CallBaseMethod()
    {
        // Act
        var result = RunGeneratorForUiElement();

        // Assert
        var generatedCode = result.GeneratedSources[0].SourceText.ToString();
        
        // Check that methods call base implementation
        Assert.IsTrue(generatedCode.Contains("base.SetMargin(margin);"), "Methods should call base implementation");
        Assert.IsTrue(generatedCode.Contains("base.SetBackgroundColor(color);"), "Methods should call base implementation");
    }

    private GeneratorDriverRunResult RunGeneratorForUiElement()
    {
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

        return RunGenerator(source);
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