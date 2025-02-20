using PlusUi.core;
using SkiaSharp;

namespace PlusUi;

public class DefaultStyle : IApplicationStyle
{
    public void ConfigureStyle(Style style)
    {
        style
            .AddStyle<Button>(element => element
                .SetBackgroundColor(SKColors.Green)
                .SetMargin(new Margin(5))
            )
            .AddStyle<Label>(element => element
                .SetTextColor(SKColors.Red));
    }
}