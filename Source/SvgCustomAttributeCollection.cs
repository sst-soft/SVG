// todo: add license

namespace Svg
{
    public sealed class SvgCustomAttributeCollection : Dictionary<string, string>
    {
        private readonly SvgElement _owner;

        public SvgCustomAttributeCollection(SvgElement owner)
        {
            _owner = owner;
        }

        public new string this[string attributeName]
        {
            get => base[attributeName];
            set
            {
                if (ContainsKey(attributeName))
                {
                    var str1 = base[attributeName];
                    base[attributeName] = value;
                    var str2 = value;
                    if (!(str1 != str2))
                    {
                        return;
                    }

                    OnAttributeChanged(attributeName, value);
                }
                else
                {
                    base[attributeName] = value;
                    OnAttributeChanged(attributeName, value);
                }
            }
        }

        public event EventHandler<AttributeEventArgs> AttributeChanged;

        private void OnAttributeChanged(string attribute, object value)
        {
            EventHandler<AttributeEventArgs> attributeChanged = AttributeChanged;
            if (attributeChanged == null)
            {
                return;
            }

            attributeChanged(_owner, new AttributeEventArgs()
            {
                Attribute = attribute,
                Value = value
            });
        }
    }
}
