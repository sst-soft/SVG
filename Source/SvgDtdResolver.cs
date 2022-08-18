// Microsoft Public License (Ms-PL). https://github.com/sst-soft/SVG which is a fork of https://github.com/svg-net/SVG.

using System.Reflection;
using System.Xml;

namespace Svg
{
    internal class SvgDtdResolver : XmlUrlResolver
    {
        public override object GetEntity(Uri absoluteUri, string role, Type ofObjectToReturn)
        {
            return absoluteUri.ToString().IndexOf("svg", StringComparison.InvariantCultureIgnoreCase) > -1 ? Assembly.GetExecutingAssembly().GetManifestResourceStream("Svg.Resources.svg11.dtd") : base.GetEntity(absoluteUri, role, ofObjectToReturn);
        }
    }
}
