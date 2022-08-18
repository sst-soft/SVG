// todo: add license

using System.Drawing;

namespace Svg
{
    public sealed class SvgColourServer : SvgPaintServer
    {
        private Color _colour;

        public SvgColourServer()
          : this(System.Drawing.Color.Black)
        {
        }

        public SvgColourServer(Color colour)
        {
            _colour = colour;
        }

        public Color Colour
        {
            get => _colour;
            set => _colour = value;
        }

        public override Brush GetBrush(
          SvgVisualElement styleOwner,
          ISvgRenderer renderer,
          float opacity,
          bool forStroke = false)
        {
            if (this == SvgPaintServer.None)
            {
                return new SolidBrush(System.Drawing.Color.Transparent);
            }

            return this == SvgPaintServer.NotSet & forStroke ? new SolidBrush(System.Drawing.Color.Transparent) : (Brush)new SolidBrush(System.Drawing.Color.FromArgb((int)Math.Round((double)opacity * (Colour.A / (double)byte.MaxValue) * byte.MaxValue), Colour));
        }

        public override string ToString()
        {
            if (this == SvgPaintServer.None)
            {
                return "none";
            }

            if (this == SvgPaintServer.NotSet)
            {
                return string.Empty;
            }

            if (this == SvgPaintServer.Inherit)
            {
                return "inherit";
            }

            Color colour = Colour;
            return colour.IsKnownColor ? colour.Name : string.Format("#{0}", colour.ToArgb().ToString("x8").Substring(2));
        }

        public override SvgElement DeepCopy()
        {
            return DeepCopy<SvgColourServer>();
        }

        public override SvgElement DeepCopy<T>()
        {
            if (this == SvgPaintServer.None || this == SvgPaintServer.Inherit || this == SvgPaintServer.NotSet)
            {
                return this;
            }

            SvgColourServer svgColourServer = base.DeepCopy<T>() as SvgColourServer;
            svgColourServer.Colour = Colour;
            return svgColourServer;
        }

        public override bool Equals(object obj)
        {
            return obj is SvgColourServer svgColourServer && (this != SvgPaintServer.None || obj == SvgPaintServer.None) && (this == SvgPaintServer.None || obj != SvgPaintServer.None) && (this != SvgPaintServer.NotSet || obj == SvgPaintServer.NotSet) && (this == SvgPaintServer.NotSet || obj != SvgPaintServer.NotSet) && (this != SvgPaintServer.Inherit || obj == SvgPaintServer.Inherit) && (this == SvgPaintServer.Inherit || obj != SvgPaintServer.Inherit) && GetHashCode() == svgColourServer.GetHashCode();
        }

        public override int GetHashCode()
        {
            return _colour.GetHashCode();
        }
    }
}
