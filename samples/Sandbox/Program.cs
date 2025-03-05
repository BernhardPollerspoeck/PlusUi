using Microsoft.Extensions.Hosting;
using PlusUi.core;
using PlusUi.desktop;
using Sandbox.Pages.ControlsGrid;
using Sandbox.Pages.Main;
using Sandbox.Pages.TextRendering;
using Sandbox.Services;

var builder = Host.CreateApplicationBuilder(args);

builder.UsePlusUi<MainPage>();
builder.StylePlusUi<DefaultStyle>();

builder.AddPage<MainPage>().WithViewModel<MainPageViewModel>();
builder.AddPage<ControlsGridPage>().WithViewModel<ControlsGridPageViewModel>();
builder.AddPage<TextRenderPage>().WithViewModel<TextRenderPageViewModel>();

var app = builder.Build();

app.Run();