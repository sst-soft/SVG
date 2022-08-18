// Microsoft Public License (Ms-PL). https://github.com/sst-soft/SVG which is a fork of https://github.com/svg-net/SVG.

namespace Svg
{
    [AttributeUsage(AttributeTargets.Class)]
    public sealed class SvgElementAttribute : Attribute
    {
        public string ElementName { get; private set; }

        public SvgElementAttribute(string elementName)
        {
            ElementName = elementName;
        }
    }
}
