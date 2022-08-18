// Microsoft Public License (Ms-PL). https://github.com/sst-soft/SVG which is a fork of https://github.com/svg-net/SVG.

using System.Drawing.Drawing2D;

namespace Svg
{
    public interface ISvgStylable
    {
        SvgPaintServer Fill { get; set; }

        SvgPaintServer Stroke { get; set; }

        SvgFillRule FillRule { get; set; }

        float Opacity { get; set; }

        float FillOpacity { get; set; }

        float StrokeOpacity { get; set; }

        SvgUnit StrokeWidth { get; set; }

        SvgStrokeLineCap StrokeLineCap { get; set; }

        SvgStrokeLineJoin StrokeLineJoin { get; set; }

        float StrokeMiterLimit { get; set; }

        SvgUnitCollection StrokeDashArray { get; set; }

        SvgUnit StrokeDashOffset { get; set; }

        GraphicsPath Path(ISvgRenderer renderer);
    }
}
