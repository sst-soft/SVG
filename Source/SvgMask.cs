// Microsoft Public License (Ms-PL). https://github.com/sst-soft/SVG which is a fork of https://github.com/svg-net/SVG.

namespace Svg
{
    [SvgElement("mask")]
    public class SvgMask : SvgElement
    {
        public override SvgElement DeepCopy()
        {
            return DeepCopy<SvgMask>();
        }
    }
}
