using PlusUi.core;

namespace UiPlus.core.Tests;

[TestClass]
public class AccessibilityTests
{
    #region Default AccessibilityRole Tests

    [TestMethod]
    public void TestButton_DefaultAccessibilityRole_IsButton()
    {
        // Arrange & Act
        var button = new Button();

        // Assert
        Assert.AreEqual(AccessibilityRole.Button, button.AccessibilityRole);
    }

    [TestMethod]
    public void TestEntry_DefaultAccessibilityRole_IsTextInput()
    {
        // Arrange & Act
        var entry = new Entry();

        // Assert
        Assert.AreEqual(AccessibilityRole.TextInput, entry.AccessibilityRole);
    }

    [TestMethod]
    public void TestCheckbox_DefaultAccessibilityRole_IsCheckbox()
    {
        // Arrange & Act
        var checkbox = new Checkbox();

        // Assert
        Assert.AreEqual(AccessibilityRole.Checkbox, checkbox.AccessibilityRole);
    }

    [TestMethod]
    public void TestToggle_DefaultAccessibilityRole_IsToggle()
    {
        // Arrange & Act
        var toggle = new Toggle();

        // Assert
        Assert.AreEqual(AccessibilityRole.Toggle, toggle.AccessibilityRole);
    }

    [TestMethod]
    public void TestRadioButton_DefaultAccessibilityRole_IsRadioButton()
    {
        // Arrange & Act
        var radioButton = new RadioButton();

        // Assert
        Assert.AreEqual(AccessibilityRole.RadioButton, radioButton.AccessibilityRole);
    }

    [TestMethod]
    public void TestLink_DefaultAccessibilityRole_IsLink()
    {
        // Arrange & Act
        var link = new Link();

        // Assert
        Assert.AreEqual(AccessibilityRole.Link, link.AccessibilityRole);
    }

    [TestMethod]
    public void TestSlider_DefaultAccessibilityRole_IsSlider()
    {
        // Arrange & Act
        var slider = new Slider();

        // Assert
        Assert.AreEqual(AccessibilityRole.Slider, slider.AccessibilityRole);
    }

    [TestMethod]
    public void TestLabel_DefaultAccessibilityRole_IsLabel()
    {
        // Arrange & Act
        var label = new Label();

        // Assert
        Assert.AreEqual(AccessibilityRole.Label, label.AccessibilityRole);
    }

    [TestMethod]
    public void TestImage_DefaultAccessibilityRole_IsImage()
    {
        // Arrange & Act
        var image = new Image();

        // Assert
        Assert.AreEqual(AccessibilityRole.Image, image.AccessibilityRole);
    }

    [TestMethod]
    public void TestProgressBar_DefaultAccessibilityRole_IsProgressBar()
    {
        // Arrange & Act
        var progressBar = new ProgressBar();

        // Assert
        Assert.AreEqual(AccessibilityRole.ProgressBar, progressBar.AccessibilityRole);
    }

    [TestMethod]
    public void TestActivityIndicator_DefaultAccessibilityRole_IsSpinner()
    {
        // Arrange & Act
        var spinner = new ActivityIndicator();

        // Assert
        Assert.AreEqual(AccessibilityRole.Spinner, spinner.AccessibilityRole);
    }

    [TestMethod]
    public void TestComboBox_DefaultAccessibilityRole_IsComboBox()
    {
        // Arrange & Act
        var comboBox = new ComboBox<string>();

        // Assert
        Assert.AreEqual(AccessibilityRole.ComboBox, comboBox.AccessibilityRole);
    }

    [TestMethod]
    public void TestDatePicker_DefaultAccessibilityRole_IsDatePicker()
    {
        // Arrange & Act
        var datePicker = new DatePicker();

        // Assert
        Assert.AreEqual(AccessibilityRole.DatePicker, datePicker.AccessibilityRole);
    }

    [TestMethod]
    public void TestTimePicker_DefaultAccessibilityRole_IsTimePicker()
    {
        // Arrange & Act
        var timePicker = new TimePicker();

        // Assert
        Assert.AreEqual(AccessibilityRole.TimePicker, timePicker.AccessibilityRole);
    }

    #endregion

    #region GetComputedAccessibilityLabel Tests

    [TestMethod]
    public void TestButton_GetComputedAccessibilityLabel_ReturnsText()
    {
        // Arrange
        var button = new Button().SetText("Submit");

        // Act
        var label = button.GetComputedAccessibilityLabel();

        // Assert
        Assert.AreEqual("Submit", label);
    }

    [TestMethod]
    public void TestButton_GetComputedAccessibilityLabel_PrefersExplicitLabel()
    {
        // Arrange
        var button = new Button()
            .SetText("Submit")
            .SetAccessibilityLabel("Submit form");

        // Act
        var label = button.GetComputedAccessibilityLabel();

        // Assert
        Assert.AreEqual("Submit form", label);
    }

    [TestMethod]
    public void TestEntry_GetComputedAccessibilityLabel_ReturnsPlaceholder()
    {
        // Arrange
        var entry = new Entry().SetPlaceholder("Enter your name");

        // Act
        var label = entry.GetComputedAccessibilityLabel();

        // Assert
        Assert.AreEqual("Enter your name", label);
    }

    [TestMethod]
    public void TestEntry_GetComputedAccessibilityLabel_DefaultsToTextInput()
    {
        // Arrange
        var entry = new Entry();

        // Act
        var label = entry.GetComputedAccessibilityLabel();

        // Assert
        Assert.AreEqual("Text input", label);
    }

    [TestMethod]
    public void TestLabel_GetComputedAccessibilityLabel_ReturnsText()
    {
        // Arrange
        var label = new Label().SetText("Hello World");

        // Act
        var accessibilityLabel = label.GetComputedAccessibilityLabel();

        // Assert
        Assert.AreEqual("Hello World", accessibilityLabel);
    }

    [TestMethod]
    public void TestCheckbox_GetComputedAccessibilityLabel_ReturnsCheckbox()
    {
        // Arrange
        var checkbox = new Checkbox();

        // Act
        var label = checkbox.GetComputedAccessibilityLabel();

        // Assert
        Assert.AreEqual("Checkbox", label);
    }

    [TestMethod]
    public void TestToggle_GetComputedAccessibilityLabel_ReturnsToggleSwitch()
    {
        // Arrange
        var toggle = new Toggle();

        // Act
        var label = toggle.GetComputedAccessibilityLabel();

        // Assert
        Assert.AreEqual("Toggle switch", label);
    }

    [TestMethod]
    public void TestRadioButton_GetComputedAccessibilityLabel_ReturnsText()
    {
        // Arrange
        var radioButton = new RadioButton().SetText("Option A");

        // Act
        var label = radioButton.GetComputedAccessibilityLabel();

        // Assert
        Assert.AreEqual("Option A", label);
    }

    [TestMethod]
    public void TestLink_GetComputedAccessibilityLabel_ReturnsText()
    {
        // Arrange
        var link = new Link().SetText("Click here");

        // Act
        var label = link.GetComputedAccessibilityLabel();

        // Assert
        Assert.AreEqual("Click here", label);
    }

    [TestMethod]
    public void TestSlider_GetComputedAccessibilityLabel_ReturnsSlider()
    {
        // Arrange
        var slider = new Slider();

        // Act
        var label = slider.GetComputedAccessibilityLabel();

        // Assert
        Assert.AreEqual("Slider", label);
    }

    #endregion

    #region GetComputedAccessibilityValue Tests

    [TestMethod]
    public void TestEntry_GetComputedAccessibilityValue_ReturnsText()
    {
        // Arrange
        var entry = new Entry();
        entry.HandleInput('H');
        entry.HandleInput('e');
        entry.HandleInput('l');
        entry.HandleInput('l');
        entry.HandleInput('o');

        // Act
        var value = entry.GetComputedAccessibilityValue();

        // Assert
        Assert.AreEqual("Hello", value);
    }

    [TestMethod]
    public void TestEntry_GetComputedAccessibilityValue_MasksPassword()
    {
        // Arrange
        var entry = new Entry().SetIsPassword(true);
        entry.HandleInput('1');
        entry.HandleInput('2');
        entry.HandleInput('3');
        entry.HandleInput('4');

        // Act
        var value = entry.GetComputedAccessibilityValue();

        // Assert
        Assert.AreEqual("****", value);
    }

    [TestMethod]
    public void TestCheckbox_GetComputedAccessibilityValue_ReturnsCheckedState()
    {
        // Arrange
        var checkbox = new Checkbox().SetIsChecked(true);

        // Act
        var value = checkbox.GetComputedAccessibilityValue();

        // Assert
        Assert.AreEqual("Checked", value);
    }

    [TestMethod]
    public void TestCheckbox_GetComputedAccessibilityValue_ReturnsUncheckedState()
    {
        // Arrange
        var checkbox = new Checkbox().SetIsChecked(false);

        // Act
        var value = checkbox.GetComputedAccessibilityValue();

        // Assert
        Assert.AreEqual("Unchecked", value);
    }

    [TestMethod]
    public void TestToggle_GetComputedAccessibilityValue_ReturnsOnState()
    {
        // Arrange
        var toggle = new Toggle().SetIsOn(true);

        // Act
        var value = toggle.GetComputedAccessibilityValue();

        // Assert
        Assert.AreEqual("On", value);
    }

    [TestMethod]
    public void TestToggle_GetComputedAccessibilityValue_ReturnsOffState()
    {
        // Arrange
        var toggle = new Toggle().SetIsOn(false);

        // Act
        var value = toggle.GetComputedAccessibilityValue();

        // Assert
        Assert.AreEqual("Off", value);
    }

    [TestMethod]
    public void TestRadioButton_GetComputedAccessibilityValue_ReturnsSelectedState()
    {
        // Arrange
        var radioButton = new RadioButton().SetIsSelected(true);

        // Act
        var value = radioButton.GetComputedAccessibilityValue();

        // Assert
        Assert.AreEqual("Selected", value);
    }

    [TestMethod]
    public void TestRadioButton_GetComputedAccessibilityValue_ReturnsNotSelectedState()
    {
        // Arrange
        var radioButton = new RadioButton().SetIsSelected(false);

        // Act
        var value = radioButton.GetComputedAccessibilityValue();

        // Assert
        Assert.AreEqual("Not selected", value);
    }

    [TestMethod]
    public void TestSlider_GetComputedAccessibilityValue_ReturnsValueWithRange()
    {
        // Arrange
        var slider = new Slider()
            .SetMinimum(0)
            .SetMaximum(100)
            .SetValue(50);

        // Act
        var value = slider.GetComputedAccessibilityValue();

        // Assert
        Assert.AreEqual("50 (0 to 100)", value);
    }

    [TestMethod]
    public void TestProgressBar_GetComputedAccessibilityValue_ReturnsPercentage()
    {
        // Arrange
        var progressBar = new ProgressBar().SetProgress(0.75f);

        // Act
        var value = progressBar.GetComputedAccessibilityValue();

        // Assert
        Assert.AreEqual("75%", value);
    }

    #endregion

    #region GetComputedAccessibilityTraits Tests

    [TestMethod]
    public void TestCheckbox_GetComputedAccessibilityTraits_IncludesCheckedWhenChecked()
    {
        // Arrange
        var checkbox = new Checkbox().SetIsChecked(true);

        // Act
        var traits = checkbox.GetComputedAccessibilityTraits();

        // Assert
        Assert.IsTrue(traits.HasFlag(AccessibilityTrait.Checked));
    }

    [TestMethod]
    public void TestCheckbox_GetComputedAccessibilityTraits_ExcludesCheckedWhenUnchecked()
    {
        // Arrange
        var checkbox = new Checkbox().SetIsChecked(false);

        // Act
        var traits = checkbox.GetComputedAccessibilityTraits();

        // Assert
        Assert.IsFalse(traits.HasFlag(AccessibilityTrait.Checked));
    }

    [TestMethod]
    public void TestToggle_GetComputedAccessibilityTraits_IncludesCheckedWhenOn()
    {
        // Arrange
        var toggle = new Toggle().SetIsOn(true);

        // Act
        var traits = toggle.GetComputedAccessibilityTraits();

        // Assert
        Assert.IsTrue(traits.HasFlag(AccessibilityTrait.Checked));
    }

    [TestMethod]
    public void TestRadioButton_GetComputedAccessibilityTraits_IncludesSelectedWhenSelected()
    {
        // Arrange
        var radioButton = new RadioButton().SetIsSelected(true);

        // Act
        var traits = radioButton.GetComputedAccessibilityTraits();

        // Assert
        Assert.IsTrue(traits.HasFlag(AccessibilityTrait.Selected));
    }

    [TestMethod]
    public void TestActivityIndicator_GetComputedAccessibilityTraits_IncludesBusyWhenRunning()
    {
        // Arrange
        var spinner = new ActivityIndicator().SetIsRunning(true);

        // Act
        var traits = spinner.GetComputedAccessibilityTraits();

        // Assert
        Assert.IsTrue(traits.HasFlag(AccessibilityTrait.Busy));
    }

    [TestMethod]
    public void TestActivityIndicator_GetComputedAccessibilityTraits_ExcludesBusyWhenNotRunning()
    {
        // Arrange
        var spinner = new ActivityIndicator().SetIsRunning(false);

        // Act
        var traits = spinner.GetComputedAccessibilityTraits();

        // Assert
        Assert.IsFalse(traits.HasFlag(AccessibilityTrait.Busy));
    }

    [TestMethod]
    public void TestComboBox_GetComputedAccessibilityTraits_IncludesExpandedWhenOpen()
    {
        // Arrange
        var comboBox = new ComboBox<string>().SetIsOpen(true);

        // Act
        var traits = comboBox.GetComputedAccessibilityTraits();

        // Assert
        Assert.IsTrue(traits.HasFlag(AccessibilityTrait.Expanded));
    }

    [TestMethod]
    public void TestComboBox_GetComputedAccessibilityTraits_IncludesHasPopup()
    {
        // Arrange
        var comboBox = new ComboBox<string>();

        // Act
        var traits = comboBox.GetComputedAccessibilityTraits();

        // Assert
        Assert.IsTrue(traits.HasFlag(AccessibilityTrait.HasPopup));
    }

    #endregion

    #region SetAccessibility Property Tests

    [TestMethod]
    public void TestUiElement_SetAccessibilityLabel_SetsProperty()
    {
        // Arrange
        var button = new Button();

        // Act
        button.SetAccessibilityLabel("Submit form button");

        // Assert
        Assert.AreEqual("Submit form button", button.AccessibilityLabel);
    }

    [TestMethod]
    public void TestUiElement_SetAccessibilityHint_SetsProperty()
    {
        // Arrange
        var button = new Button();

        // Act
        button.SetAccessibilityHint("Double tap to submit the form");

        // Assert
        Assert.AreEqual("Double tap to submit the form", button.AccessibilityHint);
    }

    [TestMethod]
    public void TestUiElement_SetAccessibilityValue_SetsProperty()
    {
        // Arrange
        var slider = new Slider();

        // Act
        slider.SetAccessibilityValue("50 percent");

        // Assert
        Assert.AreEqual("50 percent", slider.AccessibilityValue);
    }

    [TestMethod]
    public void TestVStack_AccessibilityRole_DefaultsToContainer()
    {
        // Arrange & Act
        var element = new VStack();

        // Assert - VStack defaults to Container role
        Assert.AreEqual(AccessibilityRole.Container, element.AccessibilityRole);
    }

    [TestMethod]
    public void TestUiElement_SetIsAccessibilityElement_SetsProperty()
    {
        // Arrange
        var label = new Label();

        // Act
        label.SetIsAccessibilityElement(false);

        // Assert
        Assert.IsFalse(label.IsAccessibilityElement);
    }

    [TestMethod]
    public void TestUiElement_IsAccessibilityElement_DefaultTrue()
    {
        // Arrange & Act
        var button = new Button();

        // Assert
        Assert.IsTrue(button.IsAccessibilityElement);
    }

    #endregion
}
