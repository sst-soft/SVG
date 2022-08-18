// Microsoft Public License (Ms-PL). https://github.com/sst-soft/SVG which is a fork of https://github.com/svg-net/SVG.

using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.Drawing.Text;

namespace Svg
{
    public sealed class SvgRenderer : ISvgRenderer, IDisposable, IGraphicsProvider
    {
        private readonly Graphics _innerGraphics;
        private readonly bool _disposable;
        private readonly Image _image;
        private readonly Stack<ISvgBoundable> _boundables = new Stack<ISvgBoundable>();

        public void SetBoundable(ISvgBoundable boundable)
        {
            _boundables.Push(boundable);
        }

        public ISvgBoundable GetBoundable()
        {
            return _boundables.Count <= 0 ? null : _boundables.Peek();
        }

        public ISvgBoundable PopBoundable()
        {
            return _boundables.Pop();
        }

        public float DpiY => _innerGraphics.DpiY;

        private SvgRenderer(Graphics graphics, bool disposable = true)
        {
            _innerGraphics = graphics;
            _disposable = disposable;
        }

        private SvgRenderer(Graphics graphics, Image image)
          : this(graphics)
        {
            _image = image;
        }

        public void DrawImage(
          Image image,
          RectangleF destRect,
          RectangleF srcRect,
          GraphicsUnit graphicsUnit)
        {
            _innerGraphics.DrawImage(image, destRect, srcRect, graphicsUnit);
        }

        public void DrawImage(
          Image image,
          RectangleF destRect,
          RectangleF srcRect,
          GraphicsUnit graphicsUnit,
          float opacity)
        {
            using (ImageAttributes imageAttributes = new ImageAttributes())
            {
                ColorMatrix colorMatrix = new ColorMatrix()
                {
                    Matrix33 = opacity
                };
                imageAttributes.SetColorMatrix(colorMatrix, 0, (ColorAdjustType)1);
                PointF[] pointFArray = new PointF[3]
                {
          destRect.Location,
          new PointF(destRect.X + destRect.Width, destRect.Y),
          new PointF(destRect.X, destRect.Y + destRect.Height)
                };
                _innerGraphics.DrawImage(image, pointFArray, srcRect, graphicsUnit, imageAttributes);
            }
        }

        public void DrawImageUnscaled(Image image, Point location)
        {
            _innerGraphics.DrawImageUnscaled(image, location);
        }

        public void DrawPath(Pen pen, GraphicsPath path)
        {
            _innerGraphics.DrawPath(pen, path);
        }

        public void FillPath(Brush brush, GraphicsPath path)
        {
            _innerGraphics.FillPath(brush, path);
        }

        public Region GetClip()
        {
            return _innerGraphics.Clip;
        }

        public void RotateTransform(float fAngle, MatrixOrder order = (MatrixOrder)1)
        {
            _innerGraphics.RotateTransform(fAngle, order);
        }

        public void ScaleTransform(float sx, float sy, MatrixOrder order = (MatrixOrder)1)
        {
            _innerGraphics.ScaleTransform(sx, sy, order);
        }

        public void SetClip(Region region, CombineMode combineMode = 0)
        {
            _innerGraphics.SetClip(region, combineMode);
        }

        public void TranslateTransform(float dx, float dy, MatrixOrder order = (MatrixOrder)1)
        {
            _innerGraphics.TranslateTransform(dx, dy, order);
        }

        public SmoothingMode SmoothingMode
        {
            get => _innerGraphics.SmoothingMode;
            set => _innerGraphics.SmoothingMode = value;
        }

        public Matrix Transform
        {
            get => _innerGraphics.Transform;
            set => _innerGraphics.Transform = value;
        }

        public void Dispose()
        {
            if (_disposable)
            {
                _innerGraphics.Dispose();
            }

            if (_image == null)
            {
                return;
            }

            _image.Dispose();
        }

        Graphics IGraphicsProvider.GetGraphics()
        {
            return _innerGraphics;
        }

        private static Graphics CreateGraphics(Image image)
        {
            Graphics graphics = Graphics.FromImage(image);
            graphics.PixelOffsetMode = (PixelOffsetMode)4;
            graphics.CompositingQuality = (CompositingQuality)2;
            graphics.TextRenderingHint = (TextRenderingHint)4;
            graphics.TextContrast = 1;
            return graphics;
        }

        public static ISvgRenderer FromImage(Image image)
        {
            return new SvgRenderer(SvgRenderer.CreateGraphics(image));
        }

        public static ISvgRenderer FromGraphics(Graphics graphics)
        {
            return new SvgRenderer(graphics, false);
        }

        public static ISvgRenderer FromNull()
        {
            Bitmap bitmap = new Bitmap(1, 1);
            return new SvgRenderer(SvgRenderer.CreateGraphics(bitmap), bitmap);
        }
    }
}
