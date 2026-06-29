using PlusUi.core;
using PlusUi.Demo.Pages.Shared;

namespace PlusUi.Demo.Pages.Controls;

public class ImagePage(DemoPageViewModel vm) : DemoPage(vm)
{
    protected override string ControlName => "Image";

    protected override string Description =>
        "Displays raster (PNG/JPG), vector (SVG) and animated (GIF/WebP) images. Default aspect mode is AspectFit.";

    protected override IEnumerable<UiElement> BuildSections() =>
    [
        Section("Raster (PNG)",
            Demo("AspectFit, height 120",
                new Image().SetImageSource("plusui.png").SetDesiredHeight(120))),

        Section("Vector (SVG)",
            Demo("AspectFit, height 120",
                new Image().SetImageSource("plusui.svg").SetDesiredHeight(120))),

        Section("Animated (GIF)",
            Demo("Decoded per-frame and played via a frame timer",
                new Image().SetImageSource("loading.gif").SetDesiredHeight(120))),
    ];
}
