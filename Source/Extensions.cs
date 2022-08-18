// Microsoft Public License (Ms-PL). https://github.com/sst-soft/SVG which is a fork of https://github.com/svg-net/SVG.

namespace Svg
{
    public static class Extensions
    {
        public static IEnumerable<SvgElement> Descendants<T>(
          this IEnumerable<T> source)
          where T : SvgElement
        {
            return source != null ? Extensions.GetDescendants<T>(source, false) : throw new ArgumentNullException(nameof(source));
        }

        private static IEnumerable<SvgElement> GetAncestors<T>(
          IEnumerable<T> source,
          bool self)
          where T : SvgElement
        {
            foreach (T obj in source)
            {
                if (obj != null)
                {
                    SvgElement elem;
                    for (elem = self ? obj : obj.Parent; elem != null; elem = elem.Parent)
                    {
                        yield return elem;
                    }

                    elem = null;
                }
            }
        }

        private static IEnumerable<SvgElement> GetDescendants<T>(
          IEnumerable<T> source,
          bool self)
          where T : SvgElement
        {
            Stack<int> positons = new Stack<int>();
            foreach (T obj in source)
            {
                T start = obj;
                if (start != null)
                {
                    if (self)
                    {
                        yield return start;
                    }

                    positons.Push(0);
                    SvgElement currParent = start;
                    while (positons.Count > 0)
                    {
                        var currPos = positons.Pop();
                        if (currPos < currParent.Children.Count)
                        {
                            yield return currParent.Children[currPos];
                            currParent = currParent.Children[currPos];
                            positons.Push(currPos + 1);
                            positons.Push(0);
                        }
                        else
                        {
                            currParent = currParent.Parent;
                        }
                    }
                }
                start = default(T);
            }
        }
    }
}
