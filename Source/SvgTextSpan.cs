﻿// Microsoft Public License (Ms-PL). https://github.com/sst-soft/SVG which is a fork of https://github.com/svg-net/SVG.

namespace Svg
{
    [SvgElement("tspan")]
    public class SvgTextSpan : SvgTextBase
    {
        public override SvgElement DeepCopy()
        {
            return DeepCopy<SvgTextSpan>();
        }

        public override SvgElement DeepCopy<T>()
        {
            SvgTextSpan svgTextSpan = base.DeepCopy<T>() as SvgTextSpan;
            svgTextSpan.X = X;
            svgTextSpan.Y = Y;
            svgTextSpan.Dx = Dx;
            svgTextSpan.Dy = Dy;
            svgTextSpan.Text = Text;
            return svgTextSpan;
        }
    }
}
