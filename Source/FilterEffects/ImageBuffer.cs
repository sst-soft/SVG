// todo: add license

using System.Collections;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;

namespace Svg.FilterEffects
{
    public class ImageBuffer :
    IDictionary<string, Bitmap>,
    ICollection<KeyValuePair<string, Bitmap>>,
    IEnumerable<KeyValuePair<string, Bitmap>>,
    IEnumerable
    {
        private const string BufferKey = "__!!BUFFER";
        private readonly Dictionary<string, Bitmap> _images;
        private RectangleF _bounds;
        private readonly ISvgRenderer _renderer;
        private readonly Action<ISvgRenderer> _renderMethod;
        private readonly float _inflate;

        public Matrix Transform { get; set; }

        public Bitmap Buffer => _images["__!!BUFFER"];

        public int Count => _images.Count;

        public Bitmap this[string key]
        {
            get => ProcessResult(key, _images[ProcessKey(key)]);
            set
            {
                if (!string.IsNullOrEmpty(key))
                {
                    _images[key] = value;
                }

                _images["__!!BUFFER"] = value;
            }
        }

        public ImageBuffer(
          RectangleF bounds,
          float inflate,
          ISvgRenderer renderer,
          Action<ISvgRenderer> renderMethod)
        {
            _bounds = bounds;
            _inflate = inflate;
            _renderer = renderer;
            _renderMethod = renderMethod;
            _images = new Dictionary<string, Bitmap>
            {
                ["BackgroundAlpha"] = null,
                ["BackgroundImage"] = null,
                ["FillPaint"] = null,
                ["SourceAlpha"] = null,
                ["SourceGraphic"] = null,
                ["StrokePaint"] = null
            };
        }

        public void Add(string key, Bitmap value)
        {
            _images.Add(ProcessKey(key), value);
        }

        public bool ContainsKey(string key)
        {
            return _images.ContainsKey(ProcessKey(key));
        }

        public void Clear()
        {
            _images.Clear();
        }

        public IEnumerator<KeyValuePair<string, Bitmap>> GetEnumerator()
        {
            return _images.GetEnumerator();
        }

        public bool Remove(string key)
        {
            switch (key)
            {
                case "BackgroundAlpha":
                case "BackgroundImage":
                case "FillPaint":
                case "SourceAlpha":
                case "SourceGraphic":
                case "StrokePaint":
                    return false;
                default:
                    return _images.Remove(ProcessKey(key));
            }
        }

        public bool TryGetValue(string key, out Bitmap value)
        {
            if (!_images.TryGetValue(ProcessKey(key), out value))
            {
                return false;
            }

            value = ProcessResult(key, value);
            return true;
        }

        private Bitmap ProcessResult(string key, Bitmap curr)
        {
            if (curr == null)
            {
                if (string.IsNullOrEmpty(key))
                {
                    key = "SourceGraphic";
                }

                switch (key)
                {
                    case "BackgroundAlpha":
                    case "BackgroundImage":
                    case "FillPaint":
                    case "StrokePaint":
                        return null;
                    case "SourceAlpha":
                        _images[key] = CreateSourceAlpha();
                        return _images[key];
                    case "SourceGraphic":
                        _images[key] = CreateSourceGraphic();
                        return _images[key];
                }
            }
            return curr;
        }

        private string ProcessKey(string key)
        {
            if (!string.IsNullOrEmpty(key))
            {
                return key;
            }

            return !_images.ContainsKey("__!!BUFFER") ? "SourceGraphic" : "__!!BUFFER";
        }

        private Bitmap CreateSourceGraphic()
        {
            Bitmap sourceGraphic = new Bitmap((int)((double)_bounds.Width + 2.0 * _inflate * (double)_bounds.Width + (double)_bounds.X), (int)((double)_bounds.Height + 2.0 * _inflate * (double)_bounds.Height + (double)_bounds.Y));
            using (ISvgRenderer svgRenderer = SvgRenderer.FromImage(sourceGraphic))
            {
                svgRenderer.SetBoundable(_renderer.GetBoundable());
                Matrix matrix = new Matrix();
                matrix.Translate(_bounds.Width * _inflate, _bounds.Height * _inflate);
                svgRenderer.Transform = matrix;
                _renderMethod(svgRenderer);
            }
            return sourceGraphic;
        }

        private Bitmap CreateSourceAlpha()
        {
            Bitmap bitmap = this["SourceGraphic"];
            ColorMatrix colorMatrix = new ColorMatrix(new float[5][]
            {
        new float[5],
        new float[5],
        new float[5],
        new float[5]{ 0.0f, 0.0f, 0.0f, 1f, 1f },
        new float[5]
            });
            Bitmap sourceAlpha = new Bitmap(bitmap.Width, bitmap.Height);
            using (Graphics graphics = Graphics.FromImage(sourceAlpha))
            {
                using (ImageAttributes imageAttributes = new ImageAttributes())
                {
                    imageAttributes.SetColorMatrix(colorMatrix);
                    graphics.DrawImage(bitmap, new Rectangle(0, 0, bitmap.Width, bitmap.Height), 0, 0, bitmap.Width, bitmap.Height, (GraphicsUnit)2, imageAttributes);
                    graphics.Save();
                }
            }
            return sourceAlpha;
        }

        bool ICollection<KeyValuePair<string, Bitmap>>.IsReadOnly => false;

        ICollection<string> IDictionary<string, Bitmap>.Keys => _images.Keys;

        ICollection<Bitmap> IDictionary<string, Bitmap>.Values => _images.Values;

        void ICollection<KeyValuePair<string, Bitmap>>.Add(
          KeyValuePair<string, Bitmap> item)
        {
            _images.Add(item.Key, item.Value);
        }

        bool ICollection<KeyValuePair<string, Bitmap>>.Contains(
          KeyValuePair<string, Bitmap> item)
        {
            return ((ICollection<KeyValuePair<string, Bitmap>>)_images).Contains(item);
        }

        void ICollection<KeyValuePair<string, Bitmap>>.CopyTo(
          KeyValuePair<string, Bitmap>[] array,
          int arrayIndex)
        {
            ((ICollection<KeyValuePair<string, Bitmap>>)_images).CopyTo(array, arrayIndex);
        }

        bool ICollection<KeyValuePair<string, Bitmap>>.Remove(
          KeyValuePair<string, Bitmap> item)
        {
            return _images.Remove(item.Key);
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return _images.GetEnumerator();
        }
    }
}
