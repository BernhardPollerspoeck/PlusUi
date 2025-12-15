using Microsoft.VisualStudio.TestTools.UnitTesting;
using PlusUi.core;
using SkiaSharp;

namespace UiPlus.core.Tests;

[TestClass]
public class TimePickerTests
{
    #region SelectedTime Tests

    [TestMethod]
    public void TimePicker_SetSelectedTime_ShouldSetProperty()
    {
        // Arrange
        var timePicker = new TimePicker();
        var time = new TimeOnly(14, 30);

        // Act
        timePicker.SetSelectedTime(time);

        // Assert
        Assert.AreEqual(time, timePicker.SelectedTime);
    }

    [TestMethod]
    public void TimePicker_SetSelectedTime_Null_ShouldSetNull()
    {
        // Arrange
        var timePicker = new TimePicker();
        timePicker.SetSelectedTime(new TimeOnly(9, 0));

        // Act
        timePicker.SetSelectedTime(null);

        // Assert
        Assert.IsNull(timePicker.SelectedTime);
    }

    [TestMethod]
    public void TimePicker_BindSelectedTime_ShouldBindProperty()
    {
        // Arrange
        var timePicker = new TimePicker();
        var time = new TimeOnly(10, 15);

        // Act
        timePicker.BindSelectedTime(nameof(time), () => time);

        // Assert
        Assert.AreEqual(time, timePicker.SelectedTime);
    }

    [TestMethod]
    public void TimePicker_BindSelectedTime_TwoWay_ShouldUpdateOnBinding()
    {
        // Arrange
        var timePicker = new TimePicker();
        TimeOnly? boundTime = new TimeOnly(16, 45);

        // Act
        timePicker.BindSelectedTime(nameof(boundTime), () => boundTime, v => boundTime = v);

        // Assert
        Assert.AreEqual(new TimeOnly(16, 45), timePicker.SelectedTime);
    }

    #endregion

    #region MinuteIncrement Tests

    [TestMethod]
    public void TimePicker_DefaultMinuteIncrement_ShouldBe1()
    {
        // Arrange & Act
        var timePicker = new TimePicker();

        // Assert
        Assert.AreEqual(1, timePicker.MinuteIncrement);
    }

    [TestMethod]
    public void TimePicker_SetMinuteIncrement_15_ShouldSetProperty()
    {
        // Arrange
        var timePicker = new TimePicker();

        // Act
        timePicker.SetMinuteIncrement(15);

        // Assert
        Assert.AreEqual(15, timePicker.MinuteIncrement);
    }

    [TestMethod]
    public void TimePicker_SetMinuteIncrement_30_ShouldSetProperty()
    {
        // Arrange
        var timePicker = new TimePicker();

        // Act
        timePicker.SetMinuteIncrement(30);

        // Assert
        Assert.AreEqual(30, timePicker.MinuteIncrement);
    }

    [TestMethod]
    public void TimePicker_SetMinuteIncrement_InvalidValue_ShouldDefaultTo1()
    {
        // Arrange
        var timePicker = new TimePicker();

        // Act
        timePicker.SetMinuteIncrement(7);

        // Assert
        Assert.AreEqual(1, timePicker.MinuteIncrement);
    }

    [TestMethod]
    public void TimePicker_SetSelectedTime_ShouldRoundToIncrement()
    {
        // Arrange
        var timePicker = new TimePicker();
        timePicker.SetMinuteIncrement(15);

        // Act
        timePicker.SetSelectedTime(new TimeOnly(9, 17));

        // Assert - Should round 17 to 15
        Assert.AreEqual(new TimeOnly(9, 15), timePicker.SelectedTime);
    }

    [TestMethod]
    public void TimePicker_GetAvailableMinutes_Increment15_ShouldReturn4Values()
    {
        // Arrange
        var timePicker = new TimePicker();
        timePicker.SetMinuteIncrement(15);

        // Act
        var minutes = timePicker.GetAvailableMinutes().ToList();

        // Assert
        Assert.AreEqual(4, minutes.Count);
        CollectionAssert.AreEqual(new[] { 0, 15, 30, 45 }, minutes);
    }

    [TestMethod]
    public void TimePicker_GetAvailableMinutes_Increment30_ShouldReturn2Values()
    {
        // Arrange
        var timePicker = new TimePicker();
        timePicker.SetMinuteIncrement(30);

        // Act
        var minutes = timePicker.GetAvailableMinutes().ToList();

        // Assert
        Assert.AreEqual(2, minutes.Count);
        CollectionAssert.AreEqual(new[] { 0, 30 }, minutes);
    }

    #endregion

    #region Is24HourFormat Tests

    [TestMethod]
    public void TimePicker_Default24HourFormat_ShouldBeTrue()
    {
        // Arrange & Act
        var timePicker = new TimePicker();

        // Assert
        Assert.IsTrue(timePicker.Is24HourFormat);
    }

    [TestMethod]
    public void TimePicker_Set24HourFormat_False_ShouldSetProperty()
    {
        // Arrange
        var timePicker = new TimePicker();

        // Act
        timePicker.Set24HourFormat(false);

        // Assert
        Assert.IsFalse(timePicker.Is24HourFormat);
    }

    [TestMethod]
    public void TimePicker_Set24HourFormat_False_ShouldUpdateDisplayFormat()
    {
        // Arrange
        var timePicker = new TimePicker();

        // Act
        timePicker.Set24HourFormat(false);

        // Assert
        Assert.AreEqual("hh:mm tt", timePicker.DisplayFormat);
    }

    [TestMethod]
    public void TimePicker_GetAvailableHours_24Hour_ShouldReturn24Values()
    {
        // Arrange
        var timePicker = new TimePicker();
        timePicker.Set24HourFormat(true);

        // Act
        var hours = timePicker.GetAvailableHours().ToList();

        // Assert
        Assert.AreEqual(24, hours.Count);
        Assert.AreEqual(0, hours[0]);
        Assert.AreEqual(23, hours[23]);
    }

    [TestMethod]
    public void TimePicker_GetAvailableHours_12Hour_ShouldReturn12Values()
    {
        // Arrange
        var timePicker = new TimePicker();
        timePicker.Set24HourFormat(false);

        // Act
        var hours = timePicker.GetAvailableHours().ToList();

        // Assert
        Assert.AreEqual(12, hours.Count);
        Assert.AreEqual(1, hours[0]);
        Assert.AreEqual(12, hours[11]);
    }

    #endregion

    #region MinTime/MaxTime Tests

    [TestMethod]
    public void TimePicker_SetMinTime_ShouldSetProperty()
    {
        // Arrange
        var timePicker = new TimePicker();
        var minTime = new TimeOnly(8, 0);

        // Act
        timePicker.SetMinTime(minTime);

        // Assert
        Assert.AreEqual(minTime, timePicker.MinTime);
    }

    [TestMethod]
    public void TimePicker_SetMaxTime_ShouldSetProperty()
    {
        // Arrange
        var timePicker = new TimePicker();
        var maxTime = new TimeOnly(18, 0);

        // Act
        timePicker.SetMaxTime(maxTime);

        // Assert
        Assert.AreEqual(maxTime, timePicker.MaxTime);
    }

    [TestMethod]
    public void TimePicker_IsTimeInRange_WithinRange_ShouldReturnTrue()
    {
        // Arrange
        var timePicker = new TimePicker()
            .SetMinTime(new TimeOnly(9, 0))
            .SetMaxTime(new TimeOnly(17, 0));

        // Act
        var result = timePicker.IsTimeInRange(new TimeOnly(12, 30));

        // Assert
        Assert.IsTrue(result);
    }

    [TestMethod]
    public void TimePicker_IsTimeInRange_BeforeMin_ShouldReturnFalse()
    {
        // Arrange
        var timePicker = new TimePicker()
            .SetMinTime(new TimeOnly(9, 0));

        // Act
        var result = timePicker.IsTimeInRange(new TimeOnly(8, 0));

        // Assert
        Assert.IsFalse(result);
    }

    [TestMethod]
    public void TimePicker_IsTimeInRange_AfterMax_ShouldReturnFalse()
    {
        // Arrange
        var timePicker = new TimePicker()
            .SetMaxTime(new TimeOnly(17, 0));

        // Act
        var result = timePicker.IsTimeInRange(new TimeOnly(18, 0));

        // Assert
        Assert.IsFalse(result);
    }

    #endregion

    #region DisplayFormat Tests

    [TestMethod]
    public void TimePicker_DefaultDisplayFormat_ShouldBeHHmm()
    {
        // Arrange & Act
        var timePicker = new TimePicker();

        // Assert
        Assert.AreEqual("HH:mm", timePicker.DisplayFormat);
    }

    [TestMethod]
    public void TimePicker_SetDisplayFormat_ShouldSetProperty()
    {
        // Arrange
        var timePicker = new TimePicker();

        // Act
        timePicker.SetDisplayFormat("h:mm tt");

        // Assert
        Assert.AreEqual("h:mm tt", timePicker.DisplayFormat);
    }

    #endregion

    #region Placeholder Tests

    [TestMethod]
    public void TimePicker_SetPlaceholder_ShouldSetProperty()
    {
        // Arrange
        var timePicker = new TimePicker();

        // Act
        timePicker.SetPlaceholder("Select time");

        // Assert
        Assert.AreEqual("Select time", timePicker.Placeholder);
    }

    [TestMethod]
    public void TimePicker_DefaultPlaceholder_ShouldBeNull()
    {
        // Arrange & Act
        var timePicker = new TimePicker();

        // Assert
        Assert.IsNull(timePicker.Placeholder);
    }

    #endregion

    #region IsOpen Tests

    [TestMethod]
    public void TimePicker_DefaultIsOpen_ShouldBeFalse()
    {
        // Arrange & Act
        var timePicker = new TimePicker();

        // Assert
        Assert.IsFalse(timePicker.IsOpen);
    }

    [TestMethod]
    public void TimePicker_InvokeCommand_ShouldToggleIsOpen()
    {
        // Arrange
        var timePicker = new TimePicker();

        // Act
        timePicker.InvokeCommand();

        // Assert
        Assert.IsTrue(timePicker.IsOpen);

        // Act again
        timePicker.InvokeCommand();

        // Assert
        Assert.IsFalse(timePicker.IsOpen);
    }

    #endregion

    #region Styling Tests

    [TestMethod]
    public void TimePicker_SetTextColor_ShouldSetProperty()
    {
        // Arrange
        var timePicker = new TimePicker();

        // Act
        timePicker.SetTextColor(SKColors.Green);

        // Assert
        Assert.AreEqual(SKColors.Green, timePicker.TextColor);
    }

    [TestMethod]
    public void TimePicker_SetTextSize_ShouldSetProperty()
    {
        // Arrange
        var timePicker = new TimePicker();

        // Act
        timePicker.SetTextSize(16f);

        // Assert
        Assert.AreEqual(16f, timePicker.TextSize);
    }

    [TestMethod]
    public void TimePicker_SetSelectorBackground_ShouldSetProperty()
    {
        // Arrange
        var timePicker = new TimePicker();
        var color = new SKColor(30, 30, 30);

        // Act
        timePicker.SetSelectorBackground(color);

        // Assert
        Assert.AreEqual(color, timePicker.SelectorBackground);
    }

    #endregion

    #region Method Chaining Tests

    [TestMethod]
    public void TimePicker_MethodChaining_ShouldWork()
    {
        // Arrange & Act
        var timePicker = new TimePicker()
            .SetSelectedTime(new TimeOnly(10, 30))
            .SetMinTime(new TimeOnly(8, 0))
            .SetMaxTime(new TimeOnly(18, 0))
            .SetMinuteIncrement(15)
            .Set24HourFormat(false)
            .SetPlaceholder("Pick time")
            .SetTextColor(SKColors.Cyan);

        // Assert
        Assert.AreEqual(new TimeOnly(10, 30), timePicker.SelectedTime);
        Assert.AreEqual(new TimeOnly(8, 0), timePicker.MinTime);
        Assert.AreEqual(new TimeOnly(18, 0), timePicker.MaxTime);
        Assert.AreEqual(15, timePicker.MinuteIncrement);
        Assert.IsFalse(timePicker.Is24HourFormat);
        Assert.AreEqual("Pick time", timePicker.Placeholder);
        Assert.AreEqual(SKColors.Cyan, timePicker.TextColor);
    }

    #endregion

    #region HitTest Tests

    [TestMethod]
    public void TimePicker_HitTest_InsideControl_ShouldReturnControl()
    {
        // Arrange
        var timePicker = new TimePicker();
        timePicker.Measure(new Size(200, 100), false);
        timePicker.Arrange(new Rect(10, 10, 150, 40));

        // Act
        var result = timePicker.HitTest(new Point(50, 25));

        // Assert
        Assert.AreSame(timePicker, result);
    }

    [TestMethod]
    public void TimePicker_HitTest_Outside_ShouldReturnNull()
    {
        // Arrange
        var timePicker = new TimePicker();
        timePicker.Measure(new Size(200, 100), false);
        timePicker.Arrange(new Rect(10, 10, 150, 40));

        // Act
        var result = timePicker.HitTest(new Point(300, 300));

        // Assert
        Assert.IsNull(result);
    }

    #endregion
}
