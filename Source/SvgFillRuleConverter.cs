// todo: add license

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
