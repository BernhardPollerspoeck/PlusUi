using Microsoft.VisualStudio.TestTools.UnitTesting;
using PlusUi.core;
using SkiaSharp;

namespace PlusUi.core.Tests;

[TestClass]
public class DatePickerTests
{
    #region SelectedDate Tests

    [TestMethod]
    public void DatePicker_SetSelectedDate_ShouldSetProperty()
    {
        // Arrange
        var datePicker = new DatePicker();
        var date = new DateOnly(2024, 12, 15);

        // Act
        datePicker.SetSelectedDate(date);

        // Assert
        Assert.AreEqual(date, datePicker.SelectedDate);
    }

    [TestMethod]
    public void DatePicker_SetSelectedDate_Null_ShouldSetNull()
    {
        // Arrange
        var datePicker = new DatePicker();
        datePicker.SetSelectedDate(new DateOnly(2024, 1, 1));

        // Act
        datePicker.SetSelectedDate(null);

        // Assert
        Assert.IsNull(datePicker.SelectedDate);
    }

    [TestMethod]
    public void DatePicker_BindSelectedDate_ShouldBindProperty()
    {
        // Arrange
        var datePicker = new DatePicker();
        var date = new DateOnly(2024, 6, 15);

        // Act
        datePicker.BindSelectedDate(nameof(date), () => date);

        // Assert
        Assert.AreEqual(date, datePicker.SelectedDate);
    }

    [TestMethod]
    public void DatePicker_BindSelectedDate_TwoWay_ShouldUpdateOnBinding()
    {
        // Arrange
        var datePicker = new DatePicker();
        DateOnly? boundDate = new DateOnly(2024, 3, 20);

        // Act
        datePicker.BindSelectedDate(nameof(boundDate), () => boundDate, v => boundDate = v);

        // Assert
        Assert.AreEqual(new DateOnly(2024, 3, 20), datePicker.SelectedDate);
    }

    #endregion

    #region MinDate/MaxDate Tests

    [TestMethod]
    public void DatePicker_SetMinDate_ShouldSetProperty()
    {
        // Arrange
        var datePicker = new DatePicker();
        var minDate = new DateOnly(2020, 1, 1);

        // Act
        datePicker.SetMinDate(minDate);

        // Assert
        Assert.AreEqual(minDate, datePicker.MinDate);
    }

    [TestMethod]
    public void DatePicker_SetMaxDate_ShouldSetProperty()
    {
        // Arrange
        var datePicker = new DatePicker();
        var maxDate = new DateOnly(2025, 12, 31);

        // Act
        datePicker.SetMaxDate(maxDate);

        // Assert
        Assert.AreEqual(maxDate, datePicker.MaxDate);
    }

    [TestMethod]
    public void DatePicker_IsDateInRange_WithinRange_ShouldReturnTrue()
    {
        // Arrange
        var datePicker = new DatePicker()
            .SetMinDate(new DateOnly(2024, 1, 1))
            .SetMaxDate(new DateOnly(2024, 12, 31));

        // Act
        var result = datePicker.IsDateInRange(new DateOnly(2024, 6, 15));

        // Assert
        Assert.IsTrue(result);
    }

    [TestMethod]
    public void DatePicker_IsDateInRange_BeforeMin_ShouldReturnFalse()
    {
        // Arrange
        var datePicker = new DatePicker()
            .SetMinDate(new DateOnly(2024, 1, 1));

        // Act
        var result = datePicker.IsDateInRange(new DateOnly(2023, 12, 31));

        // Assert
        Assert.IsFalse(result);
    }

    [TestMethod]
    public void DatePicker_IsDateInRange_AfterMax_ShouldReturnFalse()
    {
        // Arrange
        var datePicker = new DatePicker()
            .SetMaxDate(new DateOnly(2024, 12, 31));

        // Act
        var result = datePicker.IsDateInRange(new DateOnly(2025, 1, 1));

        // Assert
        Assert.IsFalse(result);
    }

    [TestMethod]
    public void DatePicker_IsDateInRange_NoRestrictions_ShouldReturnTrue()
    {
        // Arrange
        var datePicker = new DatePicker();

        // Act
        var result = datePicker.IsDateInRange(new DateOnly(1900, 1, 1));

        // Assert
        Assert.IsTrue(result);
    }

    #endregion

    #region DisplayFormat Tests

    [TestMethod]
    public void DatePicker_DefaultDisplayFormat_ShouldBeDdMmYyyy()
    {
        // Arrange & Act
        var datePicker = new DatePicker();

        // Assert
        Assert.AreEqual("dd.MM.yyyy", datePicker.DisplayFormat);
    }

    [TestMethod]
    public void DatePicker_SetDisplayFormat_ShouldSetProperty()
    {
        // Arrange
        var datePicker = new DatePicker();

        // Act
        datePicker.SetDisplayFormat("yyyy-MM-dd");

        // Assert
        Assert.AreEqual("yyyy-MM-dd", datePicker.DisplayFormat);
    }

    #endregion

    #region Placeholder Tests

    [TestMethod]
    public void DatePicker_SetPlaceholder_ShouldSetProperty()
    {
        // Arrange
        var datePicker = new DatePicker();

        // Act
        datePicker.SetPlaceholder("Select a date");

        // Assert
        Assert.AreEqual("Select a date", datePicker.Placeholder);
    }

    [TestMethod]
    public void DatePicker_DefaultPlaceholder_ShouldBeNull()
    {
        // Arrange & Act
        var datePicker = new DatePicker();

        // Assert
        Assert.IsNull(datePicker.Placeholder);
    }

    #endregion

    #region WeekStart Tests

    [TestMethod]
    public void DatePicker_DefaultWeekStart_ShouldBeMonday()
    {
        // Arrange & Act
        var datePicker = new DatePicker();

        // Assert
        Assert.AreEqual(DayOfWeekStart.Monday, datePicker.WeekStart);
    }

    [TestMethod]
    public void DatePicker_SetWeekStart_Sunday_ShouldSetProperty()
    {
        // Arrange
        var datePicker = new DatePicker();

        // Act
        datePicker.SetWeekStart(DayOfWeekStart.Sunday);

        // Assert
        Assert.AreEqual(DayOfWeekStart.Sunday, datePicker.WeekStart);
    }

    #endregion

    #region ShowTodayButton Tests

    [TestMethod]
    public void DatePicker_DefaultShowTodayButton_ShouldBeTrue()
    {
        // Arrange & Act
        var datePicker = new DatePicker();

        // Assert
        Assert.IsTrue(datePicker.ShowTodayButton);
    }

    [TestMethod]
    public void DatePicker_SetShowTodayButton_False_ShouldSetProperty()
    {
        // Arrange
        var datePicker = new DatePicker();

        // Act
        datePicker.SetShowTodayButton(false);

        // Assert
        Assert.IsFalse(datePicker.ShowTodayButton);
    }

    #endregion

    #region IsOpen Tests

    [TestMethod]
    public void DatePicker_DefaultIsOpen_ShouldBeFalse()
    {
        // Arrange & Act
        var datePicker = new DatePicker();

        // Assert
        Assert.IsFalse(datePicker.IsOpen);
    }

    [TestMethod]
    public void DatePicker_InvokeCommand_ShouldToggleIsOpen()
    {
        // Arrange
        var datePicker = new DatePicker();

        // Act
        datePicker.InvokeCommand();

        // Assert
        Assert.IsTrue(datePicker.IsOpen);

        // Act again
        datePicker.InvokeCommand();

        // Assert
        Assert.IsFalse(datePicker.IsOpen);
    }

    #endregion

    #region Styling Tests

    [TestMethod]
    public void DatePicker_SetTextColor_ShouldSetProperty()
    {
        // Arrange
        var datePicker = new DatePicker();

        // Act
        datePicker.SetTextColor(SKColors.Red);

        // Assert
        Assert.AreEqual(SKColors.Red, datePicker.TextColor);
    }

    [TestMethod]
    public void DatePicker_SetTextSize_ShouldSetProperty()
    {
        // Arrange
        var datePicker = new DatePicker();

        // Act
        datePicker.SetTextSize(18f);

        // Assert
        Assert.AreEqual(18f, datePicker.TextSize);
    }

    [TestMethod]
    public void DatePicker_SetCalendarBackground_ShouldSetProperty()
    {
        // Arrange
        var datePicker = new DatePicker();
        var color = new SKColor(50, 50, 50);

        // Act
        datePicker.SetCalendarBackground(color);

        // Assert
        Assert.AreEqual(color, datePicker.CalendarBackground);
    }

    #endregion

    #region Method Chaining Tests

    [TestMethod]
    public void DatePicker_MethodChaining_ShouldWork()
    {
        // Arrange & Act
        var datePicker = new DatePicker()
            .SetSelectedDate(new DateOnly(2024, 12, 25))
            .SetMinDate(new DateOnly(2024, 1, 1))
            .SetMaxDate(new DateOnly(2024, 12, 31))
            .SetDisplayFormat("yyyy-MM-dd")
            .SetPlaceholder("Pick a date")
            .SetWeekStart(DayOfWeekStart.Sunday)
            .SetShowTodayButton(false)
            .SetTextColor(SKColors.Blue);

        // Assert
        Assert.AreEqual(new DateOnly(2024, 12, 25), datePicker.SelectedDate);
        Assert.AreEqual(new DateOnly(2024, 1, 1), datePicker.MinDate);
        Assert.AreEqual(new DateOnly(2024, 12, 31), datePicker.MaxDate);
        Assert.AreEqual("yyyy-MM-dd", datePicker.DisplayFormat);
        Assert.AreEqual("Pick a date", datePicker.Placeholder);
        Assert.AreEqual(DayOfWeekStart.Sunday, datePicker.WeekStart);
        Assert.IsFalse(datePicker.ShowTodayButton);
        Assert.AreEqual(SKColors.Blue, datePicker.TextColor);
    }

    #endregion

    #region HitTest Tests

    [TestMethod]
    public void DatePicker_HitTest_InsideControl_ShouldReturnControl()
    {
        // Arrange
        var datePicker = new DatePicker();
        datePicker.Measure(new Size(300, 100), false);
        datePicker.Arrange(new Rect(10, 10, 200, 40));

        // Act
        var result = datePicker.HitTest(new Point(50, 25));

        // Assert
        Assert.AreSame(datePicker, result);
    }

    [TestMethod]
    public void DatePicker_HitTest_Outside_ShouldReturnNull()
    {
        // Arrange
        var datePicker = new DatePicker();
        datePicker.Measure(new Size(300, 100), false);
        datePicker.Arrange(new Rect(10, 10, 200, 40));

        // Act
        var result = datePicker.HitTest(new Point(300, 300));

        // Assert
        Assert.IsNull(result);
    }

    #endregion

    #region DayOfWeekStart Enum Tests

    [TestMethod]
    public void DayOfWeekStart_Sunday_ShouldHaveValueZero()
    {
        // Assert
        Assert.AreEqual(0, (int)DayOfWeekStart.Sunday);
    }

    [TestMethod]
    public void DayOfWeekStart_Monday_ShouldHaveValueOne()
    {
        // Assert
        Assert.AreEqual(1, (int)DayOfWeekStart.Monday);
    }

    #endregion
}
