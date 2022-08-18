// todo: add license

using System.Drawing;
using System.Globalization;
using System.Xml;

namespace Svg
{
    public static class SvgExtentions
    {
        public static void SetRectangle(this SvgRectangle r, RectangleF bounds)
        {
            r.X = (SvgUnit)bounds.X;
            r.Y = (SvgUnit)bounds.Y;
            r.Width = (SvgUnit)bounds.Width;
            r.Height = (SvgUnit)bounds.Height;
        }

        public static RectangleF GetRectangle(this SvgRectangle r)
        {
            return new RectangleF((float)r.X, (float)r.Y, (float)r.Width, (float)r.Height);
        }

        public static string GetXML(this SvgDocument doc)
        {
            var xml = "";
            using (MemoryStream memoryStream = new MemoryStream())
            {
                doc.Write(memoryStream);
                memoryStream.Position = 0L;
                StreamReader streamReader = new StreamReader(memoryStream);
                xml = streamReader.ReadToEnd();
                streamReader.Close();
            }
            return xml;
        }

        public static string GetXML(this SvgElement elem)
        {
            CultureInfo currentCulture = Thread.CurrentThread.CurrentCulture;
            try
            {
                Thread.CurrentThread.CurrentCulture = CultureInfo.InvariantCulture;
                using (StringWriter stringWriter = new StringWriter())
                {
                    using (XmlTextWriter writer = new XmlTextWriter(stringWriter))
                    {
                        elem.Write(writer);
                        return stringWriter.ToString();
                    }
                }
            }
            finally
            {
                Thread.CurrentThread.CurrentCulture = currentCulture;
            }
        }

        public static bool HasNonEmptyCustomAttribute(this SvgElement element, string name)
        {
            return element.CustomAttributes.ContainsKey(name) && !string.IsNullOrEmpty(element.CustomAttributes[name]);
        }

        public static void ApplyRecursive(this SvgElement elem, Action<SvgElement> action)
        {
            foreach (SvgElement svgElement in elem.Traverse<SvgElement>(e => e.Children))
            {
                action(svgElement);
            }
        }

        public static void ApplyRecursiveDepthFirst(this SvgElement elem, Action<SvgElement> action)
        {
            foreach (SvgElement svgElement in elem.TraverseDepthFirst<SvgElement>(e => e.Children))
            {
                action(svgElement);
            }
        }

        public static IEnumerable<T> Traverse<T>(
          this IEnumerable<T> items,
          Func<T, IEnumerable<T>> childrenSelector)
        {
            if (childrenSelector == null)
            {
                throw new ArgumentNullException(nameof(childrenSelector));
            }

            Queue<T> itemQueue = new Queue<T>(items);
            while (itemQueue.Count > 0)
            {
                T current = itemQueue.Dequeue();
                yield return current;
                foreach (T obj in childrenSelector(current) ?? Enumerable.Empty<T>())
                {
                    itemQueue.Enqueue(obj);
                }

                current = default(T);
            }
        }

        public static IEnumerable<T> Traverse<T>(
          this T root,
          Func<T, IEnumerable<T>> childrenSelector)
        {
            return Enumerable.Repeat<T>(root, 1).Traverse<T>(childrenSelector);
        }

        public static IEnumerable<T> TraverseDepthFirst<T>(
          this IEnumerable<T> items,
          Func<T, IEnumerable<T>> childrenSelector)
        {
            if (childrenSelector == null)
            {
                throw new ArgumentNullException(nameof(childrenSelector));
            }

            Stack<T> itemStack = new Stack<T>(items ?? Enumerable.Empty<T>());
            while (itemStack.Count > 0)
            {
                T current = itemStack.Pop();
                yield return current;
                foreach (T obj in childrenSelector(current) ?? Enumerable.Empty<T>())
                {
                    itemStack.Push(obj);
                }

                current = default(T);
            }
        }

        public static IEnumerable<T> TraverseDepthFirst<T>(
          this T root,
          Func<T, IEnumerable<T>> childrenSelector)
        {
            return Enumerable.Repeat<T>(root, 1).TraverseDepthFirst<T>(childrenSelector);
        }
    }
}
