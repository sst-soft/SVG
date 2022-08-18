// Microsoft Public License (Ms-PL). https://github.com/sst-soft/SVG which is a fork of https://github.com/svg-net/SVG.

using System.ComponentModel;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Globalization;
using Svg.FilterEffects;

namespace Svg
{
    public abstract class SvgVisualElement : SvgElement, ISvgBoundable, ISvgStylable, ISvgClipable
    {
        private bool? _requiresSmoothRendering;
        private Region _previousClip;

        public abstract GraphicsPath Path(ISvgRenderer renderer);

        PointF ISvgBoundable.Location => Bounds.Location;

        SizeF ISvgBoundable.Size => Bounds.Size;

        public abstract RectangleF Bounds { get; }

        [SvgAttribute("clip")]
        public virtual string Clip
        {
            get => GetAttribute<string>("clip", true, "auto");
            set => Attributes["clip"] = value;
        }

        [SvgAttribute("clip-path")]
        public virtual Uri ClipPath
        {
            get => GetAttribute<Uri>("clip-path", false);
            set => Attributes["clip-path"] = value;
        }

        [SvgAttribute("clip-rule")]
        public SvgClipRule ClipRule
        {
            get => GetAttribute<SvgClipRule>("clip-rule", false);
            set => Attributes["clip-rule"] = value;
        }

        [SvgAttribute("filter")]
        public virtual Uri Filter
        {
            get => GetAttribute<Uri>("filter", true);
            set => Attributes["filter"] = value;
        }

        protected virtual bool RequiresSmoothRendering
        {
            get
            {
                if (!_requiresSmoothRendering.HasValue)
                {
                    _requiresSmoothRendering = new bool?(ConvertShapeRendering2AntiAlias(ShapeRendering));
                }

                return _requiresSmoothRendering.Value;
            }
        }

        private bool ConvertShapeRendering2AntiAlias(SvgShapeRendering shapeRendering)
        {
            switch (shapeRendering)
            {
                case SvgShapeRendering.OptimizeSpeed:
                case SvgShapeRendering.CrispEdges:
                case SvgShapeRendering.GeometricPrecision:
                    return false;
                default:
                    return true;
            }
        }

        public SvgVisualElement()
        {
            IsPathDirty = true;
        }

        protected virtual bool Renderable => true;

        protected override void Render(ISvgRenderer renderer)
        {
            Render(renderer, true);
        }

        private void Render(ISvgRenderer renderer, bool renderFilter)
        {
            if (!Visible || !Displayable || Renderable && Path(renderer) == null || renderFilter && RenderFilter(renderer))
            {
                return;
            }

            try
            {
                if (!PushTransforms(renderer))
                {
                    return;
                }

                SetClip(renderer);
                if (Renderable)
                {
                    var opacity = SvgElement.FixOpacityValue(Opacity);
                    if ((double)opacity == 1.0)
                    {
                        RenderFillAndStroke(renderer);
                    }
                    else
                    {
                        IsPathDirty = true;
                        RectangleF bounds = Bounds;
                        IsPathDirty = true;
                        if ((double)bounds.Width > 0.0 && (double)bounds.Height > 0.0)
                        {
                            using (Bitmap bitmap = new Bitmap((int)Math.Ceiling((double)bounds.Width), (int)Math.Ceiling((double)bounds.Height)))
                            {
                                using (ISvgRenderer renderer1 = SvgRenderer.FromImage(bitmap))
                                {
                                    renderer1.SetBoundable(renderer.GetBoundable());
                                    renderer1.TranslateTransform(-bounds.X, -bounds.Y);
                                    RenderFillAndStroke(renderer1);
                                }
                                RectangleF srcRect = new RectangleF(0.0f, 0.0f, bounds.Width, bounds.Height);
                                renderer.DrawImage(bitmap, bounds, srcRect, (GraphicsUnit)2, opacity);
                            }
                        }
                    }
                }
                else
                {
                    RenderChildren(renderer);
                }

                ResetClip(renderer);
            }
            finally
            {
                PopTransforms(renderer);
            }
        }

        private bool RenderFilter(ISvgRenderer renderer)
        {
            var flag = false;
            Uri filter = Filter;
            if (filter != null)
            {
                SvgElement elementById = OwnerDocument.IdManager.GetElementById(filter);
                if (elementById is SvgFilter)
                {
                    try
                    {
                        ((SvgFilter)elementById).ApplyFilter(this, renderer, r => Render(r, false));
                    }
                    catch (Exception)
                    {
                    }
                    flag = true;
                }
            }
            return flag;
        }

        protected internal virtual void RenderFillAndStroke(ISvgRenderer renderer)
        {
            if (RequiresSmoothRendering)
            {
                renderer.SmoothingMode = (SmoothingMode)4;
            }

            RenderFill(renderer);
            RenderStroke(renderer);
            if (!RequiresSmoothRendering || renderer.SmoothingMode != (SmoothingMode)4)
            {
                return;
            }

            renderer.SmoothingMode = 0;
        }

        protected internal virtual void RenderFill(ISvgRenderer renderer)
        {
            if (Fill == null)
            {
                return;
            }

            using (Brush brush = Fill.GetBrush(this, renderer, SvgElement.FixOpacityValue(FillOpacity)))
            {
                if (brush == null)
                {
                    return;
                }

                GraphicsPath path = Path(renderer);
                path.FillMode = FillRule == SvgFillRule.NonZero ? (FillMode)1 : 0;
                renderer.FillPath(brush, path);
            }
        }

        protected internal virtual bool RenderStroke(ISvgRenderer renderer)
        {
            if (Stroke != null && Stroke != SvgPaintServer.None && (double)(float)StrokeWidth > 0.0)
            {
                var strokeWidth = StrokeWidth.ToDeviceValue(renderer, UnitRenderingType.Other, this);
                using (Brush brush = Stroke.GetBrush(this, renderer, SvgElement.FixOpacityValue(StrokeOpacity), true))
                {
                    if (brush != null)
                    {
                        GraphicsPath path1 = Path(renderer);
                        RectangleF bounds = path1.GetBounds();
                        if (path1.PointCount < 1)
                        {
                            return false;
                        }

                        if ((double)bounds.Width <= 0.0 && (double)bounds.Height <= 0.0)
                        {
                            switch (StrokeLineCap)
                            {
                                case SvgStrokeLineCap.Round:
                                    using (GraphicsPath path2 = new GraphicsPath())
                                    {
                                        path2.AddEllipse(path1.PathPoints[0].X - strokeWidth / 2f, path1.PathPoints[0].Y - strokeWidth / 2f, strokeWidth, strokeWidth);
                                        renderer.FillPath(brush, path2);
                                        break;
                                    }
                                case SvgStrokeLineCap.Square:
                                    using (GraphicsPath path3 = new GraphicsPath())
                                    {
                                        path3.AddRectangle(new RectangleF(path1.PathPoints[0].X - strokeWidth / 2f, path1.PathPoints[0].Y - strokeWidth / 2f, strokeWidth, strokeWidth));
                                        renderer.FillPath(brush, path3);
                                        break;
                                    }
                            }
                        }
                        else
                        {
                            using (Pen pen = new Pen(brush, strokeWidth))
                            {
                                if (StrokeDashArray != null && StrokeDashArray.Count > 0)
                                {
                                    SvgUnitCollection source = StrokeDashArray;
                                    if (source.Count % 2 != 0)
                                    {
                                        source = new SvgUnitCollection();
                                        source.AddRange(StrokeDashArray);
                                        source.AddRange(StrokeDashArray);
                                    }
                                    SvgUnit svgUnit = StrokeDashOffset;
                                    strokeWidth = Math.Max(strokeWidth, 1f);
                                    var numArray = source.Select<SvgUnit, float>(u => ((double)u.ToDeviceValue(renderer, UnitRenderingType.Other, this) <= 0.0 ? 1f : u.ToDeviceValue(renderer, UnitRenderingType.Other, this)) / strokeWidth).ToArray<float>();
                                    var newSize = numArray.Length;
                                    if (StrokeLineCap == SvgStrokeLineCap.Round)
                                    {
                                        var array = new float[newSize];
                                        var num1 = 1;
                                        for (var index1 = 0; index1 < newSize; ++index1)
                                        {
                                            array[index1] = numArray[index1] + num1;
                                            if (array[index1] <= 0.0)
                                            {
                                                if (index1 < newSize - 1)
                                                {
                                                    numArray[index1 - 1] += numArray[index1] + numArray[index1 + 1];
                                                    newSize -= 2;
                                                    for (var index2 = index1; index2 < newSize; ++index2)
                                                    {
                                                        numArray[index2] = numArray[index2 + 2];
                                                    }

                                                    index1 -= 2;
                                                }
                                                else if (index1 > 2)
                                                {
                                                    var num2 = numArray[index1 - 1] + numArray[index1];
                                                    array[0] += num2;
                                                    newSize -= 2;
                                                    svgUnit = (SvgUnit)((float)svgUnit + num2 * strokeWidth);
                                                }
                                                else
                                                {
                                                    newSize = 0;
                                                    break;
                                                }
                                            }
                                            num1 *= -1;
                                        }
                                        if (newSize > 0)
                                        {
                                            if (newSize < numArray.Length)
                                            {
                                                Array.Resize<float>(ref array, newSize);
                                            }

                                            numArray = array;
                                            pen.DashCap = (DashCap)2;
                                        }
                                    }
                                    if (newSize > 0)
                                    {
                                        pen.DashPattern = numArray;
                                        if (svgUnit != (SvgUnit)0.0f)
                                        {
                                            pen.DashOffset = ((double)svgUnit.ToDeviceValue(renderer, UnitRenderingType.Other, this) <= 0.0 ? 1f : svgUnit.ToDeviceValue(renderer, UnitRenderingType.Other, this)) / strokeWidth;
                                        }
                                    }
                                }
                                switch (StrokeLineJoin)
                                {
                                    case SvgStrokeLineJoin.Round:
                                        pen.LineJoin = (LineJoin)2;
                                        break;
                                    case SvgStrokeLineJoin.Bevel:
                                        pen.LineJoin = (LineJoin)1;
                                        break;
                                    default:
                                        pen.LineJoin = 0;
                                        break;
                                }
                                pen.MiterLimit = StrokeMiterLimit;
                                switch (StrokeLineCap)
                                {
                                    case SvgStrokeLineCap.Round:
                                        pen.StartCap = (LineCap)2;
                                        pen.EndCap = (LineCap)2;
                                        break;
                                    case SvgStrokeLineCap.Square:
                                        pen.StartCap = (LineCap)1;
                                        pen.EndCap = (LineCap)1;
                                        break;
                                }
                                renderer.DrawPath(pen, path1);
                                return true;
                            }
                        }
                    }
                }
            }
            return false;
        }

        protected internal virtual void SetClip(ISvgRenderer renderer)
        {
            if (!(ClipPath != null) && string.IsNullOrEmpty(Clip))
            {
                return;
            }

            _previousClip = renderer.GetClip();
            if (ClipPath != null)
            {
                SvgClipPath elementById = OwnerDocument.GetElementById<SvgClipPath>(ClipPath.ToString());
                if (elementById != null)
                {
                    renderer.SetClip(elementById.GetClipRegion(this, renderer), (CombineMode)1);
                }
            }
            var clip = Clip;
            if (string.IsNullOrEmpty(clip) || !clip.StartsWith("rect("))
            {
                return;
            }

            var str = clip.Trim();
            List<float> list = str.Substring(5, str.Length - 6).Split(',', StringSplitOptions.None).Select<string, float>(o => float.Parse(o.Trim(), NumberStyles.Any, CultureInfo.InvariantCulture)).ToList<float>();
            RectangleF bounds = Bounds;
            RectangleF rectangleF = new RectangleF(bounds.Left + list[3], bounds.Top + list[0], bounds.Width - (list[3] + list[1]), bounds.Height - (list[2] + list[0]));
            renderer.SetClip(new Region(rectangleF), (CombineMode)1);
        }

        protected internal virtual void ResetClip(ISvgRenderer renderer)
        {
            if (_previousClip == null)
            {
                return;
            }

            renderer.SetClip(_previousClip);
            _previousClip = null;
        }

        void ISvgClipable.SetClip(ISvgRenderer renderer)
        {
            SetClip(renderer);
        }

        void ISvgClipable.ResetClip(ISvgRenderer renderer)
        {
            ResetClip(renderer);
        }

        public override SvgElement DeepCopy<T>()
        {
            SvgVisualElement svgVisualElement = base.DeepCopy<T>() as SvgVisualElement;
            svgVisualElement.ClipPath = ClipPath;
            svgVisualElement.ClipRule = ClipRule;
            svgVisualElement.Filter = Filter;
            svgVisualElement.Visible = Visible;
            if (Fill != null)
            {
                svgVisualElement.Fill = Fill;
            }

            if (Stroke != null)
            {
                svgVisualElement.Stroke = Stroke;
            }

            svgVisualElement.FillRule = FillRule;
            svgVisualElement.FillOpacity = FillOpacity;
            svgVisualElement.StrokeWidth = StrokeWidth;
            svgVisualElement.StrokeLineCap = StrokeLineCap;
            svgVisualElement.StrokeLineJoin = StrokeLineJoin;
            svgVisualElement.StrokeMiterLimit = StrokeMiterLimit;
            svgVisualElement.StrokeDashArray = StrokeDashArray;
            svgVisualElement.StrokeDashOffset = StrokeDashOffset;
            svgVisualElement.StrokeOpacity = StrokeOpacity;
            svgVisualElement.Opacity = Opacity;
            return svgVisualElement;
        }

        [TypeConverter(typeof(SvgBoolConverter))]
        [SvgAttribute("visibility")]
        public virtual bool Visible
        {
            get => GetAttribute<bool>("visibility", true, true);
            set => Attributes["visibility"] = value;
        }

        [SvgAttribute("display")]
        public virtual string Display
        {
            get => GetAttribute<string>("display", true, "inline");
            set => Attributes["display"] = value;
        }

        protected virtual bool Displayable => !string.Equals(Display, "none", StringComparison.OrdinalIgnoreCase);

        [SvgAttribute("enable-background")]
        public virtual string EnableBackground
        {
            get => GetAttribute<string>("enable-background", true);
            set => Attributes["enable-background"] = value;
        }
    }
}
