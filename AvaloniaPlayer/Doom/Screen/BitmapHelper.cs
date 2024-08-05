using Avalonia.Media;
using Avalonia.Platform;
using Avalonia.Rendering.SceneGraph;
using Avalonia.Skia;
using InteropDoom;
using SkiaSharp;

namespace AvaloniaPlayer.Doom.Screen;
internal static class BitmapHelper
{
    private record SKBitmapDrawOp(SKBitmap Bitmap) : ICustomDrawOperation
    {
        public Rect Bounds { get; set; }

        public void Dispose() { }
        public bool Equals(ICustomDrawOperation? other) => false;
        public bool HitTest(Point p) => Bounds.Contains(p);
        public void Render(ImmediateDrawingContext context)
        {
            if (Bitmap is not { IsNull: false }) return;

            if (context.PlatformImpl.GetFeature<ISkiaSharpApiLeaseFeature>() is { } leaseFeature
                && leaseFeature.Lease() is { } lease)
            {
                using (lease)
                    lease.SkCanvas.DrawBitmap(Bitmap, Bounds.ToSKRect());
            }
        }
    }

    private class BitmapHolder : IImage, IDisposable
    {
        private readonly SKBitmap _bitmap;
        private readonly SKBitmapDrawOp _op;

        public BitmapHolder(ScreenBuffer screen)
        {
            _bitmap = new();
            _bitmap.InstallPixels(new(screen.Width, screen.Height, SKColorType.Bgra8888, SKAlphaType.Opaque), screen.Buffer);
            _op = new(_bitmap);
        }

        public PixelSize PixelSize => new(_bitmap.Info.Width, _bitmap.Info.Height);
        public Size Size => new(_bitmap.Info.Size.Width, _bitmap.Info.Size.Height);

        public void Dispose() { }

        public void Draw(DrawingContext context, Rect sourceRect, Rect destRect)
        {
            _op.Bounds = destRect;
            context.Custom(_op);
        }
    }

    public static IImage ToAvalonia(this ScreenBuffer screen)
        => new BitmapHolder(screen);
}
