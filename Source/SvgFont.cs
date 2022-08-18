// Microsoft Public License (Ms-PL). https://github.com/sst-soft/SVG which is a fork of https://github.com/svg-net/SVG.

namespace Svg
{
    [SvgElement("font")]
    public class SvgFont : SvgElement
    {
        [SvgAttribute("horiz-adv-x")]
        public float HorizAdvX
        {
            get => GetAttribute<float>("horiz-adv-x", true);
            set => Attributes["horiz-adv-x"] = value;
        }

        [SvgAttribute("horiz-origin-x")]
        public float HorizOriginX
        {
            get => GetAttribute<float>("horiz-origin-x", true);
            set => Attributes["horiz-origin-x"] = value;
        }

        [SvgAttribute("horiz-origin-y")]
        public float HorizOriginY
        {
            get => GetAttribute<float>("horiz-origin-y", true);
            set => Attributes["horiz-origin-y"] = value;
        }

        [SvgAttribute("vert-adv-y")]
        public float VertAdvY
        {
            get => GetAttribute<float>("vert-adv-y", true, Children.OfType<SvgFontFace>().First<SvgFontFace>().UnitsPerEm);
            set => Attributes["vert-adv-y"] = value;
        }

        [SvgAttribute("vert-origin-x")]
        public float VertOriginX
        {
            get => GetAttribute<float>("vert-origin-x", true, HorizAdvX / 2f);
            set => Attributes["vert-origin-x"] = value;
        }

        [SvgAttribute("vert-origin-y")]
        public float VertOriginY
        {
            get => GetAttribute<float>("vert-origin-y", true, (Children.OfType<SvgFontFace>().First<SvgFontFace>().Attributes["ascent"] as float?).GetValueOrDefault());
            set => Attributes["vert-origin-y"] = value;
        }

        public override SvgElement DeepCopy()
        {
            return DeepCopy<SvgFont>();
        }

        protected override void Render(ISvgRenderer renderer)
        {
        }
    }
}
