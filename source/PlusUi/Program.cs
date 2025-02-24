using Microsoft.Extensions.Hosting;
using PlusUi;
using PlusUi.core;
using PlusUi.desktop;

var builder = Host.CreateApplicationBuilder(args);

builder.UsePlusUi<MainPage>();
builder.StylePlusUi<DefaultStyle>();

builder.AddPage<MainPage>().WithViewModel<MainViewModel>();

var app = builder.Build();

app.Run();


