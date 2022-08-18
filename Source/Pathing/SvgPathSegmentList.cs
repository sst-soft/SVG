// todo: add license

using System.Collections;
using System.ComponentModel;

namespace Svg.Pathing
{
    [TypeConverter(typeof(SvgPathBuilder))]
    public sealed class SvgPathSegmentList :
    IList<SvgPathSegment>,
    ICollection<SvgPathSegment>,
    IEnumerable<SvgPathSegment>,
    IEnumerable
    {
        private readonly List<SvgPathSegment> _segments = new List<SvgPathSegment>();

        public ISvgPathElement Owner { get; set; }

        public SvgPathSegment Last => _segments[_segments.Count - 1];

        public int IndexOf(SvgPathSegment item)
        {
            return _segments.IndexOf(item);
        }

        public void Insert(int index, SvgPathSegment item)
        {
            _segments.Insert(index, item);
            if (Owner == null)
            {
                return;
            }

            Owner.OnPathUpdated();
        }

        public void RemoveAt(int index)
        {
            _segments.RemoveAt(index);
            if (Owner == null)
            {
                return;
            }

            Owner.OnPathUpdated();
        }

        public SvgPathSegment this[int index]
        {
            get => _segments[index];
            set
            {
                _segments[index] = value;
                if (Owner == null)
                {
                    return;
                }

                Owner.OnPathUpdated();
            }
        }

        public void Add(SvgPathSegment item)
        {
            _segments.Add(item);
            if (Owner == null)
            {
                return;
            }

            Owner.OnPathUpdated();
        }

        public void Clear()
        {
            _segments.Clear();
        }

        public bool Contains(SvgPathSegment item)
        {
            return _segments.Contains(item);
        }

        public void CopyTo(SvgPathSegment[] array, int arrayIndex)
        {
            _segments.CopyTo(array, arrayIndex);
        }

        public int Count => _segments.Count;

        public bool IsReadOnly => false;

        public bool Remove(SvgPathSegment item)
        {
            var num = _segments.Remove(item) ? 1 : 0;
            if (num == 0)
            {
                return num != 0;
            }

            if (Owner == null)
            {
                return num != 0;
            }

            Owner.OnPathUpdated();
            return num != 0;
        }

        public IEnumerator<SvgPathSegment> GetEnumerator()
        {
            return _segments.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return _segments.GetEnumerator();
        }
    }
}
