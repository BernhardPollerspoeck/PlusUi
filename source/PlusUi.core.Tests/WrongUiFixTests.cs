using PlusUi.core;
using SkiaSharp;

namespace PlusUi.core.Tests;


/// <summary>
/// Here are tests from fixing ui that looked wrong at some point
/// </summary>
[TestClass]
public class WrongUiFixTests
{

    [TestMethod]
    public void Test_VStackWith_Entry_Entry_Button_WrongVerticalOrder()
    {
        //Arrange
        var control = new VStack(
            new Entry()
                .SetText("Hello World !")
                .SetTextSize(50)
                .SetDesiredWidth(400)
                .SetHorizontalAlignment(HorizontalAlignment.Center)
                .SetVerticalAlignment(VerticalAlignment.Center),
            new Entry()
                .SetText("Hello World !")
                .SetTextSize(12)
                .SetDesiredWidth(400)
                .SetHorizontalAlignment(HorizontalAlignment.Center)
                .SetVerticalAlignment(VerticalAlignment.Center),
            new Button()
                .SetText("NAV")
                .SetTextSize(20));
        var availableSize = new Size(800, 600);

        //Act
        control.Measure(availableSize);
        control.Arrange(new Rect(0, 0, 800, 600));

        //Assert - Elements should be stacked vertically in order
        Assert.AreEqual(0, control.Children[0].Position.Y);
        // Second element starts after first element
        Assert.AreEqual(control.Children[0].ElementSize.Height, control.Children[1].Position.Y, 0.1);
        // Third element starts after first + second elements
        Assert.AreEqual(
            control.Children[0].ElementSize.Height + control.Children[1].ElementSize.Height,
            control.Children[2].Position.Y,
            0.1);
    }

    [TestMethod]
    public void Test_PropertyEditorPopup_DataGrid_Width_Calculation()
    {
        // Arrange - Create DataGrid separately to keep type
        var dataGrid = new DataGrid<TestProperty>();
        dataGrid
            .SetItemsSource(new[]
            {
                new TestProperty { Name = "X", Value = "10" },
                new TestProperty { Name = "Y", Value = "0" }
            })
            .SetCellPadding(new Margin(4))
            .AddColumn(new DataGridTextColumn<TestProperty>()
                .SetHeader("Property")
                .SetBinding(p => p.Name)
                .SetWidth(DataGridColumnWidth.Absolute(80)))
            .AddColumn(new DataGridEditorColumn<TestProperty>()
                .SetHeader("Value")
                .SetBinding(p => p.Value, (p, v) => p.Value = v)
                .SetWidth(DataGridColumnWidth.Absolute(200)))
            .SetMargin(new Margin(24, 0, 24, 12))
            .SetDesiredHeight(250);

        // Simulate PropertyEditorPopup layout
        var popup = new VStack(
            // Header
            new VStack(
                new Label()
                    .SetText("Edit VisualOffset")
                    .SetTextSize(18)
                    .SetFontWeight(FontWeight.SemiBold),
                new Label()
                    .SetText("Point")
                    .SetTextSize(12)
                    .SetMargin(new Margin(0, 4, 0, 0))
            ).SetMargin(new Margin(24, 24, 24, 12))
            .SetHorizontalAlignment(HorizontalAlignment.Center)
            .SetVerticalAlignment(VerticalAlignment.Center),

            dataGrid,

            // Buttons
            new HStack(
                new Button()
                    .SetText("Cancel")
                    .SetPadding(new Margin(20, 10))
                    .SetMargin(new Margin(0, 0, 12, 0))
                    .SetCornerRadius(6),
                new Button()
                    .SetText("Save Changes")
                    .SetBackground(new Color(0, 122, 255))
                    .SetPadding(new Margin(20, 10))
                    .SetCornerRadius(6)
            )
            .SetMargin(new Margin(24, 12, 24, 24))
            .SetHorizontalAlignment(HorizontalAlignment.Right)
        )
        .SetMargin(new Margin(40, 40, 40, 40));

        var availableSize = new Size(1920, 1080);

        // Act
        popup.Measure(availableSize);
        popup.Arrange(new Rect(0, 0, 1920, 1080));

        // Assert - Popup should respect desired width
        Assert.AreEqual(328, popup.ElementSize.Width, 1.0,
            $"Popup width should be 340, but was {popup.ElementSize.Width}");

        // DataGrid should fit within popup margins (340 - 48 margins = 292 available)
        var dataGridExpectedWidth = 328 - 48; // 24 left + 24 right margin
        Assert.AreEqual(dataGridExpectedWidth, dataGrid.ElementSize.Width, 1.0,
            $"DataGrid width should be {dataGridExpectedWidth}, but was {dataGrid.ElementSize.Width}");

        // Column widths should sum to: 80 (Property) + 200 (Value) = 280
        var expectedColumnWidth = 280f;
        var actualColumnWidth = dataGrid.Columns[0].ActualWidth + dataGrid.Columns[1].ActualWidth;
        Assert.AreEqual(expectedColumnWidth, actualColumnWidth, 1.0,
            $"Column widths should sum to {expectedColumnWidth}, but was {actualColumnWidth}");

        // DataGrid height should be ~250 (DesiredHeight)
        Assert.AreEqual(250, dataGrid.ElementSize.Height, 1.0,
            $"DataGrid height should be 250, but was {dataGrid.ElementSize.Height}");
    }

    [TestMethod]
    public void Test_FilterBar_BorderWithGrid_StarAndAutoColumn_Layout()
    {
        // Arrange - Recreate LogsView filter bar structure with simple, calculable sizes

        // HStack in Column 0 (Star) with label and buttons
        var label = new Label()
            .SetText("Log Level:")
            .SetTextSize(16)
            .SetMargin(new Margin(0, 4, 20, 4))
            .SetVerticalAlignment(VerticalAlignment.Center);

        var button1 = new Button()
            .SetText("Trace")
            .SetTextSize(14)
            .SetPadding(new Margin(14, 8))
            .SetMargin(new Margin(0, 4, 8, 4))
            .SetVerticalAlignment(VerticalAlignment.Center);

        var button2 = new Button()
            .SetText("Debug")
            .SetTextSize(14)
            .SetPadding(new Margin(14, 8))
            .SetMargin(new Margin(0, 4, 8, 4))
            .SetVerticalAlignment(VerticalAlignment.Center);

        var button3 = new Button()
            .SetText("Information")
            .SetTextSize(14)
            .SetPadding(new Margin(14, 8))
            .SetMargin(new Margin(0, 4, 8, 4))
            .SetVerticalAlignment(VerticalAlignment.Center);

        var button4 = new Button()
            .SetText("Warning")
            .SetTextSize(14)
            .SetPadding(new Margin(14, 8))
            .SetMargin(new Margin(0, 4, 8, 4))
            .SetVerticalAlignment(VerticalAlignment.Center);

        var button5 = new Button()
            .SetText("Error")
            .SetTextSize(14)
            .SetPadding(new Margin(14, 8))
            .SetMargin(new Margin(0, 4, 8, 4))
            .SetVerticalAlignment(VerticalAlignment.Center);

        var button6 = new Button()
            .SetText("Critical")
            .SetTextSize(14)
            .SetPadding(new Margin(14, 8))
            .SetMargin(new Margin(0, 4, 8, 4))
            .SetVerticalAlignment(VerticalAlignment.Center);

        var leftContent = new HStack(label, button1, button2, button3, button4, button5, button6)
            .SetMargin(new Margin(16));

        // Button in Column 1 (Auto) - should size to content
        var clearButton = new Button()
            .SetText("Clear Logs")
            .SetTextSize(15)
            .SetPadding(new Margin(20, 12))
            .SetMargin(new Margin(0, 0, 10, 0))
            .SetVerticalAlignment(VerticalAlignment.Center);

        // Grid with Star + Auto columns
        var grid = new Grid()
            .AddColumn(Column.Star)  // Should take remaining space
            .AddColumn(Column.Auto)  // Should size to clearButton
            .AddRow(Row.Auto)
            .SetMargin(new Margin(4))
            .SetVerticalAlignment(VerticalAlignment.Center)
            .AddChild(row: 0, column: 0, child: leftContent)
            .AddChild(row: 0, column: 1, child: clearButton);

        // Border containing the grid
        var border = new Border()
            .SetMargin(new Margin(8))
            .SetBackground(new Color(35, 35, 35))
            .AddChild(grid);

        var availableSize = new Size(1000, 200);

        // Act
        border.Measure(availableSize);
        border.Arrange(new Rect(0, 0, 1000, 200));

        // Assert
        // Border should take full available width minus its margin
        var expectedBorderWidth = 1000 - 16; // 8 left + 8 right margin
        Assert.AreEqual(expectedBorderWidth, border.ElementSize.Width, 1.0,
            $"Border width should be {expectedBorderWidth}, but was {border.ElementSize.Width}");

        // Border position should account for margin
        Assert.AreEqual(8, border.Position.X, 1.0,
            $"Border X position should be 8 (margin), but was {border.Position.X}");

        // Grid should take border width minus stroke thickness (2x) and grid margin (8)
        var expectedGridWidth = expectedBorderWidth - 2 - 8; // StrokeThickness=1 default, so 1*2=2
        Assert.AreEqual(expectedGridWidth, grid.ElementSize.Width, 1.0,
            $"Grid width should be {expectedGridWidth}, but was {grid.ElementSize.Width}");

        // Grid position: Border.X (8) + StrokeThickness (1) + Grid.Margin.Left (4) = 13
        var expectedGridX = 8 + 1 + 4;
        Assert.AreEqual(expectedGridX, grid.Position.X, 1.0,
            $"Grid X position should be {expectedGridX}, but was {grid.Position.X}");

        // Button width should auto-size to text + padding
        // NOTE: Now auto-sized, not fixed at 120
        // Assert.AreEqual(120, clearButton.ElementSize.Width, 1.0,
        //     $"Button width should be 120, but was {clearButton.ElementSize.Width}");

        // Column 0 (Star) width = Grid width - Button width (with margin)
        var expectedColumn0Width = expectedGridWidth - (clearButton.ElementSize.Width + clearButton.Margin.Right);

        // ===== OUTPUT ALL SIZES AND POSITIONS =====
        Console.WriteLine($"=== BORDER ===");
        Console.WriteLine($"Border Position: ({border.Position.X}, {border.Position.Y})");
        Console.WriteLine($"Border Size: {border.ElementSize.Width} x {border.ElementSize.Height}");
        Console.WriteLine($"Border Margin: {border.Margin.Left},{border.Margin.Top},{border.Margin.Right},{border.Margin.Bottom}");

        Console.WriteLine($"\n=== GRID ===");
        Console.WriteLine($"Grid Position: ({grid.Position.X}, {grid.Position.Y})");
        Console.WriteLine($"Grid Size: {grid.ElementSize.Width} x {grid.ElementSize.Height}");
        Console.WriteLine($"Grid Margin: {grid.Margin.Left},{grid.Margin.Top},{grid.Margin.Right},{grid.Margin.Bottom}");

        Console.WriteLine($"\n=== LEFT CONTENT (HStack) ===");
        Console.WriteLine($"HStack Position: ({leftContent.Position.X}, {leftContent.Position.Y})");
        Console.WriteLine($"HStack Size: {leftContent.ElementSize.Width} x {leftContent.ElementSize.Height}");
        Console.WriteLine($"HStack Margin: {leftContent.Margin.Left},{leftContent.Margin.Top},{leftContent.Margin.Right},{leftContent.Margin.Bottom}");

        Console.WriteLine($"\n=== ELEMENTS IN HSTACK ===");
        Console.WriteLine($"Label Position: ({label.Position.X}, {label.Position.Y})");
        Console.WriteLine($"Label Size: {label.ElementSize.Width} x {label.ElementSize.Height}");

        Console.WriteLine($"Button1 Position: ({button1.Position.X}, {button1.Position.Y})");
        Console.WriteLine($"Button1 Size: {button1.ElementSize.Width} x {button1.ElementSize.Height}");

        Console.WriteLine($"Button2 Position: ({button2.Position.X}, {button2.Position.Y})");
        Console.WriteLine($"Button2 Size: {button2.ElementSize.Width} x {button2.ElementSize.Height}");

        Console.WriteLine($"\n=== CLEAR BUTTON ===");
        Console.WriteLine($"Clear Button Position: ({clearButton.Position.X}, {clearButton.Position.Y})");
        Console.WriteLine($"Clear Button Size: {clearButton.ElementSize.Width} x {clearButton.ElementSize.Height}");

        // ===== VERIFY VERTICAL CENTERING =====
        Console.WriteLine($"\n=== VERTICAL CENTERING CHECK ===");

        // Calculate expected Y positions for centered elements in HStack
        var hstackY = leftContent.Position.Y;
        var hstackHeight = leftContent.ElementSize.Height;

        // Label (height 30) centered in HStack (height 48)
        var expectedLabelY = hstackY + (hstackHeight - label.ElementSize.Height) / 2f;
        Console.WriteLine($"Label Y: {label.Position.Y}, Expected: {expectedLabelY}, Diff: {label.Position.Y - expectedLabelY}");

        // Button1 (height 40) centered in HStack (height 48)
        var expectedButton1Y = hstackY + (hstackHeight - button1.ElementSize.Height) / 2f;
        Console.WriteLine($"Button1 Y: {button1.Position.Y}, Expected: {expectedButton1Y}, Diff: {button1.Position.Y - expectedButton1Y}");

        // Button2 (height 40) centered in HStack (height 48)
        var expectedButton2Y = hstackY + (hstackHeight - button2.ElementSize.Height) / 2f;
        Console.WriteLine($"Button2 Y: {button2.Position.Y}, Expected: {expectedButton2Y}, Diff: {button2.Position.Y - expectedButton2Y}");

        // Check vertical centers are aligned
        var labelCenterY = label.Position.Y + label.ElementSize.Height / 2f;
        var button1CenterY = button1.Position.Y + button1.ElementSize.Height / 2f;
        var button2CenterY = button2.Position.Y + button2.ElementSize.Height / 2f;

        Console.WriteLine($"\nVertical Centers:");
        Console.WriteLine($"Label Center Y: {labelCenterY}");
        Console.WriteLine($"Button1 Center Y: {button1CenterY}");
        Console.WriteLine($"Button2 Center Y: {button2CenterY}");
        Console.WriteLine($"All aligned? {Math.Abs(labelCenterY - button1CenterY) < 1 && Math.Abs(button1CenterY - button2CenterY) < 1}");

        // Assert vertical centering
        Assert.AreEqual(expectedLabelY, label.Position.Y, 1.0,
            $"Label should be centered at Y={expectedLabelY}, but was {label.Position.Y}");
        Assert.AreEqual(expectedButton1Y, button1.Position.Y, 1.0,
            $"Button1 should be centered at Y={expectedButton1Y}, but was {button1.Position.Y}");
        Assert.AreEqual(expectedButton2Y, button2.Position.Y, 1.0,
            $"Button2 should be centered at Y={expectedButton2Y}, but was {button2.Position.Y}");
    }

    private class TestProperty
    {
        public string Name { get; set; } = "";
        public string Value { get; set; } = "";
    }

    [TestMethod]
    public void Test_BorderWithLeftAlignment_SizesToContent()
    {
        // Demonstrates how to prevent a Border from stretching to fill available space:
        // Use SetHorizontalAlignment(Left) on the Border to size it to its content

        var breakdownRow1 = new HStack()
            .SetSpacing(16)
            .AddChild(new Label()
                .SetText("Measure")
                .SetTextSize(12))
            .AddChild(new Label()
                .SetText("0.00 ms")
                .SetTextSize(12));

        var breakdownRow2 = new HStack()
            .SetSpacing(16)
            .AddChild(new Label()
                .SetText("Arrange")
                .SetTextSize(12))
            .AddChild(new Label()
                .SetText("0.00 ms")
                .SetTextSize(12));

        var frameBreakdown = new Border()
            .SetBackground(new Color(45, 45, 45))
            .SetCornerRadius(8)
            .SetHorizontalAlignment(HorizontalAlignment.Left) // KEY: This sizes Border to content
            .AddChild(new VStack()
                .SetMargin(new Margin(16))
                .SetSpacing(12)
                .AddChild(new Label()
                    .SetText("Frame Breakdown")
                    .SetTextSize(14))
                .AddChild(breakdownRow1)
                .AddChild(breakdownRow2));

        var container = new VStack()
            .SetMargin(new Margin(16))
            .AddChild(frameBreakdown);

        var availableSize = new Size(1000, 600);

        // Act
        container.Measure(availableSize);
        container.Arrange(new Rect(0, 0, 1000, 600));

        // Assert - The Border should size to content because it has Left alignment
        Console.WriteLine($"Container Size: {container.ElementSize.Width} x {container.ElementSize.Height}");
        Console.WriteLine($"Frame Breakdown Border Size: {frameBreakdown.ElementSize.Width} x {frameBreakdown.ElementSize.Height}");

        // The border width should be much less than the available width
        Assert.IsTrue(frameBreakdown.ElementSize.Width < 300,
            $"Border should size to content (~200px), but was {frameBreakdown.ElementSize.Width}px. " +
            "Use SetHorizontalAlignment(Left) to prevent stretching.");

        // Container also sizes to content
        Assert.IsTrue(container.ElementSize.Width < 350,
            $"Container should size to content, but was {container.ElementSize.Width}px");
    }

    [TestMethod]
    public void Test_StretchChild_In_HStack_WithParentStretch_ShouldFillSpace()
    {
        // Arrange - When parent HAS Stretch, then children with Stretch should fill
        var breakdownRow = new HStack()
            .SetHorizontalAlignment(HorizontalAlignment.Stretch) // Parent has Stretch
            .AddChild(new Label()
                .SetText("Measure")
                .SetTextSize(12)
                .SetHorizontalAlignment(HorizontalAlignment.Stretch))
            .AddChild(new Label()
                .SetText("0.00 ms")
                .SetTextSize(12));

        var container = new VStack()
            .SetHorizontalAlignment(HorizontalAlignment.Stretch) // Also stretch
            .AddChild(breakdownRow);

        var availableSize = new Size(500, 100);

        // Act
        container.Measure(availableSize);
        container.Arrange(new Rect(0, 0, 500, 100));

        // Assert - Now it SHOULD stretch to fill available space
        Console.WriteLine($"Container Size: {container.ElementSize.Width} x {container.ElementSize.Height}");
        Console.WriteLine($"Breakdown Row Size: {breakdownRow.ElementSize.Width} x {breakdownRow.ElementSize.Height}");

        Assert.AreEqual(500, container.ElementSize.Width, 1.0,
            $"Container with Stretch should fill available width, but was {container.ElementSize.Width}");
        Assert.AreEqual(500, breakdownRow.ElementSize.Width, 1.0,
            $"HStack with Stretch should fill available width, but was {breakdownRow.ElementSize.Width}");
    }
}