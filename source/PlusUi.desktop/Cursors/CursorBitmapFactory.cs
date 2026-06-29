using System.Text;
using PlusUi.core;
using Silk.NET.Core;
using SkiaSharp;
using Svg.Skia;
using CursorType = PlusUi.core.CursorType;

namespace PlusUi.desktop.Cursors;

/// <summary>
/// Renders self-drawn cursor bitmaps with Skia and caches them as <see cref="RawImage"/> (RGBA,
/// straight alpha) ready to hand to GLFW as a custom cursor. This is the universal backstop: it
/// can produce any <see cref="CursorType"/>, so cursors the OS/GLFW lack still render.
/// </summary>
internal sealed class CursorBitmapFactory
{
    private const int Size = 32;
    private static readonly SKColor Ink = new(30, 30, 30);
    private static readonly SKColor Red = new(220, 50, 50);

    // The PlusUi brand mark (chevron + plus), embedded so the branded cursor never depends on a
    // file shipping with the consumer app. The chevron apex (100,30 in the 200x200 viewBox) is the tip.
    private const string LogoSvg = """
        <svg xmlns="http://www.w3.org/2000/svg" viewBox="0 0 200 200">
          <path d="M45 75 L100 30 L155 75" fill="none" stroke="#000" stroke-width="24" stroke-linecap="square" stroke-linejoin="miter"/>
          <path d="M45 75 L100 30 L155 75" fill="none" stroke="#2E7D32" stroke-width="12" stroke-linecap="square" stroke-linejoin="miter"/>
          <path d="M100 85 L100 175 M55 130 L145 130" stroke="#000" stroke-width="33" stroke-linecap="square"/>
          <path d="M100 85 L100 175 M55 130 L145 130" stroke="#FFFFFF" stroke-width="18" stroke-linecap="square"/>
        </svg>
        """;

    private readonly Dictionary<CursorType, CursorBitmap> _cache = [];

    public CursorBitmap Get(CursorType cursor)
    {
        if (!_cache.TryGetValue(cursor, out var bitmap))
        {
            bitmap = Render(cursor);
            _cache[cursor] = bitmap;
        }
        return bitmap;
    }

    private static CursorBitmap Render(CursorType cursor)
    {
        var info = new SKImageInfo(Size, Size, SKColorType.Rgba8888, SKAlphaType.Unpremul);
        using var bitmap = new SKBitmap(info);
        using (var canvas = new SKCanvas(bitmap))
        {
            canvas.Clear(SKColors.Transparent);
            var (hotX, hotY) = Draw(canvas, cursor);
            canvas.Flush();
            return new CursorBitmap(ToRawImage(bitmap), hotX, hotY);
        }
    }

    private static (int, int) Draw(SKCanvas canvas, CursorType cursor)
    {
        switch (cursor)
        {
            case CursorType.PlusUi:
                DrawLogo(canvas);
                return (16, 5); // chevron apex
            case CursorType.Wait:
                DrawWait(canvas);
                return (16, 16);
            case CursorType.Progress:
                DrawArrow(canvas);
                DrawBusyBadge(canvas);
                return (2, 1);
            case CursorType.NotAllowed:
                DrawNotAllowed(canvas);
                return (16, 16);
            case CursorType.Text:
                DrawIBeam(canvas);
                return (16, 16);
            case CursorType.Crosshair:
                DrawCrosshair(canvas);
                return (16, 16);
            case CursorType.ResizeHorizontal:
                DrawResize(canvas, 0);
                return (16, 16);
            case CursorType.ResizeVertical:
                DrawResize(canvas, 90);
                return (16, 16);
            case CursorType.ResizeNwse:
                DrawResize(canvas, 135);
                return (16, 16);
            case CursorType.ResizeNesw:
                DrawResize(canvas, 45);
                return (16, 16);
            case CursorType.ResizeAll:
                DrawResize(canvas, 0, 90);
                return (16, 16);
            default:
                DrawArrow(canvas);
                return (2, 1);
        }
    }

    // Classic pointer arrow with the tip at the top-left — that tip is the hotspot.
    private static void DrawArrow(SKCanvas canvas)
    {
        using var builder = new SKPathBuilder();
        builder.MoveTo(2, 1);
        builder.LineTo(2, 22);
        builder.LineTo(7.5f, 17);
        builder.LineTo(11, 26);
        builder.LineTo(14, 24.7f);
        builder.LineTo(10.5f, 16);
        builder.LineTo(17, 16);
        builder.Close();
        using var path = builder.Detach();

        using var outline = new SKPaint { Color = SKColors.White, IsStroke = true, StrokeWidth = 2.4f, IsAntialias = true, StrokeJoin = SKStrokeJoin.Round };
        using var fill = new SKPaint { Color = PlusUiDefaults.AccentPrimary, IsAntialias = true };
        canvas.DrawPath(path, outline);
        canvas.DrawPath(path, fill);
    }

    private static void DrawLogo(SKCanvas canvas)
    {
        using var svg = new SKSvg();
        using var stream = new MemoryStream(Encoding.UTF8.GetBytes(LogoSvg));
        svg.Load(stream);
        if (svg.Picture is not { } picture)
        {
            DrawArrow(canvas);
            return;
        }

        var bounds = picture.CullRect;
        var scale = Size / Math.Max(bounds.Width, bounds.Height);
        canvas.Save();
        canvas.Scale(scale);
        canvas.Translate(-bounds.Left, -bounds.Top);
        canvas.DrawPicture(picture);
        canvas.Restore();
    }

    private static void DrawWait(SKCanvas canvas)
    {
        var center = new SKPoint(16, 16);
        using var face = new SKPaint { Color = SKColors.White, IsAntialias = true };
        using var ring = new SKPaint { Color = PlusUiDefaults.AccentPrimary, IsStroke = true, StrokeWidth = 2.5f, IsAntialias = true };
        using var hands = new SKPaint { Color = PlusUiDefaults.AccentPrimary, IsStroke = true, StrokeWidth = 2f, IsAntialias = true, StrokeCap = SKStrokeCap.Round };

        canvas.DrawCircle(center, 10, face);
        canvas.DrawCircle(center, 10, ring);
        canvas.DrawLine(center.X, center.Y, center.X, center.Y - 6, hands);
        canvas.DrawLine(center.X, center.Y, center.X + 5, center.Y + 2, hands);
    }

    // Small busy ring tucked at the lower-right of the arrow (the "working in background" cursor).
    private static void DrawBusyBadge(SKCanvas canvas)
    {
        var c = new SKPoint(22, 22);
        using var face = new SKPaint { Color = SKColors.White, IsAntialias = true };
        using var ring = new SKPaint { Color = PlusUiDefaults.AccentPrimary, IsStroke = true, StrokeWidth = 2f, IsAntialias = true };
        canvas.DrawCircle(c, 7, face);
        canvas.DrawArc(new SKRect(c.X - 5, c.Y - 5, c.X + 5, c.Y + 5), -90, 270, false, ring);
    }

    private static void DrawNotAllowed(SKCanvas canvas)
    {
        var center = new SKPoint(16, 16);
        using var ring = new SKPaint { Color = Red, IsStroke = true, StrokeWidth = 3f, IsAntialias = true };
        using var slash = new SKPaint { Color = Red, IsStroke = true, StrokeWidth = 3f, IsAntialias = true, StrokeCap = SKStrokeCap.Round };
        canvas.DrawCircle(center, 10, ring);
        var d = 10 * 0.7071f;
        canvas.DrawLine(center.X - d, center.Y - d, center.X + d, center.Y + d, slash);
    }

    private static void DrawIBeam(SKCanvas canvas)
    {
        using var outline = new SKPaint { Color = SKColors.White, IsStroke = true, StrokeWidth = 4f, IsAntialias = true, StrokeCap = SKStrokeCap.Round };
        using var core = new SKPaint { Color = Ink, IsStroke = true, StrokeWidth = 2f, IsAntialias = true, StrokeCap = SKStrokeCap.Round };
        DrawIBeamPaths(canvas, outline);
        DrawIBeamPaths(canvas, core);
    }

    private static void DrawIBeamPaths(SKCanvas canvas, SKPaint paint)
    {
        canvas.DrawLine(16, 6, 16, 26, paint);
        canvas.DrawLine(12, 6, 20, 6, paint);
        canvas.DrawLine(12, 26, 20, 26, paint);
    }

    private static void DrawCrosshair(SKCanvas canvas)
    {
        using var outline = new SKPaint { Color = SKColors.White, IsStroke = true, StrokeWidth = 4f, IsAntialias = true };
        using var core = new SKPaint { Color = Ink, IsStroke = true, StrokeWidth = 1.6f, IsAntialias = true };
        foreach (var paint in new[] { outline, core })
        {
            canvas.DrawLine(16, 3, 16, 29, paint);
            canvas.DrawLine(3, 16, 29, 16, paint);
        }
    }

    // A double-headed resize arrow through the center, drawn once per supplied angle (two angles
    // produces the 4-way move cursor).
    private static void DrawResize(SKCanvas canvas, params float[] anglesDeg)
    {
        using var outline = new SKPaint { Color = SKColors.White, IsStroke = true, StrokeWidth = 4.5f, IsAntialias = true, StrokeCap = SKStrokeCap.Round, StrokeJoin = SKStrokeJoin.Round };
        using var core = new SKPaint { Color = Ink, IsStroke = true, StrokeWidth = 2f, IsAntialias = true, StrokeCap = SKStrokeCap.Round, StrokeJoin = SKStrokeJoin.Round };

        foreach (var angle in anglesDeg)
        {
            using var path = BuildDoubleArrow();
            canvas.Save();
            canvas.Translate(16, 16);
            canvas.RotateDegrees(angle);
            canvas.DrawPath(path, outline);
            canvas.DrawPath(path, core);
            canvas.Restore();
        }
    }

    private static SKPath BuildDoubleArrow()
    {
        const float len = 11f;
        const float head = 4f;
        using var builder = new SKPathBuilder();
        builder.MoveTo(-len, 0);
        builder.LineTo(len, 0);
        builder.MoveTo(-len, 0);
        builder.LineTo(-len + head, -head);
        builder.MoveTo(-len, 0);
        builder.LineTo(-len + head, head);
        builder.MoveTo(len, 0);
        builder.LineTo(len - head, -head);
        builder.MoveTo(len, 0);
        builder.LineTo(len - head, head);
        return builder.Detach();
    }

    private static RawImage ToRawImage(SKBitmap bitmap) => new(Size, Size, new Memory<byte>(bitmap.Bytes));
}

internal readonly record struct CursorBitmap(RawImage Image, int HotspotX, int HotspotY);
