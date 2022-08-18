// todo: add license

using System.ComponentModel;
using System.Globalization;

namespace Svg
{
    public class EnumBaseConverter<T> : BaseConverter where T : struct
    {
        public EnumBaseConverter<T>.CaseHandling CaseHandlingMode { get; }

        public T? DefaultValue { get; protected set; }

        public EnumBaseConverter()
        {
        }

        public EnumBaseConverter(T defaultValue, EnumBaseConverter<T>.CaseHandling caseHandling = EnumBaseConverter<T>.CaseHandling.CamelCase)
        {
            DefaultValue = new T?(defaultValue);
            CaseHandlingMode = caseHandling;
        }

        public override object ConvertFrom(
          ITypeDescriptorContext context,
          CultureInfo culture,
          object value)
        {
            if (value == null)
            {
                return DefaultValue.HasValue ? DefaultValue.Value : Activator.CreateInstance(typeof(T));
            }

            return value is string ? (object)(T)Enum.Parse(typeof(T), (string)value, true) : throw new ArgumentOutOfRangeException("value must be a string.");
        }

        public override object ConvertTo(
          ITypeDescriptorContext context,
          CultureInfo culture,
          object value,
          Type destinationType)
        {
            if (!(destinationType == typeof(string)) || !(value is T))
            {
                return base.ConvertTo(context, culture, value, destinationType);
            }

            if (DefaultValue.HasValue && object.Equals(value, DefaultValue.Value))
            {
                return null;
            }

            var str = ((T)value).ToString();
            return CaseHandlingMode == EnumBaseConverter<T>.CaseHandling.LowerCase ? str.ToLower() : (object)string.Format("{0}{1}", str[0].ToString().ToLower(), str.Substring(1));
        }

        public enum CaseHandling
        {
            CamelCase,
            LowerCase,
        }
    }
}
