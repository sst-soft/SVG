﻿// Microsoft Public License (Ms-PL). https://github.com/sst-soft/SVG which is a fork of https://github.com/svg-net/SVG.

using System.ComponentModel;
using System.Globalization;

namespace Svg.DataTypes
{
    public class SvgOrientConverter : TypeConverter
    {
        public override object ConvertFrom(
          ITypeDescriptorContext context,
          CultureInfo culture,
          object value)
        {
            if (value == null)
            {
                return new SvgOrient();
            }

            if (!(value is string))
            {
                throw new ArgumentOutOfRangeException("value must be a string.");
            }

            switch (value.ToString())
            {
                case "auto":
                    return new SvgOrient(true);
                case "auto-start-reverse":
                    return new SvgOrient(true, true);
                default:
                    float result;
                    if (!float.TryParse(value.ToString(), out result))
                    {
                        throw new ArgumentOutOfRangeException("value must be a valid float.");
                    }

                    return new SvgOrient(result);
            }
        }

        public override bool CanConvertFrom(ITypeDescriptorContext context, Type sourceType)
        {
            return sourceType == typeof(string) || base.CanConvertFrom(context, sourceType);
        }

        public override bool CanConvertTo(ITypeDescriptorContext context, Type destinationType)
        {
            return destinationType == typeof(string) || base.CanConvertTo(context, destinationType);
        }

        public override object ConvertTo(
          ITypeDescriptorContext context,
          CultureInfo culture,
          object value,
          Type destinationType)
        {
            return base.ConvertTo(context, culture, value, destinationType);
        }
    }
}
