using Microsoft.Extensions.DependencyInjection;
using PlusUi.core.CoreElements;
using PlusUi.core.Services;
using System.ComponentModel;

namespace PlusUi.core;

/// <summary>
/// Extension methods for IPlusUiAppBuilder used in app configuration.
/// </summary>
public static class PlusUiAppBuilderExtensions
{
    public static IPlusUiAppBuilder AddPage<TPage>(this IPlusUiAppBuilder builder)
        where TPage : UiPageElement
    {
        builder.Services.AddTransient<TPage>();
        return builder;
    }

    public static IPlusUiAppBuilder WithViewModel<TViewModel>(this IPlusUiAppBuilder builder)
        where TViewModel : class, INotifyPropertyChanged
    {
        builder.Services.AddTransient<TViewModel>();
        return builder;
    }

    public static IPlusUiAppBuilder AddPopup<TPopup>(this IPlusUiAppBuilder builder)
       where TPopup : UiPopupElement
    {
        builder.Services.AddTransient<TPopup>();
        return builder;
    }

    public static IPlusUiAppBuilder StylePlusUi<TApplicationStyle>(this IPlusUiAppBuilder builder)
        where TApplicationStyle : class, IApplicationStyle
    {
        builder.Services.AddSingleton<IApplicationStyle, TApplicationStyle>();
        return builder;
    }

    public static IPlusUiAppBuilder RegisterFont(
        this IPlusUiAppBuilder builder,
        string resourcePath,
        string fontFamily,
        FontWeight fontWeight = FontWeight.Regular,
        FontStyle fontStyle = FontStyle.Normal)
    {
        // Store font registrations to be executed when the service provider is built
        builder.Services.AddSingleton<IFontRegistration>(new FontRegistration
        {
            ResourcePath = resourcePath,
            FontFamily = fontFamily,
            FontWeight = fontWeight,
            FontStyle = fontStyle
        });
        return builder;
    }
}
