// Microsoft Public License (Ms-PL). https://github.com/sst-soft/SVG which is a fork of https://github.com/svg-net/SVG.

using System.Drawing;

namespace Svg
{
    [Obsolete("Will be removed.Use SvgDeferredPaintServer class instead.")]
    public class SvgFallbackPaintServer : SvgPaintServer
    {
        private IEnumerable<SvgPaintServer> _fallbacks;
        private SvgPaintServer _primary;

        public SvgFallbackPaintServer()
        {
        }

        public SvgFallbackPaintServer(SvgPaintServer primary, IEnumerable<SvgPaintServer> fallbacks)
          : this()
        {
            _fallbacks = fallbacks;
            _primary = primary;
        }

        public override Brush GetBrush(
          SvgVisualElement styleOwner,
          ISvgRenderer renderer,
          float opacity,
          bool forStroke = false)
        {
            try
            {
                _primary.GetCallback = () => _fallbacks.FirstOrDefault<SvgPaintServer>();
                return _primary.GetBrush(styleOwner, renderer, opacity, forStroke);
            }
            finally
            {
                _primary.GetCallback = null;
            }
        }

        public override SvgElement DeepCopy()
        {
            return base.DeepCopy<SvgFallbackPaintServer>();
        }

        public override SvgElement DeepCopy<T>()
        {
            SvgFallbackPaintServer fallbackPaintServer = base.DeepCopy<T>() as SvgFallbackPaintServer;
            fallbackPaintServer._fallbacks = _fallbacks;
            fallbackPaintServer._primary = _primary;
            return fallbackPaintServer;
        }
    }
}
