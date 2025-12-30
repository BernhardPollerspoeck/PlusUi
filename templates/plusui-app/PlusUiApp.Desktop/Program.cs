using PlusUi.desktop;
using PlusUiApp;

var app = new PlusUiApp(args);

app.CreateApp(builder =>
{
    return new App();
});
