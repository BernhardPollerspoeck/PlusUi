using Microsoft.Extensions.Hosting;
using PlusUi.core;
using PlusUi.desktop;
using Sandbox;
using Sandbox.Pages.Main;

var builder = Host.CreateApplicationBuilder(args);

builder.UsePlusUi<MainPage>();
builder.ConfigurePlusUiApp(new App());

var app = builder.Build();

app.Run();