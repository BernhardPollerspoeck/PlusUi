using Microsoft.VisualStudio.TestTools.UnitTesting;
using PlusUi.core;

namespace PlusUi.core.Tests;

[TestClass]
public class TooltipTests
{
    #region TooltipAttachment Tests

    [TestMethod]
    public void TooltipAttachment_SetContent_StringContent_ShouldSetProperty()
    {
        // Arrange
        var attachment = new TooltipAttachment();

        // Act
        attachment.SetContent("Test tooltip");

        // Assert
        Assert.AreEqual("Test tooltip", attachment.Content);
    }

    [TestMethod]
    public void TooltipAttachment_SetContent_UiElementContent_ShouldSetProperty()
    {
        // Arrange
        var attachment = new TooltipAttachment();
        var element = new Label().SetText("Complex tooltip");

        // Act
        attachment.SetContent(element);

        // Assert
        Assert.AreSame(element, attachment.Content);
    }

    [TestMethod]
    public void TooltipAttachment_SetPlacement_ShouldSetProperty()
    {
        // Arrange
        var attachment = new TooltipAttachment();

        // Act
        attachment.SetPlacement(TooltipPlacement.Bottom);

        // Assert
        Assert.AreEqual(TooltipPlacement.Bottom, attachment.Placement);
    }

    [TestMethod]
    public void TooltipAttachment_DefaultPlacement_ShouldBeAuto()
    {
        // Arrange & Act
        var attachment = new TooltipAttachment();

        // Assert
        Assert.AreEqual(TooltipPlacement.Auto, attachment.Placement);
    }

    [TestMethod]
    public void TooltipAttachment_SetShowDelay_ShouldSetProperty()
    {
        // Arrange
        var attachment = new TooltipAttachment();

        // Act
        attachment.SetShowDelay(1000);

        // Assert
        Assert.AreEqual(1000, attachment.ShowDelay);
    }

    [TestMethod]
    public void TooltipAttachment_DefaultShowDelay_ShouldBe500()
    {
        // Arrange & Act
        var attachment = new TooltipAttachment();

        // Assert
        Assert.AreEqual(500, attachment.ShowDelay);
    }

    [TestMethod]
    public void TooltipAttachment_SetShowDelay_NegativeValue_ShouldClampToZero()
    {
        // Arrange
        var attachment = new TooltipAttachment();

        // Act
        attachment.SetShowDelay(-100);

        // Assert
        Assert.AreEqual(0, attachment.ShowDelay);
    }

    [TestMethod]
    public void TooltipAttachment_SetHideDelay_ShouldSetProperty()
    {
        // Arrange
        var attachment = new TooltipAttachment();

        // Act
        attachment.SetHideDelay(200);

        // Assert
        Assert.AreEqual(200, attachment.HideDelay);
    }

    [TestMethod]
    public void TooltipAttachment_DefaultHideDelay_ShouldBeZero()
    {
        // Arrange & Act
        var attachment = new TooltipAttachment();

        // Assert
        Assert.AreEqual(0, attachment.HideDelay);
    }

    [TestMethod]
    public void TooltipAttachment_SetHideDelay_NegativeValue_ShouldClampToZero()
    {
        // Arrange
        var attachment = new TooltipAttachment();

        // Act
        attachment.SetHideDelay(-50);

        // Assert
        Assert.AreEqual(0, attachment.HideDelay);
    }

    [TestMethod]
    public void TooltipAttachment_FluentAPI_ShouldEnableChaining()
    {
        // Arrange & Act
        var attachment = new TooltipAttachment()
            .SetContent("Chained tooltip")
            .SetPlacement(TooltipPlacement.Right)
            .SetShowDelay(750)
            .SetHideDelay(100);

        // Assert
        Assert.AreEqual("Chained tooltip", attachment.Content);
        Assert.AreEqual(TooltipPlacement.Right, attachment.Placement);
        Assert.AreEqual(750, attachment.ShowDelay);
        Assert.AreEqual(100, attachment.HideDelay);
    }

    [TestMethod]
    public void TooltipAttachment_BindContent_ShouldUpdateOnBinding()
    {
        // Arrange
        var attachment = new TooltipAttachment();
        var content = "Initial";

        // Act
        attachment.BindContent(nameof(content), () => content);

        // Assert - Initial value should be set
        Assert.AreEqual("Initial", attachment.Content);

        // Update and rebind
        content = "Updated";
        attachment.UpdateBindings();

        // Assert - Value should be updated
        Assert.AreEqual("Updated", attachment.Content);
    }

    [TestMethod]
    public void TooltipAttachment_BindPlacement_ShouldUpdateOnBinding()
    {
        // Arrange
        var attachment = new TooltipAttachment();
        var placement = TooltipPlacement.Top;

        // Act
        attachment.BindPlacement(nameof(placement), () => placement);

        // Assert
        Assert.AreEqual(TooltipPlacement.Top, attachment.Placement);

        // Update and rebind
        placement = TooltipPlacement.Left;
        attachment.UpdateBindings();

        // Assert
        Assert.AreEqual(TooltipPlacement.Left, attachment.Placement);
    }

    [TestMethod]
    public void TooltipAttachment_BindShowDelay_ShouldUpdateOnBinding()
    {
        // Arrange
        var attachment = new TooltipAttachment();
        var showDelay = 300;

        // Act
        attachment.BindShowDelay(nameof(showDelay), () => showDelay);

        // Assert
        Assert.AreEqual(300, attachment.ShowDelay);

        // Update and rebind
        showDelay = 600;
        attachment.UpdateBindings();

        // Assert
        Assert.AreEqual(600, attachment.ShowDelay);
    }

    [TestMethod]
    public void TooltipAttachment_BindHideDelay_ShouldUpdateOnBinding()
    {
        // Arrange
        var attachment = new TooltipAttachment();
        var hideDelay = 50;

        // Act
        attachment.BindHideDelay(nameof(hideDelay), () => hideDelay);

        // Assert
        Assert.AreEqual(50, attachment.HideDelay);

        // Update and rebind
        hideDelay = 150;
        attachment.UpdateBindings();

        // Assert
        Assert.AreEqual(150, attachment.HideDelay);
    }

    #endregion

    #region TooltipExtensions Tests

    [TestMethod]
    public void SetTooltip_StringContent_ShouldAttachTooltipToElement()
    {
        // Arrange
        var button = new Button();

        // Act
        button.SetTooltip("Button tooltip");

        // Assert
        Assert.IsNotNull(button.Tooltip);
        Assert.AreEqual("Button tooltip", button.Tooltip.Content);
    }

    [TestMethod]
    public void SetTooltip_UiElementContent_ShouldAttachTooltipToElement()
    {
        // Arrange
        var button = new Button();
        var tooltipContent = new Label().SetText("Rich tooltip");

        // Act
        button.SetTooltip(tooltipContent);

        // Assert
        Assert.IsNotNull(button.Tooltip);
        Assert.AreSame(tooltipContent, button.Tooltip.Content);
    }

    [TestMethod]
    public void SetTooltip_ActionBuilder_ShouldConfigureTooltip()
    {
        // Arrange
        var button = new Button();

        // Act
        button.SetTooltip(t => t
            .SetContent("Configured tooltip")
            .SetPlacement(TooltipPlacement.Bottom)
            .SetShowDelay(1000));

        // Assert
        Assert.IsNotNull(button.Tooltip);
        Assert.AreEqual("Configured tooltip", button.Tooltip.Content);
        Assert.AreEqual(TooltipPlacement.Bottom, button.Tooltip.Placement);
        Assert.AreEqual(1000, button.Tooltip.ShowDelay);
    }

    [TestMethod]
    public void SetTooltipPlacement_ShouldSetPlacement()
    {
        // Arrange
        var button = new Button();

        // Act
        button.SetTooltip("Test").SetTooltipPlacement(TooltipPlacement.Left);

        // Assert
        Assert.AreEqual(TooltipPlacement.Left, button.Tooltip?.Placement);
    }

    [TestMethod]
    public void SetTooltipShowDelay_ShouldSetShowDelay()
    {
        // Arrange
        var button = new Button();

        // Act
        button.SetTooltip("Test").SetTooltipShowDelay(800);

        // Assert
        Assert.AreEqual(800, button.Tooltip?.ShowDelay);
    }

    [TestMethod]
    public void SetTooltipHideDelay_ShouldSetHideDelay()
    {
        // Arrange
        var button = new Button();

        // Act
        button.SetTooltip("Test").SetTooltipHideDelay(250);

        // Assert
        Assert.AreEqual(250, button.Tooltip?.HideDelay);
    }

    [TestMethod]
    public void BindTooltipContent_ShouldBindContent()
    {
        // Arrange
        var button = new Button();
        var tooltipText = "Dynamic tooltip";

        // Act
        button.BindTooltipContent(nameof(tooltipText), () => tooltipText);

        // Assert
        Assert.AreEqual("Dynamic tooltip", button.Tooltip?.Content);
    }

    [TestMethod]
    public void BindTooltipPlacement_ShouldBindPlacement()
    {
        // Arrange
        var button = new Button();
        var placement = TooltipPlacement.Right;

        // Act
        button.BindTooltipPlacement(nameof(placement), () => placement);

        // Assert
        Assert.AreEqual(TooltipPlacement.Right, button.Tooltip?.Placement);
    }

    [TestMethod]
    public void BindTooltipShowDelay_ShouldBindShowDelay()
    {
        // Arrange
        var button = new Button();
        var showDelay = 400;

        // Act
        button.BindTooltipShowDelay(nameof(showDelay), () => showDelay);

        // Assert
        Assert.AreEqual(400, button.Tooltip?.ShowDelay);
    }

    [TestMethod]
    public void BindTooltipHideDelay_ShouldBindHideDelay()
    {
        // Arrange
        var button = new Button();
        var hideDelay = 75;

        // Act
        button.BindTooltipHideDelay(nameof(hideDelay), () => hideDelay);

        // Assert
        Assert.AreEqual(75, button.Tooltip?.HideDelay);
    }

    [TestMethod]
    public void SetTooltip_ExtensionMethod_ShouldReturnSameElement()
    {
        // Arrange
        var button = new Button();

        // Act
        var result = button.SetTooltip("Test");

        // Assert
        Assert.AreSame(button, result);
    }

    [TestMethod]
    public void SetTooltip_FluentChain_ShouldWork()
    {
        // Arrange & Act
        var button = new Button()
            .SetText("Click me")
            .SetTooltip("Button tooltip")
            .SetTooltipPlacement(TooltipPlacement.Top)
            .SetTooltipShowDelay(300);

        // Assert
        Assert.AreEqual("Click me", button.Text);
        Assert.IsNotNull(button.Tooltip);
        Assert.AreEqual("Button tooltip", button.Tooltip.Content);
        Assert.AreEqual(TooltipPlacement.Top, button.Tooltip.Placement);
        Assert.AreEqual(300, button.Tooltip.ShowDelay);
    }

    [TestMethod]
    public void SetTooltip_OnDifferentControls_ShouldWork()
    {
        // Arrange & Act
        var label = new Label().SetText("Label").SetTooltip("Label tooltip");
        var checkbox = new Checkbox().SetTooltip("Checkbox tooltip");
        var slider = new Slider().SetTooltip("Slider tooltip");

        // Assert
        Assert.IsNotNull(label.Tooltip);
        Assert.IsNotNull(checkbox.Tooltip);
        Assert.IsNotNull(slider.Tooltip);
        Assert.AreEqual("Label tooltip", label.Tooltip.Content);
        Assert.AreEqual("Checkbox tooltip", checkbox.Tooltip.Content);
        Assert.AreEqual("Slider tooltip", slider.Tooltip.Content);
    }

    #endregion

    #region TooltipPlacement Enum Tests

    [TestMethod]
    public void TooltipPlacement_Auto_ShouldHaveValueZero()
    {
        // Assert
        Assert.AreEqual(0, (int)TooltipPlacement.Auto);
    }

    [TestMethod]
    public void TooltipPlacement_AllValues_ShouldBeDefined()
    {
        // Assert
        Assert.IsTrue(Enum.IsDefined(typeof(TooltipPlacement), TooltipPlacement.Auto));
        Assert.IsTrue(Enum.IsDefined(typeof(TooltipPlacement), TooltipPlacement.Top));
        Assert.IsTrue(Enum.IsDefined(typeof(TooltipPlacement), TooltipPlacement.Bottom));
        Assert.IsTrue(Enum.IsDefined(typeof(TooltipPlacement), TooltipPlacement.Left));
        Assert.IsTrue(Enum.IsDefined(typeof(TooltipPlacement), TooltipPlacement.Right));
    }

    #endregion
}
