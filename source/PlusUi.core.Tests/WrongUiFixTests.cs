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

    private class TestProperty
    {
        public string Name { get; set; } = "";
        public string Value { get; set; } = "";
    }
}