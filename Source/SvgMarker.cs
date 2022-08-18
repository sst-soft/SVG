// todo: add license

using System.Drawing;
using System.Drawing.Drawing2D;
using Svg.DataTypes;

namespace Svg
{
    [SvgElement("marker")]
    public class SvgMarker : SvgPathBasedElement, ISvgViewPort
    {
        private SvgVisualElement _markerElement;

        private SvgVisualElement MarkerElement
        {
            get
            {
                if (_markerElement == null)
                {
                    _markerElement = (SvgVisualElement)Children.FirstOrDefault<SvgElement>(x => x is SvgVisualElement);
                }

                return _markerElement;
            }
        }

        [SvgAttribute("refX")]
        public virtual SvgUnit RefX
        {
            get => GetAttribute<SvgUnit>("refX", false, (SvgUnit)0.0f);
            set => Attributes["refX"] = value;
        }

        [SvgAttribute("refY")]
        public virtual SvgUnit RefY
        {
            get => GetAttribute<SvgUnit>("refY", false, (SvgUnit)0.0f);
            set => Attributes["refY"] = value;
        }

        [SvgAttribute("orient")]
        public virtual SvgOrient Orient
        {
            get => GetAttribute<SvgOrient>("orient", false, (SvgOrient)0.0f);
            set => Attributes["orient"] = value;
        }

        [SvgAttribute("overflow")]
        public virtual SvgOverflow Overflow
        {
            get => GetAttribute<SvgOverflow>("overflow", false);
            set => Attributes["overflow"] = value;
        }

        [SvgAttribute("viewBox")]
        public virtual SvgViewBox ViewBox
        {
            get => GetAttribute<SvgViewBox>("viewBox", false);
            set => Attributes["viewBox"] = value;
        }

        [SvgAttribute("preserveAspectRatio")]
        public virtual SvgAspectRatio AspectRatio
        {
            get => GetAttribute<SvgAspectRatio>("preserveAspectRatio", false);
            set => Attributes["preserveAspectRatio"] = value;
        }

        [SvgAttribute("markerWidth")]
        public virtual SvgUnit MarkerWidth
        {
            get => GetAttribute<SvgUnit>("markerWidth", false, (SvgUnit)3f);
            set => Attributes["markerWidth"] = value;
        }

        [SvgAttribute("markerHeight")]
        public virtual SvgUnit MarkerHeight
        {
            get => GetAttribute<SvgUnit>("markerHeight", false, (SvgUnit)3f);
            set => Attributes["markerHeight"] = value;
        }

        [SvgAttribute("markerUnits")]
        public virtual SvgMarkerUnits MarkerUnits
        {
            get => GetAttribute<SvgMarkerUnits>("markerUnits", false);
            set => Attributes["markerUnits"] = value;
        }

        public override SvgPaintServer Fill => MarkerElement != null ? MarkerElement.Fill : base.Fill;

        public override SvgPaintServer Stroke => MarkerElement != null ? MarkerElement.Stroke : base.Stroke;

        public override GraphicsPath Path(ISvgRenderer renderer)
        {
            return MarkerElement != null ? MarkerElement.Path(renderer) : null;
        }

        public override SvgElement DeepCopy()
        {
            return DeepCopy<SvgMarker>();
        }

        public override SvgElement DeepCopy<T>()
        {
            SvgMarker svgMarker = base.DeepCopy<T>() as SvgMarker;
            svgMarker.RefX = RefX;
            svgMarker.RefY = RefY;
            svgMarker.Orient = Orient;
            svgMarker.ViewBox = ViewBox;
            svgMarker.Overflow = Overflow;
            svgMarker.AspectRatio = AspectRatio;
            return svgMarker;
        }

        public void RenderMarker(
          ISvgRenderer pRenderer,
          SvgVisualElement pOwner,
          PointF pRefPoint,
          PointF pMarkerPoint1,
          PointF pMarkerPoint2,
          bool isStartMarker)
        {
            var fAngle = 0.0f;
            if (Orient.IsAuto)
            {
                var x = pMarkerPoint2.X - pMarkerPoint1.X;
                fAngle = (float)(Math.Atan2((double)pMarkerPoint2.Y - (double)pMarkerPoint1.Y, (double)x) * 180.0 / Math.PI);
                if (isStartMarker && Orient.IsAutoStartReverse)
                {
                    fAngle += 180f;
                }
            }
            RenderPart2(fAngle, pRenderer, pOwner, pRefPoint);
        }

        public void RenderMarker(
          ISvgRenderer pRenderer,
          SvgVisualElement pOwner,
          PointF pRefPoint,
          PointF pMarkerPoint1,
          PointF pMarkerPoint2,
          PointF pMarkerPoint3)
        {
            var x1 = pMarkerPoint2.X - pMarkerPoint1.X;
            var num1 = (float)(Math.Atan2((double)pMarkerPoint2.Y - (double)pMarkerPoint1.Y, (double)x1) * 180.0 / Math.PI);
            var x2 = pMarkerPoint3.X - pMarkerPoint2.X;
            var num2 = (float)(Math.Atan2((double)pMarkerPoint3.Y - (double)pMarkerPoint2.Y, (double)x2) * 180.0 / Math.PI);
            RenderPart2((float)(((double)num1 + (double)num2) / 2.0), pRenderer, pOwner, pRefPoint);
        }

        private void RenderPart2(
          float fAngle,
          ISvgRenderer pRenderer,
          SvgVisualElement pOwner,
          PointF pMarkerPoint)
        {
            using (Pen pen = CreatePen(pOwner, pRenderer))
            {
                using (GraphicsPath clone = GetClone(pOwner))
                {
                    using (Matrix matrix1 = new Matrix())
                    {
                        matrix1.Translate(pMarkerPoint.X, pMarkerPoint.Y);
                        if (Orient.IsAuto)
                        {
                            matrix1.Rotate(fAngle);
                        }
                        else
                        {
                            matrix1.Rotate(Orient.Angle);
                        }

                        switch (MarkerUnits)
                        {
                            case SvgMarkerUnits.StrokeWidth:
                                if ((double)ViewBox.Width > 0.0 && (double)ViewBox.Height > 0.0)
                                {
                                    matrix1.Scale((float)MarkerWidth, (float)MarkerHeight);
                                    var deviceValue = pOwner.StrokeWidth.ToDeviceValue(pRenderer, UnitRenderingType.Other, this);
                                    matrix1.Translate(AdjustForViewBoxWidth(-RefX.ToDeviceValue(pRenderer, UnitRenderingType.Horizontal, this) * deviceValue), AdjustForViewBoxHeight(-RefY.ToDeviceValue(pRenderer, UnitRenderingType.Vertical, this) * deviceValue));
                                    break;
                                }
                                Matrix matrix2 = matrix1;
                                SvgUnit svgUnit1 = RefX;
                                var num1 = -(double)svgUnit1.ToDeviceValue(pRenderer, UnitRenderingType.Horizontal, this);
                                svgUnit1 = RefY;
                                var num2 = -(double)svgUnit1.ToDeviceValue(pRenderer, UnitRenderingType.Vertical, this);
                                matrix2.Translate((float)num1, (float)num2);
                                break;
                            case SvgMarkerUnits.UserSpaceOnUse:
                                Matrix matrix3 = matrix1;
                                SvgUnit svgUnit2 = RefX;
                                var num3 = -(double)svgUnit2.ToDeviceValue(pRenderer, UnitRenderingType.Horizontal, this);
                                svgUnit2 = RefY;
                                var num4 = -(double)svgUnit2.ToDeviceValue(pRenderer, UnitRenderingType.Vertical, this);
                                matrix3.Translate((float)num3, (float)num4);
                                break;
                        }
                        if (MarkerElement != null && MarkerElement.Transforms != null)
                        {
                            using (Matrix matrix4 = MarkerElement.Transforms.GetMatrix())
                            {
                                matrix1.Multiply(matrix4);
                            }
                        }
                        clone.Transform(matrix1);
                        if (pen != null)
                        {
                            pRenderer.DrawPath(pen, clone);
                        }

                        SvgPaintServer fill = Children.First<SvgElement>().Fill;
                        var fillRule = (int)FillRule;
                        if (fill == null)
                        {
                            return;
                        }

                        using (Brush brush = fill.GetBrush(this, pRenderer, SvgElement.FixOpacityValue(FillOpacity)))
                        {
                            pRenderer.FillPath(brush, clone);
                        }
                    }
                }
            }
        }

        private Pen CreatePen(SvgVisualElement pPath, ISvgRenderer renderer)
        {
            if (Stroke == null)
            {
                return null;
            }

            Brush brush = Stroke.GetBrush(this, renderer, SvgElement.FixOpacityValue(Opacity));
            switch (MarkerUnits)
            {
                case SvgMarkerUnits.StrokeWidth:
                    return new Pen(brush, pPath.StrokeWidth.ToDeviceValue(renderer, UnitRenderingType.Other, this));
                case SvgMarkerUnits.UserSpaceOnUse:
                    return new Pen(brush, StrokeWidth.ToDeviceValue(renderer, UnitRenderingType.Other, this));
                default:
                    return new Pen(brush, StrokeWidth.ToDeviceValue(renderer, UnitRenderingType.Other, this));
            }
        }

        private GraphicsPath GetClone(SvgVisualElement pPath)
        {
            GraphicsPath clone = Path(null).Clone() as GraphicsPath;
            switch (MarkerUnits)
            {
                case SvgMarkerUnits.StrokeWidth:
                    using (Matrix matrix = new Matrix())
                    {
                        matrix.Scale(AdjustForViewBoxWidth((float)pPath.StrokeWidth), AdjustForViewBoxHeight((float)pPath.StrokeWidth));
                        clone.Transform(matrix);
                        break;
                    }
            }
            return clone;
        }

        private float AdjustForViewBoxWidth(float fWidth)
        {
            return (double)ViewBox.Width > 0.0 ? fWidth / ViewBox.Width : 1f;
        }

        private float AdjustForViewBoxHeight(float fHeight)
        {
            return (double)ViewBox.Height > 0.0 ? fHeight / ViewBox.Height : 1f;
        }
    }
}
