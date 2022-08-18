// todo: add license

using System.Collections;

namespace Svg
{
    public sealed class SvgElementCollection :
    IList<SvgElement>,
    ICollection<SvgElement>,
    IEnumerable<SvgElement>,
    IEnumerable
    {
        private readonly List<SvgElement> _elements;
        private readonly SvgElement _owner;
        private readonly bool _mock;

        internal SvgElementCollection(SvgElement owner)
          : this(owner, false)
        {
        }

        internal SvgElementCollection(SvgElement owner, bool mock)
        {
            if (owner == null)
            {
                throw new ArgumentNullException(nameof(owner));
            }

            _elements = new List<SvgElement>();
            _owner = owner;
            _mock = mock;
        }

        public int IndexOf(SvgElement item)
        {
            return _elements.IndexOf(item);
        }

        public void Insert(int index, SvgElement item)
        {
            InsertAndForceUniqueID(index, item, logElementOldIDNewID: new Action<SvgElement, string, string>(LogIDChange));
        }

        private void LogIDChange(SvgElement elem, string oldId, string newID)
        {
        }

        public void InsertAndForceUniqueID(
          int index,
          SvgElement item,
          bool autoForceUniqueID = true,
          bool autoFixChildrenID = true,
          Action<SvgElement, string, string> logElementOldIDNewID = null)
        {
            AddToIdManager(item, _elements[index], autoForceUniqueID, autoFixChildrenID, logElementOldIDNewID);
            _elements.Insert(index, item);
            item._parent.OnElementAdded(item, index);
        }

        public void RemoveAt(int index)
        {
            SvgElement svgElement = this[index];
            if (svgElement == null)
            {
                return;
            }

            Remove(svgElement);
        }

        public SvgElement this[int index]
        {
            get => _elements[index];
            set => _elements[index] = value;
        }

        public void Add(SvgElement item)
        {
            AddAndForceUniqueID(item, logElementOldIDNewID: new Action<SvgElement, string, string>(LogIDChange));
        }

        public void AddAndForceUniqueID(
      SvgElement item,
      bool autoForceUniqueID = true,
      bool autoFixChildrenID = true,
      Action<SvgElement, string, string> logElementOldIDNewID = null)
        {
            AddToIdManager(item, null, autoForceUniqueID, autoFixChildrenID, logElementOldIDNewID);
            _elements.Add(item);
            item._parent.OnElementAdded(item, Count - 1);
        }

        private void AddToIdManager(
          SvgElement item,
          SvgElement sibling,
          bool autoForceUniqueID = true,
          bool autoFixChildrenID = true,
          Action<SvgElement, string, string> logElementOldIDNewID = null)
        {
            if (_mock)
            {
                return;
            }

            if (_owner.OwnerDocument != null)
            {
                _owner.OwnerDocument.IdManager.AddAndForceUniqueID(item, sibling, autoForceUniqueID, logElementOldIDNewID);
                if (!(item is SvgDocument))
                {
                    foreach (SvgElement child in item.Children)
                    {
                        child.ApplyRecursive(e => _owner.OwnerDocument.IdManager.AddAndForceUniqueID(e, null, autoFixChildrenID, logElementOldIDNewID));
                    }
                }
            }
            item._parent = _owner;
        }

        public void Clear()
        {
            while (Count > 0)
            {
                Remove(this[0]);
            }
        }

        public bool Contains(SvgElement item)
        {
            return _elements.Contains(item);
        }

        public void CopyTo(SvgElement[] array, int arrayIndex)
        {
            _elements.CopyTo(array, arrayIndex);
        }

        public int Count => _elements.Count;

        public bool IsReadOnly => false;

        public bool Remove(SvgElement item)
        {
            var num = _elements.Remove(item) ? 1 : 0;
            if (num == 0)
            {
                return num != 0;
            }

            _owner.OnElementRemoved(item);
            if (_mock)
            {
                return num != 0;
            }

            item._parent = null;
            if (_owner.OwnerDocument == null)
            {
                return num != 0;
            }

            item.ApplyRecursiveDepthFirst(new Action<SvgElement>(_owner.OwnerDocument.IdManager.Remove));
            return num != 0;
        }

        public IEnumerable<T> FindSvgElementsOf<T>() where T : SvgElement
        {
            return _elements.Where<SvgElement>(x => x is T).Select<SvgElement, T>(x => x as T).Concat<T>(_elements.SelectMany<SvgElement, T>(x => x.Children.FindSvgElementsOf<T>()));
        }

        public T FindSvgElementOf<T>() where T : SvgElement
        {
            return _elements.OfType<T>().FirstOrDefault<T>() ?? _elements.Select<SvgElement, T>(x => x.Children.FindSvgElementOf<T>()).FirstOrDefault<T>(x => x != null);
        }

        public T GetSvgElementOf<T>() where T : SvgElement
        {
            return _elements.FirstOrDefault<SvgElement>(x => x is T) as T;
        }

        public IEnumerator<SvgElement> GetEnumerator()
        {
            return _elements.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return _elements.GetEnumerator();
        }
    }
}
