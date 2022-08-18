// todo: add license

namespace Svg
{
    public sealed class SvgClipRuleConverter : EnumBaseConverter<SvgClipRule>
    {
        public SvgClipRuleConverter()
          : base(SvgClipRule.NonZero, EnumBaseConverter<SvgClipRule>.CaseHandling.LowerCase)
        {
        }
    }
}
