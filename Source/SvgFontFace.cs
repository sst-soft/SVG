// Microsoft Public License (Ms-PL). https://github.com/sst-soft/SVG which is a fork of https://github.com/svg-net/SVG.

namespace Svg
{
    [SvgElement("font-face")]
    public class SvgFontFace : SvgElement
    {
        [SvgAttribute("alphabetic")]
        public float Alphabetic
        {
            get => GetAttribute<float>("alphabetic", true);
            set => Attributes["alphabetic"] = value;
        }

        [SvgAttribute("ascent")]
        public float Ascent
        {
            get => GetAttribute<float>("ascent", true, Parent is SvgFont ? UnitsPerEm - ((SvgFont)Parent).VertOriginY : 0.0f);
            set => Attributes["ascent"] = value;
        }

        [SvgAttribute("ascent-height")]
        public float AscentHeight
        {
            get => GetAttribute<float>("ascent-height", true, Ascent);
            set => Attributes["ascent-height"] = value;
        }

        [SvgAttribute("descent")]
        public float Descent
        {
            get => GetAttribute<float>("descent", true, Parent is SvgFont ? ((SvgFont)Parent).VertOriginY : 0.0f);
            set => Attributes["descent"] = value;
        }

        [SvgAttribute("panose-1")]
        public string Panose1
        {
            get => GetAttribute<string>("panose-1", true);
            set => Attributes["panose-1"] = value;
        }

        [SvgAttribute("units-per-em")]
        public float UnitsPerEm
        {
            get => GetAttribute<float>("units-per-em", true, 1000f);
            set => Attributes["units-per-em"] = value;
        }

        [SvgAttribute("x-height")]
        public float XHeight
        {
            get => GetAttribute<float>("x-height", true, float.MinValue);
            set => Attributes["x-height"] = value;
        }

        public override SvgElement DeepCopy()
        {
            return DeepCopy<SvgFontFace>();
        }
    }
}
