using PlusUi.core;
using SkiaSharp;

namespace Sandbox.Services;

internal class DefaultStyle : IApplicationStyle
{
    public void ConfigureStyle(Style style)
    {

        style
            .AddStyle<UiElement>(element => element
                .SetMargin(new(5))
                .SetCornerRadius(10)
                //.SetDebug()
                )

            .AddStyle<UiPageElement>(element => element
                .SetMargin(new(0)))
            .AddStyle<Border>(element => element
                .SetMargin(new(0)))
            .AddStyle<Toolbar>(element => element
                .SetMargin(new(0))
                .SetCornerRadius(0))

            .AddStyle<ScrollView>(element => element
                .SetMargin(new(0)))
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
                .SetBackground(new SolidColorBackground(SKColors.SlateGray))
                .SetDesiredWidth(200))

            .AddStyle<Button>(element => element
                .SetBackground(new SolidColorBackground(SKColors.Green)))



            ;
    }
}