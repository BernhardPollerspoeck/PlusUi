using FFMpegCore.Pipes;
using SkiaSharp;

namespace PlusUi.h264;
internal class SkiaSharpVideoFrame(SKBitmap bitmap) : IVideoFrame, ISkiaVideoFrame
{
    public SKBitmap Bitmap => bitmap;
    public bool IsEofFrame { get; set; }
    public void Serialize(Stream stream)
    {
        var pixelData = bitmap.GetPixelSpan();
        stream.Write(pixelData);
    }

    public async Task SerializeAsync(Stream stream, CancellationToken token)
    {
        var pixelData = bitmap.GetPixelSpan();
        await stream.WriteAsync(new ReadOnlyMemory<byte>(pixelData.ToArray()), token);
    }

    public int Width => bitmap.Width;
    public int Height => bitmap.Height;
    public string Format => "bgra";
}