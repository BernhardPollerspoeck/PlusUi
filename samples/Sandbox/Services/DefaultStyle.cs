using PlusUi.core;
using SkiaSharp;

namespace Sandbox.Services;

internal class DefaultStyle : IApplicationStyle
{
    // High Contrast colors - deutlich sichtbar
    private static readonly Color HcBackground = Colors.White;
    private static readonly Color HcForeground = Colors.Black;
    private static readonly Color HcButtonBg = Colors.Yellow;
    private static readonly Color HcButtonText = Colors.Black;
    private static readonly Color HcLinkColor = Colors.Blue;
    private static readonly Color HcInputBg = Colors.White;

    public void ConfigureStyle(Style style)
    {

        style
            .AddStyle<UiElement>(element => element
                .SetMargin(new(5))
                .SetCornerRadius(10)
                .SetHighContrastBackground(HcBackground)
                //.SetDebug()
                )

            .AddStyle<UiPageElement>(element => element
                .SetMargin(new(0))
                .SetHighContrastBackground(HcBackground))
            .AddStyle<Border>(element => element
                .SetMargin(new(0))
                .SetHighContrastBackground(HcBackground))
            .AddStyle<Toolbar>(element => element
                .SetMargin(new(0))
                .SetCornerRadius(0)
                .SetHighContrastBackground(HcButtonBg))

            .AddStyle<ScrollView>(element => element
                .SetMargin(new(0))
                .SetHighContrastBackground(HcBackground))
            .AddStyle<HStack>(element => element
                .SetMargin(new(0))
                .SetHighContrastBackground(HcBackground))
            .AddStyle<VStack>(element => element
                .SetMargin(new(0))
                .SetHighContrastBackground(HcBackground))

            .AddStyle<Solid>(element => element
                .SetDesiredWidth(50)
                .SetDesiredHeight(50))

            .AddStyle<Label>(element => element
                .SetHighContrastForeground(HcForeground))

            .AddStyle<Entry>(element => element
                .SetPadding(new(10, 5))
                .SetTextColor(Colors.White)
                .SetBackground(new SolidColorBackground(Colors.SlateGray))
                .SetDesiredWidth(200)
                .SetHighContrastBackground(HcInputBg)
                .SetHighContrastForeground(HcForeground))

            .AddStyle<Button>(element => element
                .SetBackground(new SolidColorBackground(Colors.Green))
                .SetHighContrastBackground(HcButtonBg)
                .SetHighContrastForeground(HcButtonText))

            .AddStyle<Checkbox>(element => element
                .SetHighContrastForeground(HcForeground))

            .AddStyle<Toggle>(element => element
                .SetHighContrastForeground(HcForeground))

            .AddStyle<RadioButton>(element => element
                .SetHighContrastForeground(HcForeground))

            .AddStyle<Link>(element => element
                .SetHighContrastForeground(HcLinkColor))

            .AddStyle<Slider>(element => element
                .SetHighContrastBackground(HcBackground))

            .AddStyle<DatePicker>(element => element
                .SetHighContrastBackground(HcInputBg)
                .SetHighContrastForeground(HcForeground))

            .AddStyle<TimePicker>(element => element
                .SetHighContrastBackground(HcInputBg)
                .SetHighContrastForeground(HcForeground))

            .AddStyle<ComboBox<string>>(element => element
                .SetHighContrastBackground(HcInputBg)
                .SetHighContrastForeground(HcForeground))



            ;
    }
}