// todo: add license

using System.Drawing;
using System.Drawing.Drawing2D;
using Svg.DataTypes;

namespace Svg.FilterEffects
{
    [SvgElement("filter")]
    public sealed class SvgFilter : SvgElement
    {
        private Bitmap sourceGraphic;
        private Bitmap sourceAlpha;

        [SvgAttribute("x")]
        public SvgUnit X
        {
            get => GetAttribute<SvgUnit>("x", false);
            set => Attributes["x"] = value;
        }

        [SvgAttribute("y")]
        public SvgUnit Y
        {
            get => GetAttribute<SvgUnit>("y", false);
            set => Attributes["y"] = value;
        }

        [SvgAttribute("width")]
        public SvgUnit Width
        {
            get => GetAttribute<SvgUnit>("width", false);
            set => Attributes["width"] = value;
        }

        [SvgAttribute("height")]
        public SvgUnit Height
        {
            get => GetAttribute<SvgUnit>("height", false);
            set => Attributes["height"] = value;
        }

        [SvgAttribute("color-interpolation-filters")]
        public SvgColourInterpolation ColorInterpolationFilters
        {
            get => GetAttribute<SvgColourInterpolation>("color-interpolation-filters", false);
            set => Attributes["color-interpolation-filters"] = value;
        }

        protected override void Render(ISvgRenderer renderer)
        {
            RenderChildren(renderer);
        }

        private Matrix GetTransform(SvgVisualElement element)
        {
            Matrix transform = new Matrix();
            if (element.Transforms != null)
            {
                using (Matrix matrix = element.Transforms.GetMatrix())
                {
                    transform.Multiply(matrix);
                }
            }
            return transform;
        }

        private RectangleF GetPathBounds(
          SvgVisualElement element,
          ISvgRenderer renderer,
          Matrix transform)
        {
            RectangleF rectangleF = element is SvgGroup ? element.Path(renderer).GetBounds() : element.Bounds;
            PointF[] pointFArray = new PointF[2]
            {
        rectangleF.Location,
        new PointF(rectangleF.Right, rectangleF.Bottom)
            };
            transform.TransformPoints(pointFArray);
            return new RectangleF(Math.Min(pointFArray[0].X, pointFArray[1].X), Math.Min(pointFArray[0].Y, pointFArray[1].Y), Math.Abs(pointFArray[0].X - pointFArray[1].X), Math.Abs(pointFArray[0].Y - pointFArray[1].Y));
        }

        public void ApplyFilter(
          SvgVisualElement element,
          ISvgRenderer renderer,
          Action<ISvgRenderer> renderMethod)
        {
            var inflate = 0.5f;
            Matrix transform = GetTransform(element);
            RectangleF pathBounds = GetPathBounds(element, renderer, transform);
            if ((double)pathBounds.Width == 0.0 || (double)pathBounds.Height == 0.0)
            {
                return;
            }

            ImageBuffer buffer1 = new ImageBuffer(pathBounds, inflate, renderer, renderMethod)
            {
                Transform = transform
            };
            foreach (SvgFilterPrimitive svgFilterPrimitive in Children.OfType<SvgFilterPrimitive>())
            {
                svgFilterPrimitive.Process(buffer1);
            }

            Bitmap buffer2 = buffer1.Buffer;
            RectangleF destRect = RectangleF.Inflate(pathBounds, inflate * pathBounds.Width, inflate * pathBounds.Height);
            Region clip = renderer.GetClip();
            renderer.SetClip(new Region(destRect));
            renderer.DrawImage(buffer2, destRect, new RectangleF(pathBounds.X, pathBounds.Y, destRect.Width, destRect.Height), (GraphicsUnit)2);
            renderer.SetClip(clip);
        }

        private void ResetDefaults()
        {
            if (sourceGraphic != null)
            {
                sourceGraphic.Dispose();
                sourceGraphic = null;
            }
            if (sourceAlpha == null)
            {
                return;
            }
            sourceAlpha.Dispose();
            sourceAlpha = null;
        }

        public override SvgElement DeepCopy()
        {
            return DeepCopy<SvgFilter>();
        }

        public override SvgElement DeepCopy<T>()
        {
            SvgFilter svgFilter = base.DeepCopy<T>() as SvgFilter;
            svgFilter.Height = Height;
            svgFilter.Width = Width;
            svgFilter.X = X;
            svgFilter.Y = Y;
            svgFilter.ColorInterpolationFilters = ColorInterpolationFilters;
            return svgFilter;
        }
    }
}
