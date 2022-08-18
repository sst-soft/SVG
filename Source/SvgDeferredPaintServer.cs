// Microsoft Public License (Ms-PL). https://github.com/sst-soft/SVG which is a fork of https://github.com/svg-net/SVG.

using System.ComponentModel;
using System.Drawing;
using System.Text;

namespace Svg
{
    [TypeConverter(typeof(SvgDeferredPaintServerFactory))]
    public class SvgDeferredPaintServer : SvgPaintServer
    {
        private bool _serverLoaded;
        private SvgPaintServer _concreteServer;
        private SvgPaintServer _fallbackServer;

        [Obsolete("Will be removed.")]
        public SvgDocument Document { get; set; }

        public string DeferredId { get; set; }

        public SvgPaintServer FallbackServer { get; private set; }

        public SvgDeferredPaintServer()
        {
        }

        [Obsolete("Will be removed.")]
        public SvgDeferredPaintServer(SvgDocument document, string id)
        {
            Document = document;
            DeferredId = id;
        }

        public SvgDeferredPaintServer(string id)
          : this(id, null)
        {
        }

        public SvgDeferredPaintServer(string id, SvgPaintServer fallbackServer)
        {
            DeferredId = id;
            FallbackServer = fallbackServer;
        }

        public void EnsureServer(SvgElement styleOwner)
        {
            if (_serverLoaded || styleOwner == null)
            {
                return;
            }

            if (DeferredId == "currentColor")
            {
                _concreteServer = styleOwner.ParentsAndSelf.OfType<SvgElement>().Where<SvgElement>(e => e.Color != SvgPaintServer.None && e.Color != SvgPaintServer.NotSet && e.Color != SvgPaintServer.Inherit).FirstOrDefault<SvgElement>()?.Color;
            }
            else
            {
                _concreteServer = styleOwner.OwnerDocument.IdManager.GetElementById(DeferredId) as SvgPaintServer;
                _fallbackServer = FallbackServer;
                if (!(_fallbackServer is SvgColourServer) && (!(_fallbackServer is SvgDeferredPaintServer) || !string.Equals(((SvgDeferredPaintServer)_fallbackServer).DeferredId, "currentColor")))
                {
                    _fallbackServer = SvgPaintServer.Inherit;
                }
            }
            _serverLoaded = true;
        }

        public override Brush GetBrush(
          SvgVisualElement styleOwner,
          ISvgRenderer renderer,
          float opacity,
          bool forStroke = false)
        {
            EnsureServer(styleOwner);
            return (_concreteServer ?? _fallbackServer ?? SvgPaintServer.NotSet).GetBrush(styleOwner, renderer, opacity, forStroke);
        }

        public override SvgElement DeepCopy()
        {
            return DeepCopy<SvgDeferredPaintServer>();
        }

        public override SvgElement DeepCopy<T>()
        {
            SvgDeferredPaintServer deferredPaintServer = base.DeepCopy<T>() as SvgDeferredPaintServer;
            deferredPaintServer.DeferredId = DeferredId;
            deferredPaintServer.FallbackServer = FallbackServer?.DeepCopy() as SvgPaintServer;
            return deferredPaintServer;
        }

        public override bool Equals(object obj)
        {
            return obj is SvgDeferredPaintServer deferredPaintServer && DeferredId == deferredPaintServer.DeferredId;
        }

        public override int GetHashCode()
        {
            return DeferredId != null ? DeferredId.GetHashCode() : 0;
        }

        public override string ToString()
        {
            if (string.IsNullOrEmpty(DeferredId))
            {
                return string.Empty;
            }

            return FallbackServer == null ? DeferredId : new StringBuilder(DeferredId).Append(" ").Append(FallbackServer.ToString()).ToString();
        }

        public static T TryGet<T>(SvgPaintServer server, SvgElement parent) where T : SvgPaintServer
        {
            if (!(server is SvgDeferredPaintServer))
            {
                return server as T;
            }

            SvgDeferredPaintServer deferredPaintServer = (SvgDeferredPaintServer)server;
            deferredPaintServer.EnsureServer(parent);
            return (deferredPaintServer._concreteServer ?? deferredPaintServer._fallbackServer) as T;
        }
    }
}
