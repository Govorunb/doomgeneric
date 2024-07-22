using Avalonia.Media;
using Avalonia.Platform;
using Avalonia.Rendering.SceneGraph;
using Avalonia.Skia;
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

    private class BitmapHolder(SKBitmap bitmap) : IImage, IDisposable
    {
        private readonly SKBitmapDrawOp _op = new(bitmap);
        public PixelSize PixelSize => new(bitmap.Info.Width, bitmap.Info.Height);
        public Size Size => new(bitmap.Info.Size.Width, bitmap.Info.Size.Height);

        public void Dispose() { }

        public void Draw(DrawingContext context, Rect sourceRect, Rect destRect)
        {
            _op.Bounds = destRect;
            context.Custom(_op);
        }
    }

    public static IImage ToAvalonia(this SKBitmap bitmap)
        => new BitmapHolder(bitmap);
}
