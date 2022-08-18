// Microsoft Public License (Ms-PL). https://github.com/sst-soft/SVG which is a fork of https://github.com/svg-net/SVG.

using System.Xml;

namespace Svg
{
    [SvgElement("script")]
    public class SvgScript : SvgElement
    {
        public string Script
        {
            get => Content;
            set => Content = value;
        }

        [SvgAttribute("type")]
        public string ScriptType
        {
            get => GetAttribute<string>("type", false);
            set => Attributes["type"] = value;
        }

        [SvgAttribute("crossorigin")]
        public string CrossOrigin
        {
            get => GetAttribute<string>("crossorigin", false);
            set => Attributes["crossorigin"] = value;
        }

        [SvgAttribute("href", "http://www.w3.org/1999/xlink")]
        public string Href
        {
            get => GetAttribute<string>("href", false);
            set => Attributes["href"] = value;
        }

        public override SvgElement DeepCopy()
        {
            return DeepCopy<SvgScript>();
        }

        protected override void WriteChildren(XmlTextWriter writer)
        {
            if (string.IsNullOrEmpty(Content))
            {
                return;
            }
            writer.WriteCData(Content);
        }
    }
}
