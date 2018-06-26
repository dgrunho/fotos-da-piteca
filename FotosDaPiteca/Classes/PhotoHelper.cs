using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;
using System.IO;

namespace FotosDaPiteca.Classes
{
     class PhotoHelper
    {
        public static byte[] RenderThumb(byte[] Original, int width, int height)
        {
            using (MemoryStream ms = new MemoryStream(Original))
            {
                using (Bitmap bmOriginal = new Bitmap(ms))
                {
                    using (MemoryStream msSave = new MemoryStream())
                    {
                        bmOriginal.GetThumbnailImage(width, height, null, IntPtr.Zero).Save(msSave, System.Drawing.Imaging.ImageFormat.Jpeg);
                        return msSave.ToArray();
                    }
                }
            }
        }

        public static byte[] RenderFinal(byte[] Original, bool UseWatermark, string Watermark, string WatermarkPosition, string WatermarkColor, string WatermarkFont)
        {
            using (MemoryStream ms = new MemoryStream(Original))
            {
                using (Bitmap bm = new Bitmap(ms))
                {
                    using (Graphics gr = Graphics.FromImage(bm))
                    {
                        if (UseWatermark)
                        {
                            SizeF s = gr.MeasureString(Watermark,new Font(WatermarkFont, 24));

                            gr.DrawString(Watermark, new Font(WatermarkFont, 24), Brushes.Black, new PointF(0, 0));
                        }
                        
                    }
                    using (MemoryStream msSave = new MemoryStream())
                    {
                        bm.Save(msSave, System.Drawing.Imaging.ImageFormat.Jpeg);
                        return msSave.ToArray();
                    }
                }
            }
        }
    }
}
