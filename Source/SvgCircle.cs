// todo: add license

using System.Drawing;
using System.Drawing.Drawing2D;

namespace Svg
{
    [SvgElement("circle")]
    public class SvgCircle : SvgPathBasedElement
    {
        private SvgUnit _centerX = (SvgUnit)0.0f;
        private SvgUnit _centerY = (SvgUnit)0.0f;
        private SvgUnit _radius = (SvgUnit)0.0f;
        private GraphicsPath _path;

        public SvgPoint Center => new SvgPoint(CenterX, CenterY);

        [SvgAttribute("cx")]
        public virtual SvgUnit CenterX
        {
            get => _centerX;
            set
            {
                _centerX = value;
                Attributes["cx"] = value;
                IsPathDirty = true;
            }
        }

        [SvgAttribute("cy")]
        public virtual SvgUnit CenterY
        {
            get => _centerY;
            set
            {
                _centerY = value;
                Attributes["cy"] = value;
                IsPathDirty = true;
            }
        }

        [SvgAttribute("r")]
        public virtual SvgUnit Radius
        {
            get => _radius;
            set
            {
                _radius = value;
                Attributes["r"] = value;
                IsPathDirty = true;
            }
        }

        public override GraphicsPath Path(ISvgRenderer renderer)
        {
            if (_path == null || IsPathDirty)
            {
                var num1 = (float)StrokeWidth / 2f;
                if (renderer != null)
                {
                    num1 = 0.0f;
                    IsPathDirty = false;
                }
                _path = new GraphicsPath();
                _path.StartFigure();
                PointF deviceValue = Center.ToDeviceValue(renderer, this);
                var num2 = Radius.ToDeviceValue(renderer, UnitRenderingType.Other, this) + num1;
                _path.AddEllipse(deviceValue.X - num2, deviceValue.Y - num2, 2f * num2, 2f * num2);
                _path.CloseFigure();
            }
            return _path;
        }

        protected override void Render(ISvgRenderer renderer)
        {
            if ((double)Radius.Value <= 0.0)
            {
                return;
            }

            base.Render(renderer);
        }

        public override SvgElement DeepCopy()
        {
            return DeepCopy<SvgCircle>();
        }

        public override SvgElement DeepCopy<T>()
        {
            SvgCircle svgCircle = base.DeepCopy<T>() as SvgCircle;
            svgCircle.CenterX = CenterX;
            svgCircle.CenterY = CenterY;
            svgCircle.Radius = Radius;
            return svgCircle;
        }
    }
}
