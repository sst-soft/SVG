// todo: add license

namespace Svg
{
    [SvgElement("vkern")]
    public class SvgVerticalKern : SvgKern
    {
        public override SvgElement DeepCopy()
        {
            return DeepCopy<SvgVerticalKern>();
        }
    }
}
