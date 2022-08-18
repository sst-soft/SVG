// todo: add license

using System.Xml;

namespace Svg
{
    internal class SvgNodeReader : XmlNodeReader
    {
        private readonly Dictionary<string, string> _entities;
        private string _value;
        private bool _customValue;
        private string _localName;

        public SvgNodeReader(XmlNode node, Dictionary<string, string> entities)
          : base(node)
        {
            _entities = entities ?? new Dictionary<string, string>();
        }

        public override string Value => !_customValue ? base.Value : _value;

        public override string LocalName => !_customValue ? base.LocalName : _localName;

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
            var strArray1 = Value.Split(new string[1]
            {
        "<!ENTITY"
            }, StringSplitOptions.None);
            foreach (var str in strArray1)
            {
                if (!string.IsNullOrEmpty(str.Trim()))
                {
                    var strArray2 = str.Trim().Split(new char[2]
                    {
            ' ',
            '\t'
                    }, StringSplitOptions.RemoveEmptyEntries);
                    _entities.Add(strArray2[0], strArray2[1].Split(new char[1]
                    {
             QuoteChar
                    }, StringSplitOptions.RemoveEmptyEntries)[0]);
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
