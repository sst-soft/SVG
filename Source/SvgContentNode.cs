// Microsoft Public License (Ms-PL). https://github.com/sst-soft/SVG which is a fork of https://github.com/svg-net/SVG.

namespace Svg
{
    public class SvgContentNode : ISvgNode
    {
        public string Content { get; set; }

        public ISvgNode DeepCopy()
        {
            return new SvgContentNode()
            {
                Content = Content
            };
        }
    }
}
