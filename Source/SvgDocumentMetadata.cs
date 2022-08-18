// todo: add license

using System.Xml;

namespace Svg
{
    [SvgElement("metadata")]
    public class SvgDocumentMetadata : SvgElement
    {
        public SvgDocumentMetadata()
        {
            Content = "";
        }

        protected override void Render(ISvgRenderer renderer)
        {
        }

        protected override void WriteChildren(XmlTextWriter writer)
        {
            writer.WriteRaw(Content);
        }

        public override SvgElement DeepCopy()
        {
            return DeepCopy<SvgDocumentMetadata>();
        }

        public override void InitialiseFromXML(XmlTextReader reader, SvgDocument document)
        {
            base.InitialiseFromXML(reader, document);
            Content = reader.ReadInnerXml();
        }
    }
}
