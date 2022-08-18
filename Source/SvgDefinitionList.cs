// todo: add license

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
