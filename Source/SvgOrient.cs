// Microsoft Public License (Ms-PL). https://github.com/sst-soft/SVG which is a fork of https://github.com/svg-net/SVG.

using System.ComponentModel;
using System.Globalization;
using Svg.DataTypes;

namespace Svg
{
    [TypeConverter(typeof(SvgOrientConverter))]
    public class SvgOrient
    {
        private bool _isAuto;
        private float _angle;

        public SvgOrient()
          : this(0.0f)
        {
        }

        public SvgOrient(bool isAuto)
        {
            IsAuto = isAuto;
        }

        public SvgOrient(bool isAuto, bool isAutoStartReverse)
        {
            IsAuto = isAuto;
            IsAutoStartReverse = isAutoStartReverse;
        }

        public SvgOrient(float angle)
        {
            Angle = angle;
        }

        public float Angle
        {
            get => _angle;
            set
            {
                _angle = value;
                _isAuto = false;
            }
        }

        public bool IsAuto
        {
            get => _isAuto;
            set
            {
                _isAuto = value;
                _angle = 0.0f;
            }
        }

        public bool IsAutoStartReverse { get; set; }

        public override bool Equals(object obj)
        {
            if (!(obj is SvgOrient))
            {
                return false;
            }

            SvgOrient svgOrient = (SvgOrient)obj;
            return svgOrient.IsAuto == IsAuto && (double)svgOrient.Angle == (double)Angle;
        }

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }

        public override string ToString()
        {
            if (!IsAuto)
            {
                return Angle.ToString(CultureInfo.InvariantCulture);
            }

            return !IsAutoStartReverse ? "auto" : "auto-start-reverse";
        }

        public static implicit operator SvgOrient(float value) => new SvgOrient(value);
    }
}
