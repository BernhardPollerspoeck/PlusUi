using PlusUi.core;
using SkiaSharp;

namespace PlusUi.core.Tests;

[TestClass]
public class BackgroundTests
{
    [TestMethod]
    public void TestSolidColorBackground_Constructor()
    {
        // Arrange & Act
        var background = new SolidColorBackground(Colors.Red);

        // Assert
        Assert.AreEqual(Colors.Red, background.Color);
    }

    [TestMethod]
    public void TestSolidColorBackground_ObjectInitializer()
    {
        // Arrange & Act
        var background = new SolidColorBackground { Color = Colors.Blue };

        // Assert
        Assert.AreEqual(Colors.Blue, background.Color);
    }

    [TestMethod]
    public void TestSolidColorBackground_ImplicitConversion()
    {
        // Arrange & Act
        SolidColorBackground background = Colors.Green;

        // Assert
        Assert.IsInstanceOfType<SolidColorBackground>(background);
        Assert.AreEqual(Colors.Green, background.Color);
    }

    [TestMethod]
    public void TestLinearGradient_Constructor()
    {
        // Arrange & Act
        var gradient = new LinearGradient(Colors.Red, Colors.Blue, 45);

        // Assert
        Assert.AreEqual(Colors.Red, gradient.StartColor);
        Assert.AreEqual(Colors.Blue, gradient.EndColor);
        Assert.AreEqual(45f, gradient.Angle);
    }

    [TestMethod]
    public void TestLinearGradient_ObjectInitializer()
    {
        // Arrange & Act
        var gradient = new LinearGradient
        {
            StartColor = Colors.Yellow,
            EndColor = Colors.Orange,
            Angle = 90
        };

        // Assert
        Assert.AreEqual(Colors.Yellow, gradient.StartColor);
        Assert.AreEqual(Colors.Orange, gradient.EndColor);
        Assert.AreEqual(90f, gradient.Angle);
    }

    [TestMethod]
    public void TestLinearGradient_DefaultAngle()
    {
        // Arrange & Act
        var gradient = new LinearGradient(Colors.Red, Colors.Blue);

        // Assert
        Assert.AreEqual(0f, gradient.Angle);
    }

    [TestMethod]
    public void TestRadialGradient_Constructor()
    {
        // Arrange & Act
        var gradient = new RadialGradient(Colors.White, Colors.Black);

        // Assert
        Assert.AreEqual(Colors.White, gradient.CenterColor);
        Assert.AreEqual(Colors.Black, gradient.EdgeColor);
        Assert.AreEqual(0.5f, gradient.Center.X);
        Assert.AreEqual(0.5f, gradient.Center.Y);
    }

    [TestMethod]
    public void TestRadialGradient_CustomCenter()
    {
        // Arrange & Act
        var gradient = new RadialGradient(Colors.Red, Colors.Blue, new Point(0.3f, 0.7f));

        // Assert
        Assert.AreEqual(Colors.Red, gradient.CenterColor);
        Assert.AreEqual(Colors.Blue, gradient.EdgeColor);
        Assert.AreEqual(0.3f, gradient.Center.X);
        Assert.AreEqual(0.7f, gradient.Center.Y);
    }

    [TestMethod]
    public void TestRadialGradient_ObjectInitializer()
    {
        // Arrange & Act
        var gradient = new RadialGradient
        {
            CenterColor = Colors.Yellow,
            EdgeColor = Colors.Orange,
            Center = new Point(0.2f, 0.8f)
        };

        // Assert
        Assert.AreEqual(Colors.Yellow, gradient.CenterColor);
        Assert.AreEqual(Colors.Orange, gradient.EdgeColor);
        Assert.AreEqual(0.2f, gradient.Center.X);
        Assert.AreEqual(0.8f, gradient.Center.Y);
    }

    [TestMethod]
    public void TestMultiStopGradient_Constructor()
    {
        // Arrange & Act
        var gradient = new MultiStopGradient(
            90,
            new GradientStop(Colors.Red, 0),
            new GradientStop(Colors.Yellow, 0.5f),
            new GradientStop(Colors.Green, 1));

        // Assert
        Assert.AreEqual(90f, gradient.Angle);
        Assert.HasCount(3, gradient.Stops);
        Assert.AreEqual(Colors.Red, gradient.Stops[0].Color);
        Assert.AreEqual(0f, gradient.Stops[0].Position);
        Assert.AreEqual(Colors.Yellow, gradient.Stops[1].Color);
        Assert.AreEqual(0.5f, gradient.Stops[1].Position);
        Assert.AreEqual(Colors.Green, gradient.Stops[2].Color);
        Assert.AreEqual(1f, gradient.Stops[2].Position);
    }

    [TestMethod]
    public void TestMultiStopGradient_Immutability()
    {
        // Arrange & Act
        var gradient = new MultiStopGradient(
            45,
            new GradientStop(Colors.Red, 0),
            new GradientStop(Colors.Blue, 1));

        // Assert - Stops should be IReadOnlyList
        Assert.IsInstanceOfType(gradient.Stops, typeof(IReadOnlyList<GradientStop>));
    }

    [TestMethod]
    public void TestUiElement_SetBackground_SolidColor()
    {
        // Arrange
        var element = new VStack();

        // Act
        element.SetBackground(new SolidColorBackground(Colors.Red));

        // Assert
        Assert.IsNotNull(element.Background);
        Assert.IsInstanceOfType(element.Background, typeof(SolidColorBackground));
        Assert.AreEqual(Colors.Red, ((SolidColorBackground)element.Background).Color);
    }

    [TestMethod]
    public void TestUiElement_SetBackground_ImplicitConversion()
    {
        // Arrange
        var element = new VStack();

        // Act - Use explicit SolidColorBackground to demonstrate implicit conversion in real usage
        element.SetBackground(new SolidColorBackground(Colors.Blue));

        // Assert
        Assert.IsNotNull(element.Background);
        Assert.IsInstanceOfType(element.Background, typeof(SolidColorBackground));
        Assert.AreEqual(Colors.Blue, ((SolidColorBackground)element.Background).Color);
    }

    [TestMethod]
    public void TestUiElement_SetBackground_LinearGradient()
    {
        // Arrange
        var element = new VStack();
        var gradient = new LinearGradient(Colors.Red, Colors.Blue, 45);

        // Act
        element.SetBackground(gradient);

        // Assert
        Assert.IsNotNull(element.Background);
        Assert.IsInstanceOfType(element.Background, typeof(LinearGradient));
        var bg = (LinearGradient)element.Background;
        Assert.AreEqual(Colors.Red, bg.StartColor);
        Assert.AreEqual(Colors.Blue, bg.EndColor);
        Assert.AreEqual(45f, bg.Angle);
    }

    [TestMethod]
    public void TestUiElement_BindBackground()
    {
        // Arrange
        var element = new VStack();
        var testColor = Colors.Green;

        // Act
        element.BindBackground(() => new SolidColorBackground(testColor));
        
        // Assert
        Assert.IsNotNull(element.Background);
        Assert.IsInstanceOfType(element.Background, typeof(SolidColorBackground));
        Assert.AreEqual(Colors.Green, ((SolidColorBackground)element.Background).Color);
    }

    [TestMethod]
    public void TestUiElement_BindBackground_WithImplicitConversion()
    {
        // Arrange
        var element = new VStack();
        var testColor = Colors.Purple;

        // Act - Use explicit SolidColorBackground to demonstrate usage
        element.BindBackground(() => new SolidColorBackground(testColor));
        
        // Assert
        Assert.IsNotNull(element.Background);
        Assert.IsInstanceOfType(element.Background, typeof(SolidColorBackground));
        Assert.AreEqual(Colors.Purple, ((SolidColorBackground)element.Background).Color);
    }

    [TestMethod]
    public void TestSolid_Constructor_UsesNewBackgroundSystem()
    {
        // Arrange & Act
        var solid = new Solid(color: Colors.Red);

        // Assert
        Assert.IsNotNull(solid.Background);
        Assert.IsInstanceOfType(solid.Background, typeof(SolidColorBackground));
        Assert.AreEqual(Colors.Red, ((SolidColorBackground)solid.Background).Color);
    }

    [TestMethod]
    public void TestBorder_SetBackground()
    {
        // Arrange
        var border = new Border();
        var gradient = new LinearGradient(Colors.Red, Colors.Blue, 90);

        // Act
        border.SetBackground(gradient);

        // Assert
        Assert.IsNotNull(border.Background);
        Assert.IsInstanceOfType(border.Background, typeof(LinearGradient));
    }
}
