// Microsoft Public License (Ms-PL). https://github.com/sst-soft/SVG which is a fork of https://github.com/svg-net/SVG.

using System.ComponentModel;

namespace Svg
{
    [DefaultProperty("Text")]
    [SvgElement("desc")]
    public class SvgDescription : SvgElement, ISvgDescriptiveElement
    {
        public override string ToString()
        {
            return Content;
        }

        public override SvgElement DeepCopy()
        {
            return DeepCopy<SvgDescription>();
        }

        public override SvgElement DeepCopy<T>()
        {
            return base.DeepCopy<T>() as SvgDescription;
        }
    }
}
