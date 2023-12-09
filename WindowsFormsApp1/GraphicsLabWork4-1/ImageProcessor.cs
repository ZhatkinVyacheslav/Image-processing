using System;
using System.Collections.Generic;
using System.Drawing.Imaging;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GraphicsLabWork4_1
{
    class ImageProcessor
    {
        private string _bitmapPath;
        private Bitmap _currentBitmap;
        private Bitmap _originalBitmap;
        private bool isGrayscale = false;
        byte bitsPerPixel;

        public bool IsGrayscale
        {
            get { return isGrayscale; }
            private set { isGrayscale = value; }
        }

        public Bitmap OriginalBitmap
        {
            set { _originalBitmap = value; }
            get { return _originalBitmap; }
        }

        
        public Bitmap CurrentBitmap
        {
            get { return _currentBitmap; }
            set { _currentBitmap = value; isGrayscale = false; }
        }

    
        public string BitmapPath
        {
            get { return _bitmapPath; }
            set { _bitmapPath = value; }
        }

        public byte GetBitsPerPixel(PixelFormat pf)
        {
            byte BitsPerPixel;
            switch (pf)
            {
                case PixelFormat.Format8bppIndexed:
                    BitsPerPixel = 8;
                    break;
                case PixelFormat.Format24bppRgb:
                    BitsPerPixel = 24;
                    break;
                case PixelFormat.Format32bppArgb:
                case PixelFormat.Format32bppPArgb:
                    BitsPerPixel = 32;
                    break;
                default:
                    BitsPerPixel = 0;
                    break;
            }
            return BitsPerPixel;
        }


        public unsafe void SetGrayscale()
        {
            if (CurrentBitmap == null || isGrayscale) return;

            BitmapData bData = _currentBitmap.LockBits(new Rectangle(0, 0, _currentBitmap.Width, _currentBitmap.Height), ImageLockMode.ReadWrite, _currentBitmap.PixelFormat);
            bitsPerPixel = GetBitsPerPixel(bData.PixelFormat);
            byte* scan0 = (byte*)bData.Scan0.ToPointer();

            byte* data;
            for (int i = 0; i < bData.Height; ++i)
            {
                for (int j = 0; j < bData.Width; ++j)
                {
                    data = scan0 + i * bData.Stride + j * bitsPerPixel / 8;

                    if (bitsPerPixel >= 24)
                    {
                        var gray = (byte)(.299 * data[2] + .587 * data[1] + .114 * data[0]);

                        data[0] = gray;
                        data[1] = gray;
                        data[2] = gray;
                        //data is a pointer to the first byte of the 3-byte color data    
                    }

                }
            }

            _currentBitmap.UnlockBits(bData);
            isGrayscale = true;
        }

        public unsafe double[,] GetNormalizedMatrix()
        {
            if (_originalBitmap == null) return null;

            BitmapData bData = _originalBitmap.LockBits(new Rectangle(0, 0, _originalBitmap.Width, _originalBitmap.Height), ImageLockMode.ReadWrite, _originalBitmap.PixelFormat);
            bitsPerPixel = GetBitsPerPixel(bData.PixelFormat);
            byte* scan0 = (byte*)bData.Scan0.ToPointer();

            var normalizedMatrix = new double[_originalBitmap.Width, _originalBitmap.Height];

            byte* data;
            for (int i = 0; i < bData.Height; ++i)
            {
                for (int j = 0; j < bData.Width; ++j)
                {
                    data = scan0 + i * bData.Stride + j * bitsPerPixel / 8;

                    normalizedMatrix[j, i] = data[0] / 255d;
                }
            }

            _originalBitmap.UnlockBits(bData);

            return normalizedMatrix;
        }

        public unsafe void DenormalizeCurrent(double[,] norm)
        {
            if (norm == null) return;
            int n = norm.GetLength(0);
            int m = norm.GetLength(1);

            if (m != _currentBitmap.Height || n != _currentBitmap.Width)
            {
                throw new Exception("Sizes don't match.");
            }


            BitmapData bData = _currentBitmap.LockBits(new Rectangle(0, 0, _currentBitmap.Width, _currentBitmap.Height), ImageLockMode.ReadWrite, _currentBitmap.PixelFormat);
            bitsPerPixel = GetBitsPerPixel(bData.PixelFormat);
            byte* scan0 = (byte*)bData.Scan0.ToPointer();

            byte* data;
            for (int i = 0; i < bData.Height; ++i)
            {
                for (int j = 0; j < bData.Width; ++j)
                {
                    data = scan0 + i * bData.Stride + j * bitsPerPixel / 8;

                    byte newCol = norm[j, i] == 0 ? (byte)0 : (byte)255;
                    if (bitsPerPixel >= 24)
                    {
                        data[0] = newCol;
                        data[1] = newCol;
                        data[2] = newCol;
                    }
                    else
                    {
                        data[0] = newCol;
                    }
                }
            }

            _currentBitmap.UnlockBits(bData);
        }

        public void CleanUp()
        {
            _currentBitmap = null;
            _originalBitmap = null;
            isGrayscale = false;
            bitsPerPixel = 0;
            _bitmapPath = "";
            GC.Collect();
        }
    }
}
