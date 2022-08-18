// Microsoft Public License (Ms-PL). https://github.com/sst-soft/SVG which is a fork of https://github.com/svg-net/SVG.

using System.Diagnostics;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Net;
using System.Text;

namespace Svg
{
    [SvgElement("image")]
    public class SvgImage : SvgVisualElement
    {
        private const string MimeTypeSvg = "image/svg+xml";
        private GraphicsPath _path;
        private bool _gettingBounds;

        public SvgPoint Location => new SvgPoint(X, Y);

        [SvgAttribute("preserveAspectRatio")]
        public SvgAspectRatio AspectRatio
        {
            get => GetAttribute<SvgAspectRatio>("preserveAspectRatio", false, new SvgAspectRatio(SvgPreserveAspectRatio.xMidYMid));
            set => Attributes["preserveAspectRatio"] = value;
        }

        [SvgAttribute("x")]
        public virtual SvgUnit X
        {
            get => GetAttribute<SvgUnit>("x", false);
            set => Attributes["x"] = value;
        }

        [SvgAttribute("y")]
        public virtual SvgUnit Y
        {
            get => GetAttribute<SvgUnit>("y", false);
            set => Attributes["y"] = value;
        }

        [SvgAttribute("width")]
        public virtual SvgUnit Width
        {
            get => GetAttribute<SvgUnit>("width", false);
            set => Attributes["width"] = value;
        }

        [SvgAttribute("height")]
        public virtual SvgUnit Height
        {
            get => GetAttribute<SvgUnit>("height", false);
            set => Attributes["height"] = value;
        }

        [SvgAttribute("href", "http://www.w3.org/1999/xlink")]
        public virtual string Href
        {
            get => GetAttribute<string>("href", false);
            set => Attributes["href"] = value;
        }

        public override RectangleF Bounds
        {
            get
            {
                if (_gettingBounds)
                {
                    return new RectangleF();
                }

                _gettingBounds = true;
                PointF deviceValue1 = Location.ToDeviceValue(null, this);
                SvgUnit svgUnit = Width;
                var deviceValue2 = (double)svgUnit.ToDeviceValue(null, UnitRenderingType.Horizontal, this);
                svgUnit = Height;
                var deviceValue3 = (double)svgUnit.ToDeviceValue(null, UnitRenderingType.Vertical, this);
                SizeF size = new SizeF((float)deviceValue2, (float)deviceValue3);
                RectangleF bounds = TransformedBounds(new RectangleF(deviceValue1, size));
                _gettingBounds = false;
                return bounds;
            }
        }

        public override GraphicsPath Path(ISvgRenderer renderer)
        {
            if (_path == null)
            {
                RectangleF rectangleF = new RectangleF(Location.ToDeviceValue(renderer, this), SvgUnit.GetDeviceSize(Width, Height, renderer, this));
                _path = new GraphicsPath();
                _path.StartFigure();
                _path.AddRectangle(rectangleF);
                _path.CloseFigure();
            }
            return _path;
        }

        protected override void Render(ISvgRenderer renderer)
        {
            if (!Visible || !Displayable || (double)Width.Value <= 0.0 || (double)Height.Value <= 0.0 || Href == null)
            {
                return;
            }

            var image1 = GetImage(Href);
            Image image2 = image1 as Image;
            SvgFragment svgFragment = image1 as SvgFragment;
            if (image2 == null && svgFragment == null)
            {
                return;
            }

            try
            {
                if (!PushTransforms(renderer))
                {
                    return;
                }

                RectangleF rectangleF1 = image2 == null ? new RectangleF(new PointF(0.0f, 0.0f), svgFragment.GetDimensions()) : new RectangleF(0.0f, 0.0f, image2.Width, image2.Height);
                RectangleF rectangleF2 = new RectangleF(Location.ToDeviceValue(renderer, this), new SizeF(Width.ToDeviceValue(renderer, UnitRenderingType.Horizontal, this), Height.ToDeviceValue(renderer, UnitRenderingType.Vertical, this)));
                RectangleF destRect = rectangleF2;
                renderer.SetClip(new Region(rectangleF2), (CombineMode)1);
                SetClip(renderer);
                SvgAspectRatio aspectRatio = AspectRatio;
                if (aspectRatio.Align != SvgPreserveAspectRatio.none)
                {
                    var val1_1 = rectangleF2.Width / rectangleF1.Width;
                    var val2 = rectangleF2.Height / rectangleF1.Height;
                    var num1 = 0.0f;
                    var num2 = 0.0f;
                    float val1_2;
                    float num3;
                    if (aspectRatio.Slice)
                    {
                        val1_2 = Math.Max(val1_1, val2);
                        num3 = Math.Max(val1_2, val2);
                    }
                    else
                    {
                        val1_2 = Math.Min(val1_1, val2);
                        num3 = Math.Min(val1_2, val2);
                    }
                    switch (aspectRatio.Align)
                    {
                        case SvgPreserveAspectRatio.xMidYMid:
                            num1 = (float)(((double)rectangleF2.Width - (double)rectangleF1.Width * (double)val1_2) / 2.0);
                            num2 = (float)(((double)rectangleF2.Height - (double)rectangleF1.Height * (double)num3) / 2.0);
                            break;
                        case SvgPreserveAspectRatio.xMidYMin:
                            num1 = (float)(((double)rectangleF2.Width - (double)rectangleF1.Width * (double)val1_2) / 2.0);
                            break;
                        case SvgPreserveAspectRatio.xMaxYMin:
                            num1 = rectangleF2.Width - rectangleF1.Width * val1_2;
                            break;
                        case SvgPreserveAspectRatio.xMinYMid:
                            num2 = (float)(((double)rectangleF2.Height - (double)rectangleF1.Height * (double)num3) / 2.0);
                            break;
                        case SvgPreserveAspectRatio.xMaxYMid:
                            num1 = rectangleF2.Width - rectangleF1.Width * val1_2;
                            num2 = (float)(((double)rectangleF2.Height - (double)rectangleF1.Height * (double)num3) / 2.0);
                            break;
                        case SvgPreserveAspectRatio.xMinYMax:
                            num2 = rectangleF2.Height - rectangleF1.Height * num3;
                            break;
                        case SvgPreserveAspectRatio.xMidYMax:
                            num1 = (float)(((double)rectangleF2.Width - (double)rectangleF1.Width * (double)val1_2) / 2.0);
                            num2 = rectangleF2.Height - rectangleF1.Height * num3;
                            break;
                        case SvgPreserveAspectRatio.xMaxYMax:
                            num1 = rectangleF2.Width - rectangleF1.Width * val1_2;
                            num2 = rectangleF2.Height - rectangleF1.Height * num3;
                            break;
                    }
                    destRect = new RectangleF(rectangleF2.X + num1, rectangleF2.Y + num2, rectangleF1.Width * val1_2, rectangleF1.Height * num3);
                }
                if (image2 != null)
                {
                    var opacity = SvgElement.FixOpacityValue(Opacity);
                    if ((double)opacity == 1.0)
                    {
                        renderer.DrawImage(image2, destRect, rectangleF1, (GraphicsUnit)2);
                    }
                    else
                    {
                        renderer.DrawImage(image2, destRect, rectangleF1, (GraphicsUnit)2, opacity);
                    }
                }
                else
                {
                    using (Matrix transform = renderer.Transform)
                    {
                        PointF pointF = new PointF(transform.OffsetX, transform.OffsetY);
                        renderer.TranslateTransform(-pointF.X, -pointF.Y);
                        renderer.ScaleTransform(destRect.Width / rectangleF1.Width, destRect.Height / rectangleF1.Height);
                        renderer.TranslateTransform(pointF.X + destRect.X, pointF.Y + destRect.Y);
                    }
                    renderer.SetBoundable(new GenericBoundable(rectangleF1));
                    svgFragment.RenderElement(renderer);
                    renderer.PopBoundable();
                }
                ResetClip(renderer);
            }
            finally
            {
                PopTransforms(renderer);
                image2?.Dispose();
            }
        }

        protected object GetImage(string uriString)
        {
            var uriString1 = uriString.Length > 65519 ? uriString.Substring(0, 65519) : uriString;
            try
            {
                Uri uri = new Uri(uriString1, UriKind.RelativeOrAbsolute);
                if (uri.IsAbsoluteUri && uri.Scheme == "data")
                {
                    return GetImageFromDataUri(uriString);
                }

                if (!uri.IsAbsoluteUri)
                {
                    uri = new Uri(OwnerDocument.BaseUri, uri);
                }

                using (WebResponse response = WebRequest.Create(uri).GetResponse())
                {
                    using (Stream responseStream = response.GetResponseStream())
                    {
                        if (responseStream.CanSeek)
                        {
                            responseStream.Position = 0L;
                        }

                        return response.ContentType.StartsWith("image/svg+xml", StringComparison.InvariantCultureIgnoreCase) || uri.LocalPath.EndsWith(".svg", StringComparison.InvariantCultureIgnoreCase) ? LoadSvg(responseStream, uri) : Image.FromStream(responseStream);
                    }
                }
            }
            catch (Exception ex)
            {
                Trace.TraceError("Error loading image: '{0}', error: {1} ", uriString, ex.Message);
                return null;
            }
        }

        private object GetImageFromDataUri(string uriString)
        {
            var startIndex = 5;
            var num = uriString.IndexOf(",", startIndex);
            if (num < 0 || num + 1 >= uriString.Length)
            {
                throw new Exception("Invalid data URI");
            }

            var str1 = "text/plain";
            var str2 = "US-ASCII";
            var flag = false;
            List<string> stringList = new List<string>(uriString.Substring(startIndex, num - startIndex).Split(';', StringSplitOptions.None));
            if (stringList[0].Contains("/"))
            {
                str1 = stringList[0].Trim();
                stringList.RemoveAt(0);
                str2 = string.Empty;
            }
            if (stringList.Count > 0 && stringList[stringList.Count - 1].Trim().Equals("base64", StringComparison.InvariantCultureIgnoreCase))
            {
                flag = true;
                stringList.RemoveAt(stringList.Count - 1);
            }
            foreach (var str3 in stringList)
            {
                var strArray = str3.Split('=', StringSplitOptions.None);
                if (strArray.Length >= 2 && strArray[0].Trim().Equals("charset", StringComparison.InvariantCultureIgnoreCase))
                {
                    str2 = strArray[1].Trim();
                }
            }
            var s = uriString.Substring(num + 1);
            if (str1.Equals("image/svg+xml", StringComparison.InvariantCultureIgnoreCase))
            {
                if (flag)
                {
                    s = (string.IsNullOrEmpty(str2) ? Encoding.UTF8 : Encoding.GetEncoding(str2)).GetString(Convert.FromBase64String(s));
                }

                using (MemoryStream memoryStream = new MemoryStream(Encoding.Default.GetBytes(s)))
                {
                    return LoadSvg(memoryStream, OwnerDocument.BaseUri);
                }
            }
            else
            {
                if (!str1.StartsWith("image/") && !str1.StartsWith("img/"))
                {
                    return null;
                }

                using (MemoryStream memoryStream = new MemoryStream(flag ? Convert.FromBase64String(s) : Encoding.Default.GetBytes(s)))
                {
                    return Image.FromStream(memoryStream);
                }
            }
        }

        private SvgDocument LoadSvg(Stream stream, Uri baseUri)
        {
            SvgDocument svgDocument = SvgDocument.Open<SvgDocument>(stream);
            svgDocument.BaseUri = baseUri;
            return svgDocument;
        }

        public override SvgElement DeepCopy()
        {
            return DeepCopy<SvgImage>();
        }

        public override SvgElement DeepCopy<T>()
        {
            SvgImage svgImage = base.DeepCopy<T>() as SvgImage;
            svgImage.Height = Height;
            svgImage.Width = Width;
            svgImage.X = X;
            svgImage.Y = Y;
            svgImage.Href = Href;
            svgImage.AspectRatio = new SvgAspectRatio(AspectRatio.Align, AspectRatio.Slice, AspectRatio.Defer);
            return svgImage;
        }
    }
}
