using PlusUi.Demo;
using PlusUi.Headless;
using PlusUi.Headless.Enumerations;
using System.IO;

using var headless = PlusUiHeadless.Create(new App(loadImagesSynchronously: true), ImageFormat.Png);

var frame = await headless.GetCurrentFrameAsync();
await File.WriteAllBytesAsync("screenshot.png", frame);
