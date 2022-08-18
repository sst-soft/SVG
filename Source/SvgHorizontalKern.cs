// todo: add license

namespace Svg
{
    [SvgElement("hkern")]
    public class SvgHorizontalKern : SvgKern
    {
        public override SvgElement DeepCopy()
        {
            return DeepCopy<SvgHorizontalKern>();
        }
    }
}
