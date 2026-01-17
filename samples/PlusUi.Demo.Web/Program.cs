using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using PlusUi.Demo;
using PlusUi.Web;

var builder = WebAssemblyHostBuilder.CreateDefault(args);
var app = new PlusUiWebApp(builder);
await app.CreateApp(() => new App());
