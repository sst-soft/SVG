// Microsoft Public License (Ms-PL). https://github.com/sst-soft/SVG which is a fork of https://github.com/svg-net/SVG.

namespace Svg
{
    public sealed class SvgFillRuleConverter : EnumBaseConverter<SvgFillRule>
    {
        public SvgFillRuleConverter()
          : base(SvgFillRule.NonZero, EnumBaseConverter<SvgFillRule>.CaseHandling.LowerCase)
        {
        }
    }
}
