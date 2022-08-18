// Microsoft Public License (Ms-PL). https://github.com/sst-soft/SVG which is a fork of https://github.com/svg-net/SVG.

using System.ComponentModel;
using System.Drawing.Drawing2D;

namespace Svg.Transforms
{
    [TypeConverter(typeof(SvgTransformConverter))]
    public class SvgTransformCollection : List<SvgTransform>, ICloneable
    {
        private void AddItem(SvgTransform item)
        {
            base.Add(item);
        }

        public new void Add(SvgTransform item)
        {
            AddItem(item);
            OnTransformChanged();
        }

        public new void AddRange(IEnumerable<SvgTransform> collection)
        {
            base.AddRange(collection);
            OnTransformChanged();
        }

        public new void Remove(SvgTransform item)
        {
            base.Remove(item);
            OnTransformChanged();
        }

        public new void RemoveAt(int index)
        {
            base.RemoveAt(index);
            OnTransformChanged();
        }

        public Matrix GetMatrix()
        {
            Matrix matrix1 = new Matrix();
            foreach (SvgTransform svgTransform in (List<SvgTransform>)this)
            {
                using (Matrix matrix2 = svgTransform.Matrix)
                {
                    matrix1.Multiply(matrix2);
                }
            }
            return matrix1;
        }

        public override bool Equals(object obj)
        {
            return Count == 0 && Count == Count || base.Equals(obj);
        }

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }

        public new SvgTransform this[int i]
        {
            get => base[i];
            set
            {
                SvgTransform svgTransform1 = base[i];
                base[i] = value;
                SvgTransform svgTransform2 = value;
                if (!(svgTransform1 != svgTransform2))
                {
                    return;
                }

                OnTransformChanged();
            }
        }

        public event EventHandler<AttributeEventArgs> TransformChanged;

        protected void OnTransformChanged()
        {
            EventHandler<AttributeEventArgs> transformChanged = TransformChanged;
            if (transformChanged == null)
            {
                return;
            }

            transformChanged(this, new AttributeEventArgs()
            {
                Attribute = "transform",
                Value = Clone()
            });
        }

        public object Clone()
        {
            SvgTransformCollection transformCollection = new SvgTransformCollection();
            foreach (SvgTransform svgTransform in (List<SvgTransform>)this)
            {
                transformCollection.AddItem(svgTransform.Clone() as SvgTransform);
            }

            return transformCollection;
        }

        public override string ToString()
        {
            return Count < 1 ? string.Empty : this.Select<SvgTransform, string>(t => t.ToString()).Aggregate<string>((p, c) => p + " " + c);
        }
    }
}
