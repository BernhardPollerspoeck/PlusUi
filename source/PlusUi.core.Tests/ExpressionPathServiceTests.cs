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
        CollectionAssert.AreEqual(new[] { "SimpleValue" }, path.Segments);
    }

    [TestMethod]
    public void GetPropertyPath_1LevelNested_ReturnsPath()
    {
        // Arrange
        var vm = new TestViewModel();

        // Act
        var path = _service.GetPropertyPath(() => vm.Level1.Checked);

        // Assert
        CollectionAssert.AreEqual(new[] { "Level1", "Checked" }, path.Segments);
    }

    [TestMethod]
    public void GetPropertyPath_2LevelNested_ReturnsPath()
    {
        // Arrange
        var vm = new TestViewModel();

        // Act
        var path = _service.GetPropertyPath(() => vm.Level1.Level2.DeepValue);

        // Assert
        CollectionAssert.AreEqual(new[] { "Level1", "Level2", "DeepValue" }, path.Segments);
    }

    [TestMethod]
    public void GetPropertyPath_5LevelNested_ReturnsPath()
    {
        // Arrange
        var vm = new TestViewModel();

        // Act
        var path = _service.GetPropertyPath(() => vm.Level1.Level2.Level3.Level4.Level5.UltraDeepValue);

        // Assert
        CollectionAssert.AreEqual(new[] { "Level1", "Level2", "Level3", "Level4", "Level5", "UltraDeepValue" }, path.Segments);
    }

    [TestMethod]
    public void GetPropertyPath_SimpleProperty_HasNoAccessorForLeaf()
    {
        // Arrange
        var vm = new TestViewModel();

        // Act
        var path = _service.GetPropertyPath(() => vm.SimpleValue);

        // Assert - the only (leaf) segment is never traversed, so it has no accessor
        Assert.AreEqual(1, path.SegmentAccessors.Length);
        Assert.IsNull(path.SegmentAccessors[0]);
    }

    [TestMethod]
    public void GetPropertyPath_Nested_NonLeafSegmentsHaveAccessors_LeafIsNull()
    {
        // Arrange
        var vm = new TestViewModel();

        // Act
        var path = _service.GetPropertyPath(() => vm.Level1.Level2.DeepValue);

        // Assert
        Assert.AreEqual(3, path.SegmentAccessors.Length);
        Assert.IsNotNull(path.SegmentAccessors[0]);
        Assert.IsNotNull(path.SegmentAccessors[1]);
        Assert.IsNull(path.SegmentAccessors[2], "Leaf segment should not have an accessor");
    }

    [TestMethod]
    public void GetPropertyPath_Accessors_TraverseToLiveIntermediateObjects()
    {
        // Arrange
        var vm = new TestViewModel();

        // Act
        var path = _service.GetPropertyPath(() => vm.Level1.Level2.DeepValue);
        var level1 = path.SegmentAccessors[0]!(vm);
        var level2 = path.SegmentAccessors[1]!(level1!);

        // Assert - accessors return the actual live objects (no reflection, no copies)
        Assert.AreSame(vm.Level1, level1);
        Assert.AreSame(vm.Level1.Level2, level2);
    }

    [TestMethod]
    public void GetPropertyPath_Accessors_FollowSwappedIntermediateObject()
    {
        // Arrange
        var vm = new TestViewModel();
        var path = _service.GetPropertyPath(() => vm.Level1.Checked);

        // Act - swap the intermediate object after the path was resolved
        var newLevel1 = new Level1();
        vm.Level1 = newLevel1;

        // Assert - accessor reads from the live object, so it returns the new instance
        Assert.AreSame(newLevel1, path.SegmentAccessors[0]!(vm));
    }

    [TestMethod]
    public void GetPropertyPath_Accessor_TypeMismatch_ReturnsNull()
    {
        // Arrange
        var vm = new TestViewModel();
        var path = _service.GetPropertyPath(() => vm.Level1.Checked);

        // Act - pass an object of the wrong type into the accessor
        var result = path.SegmentAccessors[0]!("not a view model");

        // Assert - mirrors the previous reflection behavior (property not found -> null), no exception
        Assert.IsNull(result);
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
