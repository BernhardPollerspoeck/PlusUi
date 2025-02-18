using Microsoft.Extensions.Hosting;
using PlusUi;
using PlusUi.core;
using PlusUi.desktop;

var builder = Host.CreateApplicationBuilder(args);

builder.UsePlusUi<MainPage>();


builder.AddPage<MainPage>().WithViewModel<MainViewModel>();
builder.AddPage<SecondPage>().WithViewModel<SecondPageViewModel>();

var app = builder.Build();


app.Run();


