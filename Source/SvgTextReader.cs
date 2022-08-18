// todo: add license

using System.Xml;

namespace Svg
{
    internal class SvgTextReader : XmlTextReader
    {
        private Dictionary<string, string> _entities;
        private string _value;
        private bool _customValue;
        private string _localName;

        public SvgTextReader(Stream stream, Dictionary<string, string> entities)
          : base(stream)
        {
            EntityHandling = (EntityHandling)1;
            _entities = entities;
        }

        public SvgTextReader(TextReader reader, Dictionary<string, string> entities)
          : base(reader)
        {
            EntityHandling = (EntityHandling)1;
            _entities = entities;
        }

        public override string Value => !_customValue ? base.Value : _value;

        public override string LocalName => !_customValue ? base.LocalName : _localName;

        private IDictionary<string, string> Entities
        {
            get
            {
                if (_entities == null)
                {
                    _entities = new Dictionary<string, string>();
                }

                return _entities;
            }
        }

        public override bool MoveToNextAttribute()
        {
            var num = base.MoveToNextAttribute() ? 1 : 0;
            if (num == 0)
            {
                return num != 0;
            }

            _localName = base.LocalName;
            if (ReadAttributeValue())
            {
                if (NodeType == XmlNodeType.EntityReference)
                {
                    ResolveEntity();
                }
                else
                {
                    _value = base.Value;
                }
            }
            _customValue = true;
            return num != 0;
        }

        public override bool Read()
        {
            _customValue = false;
            var num = base.Read() ? 1 : 0;
            if (NodeType != XmlNodeType.DocumentType)
            {
                return num != 0;
            }

            ParseEntities();
            return num != 0;
        }

        private void ParseEntities()
        {
            var strArray = Value.Split(new string[1]
            {
        "<!ENTITY"
            }, StringSplitOptions.None);
            foreach (var str1 in strArray)
            {
                if (!string.IsNullOrEmpty(str1.Trim()))
                {
                    var str2 = str1.Trim();
                    var length = str2.IndexOf(QuoteChar);
                    if (length > 0)
                    {
                        var str3 = str2.Substring(length + 1, str2.LastIndexOf(QuoteChar) - length - 1);
                        Entities.Add(str2.Substring(0, length).Trim(), str3);
                    }
                }
            }
        }

        public override void ResolveEntity()
        {
            if (NodeType != XmlNodeType.EntityReference)
            {
                return;
            }

            _value = !_entities.ContainsKey(Name) ? string.Empty : _entities[Name];
            _customValue = true;
        }
    }
}
