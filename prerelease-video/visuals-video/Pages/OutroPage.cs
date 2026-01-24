using PlusUi.core;
using PrereleaseVideo.ViewModels;

public class OutroPage(OutroViewModel vm) : UiPageElement(vm)
{
    protected override UiElement Build()
    {
        return new VStack(
            new Label()
                .SetText("11. Outro")
                .SetTextSize(64)
                .SetTextColor(Colors.White)
                .SetHorizontalTextAlignment(HorizontalTextAlignment.Center)
        )
        .SetHorizontalAlignment(HorizontalAlignment.Center)
        .SetVerticalAlignment(VerticalAlignment.Center)
        .SetBackground(new SolidColorBackground(new Color(30, 30, 30)));
    }
}
