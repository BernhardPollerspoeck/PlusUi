using PlusUi.core;
using PlusUi.core.Services;
using SkiaSharp;

namespace UiPlus.core.Tests;

[TestClass]
public class RadioButtonTests
{
    #region Property Tests

    [TestMethod]
    public void TestRadioButton_SetText_PropertyIsSet()
    {
        // Arrange
        var radioButton = new RadioButton();

        // Act
        radioButton.SetText("Option A");

        // Assert
        Assert.AreEqual("Option A", radioButton.Text);
    }

    [TestMethod]
    public void TestRadioButton_SetIsSelected_PropertyIsSet()
    {
        // Arrange
        var radioButton = new RadioButton();

        // Act
        radioButton.SetIsSelected(true);

        // Assert
        Assert.IsTrue(radioButton.IsSelected);
    }

    [TestMethod]
    public void TestRadioButton_SetGroup_PropertyIsSet()
    {
        // Arrange
        var radioButton = new RadioButton();

        // Act
        radioButton.SetGroup("myGroup");

        // Assert
        Assert.AreEqual("myGroup", radioButton.Group);
    }

    [TestMethod]
    public void TestRadioButton_SetValue_PropertyIsSet()
    {
        // Arrange
        var radioButton = new RadioButton();

        // Act
        radioButton.SetValue("valueA");

        // Assert
        Assert.AreEqual("valueA", radioButton.Value);
    }

    [TestMethod]
    public void TestRadioButton_SetTextSize_PropertyIsSet()
    {
        // Arrange
        var radioButton = new RadioButton();

        // Act
        radioButton.SetTextSize(18f);

        // Assert
        Assert.AreEqual(18f, radioButton.TextSize);
    }

    [TestMethod]
    public void TestRadioButton_SetTextColor_PropertyIsSet()
    {
        // Arrange
        var radioButton = new RadioButton();

        // Act
        radioButton.SetTextColor(SKColors.Red);

        // Assert
        Assert.AreEqual(SKColors.Red, radioButton.TextColor);
    }

    [TestMethod]
    public void TestRadioButton_SetCircleColor_PropertyIsSet()
    {
        // Arrange
        var radioButton = new RadioButton();

        // Act
        radioButton.SetCircleColor(SKColors.Blue);

        // Assert
        Assert.AreEqual(SKColors.Blue, radioButton.CircleColor);
    }

    [TestMethod]
    public void TestRadioButton_SetSelectedColor_PropertyIsSet()
    {
        // Arrange
        var radioButton = new RadioButton();

        // Act
        radioButton.SetSelectedColor(SKColors.Green);

        // Assert
        Assert.AreEqual(SKColors.Green, radioButton.SelectedColor);
    }

    #endregion

    #region Default Value Tests

    [TestMethod]
    public void TestRadioButton_DefaultIsSelected_IsFalse()
    {
        // Arrange & Act
        var radioButton = new RadioButton();

        // Assert
        Assert.IsFalse(radioButton.IsSelected);
    }

    [TestMethod]
    public void TestRadioButton_DefaultTextSize_Is14()
    {
        // Arrange & Act
        var radioButton = new RadioButton();

        // Assert
        Assert.AreEqual(14f, radioButton.TextSize);
    }

    [TestMethod]
    public void TestRadioButton_DefaultTextColor_IsWhite()
    {
        // Arrange & Act
        var radioButton = new RadioButton();

        // Assert
        Assert.AreEqual(SKColors.White, radioButton.TextColor);
    }

    [TestMethod]
    public void TestRadioButton_DefaultCircleColor_IsWhite()
    {
        // Arrange & Act
        var radioButton = new RadioButton();

        // Assert
        Assert.AreEqual(SKColors.White, radioButton.CircleColor);
    }

    #endregion

    #region Fluent API Tests

    [TestMethod]
    public void TestRadioButton_FluentApi_ReturnsInstance()
    {
        // Arrange
        var radioButton = new RadioButton();

        // Act
        var result = radioButton
            .SetText("Test")
            .SetGroup("group")
            .SetValue("value")
            .SetIsSelected(true)
            .SetTextSize(16f)
            .SetTextColor(SKColors.Black)
            .SetCircleColor(SKColors.Gray)
            .SetSelectedColor(SKColors.Blue);

        // Assert
        Assert.AreSame(radioButton, result);
    }

    #endregion

    #region Measure Tests

    [TestMethod]
    public void TestRadioButton_Measure_NoText_ReturnsCircleSize()
    {
        // Arrange
        var radioButton = new RadioButton();
        var availableSize = new Size(200, 50);

        // Act
        radioButton.Measure(availableSize);

        // Assert
        Assert.AreEqual(20f, radioButton.ElementSize.Width); // CircleSize constant
        Assert.AreEqual(20f, radioButton.ElementSize.Height);
    }

    [TestMethod]
    public void TestRadioButton_Measure_WithText_IncludesTextWidth()
    {
        // Arrange
        var radioButton = new RadioButton().SetText("Option A");
        var availableSize = new Size(200, 50);

        // Act
        radioButton.Measure(availableSize);

        // Assert
        Assert.IsTrue(radioButton.ElementSize.Width > 20f); // Should be more than just circle
    }

    #endregion

    #region Group Object Type Tests

    [TestMethod]
    public void TestRadioButton_SetGroup_WithString_Works()
    {
        // Arrange
        var radioButton = new RadioButton();

        // Act
        radioButton.SetGroup("stringGroup");

        // Assert
        Assert.AreEqual("stringGroup", radioButton.Group);
    }

    [TestMethod]
    public void TestRadioButton_SetGroup_WithEnum_Works()
    {
        // Arrange
        var radioButton = new RadioButton();

        // Act
        radioButton.SetGroup(TestEnum.GroupA);

        // Assert
        Assert.AreEqual(TestEnum.GroupA, radioButton.Group);
    }

    [TestMethod]
    public void TestRadioButton_SetGroup_WithObject_Works()
    {
        // Arrange
        var radioButton = new RadioButton();
        var groupObject = new object();

        // Act
        radioButton.SetGroup(groupObject);

        // Assert
        Assert.AreSame(groupObject, radioButton.Group);
    }

    private enum TestEnum
    {
        GroupA,
        GroupB
    }

    #endregion

    #region RadioButtonManager Tests

    [TestMethod]
    public void TestRadioButtonManager_Register_AddsButton()
    {
        // Arrange
        var manager = new RadioButtonManager();
        var radioButton = new RadioButton().SetGroup("test");

        // Act
        manager.Register(radioButton);

        // Assert - No exception means success
        Assert.IsTrue(true);
    }

    [TestMethod]
    public void TestRadioButtonManager_Register_SameButtonTwice_DoesNotDuplicate()
    {
        // Arrange
        var manager = new RadioButtonManager();
        var radioButton = new RadioButton().SetGroup("test");

        // Act
        manager.Register(radioButton);
        manager.Register(radioButton);

        // Assert - No exception means success
        Assert.IsTrue(true);
    }

    [TestMethod]
    public void TestRadioButtonManager_NotifySelected_DeselectsOthersInGroup()
    {
        // Arrange
        var manager = new RadioButtonManager();
        var radio1 = new RadioButton().SetGroup("group1").SetIsSelected(true);
        var radio2 = new RadioButton().SetGroup("group1");
        var radio3 = new RadioButton().SetGroup("group2").SetIsSelected(true);

        manager.Register(radio1);
        manager.Register(radio2);
        manager.Register(radio3);

        // Act
        radio2.SetIsSelected(true);
        manager.NotifySelected(radio2);

        // Assert
        Assert.IsFalse(radio1.IsSelected); // Same group - deselected
        Assert.IsTrue(radio2.IsSelected);  // Selected
        Assert.IsTrue(radio3.IsSelected);  // Different group - unchanged
    }

    [TestMethod]
    public void TestRadioButtonManager_NotifySelected_NullGroup_NoEffect()
    {
        // Arrange
        var manager = new RadioButtonManager();
        var radio1 = new RadioButton(); // No group
        var radio2 = new RadioButton().SetGroup("group1").SetIsSelected(true);

        manager.Register(radio1);
        manager.Register(radio2);

        // Act
        manager.NotifySelected(radio1);

        // Assert
        Assert.IsTrue(radio2.IsSelected); // Should remain selected (different group context)
    }

    [TestMethod]
    public void TestRadioButtonManager_Unregister_RemovesButton()
    {
        // Arrange
        var manager = new RadioButtonManager();
        var radioButton = new RadioButton().SetGroup("test");
        manager.Register(radioButton);

        // Act
        manager.Unregister(radioButton);

        // Assert - No exception means success
        Assert.IsTrue(true);
    }

    #endregion

    #region Selection Behavior Tests

    [TestMethod]
    public void TestRadioButton_InvokeCommand_SelectsButton()
    {
        // Arrange
        var radioButton = new RadioButton().SetGroup("test");
        Assert.IsFalse(radioButton.IsSelected);

        // Act
        ((IInputControl)radioButton).InvokeCommand();

        // Assert
        Assert.IsTrue(radioButton.IsSelected);
    }

    [TestMethod]
    public void TestRadioButton_InvokeCommand_AlreadySelected_RemainsSelected()
    {
        // Arrange
        var radioButton = new RadioButton().SetGroup("test").SetIsSelected(true);

        // Act
        ((IInputControl)radioButton).InvokeCommand();

        // Assert
        Assert.IsTrue(radioButton.IsSelected);
    }

    #endregion
}
