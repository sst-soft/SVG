// todo: add license

namespace Svg
{
    public sealed class SvgAttributeCollection : Dictionary<string, object>
    {
        private readonly SvgElement _owner;

        public SvgAttributeCollection(SvgElement owner)
        {
            _owner = owner;
        }

        public TAttributeType GetAttribute<TAttributeType>(
          string attributeName,
          TAttributeType defaultValue = default(TAttributeType))//PIX = null)
        {
            return ContainsKey(attributeName) && base[attributeName] != null ? (TAttributeType)base[attributeName] : defaultValue;
        }

        public TAttributeType GetInheritedAttribute<TAttributeType>(
          string attributeName,
          bool inherited,
          TAttributeType defaultValue = default(TAttributeType))//PIX = null)
        {
            var flag = false;
            if (ContainsKey(attributeName))
            {
                TAttributeType inheritedAttribute = (TAttributeType)base[attributeName];
                if (IsInheritValue(inheritedAttribute))
                {
                    flag = true;
                }
                else
                {
                    if (!(inheritedAttribute is SvgDeferredPaintServer server) || SvgDeferredPaintServer.TryGet<SvgPaintServer>(server, _owner) != SvgPaintServer.Inherit)
                    {
                        return inheritedAttribute;
                    }

                    flag = true;
                }
            }
            if (inherited | flag)
            {
                var inheritedAttribute = _owner.Parent?.Attributes.GetInheritedAttribute<object>(attributeName, inherited);
                if (inheritedAttribute != null)
                {
                    return (TAttributeType)inheritedAttribute;
                }
            }
            return defaultValue;
        }

        private bool IsInheritValue(object value)
        {
            return string.Equals(value?.ToString(), "inherit", StringComparison.OrdinalIgnoreCase);
        }

        public new object this[string attributeName]
        {
            get => GetInheritedAttribute<object>(attributeName, true);
            set
            {
                if (ContainsKey(attributeName))
                {
                    if (!TryUnboxedCheck(base[attributeName], value))
                    {
                        return;
                    }

                    base[attributeName] = value;
                    OnAttributeChanged(attributeName, value);
                }
                else
                {
                    base[attributeName] = value;
                    OnAttributeChanged(attributeName, value);
                }
            }
        }

        private bool TryUnboxedCheck(object a, object b)
        {
            if (!IsValueType(a))
            {
                return a != b;
            }

            switch (a)
            {
                case SvgUnit _:
                    return UnboxAndCheck<SvgUnit>(a, b);
                case bool _:
                    return UnboxAndCheck<bool>(a, b);
                case int _:
                    return UnboxAndCheck<int>(a, b);
                case float _:
                    return UnboxAndCheck<float>(a, b);
                case SvgViewBox _:
                    return UnboxAndCheck<SvgViewBox>(a, b);
                default:
                    return true;
            }
        }

        private bool UnboxAndCheck<T>(object a, object b)
        {
            return !((T)a).Equals((T)b);
        }

        private bool IsValueType(object obj)
        {
            return obj != null && obj.GetType().IsValueType;
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
