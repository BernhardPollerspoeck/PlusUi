﻿using PlusUi.core;
using Sandbox.Controls;
using Sandbox.Pages.ControlsGrid;
using Sandbox.Pages.Form;
using Sandbox.Pages.ScrollViewDemo;
using Sandbox.Pages.TextRendering;
using SkiaSharp;

namespace Sandbox.Pages.Main;

public class MainPage(MainPageViewModel vm) : UiPageElement(vm)
{
    protected override UiElement Build()
    {
        //SetBackgroundColor(SKColors.SlateBlue);
        return new HStack(
            new VStack(
                new HStack(
                    new Solid().SetBackgroundColor(new SKColor(0, 255, 255)),
                    new Solid().SetBackgroundColor(new SKColor(255, 0, 255)),
                    new Solid().SetBackgroundColor(new SKColor(255, 255, 0))),
                new HStack(
                    new Solid().SetBackgroundColor(new SKColor(255, 0, 0)),
                    new Solid().SetBackgroundColor(new SKColor(0, 255, 0)),
                    new Solid().SetBackgroundColor(new SKColor(0, 0, 255))),
                new HStack(
                    new Solid().SetBackgroundColor(new SKColor(255, 255, 255)),
                    new Solid().SetBackgroundColor(new SKColor(128, 128, 128)),
                    new Solid().SetBackgroundColor(new SKColor(50, 50, 50))),

                new HelloWorldControl(),
                new Label()
                    .BindText(nameof(vm.Text), () => $"The entry input is: [ {vm.Text} ]"),
                new Entry()
                    .BindText(nameof(vm.Text), () => vm.Text, txt => vm.Text = txt),
                new Entry(),

                new Label()
                    .SetText("Hit the button below to Change my color")
                    .BindTextColor(nameof(vm.Color), () => vm.Color),
                new HStack(
                    new Button()
                        .SetText("Hello World!?")
                        .SetPadding(new(10, 5))
                        .SetCommand(vm.SetColorCommand),
                    new Checkbox()
                        .BindIsChecked(nameof(vm.Checked), () => vm.Checked, isChecked => vm.Checked = isChecked)
                        .SetBackgroundColor(new SKColor(255, 0, 0)),
                    new Checkbox()
                        .BindIsChecked(nameof(vm.Checked), () => vm.Checked, isChecked => vm.Checked = isChecked)
                        .SetBackgroundColor(new SKColor(0, 255, 0)),

                new HStack(
                    new Image()
                        .SetAspect(Aspect.AspectFit)
                        .SetImageSource("plusui.png")
                        .SetDesiredWidth(300)
                        .SetDesiredHeight(250),
                    new Image()
                        .SetAspect(Aspect.AspectFit)
                        .SetImageSource("plusui.png")
                        .SetVerticalAlignment(VerticalAlignment.Bottom)
                        .SetDesiredWidth(100)
                        .SetDesiredHeight(100))))
            ,
            CreateTestButtons()
                    );
    }

    protected override void ConfigurePageStyles(Style pageStyle)
    {
        pageStyle.AddStyle<UiPageElement>(element 
            => element.SetBackgroundColor(new SKColor(0, 0, 0, 220)));
    }

    private VStack CreateTestButtons()
    {
        return new VStack(
            new Button()
                .SetText("Go to Grid")
                .SetPadding(new(20, 5))
                .SetCommand(vm.NavigateCommand)
                .SetCommandParameter(typeof(ControlsGridPage)),
            new Solid().SetDesiredHeight(10).IgnoreStyling(),
            new Button()
                .SetText("Go to TextRendering")
                .SetPadding(new(20, 5))
                .SetCommand(vm.NavigateCommand)
                .SetCommandParameter(typeof(TextRenderPage)),
            new Solid().SetDesiredHeight(10).IgnoreStyling(),
            new Button()
                .SetText("Go to Form")
                .SetPadding(new(20, 5))
                .SetCommand(vm.NavigateCommand)
                .SetCommandParameter(typeof(FormDemoPage)),
            new Solid().SetDesiredHeight(10).IgnoreStyling(),
            new Button()
                .SetText("Popup")
                .SetPadding(new(20, 5))
                .SetCommand(vm.PopupCommand),
            new Solid().SetDesiredHeight(10).IgnoreStyling(),
            new Button()
                .SetText("ScrollView Example")
                .SetPadding(new(20, 5))
                .SetCommand(vm.NavigateCommand)
                .SetCommandParameter(typeof(ScrollViewExamplePage))

        );
    }

    public override void Appearing()
    {
        base.Appearing();
        vm.SetColorCommand.Execute(null);
    }
}






