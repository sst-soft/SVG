// Microsoft Public License (Ms-PL). https://github.com/sst-soft/SVG which is a fork of https://github.com/svg-net/SVG.

using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Text;
using System.Globalization;
using System.Runtime.InteropServices;
using System.Text;
using System.Xml;
using Svg.Css;
using Svg.Exceptions;
using ExCSS;

namespace Svg
{
    public class SvgDocument : SvgFragment, ITypeDescriptorContext, IServiceProvider
    {
        public static readonly int PointsPerInch = SvgDocument.GetSystemDpi();
        private SvgElementIdManager _idManager;
        private Dictionary<string, IEnumerable<SvgFontFace>> _fontDefns;
        private PrivateFontCollection _privateFonts;
        private IList<FontFaceRule> _fontFaceDirectives;

        private static int GetSystemDpi()
        {
            if (!RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
            {
                return 96;
            }

            IntPtr dc = SvgDocument.GetDC(IntPtr.Zero);
            var deviceCaps = SvgDocument.GetDeviceCaps(dc, 90);
            SvgDocument.ReleaseDC(IntPtr.Zero, dc);
            return deviceCaps;
        }

        [DllImport("gdi32.dll")]
        private static extern int GetDeviceCaps(IntPtr hdc, int nIndex);

        [DllImport("user32.dll")]
        private static extern IntPtr GetDC(IntPtr hWnd);

        [DllImport("user32.dll")]
        private static extern int ReleaseDC(IntPtr hWnd, IntPtr hDC);

        internal Dictionary<string, IEnumerable<SvgFontFace>> FontDefns()
        {
            if (_fontDefns == null)
            {
                _fontDefns = Descendants().OfType<SvgFontFace>().GroupBy<SvgFontFace, string>(f => f.FontFamily).Select<IGrouping<string, SvgFontFace>, IGrouping<string, SvgFontFace>>(family => family).ToDictionary<IGrouping<string, SvgFontFace>, string, IEnumerable<SvgFontFace>>(f => f.Key, f => f);
            }

            return _fontDefns;
        }

        internal FontFamily[] PrivateFontDefns()
        {
            if (_privateFonts == null)
            {
                _privateFonts = new PrivateFontCollection();
                if (_fontFaceDirectives != null)
                {
                    foreach (FontFaceRule fontFaceDirective in (IEnumerable<FontFaceRule>)_fontFaceDirectives)
                    {
                        try
                        {
                            var source = SvgDocument.downBytes(fontFaceDirective.Src, BaseUri);
                            IntPtr destination = Marshal.AllocHGlobal(source.Length);
                            Marshal.Copy(source, 0, destination, source.Length);
                            _privateFonts.AddMemoryFont(destination, source.Length);
                        }
                        catch (Exception ex)
                        {
                            Trace.TraceWarning(ex.Message);
                        }
                    }
                }
            }
            return _privateFonts.Families;
        }

        private static byte[] downBytes(string url, Uri baseUri)
        {
            Uri uri = new Uri(Utility.GetUrlString(url), UriKind.RelativeOrAbsolute);
            if (!uri.IsAbsoluteUri && baseUri != null)
            {
                uri = new Uri(baseUri, uri);
            }

            return Utility.GetBytesFromUri(uri)?.DataBytes;
        }

        public SvgDocument()
        {
            Ppi = SvgDocument.PointsPerInch;
        }

        public Uri BaseUri { get; set; }

        protected internal virtual SvgElementIdManager IdManager
        {
            get
            {
                if (_idManager == null)
                {
                    _idManager = new SvgElementIdManager(this);
                }

                return _idManager;
            }
        }

        public void OverwriteIdManager(SvgElementIdManager manager)
        {
            _idManager = manager;
        }

        public int Ppi { get; set; }

        public string ExternalCSSHref { get; set; }

        IContainer ITypeDescriptorContext.Container => throw new NotImplementedException();

        object ITypeDescriptorContext.Instance => this;

        void ITypeDescriptorContext.OnComponentChanged()
        {
            throw new NotImplementedException();
        }

        bool ITypeDescriptorContext.OnComponentChanging()
        {
            throw new NotImplementedException();
        }

        PropertyDescriptor ITypeDescriptorContext.PropertyDescriptor => throw new NotImplementedException();

        object IServiceProvider.GetService(Type serviceType)
        {
            throw new NotImplementedException();
        }

        public virtual SvgElement GetElementById(string id)
        {
            return IdManager.GetElementById(id);
        }

        public virtual TSvgElement GetElementById<TSvgElement>(string id) where TSvgElement : SvgElement
        {
            return GetElementById(id) as TSvgElement;
        }

        public static bool SystemIsGdiPlusCapable()
        {
            try
            {
                SvgDocument.EnsureSystemIsGdiPlusCapable();
            }
            catch (SvgGdiPlusCannotBeLoadedException)
            {
                return false;
            }
            catch (Exception)
            {
                throw;
            }
            return true;
        }

        public static void EnsureSystemIsGdiPlusCapable()
        {
            try
            {
                using (new Matrix(0.0f, 0.0f, 0.0f, 0.0f, 0.0f, 0.0f)) { }
            }
            catch (Exception ex)
            {
                if (SvgDocument.ExceptionCaughtIsGdiPlusRelated(ex))
                {
                    throw new SvgGdiPlusCannotBeLoadedException(ex);
                }

                throw;
            }
        }

        private static bool ExceptionCaughtIsGdiPlusRelated(Exception e)
        {
            Exception exception = e;
            for (var index = 0; exception != null && index < 10; ++index)
            {
                int num1;
                if (!(exception is DllNotFoundException notFoundException))
                {
                    num1 = 0;
                }
                else
                {
                    var nullable = notFoundException.Message?.LastIndexOf("libgdiplus", StringComparison.OrdinalIgnoreCase);
                    var num2 = -1;
                    num1 = nullable.GetValueOrDefault() > num2 & nullable.HasValue ? 1 : 0;
                }
                if (num1 != 0)
                {
                    return true;
                }

                exception = exception.InnerException;
            }
            return false;
        }

        public static SvgDocument Open(string path)
        {
            return SvgDocument.Open<SvgDocument>(path, null);
        }

        public static T Open<T>(string path) where T : SvgDocument, new()
        {
            return SvgDocument.Open<T>(path, null);
        }

        public static T Open<T>(string path, Dictionary<string, string> entities) where T : SvgDocument, new()
        {
            if (string.IsNullOrEmpty(path))
            {
                throw new ArgumentNullException(nameof(path));
            }

            if (!File.Exists(path))
            {
                throw new FileNotFoundException("The specified document cannot be found.", path);
            }

            using (FileStream fileStream = File.OpenRead(path))
            {
                T obj = SvgDocument.Open<T>(fileStream, entities);
                obj.BaseUri = new Uri(System.IO.Path.GetFullPath(path));
                return obj;
            }
        }

        public static T Open<T>(Stream stream) where T : SvgDocument, new()
        {
            return SvgDocument.Open<T>(stream, null);
        }

        public static T FromSvg<T>(string svg) where T : SvgDocument, new()
        {
            if (string.IsNullOrEmpty(svg))
            {
                throw new ArgumentNullException(nameof(svg));
            }

            using (StringReader reader1 = new StringReader(svg))
            {
                SvgTextReader reader2 = new SvgTextReader(reader1, null)
                {
                    XmlResolver = new SvgDtdResolver(),
                    WhitespaceHandling = (WhitespaceHandling)2
                };
                return SvgDocument.Open<T>(reader2);
            }
        }

        public static T Open<T>(Stream stream, Dictionary<string, string> entities) where T : SvgDocument, new()
        {
            SvgTextReader reader = stream != null ? new SvgTextReader(stream, entities) : throw new ArgumentNullException(nameof(stream));
            reader.XmlResolver = new SvgDtdResolver();
            reader.WhitespaceHandling = (WhitespaceHandling)2;
            return SvgDocument.Open<T>(reader);
        }

        private static T Open<T>(XmlReader reader) where T : SvgDocument, new()
        {
            SvgDocument.EnsureSystemIsGdiPlusCapable();
            Stack<SvgElement> svgElementStack = new Stack<SvgElement>();
            T document = default(T);
            SvgElementFactory elementFactory = new SvgElementFactory();
            List<ISvgNode> source = new List<ISvgNode>();
            while (reader.Read())
            {
                try
                {
                    switch (reader.NodeType)
                    {
                        case XmlNodeType.Element:
                            var num = reader.IsEmptyElement ? 1 : 0;
                            SvgElement svgElement1;
                            if (svgElementStack.Count > 0)
                            {
                                svgElement1 = elementFactory.CreateElement(reader, document);
                            }
                            else
                            {
                                document = elementFactory.CreateDocument<T>(reader);
                                svgElement1 = document;
                            }
                            if (svgElementStack.Count > 0)
                            {
                                SvgElement svgElement2 = svgElementStack.Peek();
                                if (svgElement2 != null && svgElement1 != null)
                                {
                                    svgElement2.Children.Add(svgElement1);
                                    svgElement2.Nodes.Add(svgElement1);
                                }
                            }
                            svgElementStack.Push(svgElement1);
                            if (num == 0)
                            {
                                continue;
                            }

                            goto case XmlNodeType.EndElement;
                        case XmlNodeType.Text:
                        case XmlNodeType.CDATA:
                            svgElementStack.Peek().Nodes.Add(new SvgContentNode()
                            {
                                Content = reader.Value
                            });
                            continue;
                        case XmlNodeType.EntityReference:
                            reader.ResolveEntity();
                            svgElementStack.Peek().Nodes.Add(new SvgContentNode()
                            {
                                Content = reader.Value
                            });
                            continue;
                        case XmlNodeType.EndElement:
                            SvgElement svgElement3 = svgElementStack.Pop();
                            if (svgElement3.Nodes.OfType<SvgContentNode>().Any<SvgContentNode>())
                            {
                                svgElement3.Content = svgElement3.Nodes.Select<ISvgNode, string>(e => e.Content).Aggregate<string>((p, c) => p + c);
                            }
                            else
                            {
                                svgElement3.Nodes.Clear();
                            }

                            if (svgElement3 is SvgUnknownElement svgUnknownElement)
                            {
                                if (svgUnknownElement.ElementName == "style")
                                {
                                    source.Add(svgUnknownElement);
                                    continue;
                                }
                                continue;
                            }
                            continue;
                        default:
                            continue;
                    }
                }
                catch (Exceptions.FizzlerException ex)
                {
                    throw ex;
                }
                catch (Exception ex)
                {
                    Trace.TraceError(ex.Message);
                }
            }
            if (source.Any<ISvgNode>())
            {
                StyleSheet styleSheet = new Parser().Parse(source.Select<ISvgNode, string>(s => s.Content).Aggregate<string>((p, c) => p + Environment.NewLine + c));
                foreach (StyleRule styleRule in (IEnumerable<StyleRule>)styleSheet.StyleRules)
                {
                    foreach (BaseSelector baseSelector in !(styleRule.Selector is AggregateSelectorList selector) || !(selector.Delimiter == ",") ? Enumerable.Repeat<BaseSelector>(styleRule.Selector, 1) : selector)
                    {
                        try
                        {
                            NonSvgElement elem = new NonSvgElement();
                            elem.Children.Add(document);
                            foreach (SvgElement svgElement in elem.QuerySelectorAll(styleRule.Selector.ToString(), elementFactory))
                            {
                                foreach (Property declaration in styleRule.Declarations)
                                {
                                    svgElement.AddStyle(declaration.Name, declaration.Term.ToString(), styleRule.Selector.GetSpecificity());
                                }
                            }
                        }
                        catch (FizzlerException ex)
                        {
                            throw ex;
                        }
                        catch (Exception ex)
                        {
                            Trace.TraceWarning(ex.Message);
                        }
                    }
                }
                document._fontFaceDirectives = styleSheet.FontFaceDirectives;
            }
            document?.FlushStyles(true);
            return document;
        }

        public static SvgDocument Open(XmlDocument document)
        {
            return document != null ? SvgDocument.Open<SvgDocument>(new SvgNodeReader(document.DocumentElement, null)) : throw new ArgumentNullException(nameof(document));
        }

        public static Bitmap OpenAsBitmap(string path)
        {
            return null;
        }

        public static Bitmap OpenAsBitmap(XmlDocument document)
        {
            return null;
        }

        private void Draw(ISvgRenderer renderer, ISvgBoundable boundable)
        {
            renderer.SetBoundable(boundable);
            Render(renderer);
        }

        public void Draw(ISvgRenderer renderer)
        {
            if (renderer == null)
            {
                throw new ArgumentNullException(nameof(renderer));
            }

            Draw(renderer, this);
        }

        public void Draw(Graphics graphics)
        {
            Draw(graphics, new SizeF?());
        }

        public void Draw(Graphics graphics, SizeF? size)
        {
            if (graphics == null)
            {
                throw new ArgumentNullException(nameof(graphics));
            }

            using (ISvgRenderer renderer = SvgRenderer.FromGraphics(graphics))
            {
                ISvgBoundable boundable = size.HasValue ? new GenericBoundable(0.0f, 0.0f, size.Value.Width, size.Value.Height) : this;
                Draw(renderer, boundable);
            }
        }

        public virtual Bitmap Draw()
        {
            Bitmap bitmap = null;
            try
            {
                try
                {
                    SizeF dimensions = GetDimensions();
                    bitmap = new Bitmap((int)Math.Round((double)dimensions.Width), (int)Math.Round((double)dimensions.Height));
                }
                catch (ArgumentException ex)
                {
                    throw new SvgMemoryException("Cannot process SVG file, cannot allocate the required memory", ex);
                }
                Draw(bitmap);
            }
            catch
            {
                ((Image)bitmap)?.Dispose();
                throw;
            }
            return bitmap;
        }

        public virtual void Draw(Bitmap bitmap)
        {
            using (ISvgRenderer renderer = SvgRenderer.FromImage(bitmap))
            {
                Overflow = SvgOverflow.Auto;
                GenericBoundable boundable = new GenericBoundable(0.0f, 0.0f, bitmap.Width, bitmap.Height);
                Draw(renderer, boundable);
            }
        }

        public virtual Bitmap Draw(int rasterWidth, int rasterHeight)
        {
            SizeF dimensions = GetDimensions();
            SizeF size = dimensions;
            RasterizeDimensions(ref size, rasterWidth, rasterHeight);
            if ((double)size.Width == 0.0 || (double)size.Height == 0.0)
            {
                return null;
            }

            Bitmap bitmap = null;
            try
            {
                try
                {
                    bitmap = new Bitmap((int)Math.Round((double)size.Width), (int)Math.Round((double)size.Height));
                }
                catch (ArgumentException ex)
                {
                    throw new SvgMemoryException("Cannot process SVG file, cannot allocate the required memory", ex);
                }
                using (ISvgRenderer renderer = SvgRenderer.FromImage(bitmap))
                {
                    renderer.ScaleTransform(size.Width / dimensions.Width, size.Height / dimensions.Height);
                    GenericBoundable boundable = new GenericBoundable(0.0f, 0.0f, dimensions.Width, dimensions.Height);
                    Draw(renderer, boundable);
                }
            }
            catch
            {
                ((Image)bitmap)?.Dispose();
                throw;
            }
            return bitmap;
        }

        public virtual void RasterizeDimensions(ref SizeF size, int rasterWidth, int rasterHeight)
        {
            if ((double)size.Width == 0.0)
            {
                return;
            }

            var num = size.Height / size.Width;
            size.Width = rasterWidth > 0 ? rasterWidth : size.Width;
            size.Height = rasterHeight > 0 ? rasterHeight : size.Height;
            if (rasterHeight == 0 && rasterWidth > 0)
            {
                size.Height = (int)(rasterWidth * (double)num);
            }
            else
            {
                if (rasterHeight <= 0 || rasterWidth != 0)
                {
                    return;
                }

                size.Width = (int)(rasterHeight / (double)num);
            }
        }

        public override void Write(XmlTextWriter writer)
        {
            CultureInfo currentCulture = Thread.CurrentThread.CurrentCulture;
            try
            {
                Thread.CurrentThread.CurrentCulture = CultureInfo.InvariantCulture;
                base.Write(writer);
            }
            finally
            {
                Thread.CurrentThread.CurrentCulture = currentCulture;
            }
        }

        public void Write(Stream stream, bool useBom = true)
        {
            XmlTextWriter writer = new XmlTextWriter(stream, useBom ? Encoding.UTF8 : new UTF8Encoding(false))
            {
                Formatting = (Formatting)1
            };
            writer.WriteStartDocument();
            writer.WriteDocType("svg", "-//W3C//DTD SVG 1.1//EN", "http://www.w3.org/Graphics/SVG/1.1/DTD/svg11.dtd", null);
            if (!string.IsNullOrEmpty(ExternalCSSHref))
            {
                writer.WriteProcessingInstruction("xml-stylesheet", string.Format("type=\"text/css\" href=\"{0}\"", ExternalCSSHref));
            }

            Write(writer);
            writer.Flush();
        }

        public void Write(string path, bool useBom = true)
        {
            using (FileStream fileStream = new FileStream(path, (FileMode)2, (FileAccess)2))
            {
                Write(fileStream, useBom);
            }
        }
    }
}
