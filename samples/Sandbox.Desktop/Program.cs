using PlusUi.desktop;
using Sandbox;

var app = new PlusUiApp(args);

app.CreateApp(builder =>
{
    return new App();
});

