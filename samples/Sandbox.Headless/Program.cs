using PlusUi.Headless;
using PlusUi.Headless.Enumerations;
using Sandbox;

Console.WriteLine("Starting Headless Sandbox Demo...");

// Create headless instance with Sandbox app
using var headless = PlusUiHeadless.Create(new App(), ImageFormat.Png);

// Wait a bit for initial rendering
await Task.Delay(100);

// 1. Initial screenshot
Console.WriteLine("Capturing initial screenshot...");
var frame = await headless.GetCurrentFrameAsync();
await File.WriteAllBytesAsync("screenshot_01_initial.png", frame);
Console.WriteLine("  -> screenshot_01_initial.png");

// 2. Click in Entry
// TODO: Determine exact position for Entry control
Console.WriteLine("\nClicking in Entry field...");
float entryX = 400; // TODO: Update with actual position
float entryY = 300; // TODO: Update with actual position
headless.MouseMove(entryX, entryY);
headless.MouseDown();
headless.MouseUp();
await Task.Delay(50);

// 3. Type "Hello World"
Console.WriteLine("Typing 'Hello World'...");
string textToType = "Hello World";
foreach (char c in textToType)
{
    headless.CharInput(c);
    await Task.Delay(10); // Small delay between characters
}
await Task.Delay(50);

frame = await headless.GetCurrentFrameAsync();
await File.WriteAllBytesAsync("screenshot_02_text_entered.png", frame);
Console.WriteLine("  -> screenshot_02_text_entered.png");

// 4. Click Checkbox
// TODO: Determine exact position for Checkbox control
Console.WriteLine("\nClicking Checkbox...");
float checkboxX = 400; // TODO: Update with actual position
float checkboxY = 400; // TODO: Update with actual position
headless.MouseMove(checkboxX, checkboxY);
headless.MouseDown();
headless.MouseUp();
await Task.Delay(50);

frame = await headless.GetCurrentFrameAsync();
await File.WriteAllBytesAsync("screenshot_03_checkbox_clicked.png", frame);
Console.WriteLine("  -> screenshot_03_checkbox_clicked.png");

// 5. Click Button
// TODO: Determine exact position for Button control
Console.WriteLine("\nClicking Button...");
float buttonX = 400; // TODO: Update with actual position
float buttonY = 500; // TODO: Update with actual position
headless.MouseMove(buttonX, buttonY);
headless.MouseDown();
headless.MouseUp();
await Task.Delay(50);

frame = await headless.GetCurrentFrameAsync();
await File.WriteAllBytesAsync("screenshot_04_button_clicked.png", frame);
Console.WriteLine("  -> screenshot_04_button_clicked.png");

Console.WriteLine("\nHeadless Sandbox Demo completed!");
Console.WriteLine("Screenshots saved to current directory.");
