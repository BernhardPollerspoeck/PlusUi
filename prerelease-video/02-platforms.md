# 2. Platforms

## Supported Platforms (8)

- Windows
- macOS
- Linux
- Web (Blazor WebAssembly)
- iOS
- Android
- Headless (server-side rendering, API image generation)
- Video Export (H264/MP4)

**Narration:**

> "So, where does PlusUi actually run?
>
> **Windows** - obviously. Full OpenGL rendering, smooth performance, everything you'd expect.
>
> **macOS and Linux** - and yes, that's actually worth mentioning for a .NET UI framework. No weird workarounds, no 'partial support', no 'works but looks different'. Same rendering engine, same pixel-perfect result.
>
> **Web** - your app runs in the browser via Blazor WebAssembly. Your entire UI compiled to WebAssembly, running client-side. No server round-trips for rendering.
>
> **iOS and Android** - native apps, same codebase. Your users get your design, not whatever the platform decides your button should look like today.
>
> But here's where it gets interesting.
>
> **Headless mode**. No window. No screen. Just your UI rendering in memory. Put it behind an API and suddenly you have a dynamic image generator. Build invoice templates, social media cards, certificates, personalized graphics - whatever you can design, you can render on demand. Use the same components you already have in your app. Or use it for automated screenshot testing in your CI pipeline. Your UI as a service - get creative.
>
> And finally - **Video Export**. This one's unusual. You can render your entire application directly to an H264 MP4 file. Frame by frame. With audio sync. Create product demos from your actual running code. Generate tutorial videos programmatically. Build animated marketing content. Your app becomes a video production tool.
>
> In fact - the visuals you're watching right now? Made with PlusUi. The code is in a branch in the repo if you want to see how it's done.
>
> Eight platforms. Your design - not the platform's interpretation of it."

## Visuals

- Dunkler Hintergrund (#1E1E1E)
- 3x3 Grid Layout:

```
┌─────────┬─────────┬─────────┐
│ Windows │  macOS  │  Linux  │
├─────────┼─────────┼─────────┤
│   Web   │   iOS   │ Android │
├─────────┼─────────┼─────────┤
│Headless │  Video  │ Sprecher│
└─────────┴─────────┴─────────┘
```

- Squares erscheinen nacheinander in Narration-Reihenfolge
- Platform-Icon + Name pro Square
- Sprecher rechts unten (Overlay im finalen Render)

**Übergang zu Section 3:** Slide (Page Transition)
