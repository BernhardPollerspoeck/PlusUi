using PlusUi.desktop;
using PlusUi.Demo;

var app = new PlusUiApp(args);

app.CreateApp(builder =>
{
    return new App();
});