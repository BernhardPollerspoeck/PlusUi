using Microsoft.Extensions.Hosting;
using PlusUi;
using PlusUi.core.Extensions;
using Silk.NET.Windowing;

var builder = Host.CreateApplicationBuilder(args);

builder.UsePlusUi<MainPage>();


builder.AddPage<MainPage>().WithViewModel<MainViewModel>();

var app = builder.Build();


app.Run();


