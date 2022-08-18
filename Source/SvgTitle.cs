// todo: add license

namespace Svg
{
    [SvgElement("title")]
    public class SvgTitle : SvgElement, ISvgDescriptiveElement
    {
        public override string ToString()
        {
            return Content;
        }

        public override SvgElement DeepCopy()
        {
            return DeepCopy<SvgTitle>();
        }

        public override SvgElement DeepCopy<T>()
        {
            return base.DeepCopy<T>() as SvgTitle;
        }
    }
}
