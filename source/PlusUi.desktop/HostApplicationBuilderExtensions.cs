﻿using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using PlusUi.core;

namespace PlusUi.desktop;

public static class HostApplicationBuilderExtensions
{
    public static HostApplicationBuilder UsePlusUi<TRootPage>(
        this HostApplicationBuilder builder)
        where TRootPage : UiPageElement
    {
        builder.UsePlusUiInternal<TRootPage>();

        builder.Services.AddHostedService<WindowManager>();

        return builder;
    }
}
