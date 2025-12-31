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

    
}