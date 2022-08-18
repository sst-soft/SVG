// todo: add license

namespace Svg
{
    [AttributeUsage(AttributeTargets.Property | AttributeTargets.Event)]
    public class SvgAttributeAttribute : Attribute
    {
        public const string SvgNamespace = "http://www.w3.org/2000/svg";
        public const string XLinkPrefix = "xlink";
        public const string XLinkNamespace = "http://www.w3.org/1999/xlink";
        public const string XmlPrefix = "xml";
        public const string XmlNamespace = "http://www.w3.org/XML/1998/namespace";
        public static readonly List<KeyValuePair<string, string>> Namespaces = new List<KeyValuePair<string, string>>()
    {
      new KeyValuePair<string, string>(string.Empty, "http://www.w3.org/2000/svg"),
      new KeyValuePair<string, string>("xlink", "http://www.w3.org/1999/xlink"),
      new KeyValuePair<string, string>("xml", "http://www.w3.org/XML/1998/namespace")
    };

        public override bool Equals(object obj)
        {
            if (!(obj is SvgAttributeAttribute))
            {
                return false;
            }

            SvgAttributeAttribute attributeAttribute = (SvgAttributeAttribute)obj;
            return !(attributeAttribute.Name == string.Empty) && string.Equals(Name, attributeAttribute.Name);
        }

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }

        public string NamespaceAndName => NameSpace == "http://www.w3.org/2000/svg" ? Name : SvgAttributeAttribute.Namespaces.First<KeyValuePair<string, string>>(x => x.Value == NameSpace).Key + ":" + Name;

        public string Name { get; private set; }

        public string NameSpace { get; private set; }

        internal SvgAttributeAttribute()
        {
            Name = string.Empty;
        }

        internal SvgAttributeAttribute(string name)
        {
            Name = name;
            NameSpace = "http://www.w3.org/2000/svg";
        }

        public SvgAttributeAttribute(string name, string nameSpace)
        {
            Name = name;
            NameSpace = nameSpace;
        }
    }
}
