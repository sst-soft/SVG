// Microsoft Public License (Ms-PL). https://github.com/sst-soft/SVG which is a fork of https://github.com/svg-net/SVG.

namespace Svg.FilterEffects
{
    public abstract class SvgFilterPrimitive : SvgElement
    {
        public const string SourceGraphic = "SourceGraphic";
        public const string SourceAlpha = "SourceAlpha";
        public const string BackgroundImage = "BackgroundImage";
        public const string BackgroundAlpha = "BackgroundAlpha";
        public const string FillPaint = "FillPaint";
        public const string StrokePaint = "StrokePaint";

        [SvgAttribute("in")]
        public string Input
        {
            get => GetAttribute<string>("in", false);
            set => Attributes["in"] = value;
        }

        [SvgAttribute("result")]
        public string Result
        {
            get => GetAttribute<string>("result", false);
            set => Attributes["result"] = value;
        }

        protected SvgFilter Owner => (SvgFilter)Parent;

        public abstract void Process(ImageBuffer buffer);
    }
}
