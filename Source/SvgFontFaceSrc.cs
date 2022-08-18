// todo: add license

namespace Svg
{
    [SvgElement("font-face-src")]
    public class SvgFontFaceSrc : SvgElement
    {
        public override SvgElement DeepCopy()
        {
            return DeepCopy<SvgFontFaceSrc>();
        }
    }
}
