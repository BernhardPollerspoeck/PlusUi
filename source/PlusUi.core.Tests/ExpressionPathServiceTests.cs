using System.ComponentModel;
using PlusUi.core.Binding;

namespace PlusUi.core.Tests;

[TestClass]
public class ExpressionPathServiceTests
{
    private readonly ExpressionPathService _service = new();

    [TestMethod]
    public void GetPropertyPath_SimpleProperty_ReturnsPropertyName()
    {
        // Arrange
        var vm = new TestViewModel();

        // Act
        var path = _service.GetPropertyPath(() => vm.SimpleValue);

        // Assert
        CollectionAssert.AreEqual(new[] { "SimpleValue" }, path);
    }

    [TestMethod]
    public void GetPropertyPath_1LevelNested_ReturnsPath()
    {
        // Arrange
        var vm = new TestViewModel();

        // Act
        var path = _service.GetPropertyPath(() => vm.Level1.Checked);

        // Assert
        CollectionAssert.AreEqual(new[] { "Level1", "Checked" }, path);
    }

    [TestMethod]
    public void GetPropertyPath_2LevelNested_ReturnsPath()
    {
        // Arrange
        var vm = new TestViewModel();

        // Act
        var path = _service.GetPropertyPath(() => vm.Level1.Level2.DeepValue);

        // Assert
        CollectionAssert.AreEqual(new[] { "Level1", "Level2", "DeepValue" }, path);
    }

    [TestMethod]
    public void GetPropertyPath_5LevelNested_ReturnsPath()
    {
        // Arrange
        var vm = new TestViewModel();

        // Act
        var path = _service.GetPropertyPath(() => vm.Level1.Level2.Level3.Level4.Level5.UltraDeepValue);

        // Assert
        CollectionAssert.AreEqual(new[] { "Level1", "Level2", "Level3", "Level4", "Level5", "UltraDeepValue" }, path);
    }

    private class TestViewModel
    {
        public bool SimpleValue { get; set; }
        public Level1 Level1 { get; set; } = new();
    }

    private class Level1
    {
        public bool Checked { get; set; }
        public Level2 Level2 { get; set; } = new();
    }

    private class Level2
    {
        public bool DeepValue { get; set; }
        public Level3 Level3 { get; set; } = new();
    }

    private class Level3
    {
        public Level4 Level4 { get; set; } = new();
    }

    private class Level4
    {
        public Level5 Level5 { get; set; } = new();
    }

    private class Level5
    {
        public bool UltraDeepValue { get; set; }
    }
}
