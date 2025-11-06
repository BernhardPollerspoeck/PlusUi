using PlusUi.Headless;
using PlusUi.Headless.Enumerations;
using Sandbox;

using var headless = PlusUiHeadless.Create(new App(), ImageFormat.Png);

var frame = await headless.GetCurrentFrameAsync();
await File.WriteAllBytesAsync("screenshot_01_initial.png", frame);

headless.MouseMove(400, 300);
headless.MouseDown();
headless.MouseUp();

foreach (char c in "Hello World")
{
    headless.CharInput(c);
}

frame = await headless.GetCurrentFrameAsync();
await File.WriteAllBytesAsync("screenshot_02_text_entered.png", frame);

headless.MouseMove(400, 400);
headless.MouseDown();
headless.MouseUp();

frame = await headless.GetCurrentFrameAsync();
await File.WriteAllBytesAsync("screenshot_03_checkbox_clicked.png", frame);

headless.MouseMove(400, 500);
headless.MouseDown();
headless.MouseUp();

frame = await headless.GetCurrentFrameAsync();
await File.WriteAllBytesAsync("screenshot_04_button_clicked.png", frame);
