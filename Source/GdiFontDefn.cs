// Microsoft Public License (Ms-PL). https://github.com/sst-soft/SVG which is a fork of https://github.com/svg-net/SVG.

using System.Drawing;
using System.Drawing.Drawing2D;

namespace Svg
{
    public class GdiFontDefn : IFontDefn, IDisposable
    {
        private readonly Font _font;

        public float Size => _font.Size;

        public float SizeInPoints => _font.SizeInPoints;

        public GdiFontDefn(Font font)
        {
            _font = font;
        }

        public void AddStringToPath(
      ISvgRenderer renderer,
      GraphicsPath path,
      string text,
      PointF location)
        {
            path.AddString(text, _font.FontFamily, (int)_font.Style, _font.Size, location, StringFormat.GenericTypographic);
        }

        public float Ascent(ISvgRenderer renderer)
        {
            FontFamily fontFamily = _font.FontFamily;
            var cellAscent = fontFamily.GetCellAscent(_font.Style);
            var num = _font.SizeInPoints / fontFamily.GetEmHeight(_font.Style) * cellAscent;
            return SvgDocument.PointsPerInch / 72f * num;
        }

        public IList<RectangleF> MeasureCharacters(ISvgRenderer renderer, string text)
        {
            Graphics g = GetGraphics(renderer);
            List<RectangleF> rectangleFList = new List<RectangleF>();
            using (StringFormat stringFormat1 = new StringFormat(StringFormat.GenericTypographic))
            {
                StringFormat stringFormat2 = stringFormat1;
                stringFormat2.FormatFlags = stringFormat2.FormatFlags | (StringFormatFlags)2048;
                for (var index = 0; index <= (text.Length - 1) / 32; ++index)
                {
                    var count = Math.Min(32, text.Length - 32 * index);
                    stringFormat1.SetMeasurableCharacterRanges(Enumerable.Range(32 * index, count).Select<int, CharacterRange>(r => new CharacterRange(r, 1)).ToArray<CharacterRange>());
                    rectangleFList.AddRange(g.MeasureCharacterRanges(text, _font, (RectangleF)new Rectangle(0, 0, count * _font.Height, 1000), stringFormat1).Select<Region, RectangleF>(r => r.GetBounds(g)));
                }
            }
            return rectangleFList;
        }

        public SizeF MeasureString(ISvgRenderer renderer, string text)
        {
            Graphics graphics = GetGraphics(renderer);
            using (StringFormat stringFormat1 = new StringFormat(StringFormat.GenericTypographic))
            {
                stringFormat1.SetMeasurableCharacterRanges(new CharacterRange[1]
                {
          new CharacterRange(0, text.Length)
                });
                StringFormat stringFormat2 = stringFormat1;
                stringFormat2.FormatFlags = stringFormat2.FormatFlags | (StringFormatFlags)2048;
                return new SizeF(graphics.MeasureCharacterRanges(text, _font, (RectangleF)new Rectangle(0, 0, 1000, 1000), stringFormat1)[0].GetBounds(graphics).Width, Ascent(renderer));
            }
        }

        private Graphics GetGraphics(ISvgRenderer renderer)
        {
            return renderer is IGraphicsProvider graphicsProvider ? graphicsProvider.GetGraphics() : throw new NotImplementedException("renderer is not IGraphicsProvider");
        }

        public void Dispose()
        {
            _font.Dispose();
        }
    }
}
