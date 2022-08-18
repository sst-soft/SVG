// todo: add license

using System.ComponentModel;
using System.Drawing;

namespace Svg
{
    [TypeConverter(typeof(SvgPaintServerFactory))]
    public abstract class SvgPaintServer : SvgElement
    {
        public static readonly SvgPaintServer None = new SvgColourServer();
        public static readonly SvgPaintServer Inherit = new SvgColourServer();
        public static readonly SvgPaintServer NotSet = new SvgColourServer();

        public Func<SvgPaintServer> GetCallback { get; set; }

        protected override void Render(ISvgRenderer renderer)
        {
        }

        public abstract Brush GetBrush(
          SvgVisualElement styleOwner,
          ISvgRenderer renderer,
          float opacity,
          bool forStroke = false);

        public override string ToString()
        {
            return string.Format("url(#{0})", ID);
        }
    }
}
