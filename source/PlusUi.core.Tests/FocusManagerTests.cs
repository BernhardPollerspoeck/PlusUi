using PlusUi.core;
using PlusUi.core.Services.Focus;

namespace PlusUi.core.Tests;

[TestClass]
public class FocusManagerTests
{
    #region IFocusable Default Value Tests

    [TestMethod]
    public void TestButton_IsFocusable_DefaultTrue()
    {
        // Arrange & Act
        var button = new Button();

        // Assert
        Assert.IsTrue(button.IsFocusable);
    }

    [TestMethod]
    public void TestEntry_IsFocusable_DefaultTrue()
    {
        // Arrange & Act
        var entry = new Entry();

        // Assert
        Assert.IsTrue(entry.IsFocusable);
    }

    [TestMethod]
    public void TestLabel_IsFocusable_DefaultFalse()
    {
        // Arrange & Act
        var label = new Label();

        // Assert
        Assert.IsFalse(label.IsFocusable);
    }

    [TestMethod]
    public void TestCheckbox_IsFocusable_DefaultTrue()
    {
        // Arrange & Act
        var checkbox = new Checkbox();

        // Assert
        Assert.IsTrue(checkbox.IsFocusable);
    }

    [TestMethod]
    public void TestToggle_IsFocusable_DefaultTrue()
    {
        // Arrange & Act
        var toggle = new Toggle();

        // Assert
        Assert.IsTrue(toggle.IsFocusable);
    }

    [TestMethod]
    public void TestSlider_IsFocusable_DefaultTrue()
    {
        // Arrange & Act
        var slider = new Slider();

        // Assert
        Assert.IsTrue(slider.IsFocusable);
    }

    [TestMethod]
    public void TestRadioButton_IsFocusable_DefaultTrue()
    {
        // Arrange & Act
        var radioButton = new RadioButton();

        // Assert
        Assert.IsTrue(radioButton.IsFocusable);
    }

    [TestMethod]
    public void TestLink_IsFocusable_DefaultTrue()
    {
        // Arrange & Act
        var link = new Link();

        // Assert
        Assert.IsTrue(link.IsFocusable);
    }

    [TestMethod]
    public void TestComboBox_IsFocusable_DefaultTrue()
    {
        // Arrange & Act
        var comboBox = new ComboBox<string>();

        // Assert
        Assert.IsTrue(comboBox.IsFocusable);
    }

    [TestMethod]
    public void TestDatePicker_IsFocusable_DefaultTrue()
    {
        // Arrange & Act
        var datePicker = new DatePicker();

        // Assert
        Assert.IsTrue(datePicker.IsFocusable);
    }

    [TestMethod]
    public void TestTimePicker_IsFocusable_DefaultTrue()
    {
        // Arrange & Act
        var timePicker = new TimePicker();

        // Assert
        Assert.IsTrue(timePicker.IsFocusable);
    }

    #endregion

    #region TabIndex Tests

    [TestMethod]
    public void TestUiElement_SetTabIndex_SetsProperty()
    {
        // Arrange
        var button = new Button();

        // Act
        button.SetTabIndex(5);

        // Assert
        Assert.AreEqual(5, button.TabIndex);
    }

    [TestMethod]
    public void TestUiElement_TabIndex_DefaultNull()
    {
        // Arrange & Act
        var button = new Button();

        // Assert
        Assert.IsNull(button.TabIndex);
    }

    [TestMethod]
    public void TestUiElement_SetTabIndex_Negative_SetsProperty()
    {
        // Arrange
        var button = new Button();

        // Act
        button.SetTabIndex(-1);

        // Assert
        Assert.AreEqual(-1, button.TabIndex);
    }

    #endregion

    #region IFocused State Tests

    [TestMethod]
    public void TestUiElement_IsFocused_DefaultFalse()
    {
        // Arrange & Act
        var button = new Button();

        // Assert
        Assert.IsFalse(button.IsFocused);
    }

    #endregion

    #region Focus Ring Properties Tests

    [TestMethod]
    public void TestUiElement_SetFocusRingColor_SetsProperty()
    {
        // Arrange
        var button = new Button();
        var color = new Color(255, 0, 0);

        // Act
        button.SetFocusRingColor(color);

        // Assert
        Assert.AreEqual(color, button.FocusRingColor);
    }

    [TestMethod]
    public void TestUiElement_SetFocusRingWidth_SetsProperty()
    {
        // Arrange
        var button = new Button();

        // Act
        button.SetFocusRingWidth(4f);

        // Assert
        Assert.AreEqual(4f, button.FocusRingWidth);
    }

    [TestMethod]
    public void TestUiElement_SetFocusRingOffset_SetsProperty()
    {
        // Arrange
        var button = new Button();

        // Act
        button.SetFocusRingOffset(3f);

        // Assert
        Assert.AreEqual(3f, button.FocusRingOffset);
    }

    [TestMethod]
    public void TestUiElement_FocusRingWidth_Default2()
    {
        // Arrange & Act
        var button = new Button();

        // Assert
        Assert.AreEqual(2f, button.FocusRingWidth);
    }

    [TestMethod]
    public void TestUiElement_FocusRingOffset_Default2()
    {
        // Arrange & Act
        var button = new Button();

        // Assert
        Assert.AreEqual(2f, button.FocusRingOffset);
    }

    #endregion

    #region IFocusable Interface Implementation Tests

    [TestMethod]
    public void TestButton_ImplementsIFocusable()
    {
        // Arrange & Act
        var button = new Button();

        // Assert
        Assert.IsInstanceOfType<IFocusable>(button);
    }

    [TestMethod]
    public void TestEntry_ImplementsIFocusable()
    {
        // Arrange & Act
        var entry = new Entry();

        // Assert
        Assert.IsInstanceOfType<IFocusable>(entry);
    }

    [TestMethod]
    public void TestCheckbox_ImplementsIFocusable()
    {
        // Arrange & Act
        var checkbox = new Checkbox();

        // Assert
        Assert.IsInstanceOfType<IFocusable>(checkbox);
    }

    [TestMethod]
    public void TestToggle_ImplementsIFocusable()
    {
        // Arrange & Act
        var toggle = new Toggle();

        // Assert
        Assert.IsInstanceOfType<IFocusable>(toggle);
    }

    [TestMethod]
    public void TestSlider_ImplementsIFocusable()
    {
        // Arrange & Act
        var slider = new Slider();

        // Assert
        Assert.IsInstanceOfType<IFocusable>(slider);
    }

    [TestMethod]
    public void TestComboBox_ImplementsIFocusable()
    {
        // Arrange & Act
        var comboBox = new ComboBox<string>();

        // Assert
        Assert.IsInstanceOfType<IFocusable>(comboBox);
    }

    [TestMethod]
    public void TestDatePicker_ImplementsIFocusable()
    {
        // Arrange & Act
        var datePicker = new DatePicker();

        // Assert
        Assert.IsInstanceOfType<IFocusable>(datePicker);
    }

    [TestMethod]
    public void TestTimePicker_ImplementsIFocusable()
    {
        // Arrange & Act
        var timePicker = new TimePicker();

        // Assert
        Assert.IsInstanceOfType<IFocusable>(timePicker);
    }

    #endregion
}
