using PlusUi.core;
using SkiaSharp;

namespace UiPlus.core.Tests;


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
        //Assert
        Assert.AreEqual(0, control.Children[0].Position.Y);
        Assert.AreEqual(66.50390625, control.Children[1].Position.Y);
        Assert.AreEqual(82.46484375, control.Children[2].Position.Y);
    }

    [TestMethod]
    public void Test_GridWith_FourSolid_VStackWithTwoButton_WrongColumnRowLayout()
    {
        //Arrange
        var control = new Grid()
            .AddColumn(Column.Star)
            .AddColumn(Column.Auto)
            .AddColumn(50)
            .AddRow(Row.Star)
            .AddRow(Row.Star, 2)
            .AddBoundRow(() => 20)

            .AddChild(new Solid().IgnoreStyling())
            .AddChild(column: 1, child: new Solid().IgnoreStyling())
            .AddChild(row: 1, child: new Solid().IgnoreStyling())
            .AddChild(row: 2, columnSpan: 2, child: new Solid().IgnoreStyling())
            .AddChild(row: 1, column: 2, rowSpan: 2, child: new Solid().IgnoreStyling())

            .AddChild(row: 1, column: 1, child: new VStack(
                new Button()
                    .SetPadding(new(10, 5))
                    .SetText("Increment")
                    .SetTextSize(20),
                new Button()
                    .SetPadding(new(10, 5))
                    .SetText("Back")
                    .SetTextSize(20))
            );
        var availableSize = new Size(800, 600);
        //Act
        control.Measure(availableSize);
        control.Arrange(new Rect(0, 0, 800, 600));

        //Assert
        Assert.AreEqual(0, control.Position.X);
        Assert.AreEqual(0, control.Position.Y);
        Assert.AreEqual(800, control.ElementSize.Width);
        Assert.AreEqual(600, control.ElementSize.Height);

        Assert.AreEqual(640.927734375, control.Columns[0].MeasuredSize);
        Assert.AreEqual(109.072265625, control.Columns[1].MeasuredSize);
        Assert.AreEqual(50, control.Columns[2].MeasuredSize);

        Assert.AreEqual(193.3333282470703, control.Rows[0].MeasuredSize);
        Assert.AreEqual(386.6666564941406, control.Rows[1].MeasuredSize);
        Assert.AreEqual(20, control.Rows[2].MeasuredSize);


        Assert.AreEqual(0, control.Children[0].Position.X);
        Assert.AreEqual(0, control.Children[0].Position.Y);
        //TODO: we need to differenciate between minimum size and render size
        Assert.AreEqual(640.927734375, control.Children[0].ElementSize.Width);
        Assert.AreEqual(193.3333282470703, control.Children[0].ElementSize.Height);

        Assert.AreEqual(640.927734375, control.Children[1].Position.X);
        Assert.AreEqual(0, control.Children[1].Position.Y);
        Assert.AreEqual(109.072265625, control.Children[1].ElementSize.Width);
        Assert.AreEqual(193.3333282470703, control.Children[1].ElementSize.Height);

        Assert.AreEqual(0, control.Children[2].Position.X);
        Assert.AreEqual(193.3333282470703, control.Children[2].Position.Y);
        Assert.AreEqual(640.927734375, control.Children[2].ElementSize.Width);
        Assert.AreEqual(386.6666564941406, control.Children[2].ElementSize.Height);

        Assert.AreEqual(0, control.Children[3].Position.X);
        Assert.AreEqual(580, control.Children[3].Position.Y);
        Assert.AreEqual(750, control.Children[3].ElementSize.Width);
        Assert.AreEqual(20, control.Children[3].ElementSize.Height);

        Assert.AreEqual(750, control.Children[4].Position.X);
        Assert.AreEqual(193.3333282470703, control.Children[4].Position.Y);
        Assert.AreEqual(50, control.Children[4].ElementSize.Width);
        Assert.AreEqual(406.6666564941406, control.Children[4].ElementSize.Height);

        //stack
        Assert.AreEqual(640.927734375, control.Children[5].Position.X);
        Assert.AreEqual(193.3333282470703, control.Children[5].Position.Y);
        Assert.AreEqual(109.072265625, control.Children[5].ElementSize.Width);
        Assert.AreEqual(386.6666564941406, control.Children[5].ElementSize.Height);

        var stack = control.Children[5] as VStack;
        Assert.IsNotNull(stack);

        Assert.AreEqual(640.927734375, stack.Children[0].Position.X);
        Assert.AreEqual(193.3333282470703, stack.Children[0].Position.Y);
        Assert.AreEqual(109.072265625, stack.Children[0].ElementSize.Width);
        Assert.AreEqual(36.6015625, stack.Children[0].ElementSize.Height);

        Assert.AreEqual(640.927734375, stack.Children[1].Position.X);
        Assert.AreEqual(229.9348907470703, stack.Children[1].Position.Y);
        Assert.AreEqual(60.8203125, stack.Children[1].ElementSize.Width);
        Assert.AreEqual(36.6015625, stack.Children[1].ElementSize.Height);
    }
}
