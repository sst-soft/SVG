// Microsoft Public License (Ms-PL). https://github.com/sst-soft/SVG which is a fork of https://github.com/svg-net/SVG.

namespace Svg
{
    public abstract class SvgKern : SvgElement
    {
        [SvgAttribute("g1")]
        public string Glyph1
        {
            get => GetAttribute<string>("g1", true);
            set => Attributes["g1"] = value;
        }

        [SvgAttribute("g2")]
        public string Glyph2
        {
            get => GetAttribute<string>("g2", true);
            set => Attributes["g2"] = value;
        }

        [SvgAttribute("u1")]
        public string Unicode1
        {
            get => GetAttribute<string>("u1", true);
            set => Attributes["u1"] = value;
        }

        [SvgAttribute("u2")]
        public string Unicode2
        {
            get => GetAttribute<string>("u2", true);
            set => Attributes["u2"] = value;
        }

        [SvgAttribute("k")]
        public float Kerning
        {
            get => GetAttribute<float>("k", true);
            set => Attributes["k"] = value;
        }
    }
}
