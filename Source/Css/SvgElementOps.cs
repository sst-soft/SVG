// todo: add license

using Fizzler;

namespace Svg.Css
{
    internal class SvgElementOps : IElementOps<SvgElement>
    {
        private readonly SvgElementFactory _elementFactory;

        public SvgElementOps(SvgElementFactory elementFactory) => this._elementFactory = elementFactory;

        public Selector<SvgElement> Type(NamespacePrefix prefix, string name)
        {
            SvgElementFactory.ElementInfo type = (SvgElementFactory.ElementInfo)null;
            return this._elementFactory.AvailableElements.TryGetValue(name, out type) ? (Selector<SvgElement>)(nodes => nodes.Where<SvgElement>((Func<SvgElement, bool>)(n => n.GetType() == type.ElementType))) : (Selector<SvgElement>)(nodes => Enumerable.Empty<SvgElement>());
        }

        public Selector<SvgElement> Universal(NamespacePrefix prefix) => (Selector<SvgElement>)(nodes => nodes);

        public Selector<SvgElement> Id(string id) => (Selector<SvgElement>)(nodes => nodes.Where<SvgElement>((Func<SvgElement, bool>)(n => n.ID == id)));

        public Selector<SvgElement> Class(string clazz) => this.AttributeIncludes(NamespacePrefix.None, "class", clazz);

        public Selector<SvgElement> AttributeExists(
          NamespacePrefix prefix,
          string name)
        {
            return (Selector<SvgElement>)(nodes => nodes.Where<SvgElement>((Func<SvgElement, bool>)(n => n.ContainsAttribute(name))));
        }

        public Selector<SvgElement> AttributeExact(
          NamespacePrefix prefix,
          string name,
          string value)
        {
            return (Selector<SvgElement>)(nodes => nodes.Where<SvgElement>((Func<SvgElement, bool>)(n =>
            {
                string str = (string)null;
                return n.TryGetAttribute(name, out str) && str == value;
            })));
        }

        public Selector<SvgElement> AttributeIncludes(
          NamespacePrefix prefix,
          string name,
          string value)
        {
            return (Selector<SvgElement>)(nodes => nodes.Where<SvgElement>((Func<SvgElement, bool>)(n =>
            {
                string str = (string)null;
                return n.TryGetAttribute(name, out str) && ((IEnumerable<string>)str.Split(' ', StringSplitOptions.None)).Contains<string>(value);
            })));
        }

        public Selector<SvgElement> AttributeDashMatch(
          NamespacePrefix prefix,
          string name,
          string value)
        {
            return !string.IsNullOrEmpty(value) ? (Selector<SvgElement>)(nodes => nodes.Where<SvgElement>((Func<SvgElement, bool>)(n =>
            {
                string str = (string)null;
                return n.TryGetAttribute(name, out str) && ((IEnumerable<string>)str.Split('-', StringSplitOptions.None)).Contains<string>(value);
            }))) : (Selector<SvgElement>)(nodes => Enumerable.Empty<SvgElement>());
        }

        public Selector<SvgElement> AttributePrefixMatch(
          NamespacePrefix prefix,
          string name,
          string value)
        {
            return !string.IsNullOrEmpty(value) ? (Selector<SvgElement>)(nodes => nodes.Where<SvgElement>((Func<SvgElement, bool>)(n =>
            {
                string str = (string)null;
                return n.TryGetAttribute(name, out str) && str.StartsWith(value);
            }))) : (Selector<SvgElement>)(nodes => Enumerable.Empty<SvgElement>());
        }

        public Selector<SvgElement> AttributeSuffixMatch(
          NamespacePrefix prefix,
          string name,
          string value)
        {
            return !string.IsNullOrEmpty(value) ? (Selector<SvgElement>)(nodes => nodes.Where<SvgElement>((Func<SvgElement, bool>)(n =>
            {
                string str = (string)null;
                return n.TryGetAttribute(name, out str) && str.EndsWith(value);
            }))) : (Selector<SvgElement>)(nodes => Enumerable.Empty<SvgElement>());
        }

        public Selector<SvgElement> AttributeSubstring(
          NamespacePrefix prefix,
          string name,
          string value)
        {
            return !string.IsNullOrEmpty(value) ? (Selector<SvgElement>)(nodes => nodes.Where<SvgElement>((Func<SvgElement, bool>)(n =>
            {
                string str = (string)null;
                return n.TryGetAttribute(name, out str) && str.Contains(value);
            }))) : (Selector<SvgElement>)(nodes => Enumerable.Empty<SvgElement>());
        }

        public Selector<SvgElement> FirstChild() => (Selector<SvgElement>)(nodes => nodes.Where<SvgElement>((Func<SvgElement, bool>)(n => n.Parent == null || n.Parent.Children.First<SvgElement>() == n)));

        public Selector<SvgElement> LastChild() => (Selector<SvgElement>)(nodes => nodes.Where<SvgElement>((Func<SvgElement, bool>)(n => n.Parent == null || n.Parent.Children.Last<SvgElement>() == n)));

        private IEnumerable<T> GetByIds<T>(IList<T> items, IEnumerable<int> indices)
        {
            foreach (int index in indices)
            {
                if (index >= 0 && index < ((ICollection<T>)items).Count)
                    yield return items[index];
            }
        }

        public Selector<SvgElement> NthChild(int a, int b) => (Selector<SvgElement>)(nodes => nodes.Where<SvgElement>((Func<SvgElement, bool>)(n => n.Parent != null && this.GetByIds<SvgElement>((IList<SvgElement>)n.Parent.Children, Enumerable.Range(0, n.Parent.Children.Count / a).Select<int, int>((Func<int, int>)(i => a * i + b))).Contains<SvgElement>(n))));

        public Selector<SvgElement> OnlyChild() => (Selector<SvgElement>)(nodes => nodes.Where<SvgElement>((Func<SvgElement, bool>)(n => n.Parent == null || n.Parent.Children.Count == 1)));

        public Selector<SvgElement> Empty() => (Selector<SvgElement>)(nodes => nodes.Where<SvgElement>((Func<SvgElement, bool>)(n => n.Children.Count == 0)));

        public Selector<SvgElement> Child() => (Selector<SvgElement>)(nodes => nodes.SelectMany<SvgElement, SvgElement>((Func<SvgElement, IEnumerable<SvgElement>>)(n => (IEnumerable<SvgElement>)n.Children)));

        public Selector<SvgElement> Descendant() => (Selector<SvgElement>)(nodes => nodes.SelectMany<SvgElement, SvgElement>((Func<SvgElement, IEnumerable<SvgElement>>)(n => this.Descendants(n))));

        private IEnumerable<SvgElement> Descendants(SvgElement elem)
        {
            foreach (SvgElement child in elem.Children)
            {
                yield return child;
                foreach (SvgElement descendant in child.Descendants())
                    yield return descendant;
            }
        }

        public Selector<SvgElement> Adjacent() => (Selector<SvgElement>)(nodes => nodes.SelectMany<SvgElement, SvgElement>((Func<SvgElement, IEnumerable<SvgElement>>)(n => this.ElementsAfterSelf(n).Take<SvgElement>(1))));

        public Selector<SvgElement> GeneralSibling() => (Selector<SvgElement>)(nodes => nodes.SelectMany<SvgElement, SvgElement>((Func<SvgElement, IEnumerable<SvgElement>>)(n => this.ElementsAfterSelf(n))));

        private IEnumerable<SvgElement> ElementsAfterSelf(SvgElement self) => self.Parent != null ? self.Parent.Children.Skip<SvgElement>(self.Parent.Children.IndexOf(self) + 1) : Enumerable.Empty<SvgElement>();

        public Selector<SvgElement> NthLastChild(int a, int b) => throw new NotImplementedException();
    }
}
