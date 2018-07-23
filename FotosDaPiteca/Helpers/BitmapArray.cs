using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Security;
using System.Text;
using System.Threading.Tasks;
using Microsoft.VisualBasic;
using System.Drawing.Imaging;
using System.Runtime.InteropServices;
using System.Drawing;

namespace FotosDaPiteca.Helpers
{
    class BitmapArray
    {
        public byte[] ImageBytes;
        public int RowSizeBytes;
        public const int PixelSizeBytes = 4;
        public const int PixelSizeBits = PixelSizeBytes * 8;
        public int Width;
        public int Height;
        private Bitmap m_Bitmap;
        public BitmapArray(Bitmap bm)
        {
            m_Bitmap = bm;
        }
        // teste
        private BitmapData m_BitmapData;

        public void LockBitmap()
        {
            Rectangle bounds = new Rectangle(0, 0, m_Bitmap.Width, m_Bitmap.Height);
            Width = m_Bitmap.Width;
            Height = m_Bitmap.Height;
            m_BitmapData = m_Bitmap.LockBits(bounds, System.Drawing.Imaging.ImageLockMode.ReadWrite, System.Drawing.Imaging.PixelFormat.Format32bppArgb);
            RowSizeBytes = m_BitmapData.Stride;

            int total_size = m_BitmapData.Stride * m_BitmapData.Height;
            ImageBytes = new byte[total_size + 1];
            Marshal.Copy(m_BitmapData.Scan0, ImageBytes, 0, total_size);
        }

        public void UnlockBitmap()
        {
            int total_size = m_BitmapData.Stride * m_BitmapData.Height;
            Marshal.Copy(ImageBytes, 0, m_BitmapData.Scan0, total_size);
            m_Bitmap.UnlockBits(m_BitmapData);

            ImageBytes = null;
            m_BitmapData = null;
        }

        public Color getPixel(int x, int y)
        {
            Int64 k;
            k = (RowSizeBytes * y) + (4 * x);
            return Color.FromArgb(ImageBytes[k + 3], ImageBytes[k + 2], ImageBytes[k + 1], ImageBytes[k + 0]);
        }

        public void setPixel(int x, int y, Color cor)
        {
            Int64 k;
            k = (RowSizeBytes * y) + (4 * x);
            ImageBytes[k + 3] = cor.A;
            ImageBytes[k + 2] = cor.R;
            ImageBytes[k + 1] = cor.G;
            ImageBytes[k + 0] = cor.B;
        }
    }
}
