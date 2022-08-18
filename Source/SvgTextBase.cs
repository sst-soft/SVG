// Microsoft Public License (Ms-PL). https://github.com/sst-soft/SVG which is a fork of https://github.com/svg-net/SVG.

using System.Collections.Specialized;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Globalization;
using System.Text;
using System.Text.RegularExpressions;

namespace Svg
{
    public abstract class SvgTextBase : SvgVisualElement
    {
        private SvgUnitCollection _x = new SvgUnitCollection();
        private SvgUnitCollection _y = new SvgUnitCollection();
        private SvgUnitCollection _dy = new SvgUnitCollection();
        private SvgUnitCollection _dx = new SvgUnitCollection();
        private string _rotate;
        private readonly List<float> _rotations = new List<float>();
        private GraphicsPath _path;
        private static readonly Regex MultipleSpaces = new Regex(" {2,}", RegexOptions.Compiled);

        public SvgTextBase()
        {
            _x.CollectionChanged += new NotifyCollectionChangedEventHandler(OnXChanged);
            _dx.CollectionChanged += new NotifyCollectionChangedEventHandler(OnDxChanged);
            _y.CollectionChanged += new NotifyCollectionChangedEventHandler(OnYChanged);
            _dy.CollectionChanged += new NotifyCollectionChangedEventHandler(OnDyChanged);
        }

        public virtual string Text
        {
            get => Content;
            set
            {
                Nodes.Clear();
                Children.Clear();
                if (value != null)
                {
                    Nodes.Add(new SvgContentNode()
                    {
                        Content = value
                    });
                }

                IsPathDirty = true;
                Content = value;
            }
        }

        public override XmlSpaceHandling SpaceHandling
        {
            get => base.SpaceHandling;
            set
            {
                base.SpaceHandling = value;
                IsPathDirty = true;
            }
        }

        [SvgAttribute("x")]
        public virtual SvgUnitCollection X
        {
            get => _x;
            set
            {
                if (_x != value)
                {
                    if (_x != null)
                    {
                        _x.CollectionChanged -= new NotifyCollectionChangedEventHandler(OnXChanged);
                    }

                    _x = value;
                    if (_x != null)
                    {
                        _x.CollectionChanged += new NotifyCollectionChangedEventHandler(OnXChanged);
                    }

                    IsPathDirty = true;
                }
                Attributes["x"] = value;
            }
        }

        private void OnXChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            Attributes["x"] = X.FirstOrDefault<SvgUnit>();
            IsPathDirty = true;
        }

        [SvgAttribute("dx")]
        public virtual SvgUnitCollection Dx
        {
            get => _dx;
            set
            {
                if (_dx != value)
                {
                    if (_dx != null)
                    {
                        _dx.CollectionChanged -= new NotifyCollectionChangedEventHandler(OnDxChanged);
                    }

                    _dx = value;
                    if (_dx != null)
                    {
                        _dx.CollectionChanged += new NotifyCollectionChangedEventHandler(OnDxChanged);
                    }

                    IsPathDirty = true;
                }
                Attributes["dx"] = value;
            }
        }

        private void OnDxChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            Attributes["dx"] = X.FirstOrDefault<SvgUnit>();
            IsPathDirty = true;
        }

        [SvgAttribute("y")]
        public virtual SvgUnitCollection Y
        {
            get => _y;
            set
            {
                if (_y != value)
                {
                    if (_y != null)
                    {
                        _y.CollectionChanged -= new NotifyCollectionChangedEventHandler(OnYChanged);
                    }

                    _y = value;
                    if (_y != null)
                    {
                        _y.CollectionChanged += new NotifyCollectionChangedEventHandler(OnYChanged);
                    }

                    IsPathDirty = true;
                }
                Attributes["y"] = value;
            }
        }

        private void OnYChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            Attributes["y"] = Y.FirstOrDefault<SvgUnit>();
            IsPathDirty = true;
        }

        [SvgAttribute("dy")]
        public virtual SvgUnitCollection Dy
        {
            get => _dy;
            set
            {
                if (_dy != value)
                {
                    if (_dy != null)
                    {
                        _dy.CollectionChanged -= new NotifyCollectionChangedEventHandler(OnDyChanged);
                    }

                    _dy = value;
                    if (_dy != null)
                    {
                        _dy.CollectionChanged += new NotifyCollectionChangedEventHandler(OnDyChanged);
                    }

                    IsPathDirty = true;
                }
                Attributes["dy"] = value;
            }
        }

        private void OnDyChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            Attributes["dy"] = Y.FirstOrDefault<SvgUnit>();
            IsPathDirty = true;
        }

        [SvgAttribute("rotate")]
        public virtual string Rotate
        {
            get => _rotate;
            set
            {
                if (_rotate != value)
                {
                    _rotate = value;
                    _rotations.Clear();
                    _rotations.AddRange(_rotate.Split(new char[5]
                    {
            ',',
            ' ',
            '\r',
            '\n',
            '\t'
                    }, StringSplitOptions.RemoveEmptyEntries).Select<string, float>(r => float.Parse(r, NumberStyles.Any, CultureInfo.InvariantCulture)));
                    IsPathDirty = true;
                }
                Attributes["rotate"] = value;
            }
        }

        [SvgAttribute("textLength")]
        public virtual SvgUnit TextLength
        {
            get => GetAttribute<SvgUnit>("textLength", true, SvgUnit.None);
            set
            {
                Attributes["textLength"] = value;
                IsPathDirty = true;
            }
        }

        [SvgAttribute("lengthAdjust")]
        public virtual SvgTextLengthAdjust LengthAdjust
        {
            get => GetAttribute<SvgTextLengthAdjust>("lengthAdjust", true);
            set
            {
                Attributes["lengthAdjust"] = value;
                IsPathDirty = true;
            }
        }

        [SvgAttribute("letter-spacing")]
        public virtual SvgUnit LetterSpacing
        {
            get => GetAttribute<SvgUnit>("letter-spacing", true, SvgUnit.None);
            set
            {
                Attributes["letter-spacing"] = value;
                IsPathDirty = true;
            }
        }

        [SvgAttribute("word-spacing")]
        public virtual SvgUnit WordSpacing
        {
            get => GetAttribute<SvgUnit>("word-spacing", true, SvgUnit.None);
            set
            {
                Attributes["word-spacing"] = value;
                IsPathDirty = true;
            }
        }

        public override SvgPaintServer Fill
        {
            get => GetAttribute<SvgPaintServer>("fill", true, new SvgColourServer(System.Drawing.Color.Black));
            set => Attributes["fill"] = value;
        }

        public override string ToString()
        {
            return Text;
        }

        public override RectangleF Bounds
        {
            get
            {
                GraphicsPath graphicsPath = Path(null);
                foreach (SvgVisualElement svgVisualElement in Children.OfType<SvgVisualElement>())
                {
                    if (!(svgVisualElement is SvgTextSpan svgTextSpan) || svgTextSpan.Text != null)
                    {
                        graphicsPath.AddPath(svgVisualElement.Path(null), false);
                    }
                }
                if (Transforms != null && Transforms.Count > 0)
                {
                    using (Matrix matrix = Transforms.GetMatrix())
                    {
                        graphicsPath.Transform(matrix);
                    }
                }
                return graphicsPath.GetBounds();
            }
        }

        protected internal override void RenderFillAndStroke(ISvgRenderer renderer)
        {
            base.RenderFillAndStroke(renderer);
            RenderChildren(renderer);
        }

        internal virtual IEnumerable<ISvgNode> GetContentNodes()
        {
            return Nodes != null && Nodes.Count >= 1 ? Nodes : Children.OfType<ISvgNode>().Where<ISvgNode>(o => !(o is ISvgDescriptiveElement));
        }

        protected virtual GraphicsPath GetBaselinePath(ISvgRenderer renderer)
        {
            return null;
        }

        protected virtual float GetAuthorPathLength()
        {
            return 0.0f;
        }

        public override GraphicsPath Path(ISvgRenderer renderer)
        {
            IEnumerable<ISvgNode> source = GetContentNodes().Where<ISvgNode>(x =>
            {
                if (!(x is SvgContentNode))
                {
                    return false;
                }

                return string.IsNullOrEmpty(x.Content.Trim('\r', '\n', '\t'));
            });
            if (_path == null || IsPathDirty || source.Count<ISvgNode>() == 1)
            {
                if (renderer != null && renderer is IGraphicsProvider)
                {
                    SetPath(new SvgTextBase.TextDrawingState(renderer, this));
                }
                else
                {
                    using (ISvgRenderer renderer1 = SvgRenderer.FromNull())
                    {
                        SetPath(new SvgTextBase.TextDrawingState(renderer1, this));
                    }
                }
            }
            return _path;
        }

        private void SetPath(SvgTextBase.TextDrawingState state)
        {
            SetPath(state, true);
        }

        private void SetPath(SvgTextBase.TextDrawingState state, bool doMeasurements)
        {
            SvgTextBase.TextDrawingState state1 = null;
            var flag = state.BaselinePath != null && (TextAnchor == SvgTextAnchor.Middle || TextAnchor == SvgTextAnchor.End);
            if (doMeasurements)
            {
                if (TextLength != SvgUnit.None)
                {
                    state1 = state.Clone();
                }
                else if (flag)
                {
                    state1 = state.Clone();
                    state.BaselinePath = null;
                }
            }
            foreach (ISvgNode contentNode in GetContentNodes())
            {
                if (!(contentNode is SvgTextBase element))
                {
                    if (!string.IsNullOrEmpty(contentNode.Content))
                    {
                        state.DrawString(PrepareText(contentNode.Content));
                    }
                }
                else
                {
                    SvgTextBase.TextDrawingState state2 = new SvgTextBase.TextDrawingState(state, element);
                    element.SetPath(state2);
                    state.NumChars += state2.NumChars;
                    state.Current = state2.Current;
                }
            }
            GraphicsPath graphicsPath = state.GetPath() ?? new GraphicsPath();
            if (doMeasurements)
            {
                if (TextLength != SvgUnit.None)
                {
                    var deviceValue = TextLength.ToDeviceValue(state.Renderer, UnitRenderingType.Horizontal, this);
                    RectangleF textBounds = state.TextBounds;
                    var width = textBounds.Width;
                    var num1 = width - deviceValue;
                    if ((double)Math.Abs(num1) > 1.5)
                    {
                        if (LengthAdjust == SvgTextLengthAdjust.Spacing)
                        {
                            if (X.Count < 2)
                            {
                                var num2 = state.NumChars - state1.NumChars - 1;
                                if (num2 != 0)
                                {
                                    state1.LetterSpacingAdjust = -1f * num1 / num2;
                                    SetPath(state1, false);
                                    return;
                                }
                            }
                        }
                        else
                        {
                            using (Matrix matrix1 = new Matrix())
                            {
                                Matrix matrix2 = matrix1;
                                textBounds = state.TextBounds;
                                var num3 = -1.0 * (double)textBounds.X;
                                matrix2.Translate((float)num3, 0.0f, (MatrixOrder)1);
                                matrix1.Scale(deviceValue / width, 1f, (MatrixOrder)1);
                                Matrix matrix3 = matrix1;
                                textBounds = state.TextBounds;
                                var x = (double)textBounds.X;
                                matrix3.Translate((float)x, 0.0f, (MatrixOrder)1);
                                graphicsPath.Transform(matrix1);
                            }
                        }
                    }
                }
                else if (flag)
                {
                    RectangleF bounds = graphicsPath.GetBounds();
                    state1.StartOffsetAdjust = TextAnchor != SvgTextAnchor.Middle ? -1f * bounds.Width : (float)(-1.0 * (double)bounds.Width / 2.0);
                    SetPath(state1, false);
                    return;
                }
            }
            _path = graphicsPath;
            IsPathDirty = false;
        }

        protected string PrepareText(string value)
        {
            value = ApplyTransformation(value);
            value = new StringBuilder(value).Replace("\r\n", " ").Replace('\r', ' ').Replace('\n', ' ').Replace('\t', ' ').ToString();
            return SpaceHandling != XmlSpaceHandling.preserve ? SvgTextBase.MultipleSpaces.Replace(value.Trim(), " ") : value;
        }

        private string ApplyTransformation(string value)
        {
            switch (TextTransformation)
            {
                case SvgTextTransformation.Capitalize:
                    return value.ToUpper();
                case SvgTextTransformation.Uppercase:
                    return value.ToUpper();
                case SvgTextTransformation.Lowercase:
                    return CultureInfo.CurrentCulture.TextInfo.ToTitleCase(value);
                default:
                    return value;
            }
        }

        [SvgAttribute("onchange")]
        public event EventHandler<StringArg> Change;

        protected void OnChange(string newString, string sessionID)
        {
            StringArg s = new StringArg
            {
                s = newString,
                SessionID = sessionID
            };
            RaiseChange(this, s);
        }

        protected void RaiseChange(object sender, StringArg s)
        {
            EventHandler<StringArg> change = Change;
            if (change == null)
            {
                return;
            }

            change(sender, s);
        }

        public override bool ShouldWriteElement()
        {
            return HasChildren() || Nodes.Count > 0;
        }

        private class FontBoundable : ISvgBoundable
        {
            private readonly IFontDefn _font;
            private readonly float _width = 1f;

            public FontBoundable(IFontDefn font)
            {
                _font = font;
            }

            public FontBoundable(IFontDefn font, float width)
            {
                _font = font;
                _width = width;
            }

            public PointF Location => PointF.Empty;

            public SizeF Size => new SizeF(_width, _font.Size);

            public RectangleF Bounds => new RectangleF(Location, Size);
        }

        private class TextDrawingState
        {
            private float _xAnchor = float.MinValue;
            private IList<GraphicsPath> _anchoredPaths = new List<GraphicsPath>();
            private GraphicsPath _currPath;
            private GraphicsPath _finalPath;
            private readonly float _authorPathLength;

            public GraphicsPath BaselinePath { get; set; }

            public PointF Current { get; set; }

            public RectangleF TextBounds { get; set; }

            public SvgTextBase Element { get; set; }

            public float LetterSpacingAdjust { get; set; }

            public int NumChars { get; set; }

            public SvgTextBase.TextDrawingState Parent { get; set; }

            public ISvgRenderer Renderer { get; set; }

            public float StartOffsetAdjust { get; set; }

            private TextDrawingState()
            {
            }

            public TextDrawingState(ISvgRenderer renderer, SvgTextBase element)
            {
                Element = element;
                Renderer = renderer;
                Current = PointF.Empty;
                TextBounds = RectangleF.Empty;
                _xAnchor = 0.0f;
                BaselinePath = element.GetBaselinePath(renderer);
                _authorPathLength = element.GetAuthorPathLength();
            }

            public TextDrawingState(SvgTextBase.TextDrawingState parent, SvgTextBase element)
            {
                Element = element;
                Renderer = parent.Renderer;
                Parent = parent;
                Current = parent.Current;
                TextBounds = parent.TextBounds;
                BaselinePath = element.GetBaselinePath(parent.Renderer) ?? parent.BaselinePath;
                var authorPathLength = element.GetAuthorPathLength();
                _authorPathLength = (double)authorPathLength == 0.0 ? parent._authorPathLength : authorPathLength;
            }

            public GraphicsPath GetPath()
            {
                FlushPath();
                return _finalPath;
            }

            public SvgTextBase.TextDrawingState Clone()
            {
                return new SvgTextBase.TextDrawingState()
                {
                    _anchoredPaths = _anchoredPaths.ToList<GraphicsPath>(),
                    BaselinePath = BaselinePath,
                    _xAnchor = _xAnchor,
                    Current = Current,
                    TextBounds = TextBounds,
                    Element = Element,
                    NumChars = NumChars,
                    Parent = Parent,
                    Renderer = Renderer
                };
            }

            public void DrawString(string value)
            {
                IList<float> values1 = GetValues(value.Length, e => e._x, UnitRenderingType.HorizontalOffset);
                IList<float> values2 = GetValues(value.Length, e => e._y, UnitRenderingType.VerticalOffset);
                using (IFontDefn font = Element.GetFont(Renderer))
                {
                    var fontBaselineHeight = font.Ascent(Renderer);
                    PathStatistics pathStatistics = null;
                    var num1 = 1.0;
                    if (BaselinePath != null)
                    {
                        pathStatistics = new PathStatistics(BaselinePath.PathData);
                        if (_authorPathLength > 0.0)
                        {
                            num1 = _authorPathLength / pathStatistics.TotalLength;
                        }
                    }
                    var num2 = 0.0f;
                    IList<float> values3;
                    IList<float> values4;
                    IList<float> values5;
                    try
                    {
                        Renderer.SetBoundable(new SvgTextBase.FontBoundable(font, pathStatistics == null ? 1f : (float)pathStatistics.TotalLength));
                        values3 = GetValues(value.Length, e => e._dx, UnitRenderingType.Horizontal);
                        values4 = GetValues(value.Length, e => e._dy, UnitRenderingType.Vertical);
                        if ((double)StartOffsetAdjust != 0.0)
                        {
                            if (values3.Count < 1)
                            {
                                values3.Add(StartOffsetAdjust);
                            }
                            else
                            {
                                values3[0] += StartOffsetAdjust;
                            }
                        }
                        if ((double)Element.LetterSpacing.Value != 0.0 || (double)Element.WordSpacing.Value != 0.0 || (double)LetterSpacingAdjust != 0.0)
                        {
                            var num3 = Element.LetterSpacing.ToDeviceValue(Renderer, UnitRenderingType.Horizontal, Element) + LetterSpacingAdjust;
                            var deviceValue = Element.WordSpacing.ToDeviceValue(Renderer, UnitRenderingType.Horizontal, Element);
                            if (Parent == null && NumChars == 0 && values3.Count < 1)
                            {
                                values3.Add(0.0f);
                            }

                            for (var index = Parent != null || NumChars != 0 ? 0 : 1; index < value.Length; ++index)
                            {
                                if (index >= values3.Count)
                                {
                                    values3.Add(num3 + (char.IsWhiteSpace(value[index]) ? deviceValue : 0.0f));
                                }
                                else
                                {
                                    values3[index] += num3 + (char.IsWhiteSpace(value[index]) ? deviceValue : 0.0f);
                                }
                            }
                        }
                        values5 = GetValues(value.Length, e => e._rotations);
                        var attribute = Element.Attributes.GetAttribute<string>("baseline-shift");
                        if (attribute != null && (attribute == null || attribute.Length != 0) && !(attribute == "baseline") && !(attribute == "inherit"))
                        {
                            num2 = attribute == "sub" ? new SvgUnit(SvgUnitType.Ex, 1f).ToDeviceValue(Renderer, UnitRenderingType.Vertical, Element) : (attribute == "super" ? -1f * new SvgUnit(SvgUnitType.Ex, 1f).ToDeviceValue(Renderer, UnitRenderingType.Vertical, Element) : -1f * ((SvgUnit)new SvgUnitConverter().ConvertFromInvariantString(attribute)).ToDeviceValue(Renderer, UnitRenderingType.Vertical, Element));
                        }

                        if ((double)num2 != 0.0)
                        {
                            if (values4.Any<float>())
                            {
                                values4[0] += num2;
                            }
                            else
                            {
                                values4.Add(num2);
                            }
                        }
                    }
                    finally
                    {
                        Renderer.PopBoundable();
                    }
                    var num4 = Current.X;
                    var y = Current.Y;
                    PointF current;
                    for (var index = 0; index < values1.Count - 1; ++index)
                    {
                        FlushPath();
                        _xAnchor = values1[index] + (values3.Count > index ? values3[index] : 0.0f);
                        EnsurePath();
                        y = (float)((values2.Count > index ? (double)values2[index] : (double)y) + (values4.Count > index ? (double)values4[index] : 0.0));
                        ref var local = ref num4;
                        current = Current;
                        var x = (double)current.X;
                        num4 = local.Equals((float)x) ? _xAnchor : num4;
                        DrawStringOnCurrPath(value[index].ToString(), font, new PointF(_xAnchor, y), fontBaselineHeight, values5.Count > index ? values5[index] : values5.LastOrDefault<float>());
                    }
                    var startIndex = 0;
                    current = Current;
                    var num5 = current.X;
                    if (values1.Any<float>())
                    {
                        FlushPath();
                        startIndex = values1.Count - 1;
                        num5 = values1.Last<float>();
                        _xAnchor = num5;
                    }
                    EnsurePath();
                    var num6 = startIndex + Math.Max(Math.Max(Math.Max(Math.Max(values3.Count, values4.Count), values2.Count), values5.Count) - startIndex - 1, 0);
                    if ((double)values5.LastOrDefault<float>() != 0.0 || pathStatistics != null)
                    {
                        num6 = value.Length;
                    }

                    if (num6 > startIndex)
                    {
                        IList<RectangleF> source = font.MeasureCharacters(Renderer, value.Substring(startIndex, Math.Min(num6 + 1, value.Length) - startIndex));
                        RectangleF rectangleF;
                        for (var index = startIndex; index < num6; ++index)
                        {
                            var num7 = (double)num5;
                            var num8 = num1 * (values3.Count > index ? (double)values3[index] : 0.0);
                            rectangleF = source[index - startIndex];
                            var x1 = (double)rectangleF.X;
                            double num9;
                            if (index != startIndex)
                            {
                                rectangleF = source[index - startIndex - 1];
                                num9 = (double)rectangleF.X;
                            }
                            else
                            {
                                num9 = 0.0;
                            }

                            var num10 = x1 - num9;
                            var num11 = num8 + num10;
                            num5 = (float)(num7 + num11);
                            y = (float)((values2.Count > index ? (double)values2[index] : (double)y) + (values4.Count > index ? (double)values4[index] : 0.0));
                            char ch;
                            if (pathStatistics == null)
                            {
                                ref var local = ref num4;
                                current = Current;
                                var x2 = (double)current.X;
                                num4 = local.Equals((float)x2) ? num5 : num4;
                                ch = value[index];
                                DrawStringOnCurrPath(ch.ToString(), font, new PointF(num5, y), fontBaselineHeight, values5.Count > index ? values5[index] : values5.LastOrDefault<float>());
                            }
                            else
                            {
                                num5 = Math.Max(num5, 0.0f);
                                rectangleF = source[index - startIndex];
                                var num12 = rectangleF.Width / 2f;
                                if (pathStatistics.OffsetOnPath((double)num5 + (double)num12))
                                {
                                    pathStatistics.LocationAngleAtOffset((double)num5 + (double)num12, out PointF point, out var angle);
                                    point = new PointF((float)((double)point.X - (double)num12 * Math.Cos((double)angle * Math.PI / 180.0) - num1 * (double)y * Math.Sin((double)angle * Math.PI / 180.0)), (float)((double)point.Y - (double)num12 * Math.Sin((double)angle * Math.PI / 180.0) + num1 * (double)y * Math.Cos((double)angle * Math.PI / 180.0)));
                                    ref var local = ref num4;
                                    current = Current;
                                    var x3 = (double)current.X;
                                    num4 = local.Equals((float)x3) ? point.X : num4;
                                    ch = value[index];
                                    DrawStringOnCurrPath(ch.ToString(), font, point, fontBaselineHeight, angle);
                                }
                            }
                        }
                        if (num6 < value.Length)
                        {
                            var num13 = (double)num5;
                            rectangleF = source[source.Count - 1];
                            var x4 = (double)rectangleF.X;
                            rectangleF = source[source.Count - 2];
                            var x5 = (double)rectangleF.X;
                            var num14 = x4 - x5;
                            num5 = (float)(num13 + num14);
                        }
                        else
                        {
                            var num15 = (double)num5;
                            rectangleF = source.Last<RectangleF>();
                            var width = (double)rectangleF.Width;
                            num5 = (float)(num15 + width);
                        }
                    }
                    if (num6 < value.Length)
                    {
                        var x6 = num5 + (values3.Count > num6 ? values3[num6] : 0.0f);
                        y = (float)((values2.Count > num6 ? (double)values2[num6] : (double)y) + (values4.Count > num6 ? (double)values4[num6] : 0.0));
                        ref var local = ref num4;
                        current = Current;
                        var x7 = (double)current.X;
                        num4 = local.Equals((float)x7) ? x6 : num4;
                        DrawStringOnCurrPath(value.Substring(num6), font, new PointF(x6, y), fontBaselineHeight, values5.LastOrDefault<float>());
                        SizeF sizeF = font.MeasureString(Renderer, value.Substring(num6));
                        num5 = x6 + sizeF.Width;
                    }
                    NumChars += value.Length;
                    Current = new PointF(num5, y - num2);
                    var x8 = (double)num4;
                    current = Current;
                    var width1 = (double)current.X - (double)num4;
                    TextBounds = new RectangleF((float)x8, 0.0f, (float)width1, 0.0f);
                }
            }

            private void DrawStringOnCurrPath(
              string value,
              IFontDefn font,
              PointF location,
              float fontBaselineHeight,
              float rotation)
            {
                GraphicsPath path = _currPath;
                if ((double)rotation != 0.0)
                {
                    path = new GraphicsPath();
                }

                font.AddStringToPath(Renderer, path, value, new PointF(location.X, location.Y - fontBaselineHeight));
                if ((double)rotation == 0.0 || path.PointCount <= 0)
                {
                    return;
                }

                using (Matrix matrix = new Matrix())
                {
                    matrix.Translate(-1f * location.X, -1f * location.Y, (MatrixOrder)1);
                    matrix.Rotate(rotation, (MatrixOrder)1);
                    matrix.Translate(location.X, location.Y, (MatrixOrder)1);
                    path.Transform(matrix);
                    _currPath.AddPath(path, false);
                }
            }

            private void EnsurePath()
            {
                if (_currPath != null)
                {
                    return;
                }

                _currPath = new GraphicsPath();
                _currPath.StartFigure();
                SvgTextBase.TextDrawingState textDrawingState = this;
                while (textDrawingState != null && textDrawingState._xAnchor <= -3.4028234663852886E+38)
                {
                    textDrawingState = textDrawingState.Parent;
                }
                textDrawingState._anchoredPaths.Add(_currPath);
            }

            private void FlushPath()
            {
                if (_currPath == null)
                {
                    return;
                }

                _currPath.CloseFigure();
                if (_currPath.PointCount < 1)
                {
                    _anchoredPaths.Clear();
                    _xAnchor = float.MinValue;
                    _currPath = null;
                }
                else
                {
                    if (_xAnchor > -3.4028234663852886E+38)
                    {
                        var num1 = float.MaxValue;
                        var num2 = float.MinValue;
                        foreach (GraphicsPath anchoredPath in (IEnumerable<GraphicsPath>)_anchoredPaths)
                        {
                            RectangleF bounds = anchoredPath.GetBounds();
                            if ((double)bounds.Left < (double)num1)
                            {
                                num1 = bounds.Left;
                            }

                            if ((double)bounds.Right > (double)num2)
                            {
                                num2 = bounds.Right;
                            }
                        }
                        var num3 = 0.0f;
                        switch (Element.TextAnchor)
                        {
                            case SvgTextAnchor.Middle:
                                if (_anchoredPaths.Count<GraphicsPath>() == 1)
                                {
                                    num3 -= TextBounds.Width / 2f;
                                    break;
                                }
                                num3 -= (float)(((double)num2 - (double)num1) / 2.0);
                                break;
                            case SvgTextAnchor.End:
                                if (_anchoredPaths.Count<GraphicsPath>() == 1)
                                {
                                    num3 -= TextBounds.Width;
                                    break;
                                }
                                num3 -= num2 - num1;
                                break;
                        }
                        if ((double)num3 != 0.0)
                        {
                            using (Matrix matrix = new Matrix())
                            {
                                matrix.Translate(num3, 0.0f);
                                foreach (GraphicsPath anchoredPath in (IEnumerable<GraphicsPath>)_anchoredPaths)
                                {
                                    anchoredPath.Transform(matrix);
                                }
                            }
                        }
                        _anchoredPaths.Clear();
                        _xAnchor = float.MinValue;
                    }
                    if (_finalPath == null)
                    {
                        _finalPath = _currPath;
                    }
                    else
                    {
                        _finalPath.AddPath(_currPath, false);
                    }

                    _currPath = null;
                }
            }

            private IList<float> GetValues(
              int maxCount,
              Func<SvgTextBase, IEnumerable<float>> listGetter)
            {
                SvgTextBase.TextDrawingState textDrawingState = this;
                var count = 0;
                List<float> values = new List<float>();
                var num = 0;
                for (; textDrawingState != null; textDrawingState = textDrawingState.Parent)
                {
                    count += textDrawingState.NumChars;
                    values.AddRange(listGetter(textDrawingState.Element).Skip<float>(count).Take<float>(maxCount));
                    if (values.Count > num)
                    {
                        maxCount -= values.Count - num;
                        count += values.Count - num;
                        num = values.Count;
                    }
                    if (maxCount < 1)
                    {
                        return values;
                    }
                }
                return values;
            }

            private IList<float> GetValues(
              int maxCount,
              Func<SvgTextBase, IEnumerable<SvgUnit>> listGetter,
              UnitRenderingType renderingType)
            {
                SvgTextBase.TextDrawingState currState = this;
                var count = 0;
                List<float> values = new List<float>();
                var num = 0;
                for (; currState != null; currState = currState.Parent)
                {
                    count += currState.NumChars;
                    values.AddRange(listGetter(currState.Element).Skip<SvgUnit>(count).Take<SvgUnit>(maxCount).Select<SvgUnit, float>(p => p.ToDeviceValue(currState.Renderer, renderingType, currState.Element)));
                    if (values.Count > num)
                    {
                        maxCount -= values.Count - num;
                        count += values.Count - num;
                        num = values.Count;
                    }
                    if (maxCount < 1)
                    {
                        return values;
                    }
                }
                return values;
            }
        }
    }
}
