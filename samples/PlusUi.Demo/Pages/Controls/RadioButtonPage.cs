using PlusUi.core;
using PlusUi.Demo.Pages.Shared;

namespace PlusUi.Demo.Pages.Controls;

public class RadioButtonPage(DemoPageViewModel vm) : DemoPage(vm)
{
    protected override string ControlName => "RadioButton";

    protected override string Description =>
        "Mutually exclusive selection. Radio buttons sharing the same Group form one logical group where only one can be selected.";

    protected override IEnumerable<UiElement> BuildSections() =>
    [
        Section("Single group (pick one)",
            new VStack()
                .SetSpacing(8)
                .AddChild(new RadioButton().SetText("Small").SetGroup("size").SetValue("S"))
                .AddChild(new RadioButton().SetText("Medium").SetGroup("size").SetValue("M").SetIsSelected(true))
                .AddChild(new RadioButton().SetText("Large").SetGroup("size").SetValue("L"))),

        Section("Independent group",
            Note("A different Group value means selections do not interfere with each other."),
            new VStack()
                .SetSpacing(8)
                .AddChild(new RadioButton().SetText("Red").SetGroup("color").SetValue("R").SetIsSelected(true))
                .AddChild(new RadioButton().SetText("Green").SetGroup("color").SetValue("G"))
                .AddChild(new RadioButton().SetText("Blue").SetGroup("color").SetValue("B"))),
    ];
}
