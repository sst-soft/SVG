// todo: add license

using System.Reflection;
using System.Xml;

namespace Svg
{
    internal class SvgDtdResolver : XmlUrlResolver
    {
        public override object GetEntity(Uri absoluteUri, string role, Type ofObjectToReturn)
        {
            return absoluteUri.ToString().IndexOf("svg", StringComparison.InvariantCultureIgnoreCase) > -1 ? Assembly.GetExecutingAssembly().GetManifestResourceStream("SST.Svg.Resources.svg11.dtd") : base.GetEntity(absoluteUri, role, ofObjectToReturn);
        }
    }
}
