// todo: add license

using System.ComponentModel;
using System.Diagnostics;
using System.Globalization;
using System.Xml;
using ExCSS;

namespace Svg
{
    internal class SvgElementFactory
    {
        private Dictionary<string, SvgElementFactory.ElementInfo> availableElements;
        private readonly Parser cssParser = new Parser();
        private static readonly Dictionary<Type, Dictionary<string, PropertyDescriptorCollection>> _propertyDescriptors = new Dictionary<Type, Dictionary<string, PropertyDescriptorCollection>>();
        private static readonly object syncLock = new object();

        public Dictionary<string, SvgElementFactory.ElementInfo> AvailableElements
        {
            get
            {
                if (availableElements == null)
                {
                    availableElements = typeof(SvgDocument).Assembly.GetExportedTypes().Where<Type>(t => t.GetCustomAttributes(typeof(SvgElementAttribute), true).Length != 0 && t.IsSubclassOf(typeof(SvgElement))).Select<Type, SvgElementFactory.ElementInfo>(t => new SvgElementFactory.ElementInfo()
                    {
                        ElementName = ((SvgElementAttribute)t.GetCustomAttributes(typeof(SvgElementAttribute), true)[0]).ElementName,
                        ElementType = t
                    }).Where<SvgElementFactory.ElementInfo>(t => t.ElementName != "svg").GroupBy<SvgElementFactory.ElementInfo, string>(t => t.ElementName).Select<IGrouping<string, SvgElementFactory.ElementInfo>, IGrouping<string, SvgElementFactory.ElementInfo>>(types => types).ToDictionary<IGrouping<string, SvgElementFactory.ElementInfo>, string, SvgElementFactory.ElementInfo>(e => e.Key, e => e.SingleOrDefault<SvgElementFactory.ElementInfo>());
                }

                return availableElements;
            }
        }

        public T CreateDocument<T>(XmlReader reader) where T : SvgDocument, new()
        {
            if (reader == null)
            {
                throw new ArgumentNullException(nameof(reader));
            }

            return !(reader.LocalName != "svg") ? (T)CreateElement<T>(reader, true, null) : throw new InvalidOperationException("The CreateDocument method can only be used to parse root <svg> elements.");
        }

        public SvgElement CreateElement(XmlReader reader, SvgDocument document)
        {
            return reader != null ? CreateElement<SvgDocument>(reader, false, document) : throw new ArgumentNullException(nameof(reader));
        }

        private SvgElement CreateElement<T>(
          XmlReader reader,
          bool fragmentIsDocument,
          SvgDocument document)
          where T : SvgDocument, new()
        {
            var localName = reader.LocalName;
            var namespaceUri = reader.NamespaceURI;
            SvgElement element;
            if (namespaceUri == "http://www.w3.org/2000/svg" || string.IsNullOrEmpty(namespaceUri))
            {
                if (localName == "svg")
                {
                    element = fragmentIsDocument ? new T() : (SvgElement)new SvgFragment();
                }
                else
                {
                    element = !AvailableElements.TryGetValue(localName, out ElementInfo elementInfo) ? new SvgUnknownElement(localName) : (SvgElement)Activator.CreateInstance(elementInfo.ElementType);
                }
                if (element != null)
                {
                    SetAttributes(element, reader, document);
                }
            }
            else
            {
                element = new NonSvgElement(localName);
                SetAttributes(element, reader, document);
            }
            return element;
        }

        private void SetAttributes(SvgElement element, XmlReader reader, SvgDocument document)
        {
            while (reader.MoveToNextAttribute())
            {
                if (reader.LocalName.Equals("style") && !(element is NonSvgElement))
                {
                    foreach (StyleRule styleRule in (IEnumerable<StyleRule>)cssParser.Parse("#a{" + reader.Value + "}").StyleRules)
                    {
                        foreach (Property declaration in styleRule.Declarations)
                        {
                            element.AddStyle(declaration.Name, declaration.Term.ToString(), 65536);
                        }
                    }
                }
                else if (SvgElementFactory.IsStyleAttribute(reader.LocalName))
                {
                    element.AddStyle(reader.LocalName, reader.Value, 0);
                }
                else
                {
                    SvgElementFactory.SetPropertyValue(element, reader.LocalName, reader.Value, document);
                }
            }
        }

        private static bool IsStyleAttribute(string name)
        {
            switch (name)
            {
                case "alignment-baseline":
                case "baseline-shift":
                case "clip":
                case "clip-path":
                case "clip-rule":
                case "color":
                case "color-interpolation":
                case "color-interpolation-filters":
                case "color-profile":
                case "color-rendering":
                case "cursor":
                case "direction":
                case "display":
                case "dominant-baseline":
                case "enable-background":
                case "fill":
                case "fill-opacity":
                case "fill-rule":
                case "filter":
                case "flood-color":
                case "flood-opacity":
                case "font":
                case "font-family":
                case "font-size":
                case "font-size-adjust":
                case "font-stretch":
                case "font-style":
                case "font-variant":
                case "font-weight":
                case "glyph-orientation-horizontal":
                case "glyph-orientation-vertical":
                case "image-rendering":
                case "kerning":
                case "letter-spacing":
                case "lighting-color":
                case "marker":
                case "marker-end":
                case "marker-mid":
                case "marker-start":
                case "mask":
                case "opacity":
                case "overflow":
                case "pointer-events":
                case "shape-rendering":
                case "stop-color":
                case "stop-opacity":
                case "stroke":
                case "stroke-dasharray":
                case "stroke-dashoffset":
                case "stroke-linecap":
                case "stroke-linejoin":
                case "stroke-miterlimit":
                case "stroke-opacity":
                case "stroke-width":
                case "text-anchor":
                case "text-decoration":
                case "text-rendering":
                case "text-transform":
                case "unicode-bidi":
                case "visibility":
                case "word-spacing":
                case "writing-mode":
                    return true;
                default:
                    return false;
            }
        }

        internal static bool SetPropertyValue(
          SvgElement element,
          string attributeName,
          string attributeValue,
          SvgDocument document,
          bool isStyle = false)
        {
            Type type = element.GetType();
            PropertyDescriptorCollection properties;
            lock (SvgElementFactory.syncLock)
            {
                if (SvgElementFactory._propertyDescriptors.Keys.Contains<Type>(type))
                {
                    if (SvgElementFactory._propertyDescriptors[type].Keys.Contains<string>(attributeName))
                    {
                        properties = SvgElementFactory._propertyDescriptors[type][attributeName];
                    }
                    else
                    {
                        properties = TypeDescriptor.GetProperties(type, (Attribute[])new SvgAttributeAttribute[1]
                        {
              new SvgAttributeAttribute(attributeName)
                        });
                        SvgElementFactory._propertyDescriptors[type].Add(attributeName, properties);
                    }
                }
                else
                {
                    properties = TypeDescriptor.GetProperties(type, (Attribute[])new SvgAttributeAttribute[1]
                    {
            new SvgAttributeAttribute(attributeName)
                    });
                    SvgElementFactory._propertyDescriptors.Add(type, new Dictionary<string, PropertyDescriptorCollection>());
                    SvgElementFactory._propertyDescriptors[type].Add(attributeName, properties);
                }
            }
            if (properties.Count > 0)
            {
                PropertyDescriptor propertyDescriptor = properties[0];
                try
                {
                    if (attributeName == "opacity" && attributeValue == "undefined")
                    {
                        attributeValue = "1";
                    }

                    propertyDescriptor.SetValue(element, propertyDescriptor.Converter.ConvertFrom(document, CultureInfo.InvariantCulture, attributeValue));
                }
                catch
                {
                    Trace.TraceWarning(string.Format("Attribute '{0}' cannot be set - type '{1}' cannot convert from string '{2}'.", attributeName, propertyDescriptor.PropertyType.FullName, attributeValue));
                }
            }
            else if (string.Equals(element.ElementName, "svg", StringComparison.OrdinalIgnoreCase))
            {
                if (!string.Equals(attributeName, "xmlns", StringComparison.OrdinalIgnoreCase) && !string.Equals(attributeName, "xlink", StringComparison.OrdinalIgnoreCase) && !string.Equals(attributeName, "xmlns:xlink", StringComparison.OrdinalIgnoreCase) && !string.Equals(attributeName, "version", StringComparison.OrdinalIgnoreCase))
                {
                    element.CustomAttributes[attributeName] = attributeValue;
                }
            }
            else
            {
                if (isStyle)
                {
                    return false;
                }

                element.CustomAttributes[attributeName] = attributeValue;
            }
            return true;
        }

        [DebuggerDisplay("{ElementName}, {ElementType}")]
        internal sealed class ElementInfo
        {
            public string ElementName { get; set; }

            public Type ElementType { get; set; }

            public ElementInfo(string elementName, Type elementType)
            {
                ElementName = elementName;
                ElementType = elementType;
            }

            public ElementInfo()
            {
            }
        }
    }
}
