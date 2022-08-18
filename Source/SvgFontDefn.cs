// todo: add license

using System.Drawing;
using System.Drawing.Drawing2D;

namespace Svg
{
    public class SvgFontDefn : IFontDefn, IDisposable
    {
        private readonly SvgFont _font;
        private readonly float _emScale;
        private readonly float _ppi;
        private readonly float _size;
        private Dictionary<string, SvgGlyph> _glyphs;
        private Dictionary<string, SvgKern> _kerning;

        public float Size => _size;

        public float SizeInPoints => _size * 72f / _ppi;

        public SvgFontDefn(SvgFont font, float size, float ppi)
        {
            _font = font;
            _size = size;
            _ppi = ppi;
            _emScale = _size / _font.Children.OfType<SvgFontFace>().First<SvgFontFace>().UnitsPerEm;
        }

        public float Ascent(ISvgRenderer renderer)
        {
            var num = SizeInPoints * (_emScale / _size) * _font.Descendants().OfType<SvgFontFace>().First<SvgFontFace>().Ascent;
            return SvgDocument.PointsPerInch / 72f * num;
        }

        public IList<RectangleF> MeasureCharacters(ISvgRenderer renderer, string text)
        {
            List<RectangleF> ranges = new List<RectangleF>();
            using (GetPath(renderer, text, ranges, false)) { }
            return ranges;
        }

        public SizeF MeasureString(ISvgRenderer renderer, string text)
        {
            List<RectangleF> rectangleFList = new List<RectangleF>();
            using (GetPath(renderer, text, rectangleFList, true)) { }

            IEnumerable<RectangleF> source = rectangleFList.Where<RectangleF>(r => r != RectangleF.Empty);
            if (!source.Any<RectangleF>())
            {
                return SizeF.Empty;
            }

            RectangleF rectangleF = source.Last<RectangleF>();
            var right = (double)rectangleF.Right;
            rectangleF = source.First<RectangleF>();
            var left = (double)rectangleF.Left;
            return new SizeF((float)(right - left), Ascent(renderer));
        }

        public void AddStringToPath(
          ISvgRenderer renderer,
          GraphicsPath path,
          string text,
          PointF location)
        {
            GraphicsPath path1 = GetPath(renderer, text, null, false);
            if (path1.PointCount <= 0)
            {
                return;
            }

            using (Matrix matrix = new Matrix())
            {
                matrix.Translate(location.X, location.Y);
                path1.Transform(matrix);
                path.AddPath(path1, false);
            }
        }

        private GraphicsPath GetPath(
          ISvgRenderer renderer,
          string text,
          IList<RectangleF> ranges,
          bool measureSpaces)
        {
            EnsureDictionaries();
            SvgGlyph svgGlyph1 = null;
            var x = 0.0f;
            var height = Ascent(renderer);
            GraphicsPath path = new GraphicsPath();
            if (string.IsNullOrEmpty(text))
            {
                return path;
            }

            for (var startIndex = 0; startIndex < text.Length; ++startIndex)
            {
                if (!_glyphs.TryGetValue(text.Substring(startIndex, 1), out SvgGlyph svgGlyph2))
                {
                    svgGlyph2 = _font.Descendants().OfType<SvgMissingGlyph>().First<SvgMissingGlyph>();
                }

                if (svgGlyph1 != null && _kerning.TryGetValue(svgGlyph1.GlyphName + "|" + svgGlyph2.GlyphName, out SvgKern svgKern))
                {
                    x -= svgKern.Kerning * _emScale;
                }

                GraphicsPath graphicsPath = (GraphicsPath)svgGlyph2.Path(renderer).Clone();
                Matrix matrix = new Matrix();
                matrix.Scale(_emScale, -1f * _emScale, (MatrixOrder)1);
                matrix.Translate(x, height, (MatrixOrder)1);
                graphicsPath.Transform(matrix);
                matrix.Dispose();
                RectangleF bounds = graphicsPath.GetBounds();
                if (ranges != null)
                {
                    if (measureSpaces && bounds == RectangleF.Empty)
                    {
                        ranges.Add(new RectangleF(x, 0.0f, svgGlyph2.HorizAdvX * _emScale, height));
                    }
                    else
                    {
                        ranges.Add(bounds);
                    }
                }
                if (graphicsPath.PointCount > 0)
                {
                    path.AddPath(graphicsPath, false);
                }

                x += svgGlyph2.HorizAdvX * _emScale;
                svgGlyph1 = svgGlyph2;
            }
            return path;
        }

        private void EnsureDictionaries()
        {
            if (_glyphs == null)
            {
                _glyphs = _font.Descendants().OfType<SvgGlyph>().ToDictionary<SvgGlyph, string>(g => g.Unicode ?? g.GlyphName ?? g.ID);
            }

            if (_kerning != null)
            {
                return;
            }

            _kerning = _font.Descendants().OfType<SvgKern>().ToDictionary<SvgKern, string>(k => k.Glyph1 + "|" + k.Glyph2);
        }

        public void Dispose()
        {
            _glyphs = null;
            _kerning = null;
        }
    }
}
