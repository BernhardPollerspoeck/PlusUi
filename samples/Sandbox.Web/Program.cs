using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using PlusUi.Web;
using Sandbox;

var builder = WebAssemblyHostBuilder.CreateDefault(args);
var app = new PlusUiWebApp(builder);
await app.CreateApp(() => new App());
