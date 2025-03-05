using Microsoft.Extensions.Hosting;
using PlusUi.core;
using PlusUi.desktop;
using Sandbox.Pages.Main;
using Sandbox.Pages.Secondary;
using Sandbox.Services;

var builder = Host.CreateApplicationBuilder(args);

builder.UsePlusUi<MainPage>();
builder.StylePlusUi<DefaultStyle>();

builder.AddPage<MainPage>().WithViewModel<MainViewModel>();
builder.AddPage<SecondaryPage>().WithViewModel<SecondaryPageViewModel>();

var app = builder.Build();

app.Run();