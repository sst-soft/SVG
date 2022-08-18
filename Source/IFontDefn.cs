// Microsoft Public License (Ms-PL). https://github.com/sst-soft/SVG which is a fork of https://github.com/svg-net/SVG.

using System.Drawing;
using System.Drawing.Drawing2D;

namespace Svg
{
    public interface IFontDefn : IDisposable
    {
        float Size { get; }

        float SizeInPoints { get; }

        void AddStringToPath(ISvgRenderer renderer, GraphicsPath path, string text, PointF location);

        float Ascent(ISvgRenderer renderer);

        IList<RectangleF> MeasureCharacters(ISvgRenderer renderer, string text);

        SizeF MeasureString(ISvgRenderer renderer, string text);
    }
}
