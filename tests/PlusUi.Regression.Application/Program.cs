

using PlusUi.Headless;
using PlusUi.Headless.Enumerations;

using var headless = PlusUiHeadless.Create(new App(), ImageFormat.Png);

var frame = await headless.GetCurrentFrameAsync();
await File.WriteAllBytesAsync("001.png", frame);
