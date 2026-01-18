using PlusUi.core;

namespace PlusUi.Demo.Pages.Main;

public class MainPage(MainPageViewModel vm) : UiPageElement(vm)
{
    protected override UiElement Build()
    {
        return new HStack()
            .AddChild(
                new Border()
                    .SetHorizontalAlignment(HorizontalAlignment.Left)
                    .SetBackground(PlusUiDefaults.BackgroundPrimary)
                    .SetStrokeThickness(0)
                    .AddChild(
                        new VStack()
                            .AddChild(
                                new VStack()
                                    .SetSpacing(0)
                                    .AddChild(
                                        new Label()
                                            .SetText("Controls")
                                            .SetTextSize(PlusUiDefaults.FontSizeLarge)
                                            .SetFontWeight(FontWeight.SemiBold)
                                            .SetMargin(new Margin(16, 12)))
                                    .AddChild(
                                        new Border()
                                            .SetDesiredHeight(2)
                                            .SetBackground(PlusUiDefaults.AccentPrimary)
                                            .SetStrokeThickness(0)
                                            .SetHorizontalAlignment(HorizontalAlignment.Stretch)))
                            .AddChild(
                                new ItemsList<string>()
                                    .BindItemsSource(() => vm.Controls)
                                    .SetItemTemplate((name, _) =>
                                        new Button()
                                            .SetText(name)
                                            .SetMargin(new Margin(4))
                                            .SetHorizontalAlignment(HorizontalAlignment.Stretch)))))
            .AddChild(
                new VStack()
                    .SetHorizontalAlignment(HorizontalAlignment.Center)
                    .SetVerticalAlignment(VerticalAlignment.Center)
                    .AddChild(
                        new Image()
                            .SetImageSource("plusui.png")
                            .SetDesiredHeight(100))
                    .AddChild(
                        new Label()
                            .SetText("Welcome to PlusUi"))
                    .AddChild(
                        new Label()
                            .SetText("""
                                PlusUi is a passion project born from the desire to create something truly cross-platform.
                                No more platform-specific quirks, no more "it works differently on iOS" moments.
                                Just pure, consistent UI rendering powered by SkiaSharp.

                                I started this project because I was frustrated with existing solutions.
                                MAUI was unreliable - every update broke something else, and debugging
                                platform-specific issues became a full-time job. Avalonia had its own set
                                of quirks. I wanted something that just works - everywhere, the same way, every time.

                                What you see here is the result of countless late nights, debugging sessions,
                                and "aha!" moments. Every control, every animation, every pixel is rendered
                                by our own engine. No native controls, no platform abstractions - just Skia
                                drawing directly to the screen.

                                Select a control from the sidebar to explore what PlusUi can do.
                                Each demo shows the control in action with real, working examples.
                                Feel free to play around, break things, and discover what's possible.

                                This is just the beginning. There's so much more to come.
                                """)
                            .SetTextWrapping(TextWrapping.WordWrap)));
    }
}
