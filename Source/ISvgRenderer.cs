// todo: add license

using System.Drawing;
using System.Drawing.Drawing2D;

namespace Svg
{
    public interface ISvgRenderer : IDisposable
    {
        float DpiY { get; }

        void DrawImage(
          Image image,
          RectangleF destRect,
          RectangleF srcRect,
          GraphicsUnit graphicsUnit);

        void DrawImageUnscaled(Image image, Point location);

        void DrawPath(Pen pen, GraphicsPath path);

        void FillPath(Brush brush, GraphicsPath path);

        ISvgBoundable GetBoundable();

        Region GetClip();

        ISvgBoundable PopBoundable();

        void RotateTransform(float fAngle, MatrixOrder order = (MatrixOrder)1);

        void ScaleTransform(float sx, float sy, MatrixOrder order = (MatrixOrder)1);

        void SetBoundable(ISvgBoundable boundable);

        void SetClip(Region region, CombineMode combineMode = 0);

        SmoothingMode SmoothingMode { get; set; }

        Matrix Transform { get; set; }

        void TranslateTransform(float dx, float dy, MatrixOrder order = (MatrixOrder)1);

        void DrawImage(
          Image image,
          RectangleF destRect,
          RectangleF srcRect,
          GraphicsUnit graphicsUnit,
          float opacity);
    }
}
