// Microsoft Public License (Ms-PL). https://github.com/sst-soft/SVG which is a fork of https://github.com/svg-net/SVG.

namespace Svg
{
    [SvgElement("defs")]
    public class SvgDefinitionList : SvgElement
    {
        protected override void Render(ISvgRenderer renderer)
        {
        }

        public override SvgElement DeepCopy()
        {
            return DeepCopy<SvgDefinitionList>();
        }
    }
}
