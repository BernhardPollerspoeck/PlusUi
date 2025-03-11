using PlusUi.core;
using SkiaSharp;

namespace Sandbox.Pages.Form;

public class FormDemoPage(FormDemoPageViewModel vm) : UiPageElement(vm)
{
    protected override UiElement Build()
    {
        return new VStack(
            new Label()
                .SetText("Modern Form Demo")
                .SetTextSize(28)
                .SetHorizontalTextAlignment(HorizontalTextAlignment.Center)
                .SetTextColor(SKColors.White)
                .SetHorizontalAlignment(HorizontalAlignment.Center)
                .SetMargin(new Margin(0, 20, 0, 30)),

            new Grid()
                .AddColumn(Column.Star)
                .AddColumn(Column.Star)
                .AddChild(
                    // Left Column
                    new VStack(
                        CreateFormGroup("Personal Information",
                            new VStack(
                                CreateFormField("Name", vm.Name, (v) => vm.Name = v),
                                CreateFormField("Email", vm.Email, (v) => vm.Email = v),
                                CreateFormField("Phone", vm.Phone, (v) => vm.Phone = v)
                            )
                        ),
                        CreateFormGroup("Account",
                            new VStack(
                                CreateFormField("Username", vm.Username, (v) => vm.Username = v),
                                CreateFormField("Password", vm.Password, (v) => vm.Password = v)
                            )
                        )
                    ).SetHorizontalAlignment(HorizontalAlignment.Stretch))
                .AddChild(column: 1, child:
                    // Right Column
                    new VStack(
                        CreateFormGroup("Preferences",
                            new VStack(
                                new HStack(
                                    new Label()
                                        .SetText("Receive notifications")
                                        .SetTextColor(SKColors.White)
                                        .SetVerticalAlignment(VerticalAlignment.Center),
                                    new Checkbox()
                                        .BindIsChecked(nameof(vm.ReceiveNotifications),
                                            () => vm.ReceiveNotifications,
                                            (v) => vm.ReceiveNotifications = v)
                                        .SetMargin(new Margin(5, 0, 0, 0))
                                ).SetMargin(new Margin(0, 5)),
                                new HStack(
                                    new Label()
                                        .SetText("Dark mode")
                                        .SetTextColor(SKColors.White)
                                        .SetVerticalAlignment(VerticalAlignment.Center),
                                    new Checkbox()
                                        .BindIsChecked(nameof(vm.DarkMode),
                                            () => vm.DarkMode,
                                            (v) => vm.DarkMode = v)
                                        .SetMargin(new Margin(5, 0, 0, 0))
                                ).SetMargin(new Margin(0, 5))
                            )
                        ),
                        CreateFormGroup("Profile Image",
                            new VStack(
                                new Image()
                                    .SetAspect(Aspect.AspectFit)
                                    .SetImageSource("plusui.png")
                                    .SetDesiredSize(new(120, 120))
                                    .SetHorizontalAlignment(HorizontalAlignment.Center)
                                    .SetMargin(new Margin(0, 10)),
                                new Button()
                                    .SetText("Upload Image")
                                    .SetCommand(vm.UploadImageCommand)
                                    .SetPadding(new Margin(15, 8))
                                    .SetMargin(new Margin(0, 10))
                                    .SetHorizontalAlignment(HorizontalAlignment.Center)
                                    .SetCornerRadius(20)
                            )
                        ),
                        new Solid()
                            .SetMargin(new Margin(0, 20, 0, 0))
                            .SetDesiredHeight(1)
                            .SetVerticalAlignment(VerticalAlignment.Bottom)
                            .SetBackgroundColor(new SKColor(255, 255, 255, 60)),
                        new Button()
                            .SetText("Submit Form")
                            .SetCommand(vm.SubmitFormCommand)
                            .SetPadding(new Margin(30, 12))
                            .SetMargin(new Margin(0, 20))
                            .SetHorizontalAlignment(HorizontalAlignment.Center)
                            .SetBackgroundColor(new SKColor(48, 209, 185))
                            .SetCornerRadius(20)
                    ).SetHorizontalAlignment(HorizontalAlignment.Stretch)
                )
                //IDEA: .IgnoreGlobalStyling()
                //IDEA: .IgnorePageStyling()
                .SetBackgroundColor(new SKColor(85, 70, 185))
        );
    }

    private static UiElement CreateFormGroup(string title, UiElement content)
    {
        return new VStack(
            new Label()
                .SetText(title)
                .SetTextColor(new SKColor(255, 255, 255))
                .SetTextSize(18)
                .SetMargin(new Margin(0, 0, 0, 10)),
            content
                .SetMargin(new Margin(15))
                .SetBackgroundColor(new SKColor(255, 255, 255, 40))
                .SetCornerRadius(15)
        )
        .SetMargin(new Margin(0, 0, 0, 20));
    }

    private static UiElement CreateFormField(string label, string bindingValue, Action<string> setter)
    {
        return new VStack(
            new Label()
                .SetText(label)
                .SetTextColor(new SKColor(255, 255, 255))
                .SetMargin(new Margin(0, 0, 0, 5)),
            new Entry()
                .BindText(label, () => bindingValue, setter)
                .SetPadding(new Margin(10, 8))
                .SetTextColor(SKColors.White)
                .SetCornerRadius(10)
                .SetBackgroundColor(new SKColor(255, 255, 255, 30))
        ).SetMargin(new Margin(0, 0, 0, 10));
    }

    protected override void ConfigurePageStyles(Style pageStyle)
    {
        pageStyle.AddStyle<UiPageElement>(element =>
            element
                .SetBackgroundColor(new SKColor(105, 90, 205))
                .SetCornerRadius(0)
        );

        pageStyle.AddStyle<Button>(button =>
            button
                .SetTextColor(SKColors.White)
                .SetBackgroundColor(new SKColor(0, 0, 0))
        );

        pageStyle.AddStyle<Entry>(entry =>
            entry.SetCornerRadius(10)
        );

        pageStyle.AddStyle<Checkbox>(checkbox =>
            checkbox.SetBackgroundColor(new SKColor(0, 0, 0))
        );
    }
}
