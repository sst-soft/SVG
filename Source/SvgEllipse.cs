// Microsoft Public License (Ms-PL). https://github.com/sst-soft/SVG which is a fork of https://github.com/svg-net/SVG.

using System.Drawing;
using System.Drawing.Drawing2D;

namespace Svg
{
    [SvgElement("ellipse")]
    public class SvgEllipse : SvgPathBasedElement
    {
        private SvgUnit _centerX = (SvgUnit)0.0f;
        private SvgUnit _centerY = (SvgUnit)0.0f;
        private SvgUnit _radiusX = (SvgUnit)0.0f;
        private SvgUnit _radiusY = (SvgUnit)0.0f;
        private GraphicsPath _path;

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

        [SvgAttribute("rx")]
        public virtual SvgUnit RadiusX
        {
            get => _radiusX;
            set
            {
                _radiusX = value;
                Attributes["rx"] = value;
                IsPathDirty = true;
            }
        }

        [SvgAttribute("ry")]
        public virtual SvgUnit RadiusY
        {
            get => _radiusY;
            set
            {
                _radiusY = value;
                Attributes["ry"] = value;
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
                PointF devicePoint = SvgUnit.GetDevicePoint(CenterX, CenterY, renderer, this);
                SvgUnit svgUnit = RadiusX;
                var num2 = svgUnit.ToDeviceValue(renderer, UnitRenderingType.Other, this) + num1;
                svgUnit = RadiusY;
                var num3 = svgUnit.ToDeviceValue(renderer, UnitRenderingType.Other, this) + num1;
                _path = new GraphicsPath();
                _path.StartFigure();
                _path.AddEllipse(devicePoint.X - num2, devicePoint.Y - num3, 2f * num2, 2f * num3);
                _path.CloseFigure();
            }
            return _path;
        }

        protected override void Render(ISvgRenderer renderer)
        {
            if ((double)RadiusX.Value <= 0.0 || (double)RadiusY.Value <= 0.0)
            {
                return;
            }

            base.Render(renderer);
        }

        public override SvgElement DeepCopy()
        {
            return DeepCopy<SvgEllipse>();
        }

        public override SvgElement DeepCopy<T>()
        {
            SvgEllipse svgEllipse = base.DeepCopy<T>() as SvgEllipse;
            svgEllipse.CenterX = CenterX;
            svgEllipse.CenterY = CenterY;
            svgEllipse.RadiusX = RadiusX;
            svgEllipse.RadiusY = RadiusY;
            return svgEllipse;
        }
    }
}
