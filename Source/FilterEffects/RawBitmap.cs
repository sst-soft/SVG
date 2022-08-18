// todo: add license

using System.Drawing;
using System.Drawing.Imaging;
using System.Runtime.InteropServices;

namespace Svg.FilterEffects
{
    internal sealed class RawBitmap : IDisposable
    {
        private readonly Bitmap _originBitmap;
        private readonly BitmapData _bitmapData;
        private readonly IntPtr _ptr;
        private readonly int _bytes;
        private byte[] _argbValues;

        public RawBitmap(Bitmap originBitmap)
        {
            _originBitmap = originBitmap;
            _bitmapData = _originBitmap.LockBits(new Rectangle(0, 0, _originBitmap.Width, _originBitmap.Height), (ImageLockMode)3, (PixelFormat)2498570);
            _ptr = _bitmapData.Scan0;
            _bytes = Stride * _originBitmap.Height;
            _argbValues = new byte[_bytes];
            Marshal.Copy(_ptr, _argbValues, 0, _bytes);
        }

        public void Dispose()
        {
            _originBitmap.UnlockBits(_bitmapData);
        }

        public int Stride => _bitmapData.Stride;

        public int Width => _bitmapData.Width;

        public int Height => _bitmapData.Height;

        public byte[] ArgbValues
        {
            get => _argbValues;
            set => _argbValues = value;
        }

        public Bitmap Bitmap
        {
            get
            {
                Marshal.Copy(_argbValues, 0, _ptr, _bytes);
                return _originBitmap;
            }
        }
    }
}
