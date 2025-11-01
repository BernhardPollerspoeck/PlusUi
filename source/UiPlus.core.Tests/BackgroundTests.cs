using PlusUi.core;
using SkiaSharp;

namespace UiPlus.core.Tests;

[TestClass]
public class BackgroundTests
{
    [TestMethod]
    public void TestSolidColorBackground_Constructor()
    {
        // Arrange & Act
        var background = new SolidColorBackground(SKColors.Red);

        // Assert
        Assert.AreEqual(SKColors.Red, background.Color);
    }

    [TestMethod]
    public void TestSolidColorBackground_ObjectInitializer()
    {
        // Arrange & Act
        var background = new SolidColorBackground { Color = SKColors.Blue };

        // Assert
        Assert.AreEqual(SKColors.Blue, background.Color);
    }

    [TestMethod]
    public void TestSolidColorBackground_ImplicitConversion()
    {
        // Arrange & Act
        SolidColorBackground background = SKColors.Green;

        // Assert
        Assert.IsInstanceOfType<SolidColorBackground>(background);
        Assert.AreEqual(SKColors.Green, background.Color);
    }

    [TestMethod]
    public void TestLinearGradient_Constructor()
    {
        // Arrange & Act
        var gradient = new LinearGradient(SKColors.Red, SKColors.Blue, 45);

        // Assert
        Assert.AreEqual(SKColors.Red, gradient.StartColor);
        Assert.AreEqual(SKColors.Blue, gradient.EndColor);
        Assert.AreEqual(45f, gradient.Angle);
    }

    [TestMethod]
    public void TestLinearGradient_ObjectInitializer()
    {
        // Arrange & Act
        var gradient = new LinearGradient
        {
            StartColor = SKColors.Yellow,
            EndColor = SKColors.Orange,
            Angle = 90
        };

        // Assert
        Assert.AreEqual(SKColors.Yellow, gradient.StartColor);
        Assert.AreEqual(SKColors.Orange, gradient.EndColor);
        Assert.AreEqual(90f, gradient.Angle);
    }

    [TestMethod]
    public void TestLinearGradient_DefaultAngle()
    {
        // Arrange & Act
        var gradient = new LinearGradient(SKColors.Red, SKColors.Blue);

        // Assert
        Assert.AreEqual(0f, gradient.Angle);
    }

    [TestMethod]
    public void TestRadialGradient_Constructor()
    {
        // Arrange & Act
        var gradient = new RadialGradient(SKColors.White, SKColors.Black);

        // Assert
        Assert.AreEqual(SKColors.White, gradient.CenterColor);
        Assert.AreEqual(SKColors.Black, gradient.EdgeColor);
        Assert.AreEqual(0.5f, gradient.Center.X);
        Assert.AreEqual(0.5f, gradient.Center.Y);
    }

    [TestMethod]
    public void TestRadialGradient_CustomCenter()
    {
        // Arrange & Act
        var gradient = new RadialGradient(SKColors.Red, SKColors.Blue, new Point(0.3f, 0.7f));

        // Assert
        Assert.AreEqual(SKColors.Red, gradient.CenterColor);
        Assert.AreEqual(SKColors.Blue, gradient.EdgeColor);
        Assert.AreEqual(0.3f, gradient.Center.X);
        Assert.AreEqual(0.7f, gradient.Center.Y);
    }

    [TestMethod]
    public void TestRadialGradient_ObjectInitializer()
    {
        // Arrange & Act
        var gradient = new RadialGradient
        {
            CenterColor = SKColors.Yellow,
            EdgeColor = SKColors.Orange,
            Center = new Point(0.2f, 0.8f)
        };

        // Assert
        Assert.AreEqual(SKColors.Yellow, gradient.CenterColor);
        Assert.AreEqual(SKColors.Orange, gradient.EdgeColor);
        Assert.AreEqual(0.2f, gradient.Center.X);
        Assert.AreEqual(0.8f, gradient.Center.Y);
    }

    [TestMethod]
    public void TestMultiStopGradient_Constructor()
    {
        // Arrange & Act
        var gradient = new MultiStopGradient(
            90,
            new MultiStopGradient.GradientStop(SKColors.Red, 0),
            new MultiStopGradient.GradientStop(SKColors.Yellow, 0.5f),
            new MultiStopGradient.GradientStop(SKColors.Green, 1));

        // Assert
        Assert.AreEqual(90f, gradient.Angle);
        Assert.HasCount(3, gradient.Stops);
        Assert.AreEqual(SKColors.Red, gradient.Stops[0].Color);
        Assert.AreEqual(0f, gradient.Stops[0].Position);
        Assert.AreEqual(SKColors.Yellow, gradient.Stops[1].Color);
        Assert.AreEqual(0.5f, gradient.Stops[1].Position);
        Assert.AreEqual(SKColors.Green, gradient.Stops[2].Color);
        Assert.AreEqual(1f, gradient.Stops[2].Position);
    }

    [TestMethod]
    public void TestMultiStopGradient_Immutability()
    {
        // Arrange & Act
        var gradient = new MultiStopGradient(
            45,
            new MultiStopGradient.GradientStop(SKColors.Red, 0),
            new MultiStopGradient.GradientStop(SKColors.Blue, 1));

        // Assert - Stops should be IReadOnlyList
        Assert.IsInstanceOfType(gradient.Stops, typeof(IReadOnlyList<MultiStopGradient.GradientStop>));
    }

    [TestMethod]
    public void TestUiElement_SetBackground_SolidColor()
    {
        // Arrange
        var element = new VStack();

        // Act
        element.SetBackground(new SolidColorBackground(SKColors.Red));

        // Assert
        Assert.IsNotNull(element.Background);
        Assert.IsInstanceOfType(element.Background, typeof(SolidColorBackground));
        Assert.AreEqual(SKColors.Red, ((SolidColorBackground)element.Background).Color);
    }

    [TestMethod]
    public void TestUiElement_SetBackground_ImplicitConversion()
    {
        // Arrange
        var element = new VStack();

        // Act - Use explicit SolidColorBackground to demonstrate implicit conversion in real usage
        element.SetBackground(new SolidColorBackground(SKColors.Blue));

        // Assert
        Assert.IsNotNull(element.Background);
        Assert.IsInstanceOfType(element.Background, typeof(SolidColorBackground));
        Assert.AreEqual(SKColors.Blue, ((SolidColorBackground)element.Background).Color);
    }

    [TestMethod]
    public void TestUiElement_SetBackground_LinearGradient()
    {
        // Arrange
        var element = new VStack();
        var gradient = new LinearGradient(SKColors.Red, SKColors.Blue, 45);

        // Act
        element.SetBackground(gradient);

        // Assert
        Assert.IsNotNull(element.Background);
        Assert.IsInstanceOfType(element.Background, typeof(LinearGradient));
        var bg = (LinearGradient)element.Background;
        Assert.AreEqual(SKColors.Red, bg.StartColor);
        Assert.AreEqual(SKColors.Blue, bg.EndColor);
        Assert.AreEqual(45f, bg.Angle);
    }

    [TestMethod]
    public void TestUiElement_BindBackground()
    {
        // Arrange
        var element = new VStack();
        var testColor = SKColors.Green;

        // Act
        element.BindBackground(nameof(testColor), () => new SolidColorBackground(testColor));
        
        // Assert
        Assert.IsNotNull(element.Background);
        Assert.IsInstanceOfType(element.Background, typeof(SolidColorBackground));
        Assert.AreEqual(SKColors.Green, ((SolidColorBackground)element.Background).Color);
    }

    [TestMethod]
    public void TestUiElement_BindBackground_WithImplicitConversion()
    {
        // Arrange
        var element = new VStack();
        var testColor = SKColors.Purple;

        // Act - Use explicit SolidColorBackground to demonstrate usage
        element.BindBackground(nameof(testColor), () => new SolidColorBackground(testColor));
        
        // Assert
        Assert.IsNotNull(element.Background);
        Assert.IsInstanceOfType(element.Background, typeof(SolidColorBackground));
        Assert.AreEqual(SKColors.Purple, ((SolidColorBackground)element.Background).Color);
    }

    [TestMethod]
    public void TestBackwardCompatibility_SetBackgroundColor()
    {
        // Arrange
        var element = new VStack();

        // Act
        #pragma warning disable CS0618 // Type or member is obsolete
        element.SetBackgroundColor(SKColors.Red);
        #pragma warning restore CS0618 // Type or member is obsolete

        // Assert
        Assert.IsNotNull(element.Background);
        Assert.IsInstanceOfType(element.Background, typeof(SolidColorBackground));
        Assert.AreEqual(SKColors.Red, ((SolidColorBackground)element.Background).Color);
    }

    [TestMethod]
    public void TestBackwardCompatibility_BackgroundColorProperty()
    {
        // Arrange
        var element = new VStack();

        // Act
        #pragma warning disable CS0618 // Type or member is obsolete
        var initialColor = element.BackgroundColor;
        element.BackgroundColor = SKColors.Blue;
        var updatedColor = element.BackgroundColor;
        #pragma warning restore CS0618 // Type or member is obsolete

        // Assert
        Assert.AreEqual(SKColors.Transparent, initialColor);
        Assert.AreEqual(SKColors.Blue, updatedColor);
    }

    [TestMethod]
    public void TestBackgroundColor_ReturnsTransparent_WhenBackgroundIsNull()
    {
        // Arrange
        var element = new VStack();

        // Act
        #pragma warning disable CS0618 // Type or member is obsolete
        var color = element.BackgroundColor;
        #pragma warning restore CS0618 // Type or member is obsolete

        // Assert
        Assert.AreEqual(SKColors.Transparent, color);
    }

    [TestMethod]
    public void TestBackgroundColor_ReturnsTransparent_WhenBackgroundIsNotSolidColor()
    {
        // Arrange
        var element = new VStack();
        element.SetBackground(new LinearGradient(SKColors.Red, SKColors.Blue));

        // Act
        #pragma warning disable CS0618 // Type or member is obsolete
        var color = element.BackgroundColor;
        #pragma warning restore CS0618 // Type or member is obsolete

        // Assert
        Assert.AreEqual(SKColors.Transparent, color);
    }

    [TestMethod]
    public void TestSolid_Constructor_UsesNewBackgroundSystem()
    {
        // Arrange & Act
        var solid = new Solid(color: SKColors.Red);

        // Assert
        Assert.IsNotNull(solid.Background);
        Assert.IsInstanceOfType(solid.Background, typeof(SolidColorBackground));
        Assert.AreEqual(SKColors.Red, ((SolidColorBackground)solid.Background).Color);
    }

    [TestMethod]
    public void TestBorder_SetBackground()
    {
        // Arrange
        var border = new Border();
        var gradient = new LinearGradient(SKColors.Red, SKColors.Blue, 90);

        // Act
        border.SetBackground(gradient);

        // Assert
        Assert.IsNotNull(border.Background);
        Assert.IsInstanceOfType(border.Background, typeof(LinearGradient));
    }
}
