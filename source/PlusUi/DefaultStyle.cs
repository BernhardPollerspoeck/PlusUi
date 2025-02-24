using PlusUi.core;
using SkiaSharp;

namespace PlusUi;

public class DefaultStyle : IApplicationStyle
{
    public void ConfigureStyle(Style style)
    {
        style
            .AddStyle<UiElement>(element => element
                .SetMargin(new(5))
                .SetCornerRadius(10))

            .AddStyle<HStack>(element => element
                .SetMargin(new(0)))
            .AddStyle<VStack>(element => element
                .SetMargin(new(0)))

            .AddStyle<Solid>(element => element
                .SetDesiredWidth(50)
                .SetDesiredHeight(50))

            .AddStyle<Entry>(element => element
                .SetPadding(new(10, 5))
                .SetTextColor(SKColors.White)
                .SetBackgroundColor(SKColors.SlateGray)
                .SetDesiredWidth(200))

            .AddStyle<Button>(element => element
                .SetBackgroundColor(SKColors.Green));

    }
}