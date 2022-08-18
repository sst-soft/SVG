// Microsoft Public License (Ms-PL). https://github.com/sst-soft/SVG which is a fork of https://github.com/svg-net/SVG.

namespace Svg
{
    public class NonSvgElement : SvgElement
    {
        public NonSvgElement()
        {
        }

        public NonSvgElement(string elementName)
        {
            ElementName = elementName;
        }

        public override SvgElement DeepCopy()
        {
            return DeepCopy<NonSvgElement>();
        }

        public string Name => ElementName;
    }
}
