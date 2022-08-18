// Microsoft Public License (Ms-PL). https://github.com/sst-soft/SVG which is a fork of https://github.com/svg-net/SVG.

namespace Svg.FilterEffects
{
    [SvgElement("feMergeNode")]
    public class SvgMergeNode : SvgElement
    {
        [SvgAttribute("in")]
        public string Input
        {
            get => GetAttribute<string>("in", false);
            set => Attributes["in"] = value;
        }

        public override SvgElement DeepCopy()
        {
            return DeepCopy<SvgMergeNode>();
        }
    }
}
