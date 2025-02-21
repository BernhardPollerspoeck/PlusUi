using PlusUi.core;
using SkiaSharp;

namespace PlusUi;

public class DefaultStyle : IApplicationStyle
{
    public void ConfigureStyle(Style style)
    {
        style
            .AddStyle<Label>(element => element
                .SetTextColor(SKColors.Red))

            .AddStyle<Label>(Theme.Light, element => element
                .SetTextSize(30))
            .AddStyle<Label>(Theme.Dark, element => element
                .SetTextSize(40))

            .AddStyle<Label>("Blue", element => element
                .SetTextSize(50));

    }
}