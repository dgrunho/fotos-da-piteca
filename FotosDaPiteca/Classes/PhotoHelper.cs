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

        public static byte[] RenderFinal(byte[] Original, bool UseWatermark, string Watermark, string WatermarkPosition, string WatermarkColor, string WatermarkFont, int WatermarkFontSize)
        {
            using (MemoryStream ms = new MemoryStream(Original))
            {
                using (Bitmap bm = new Bitmap(ms))
                {
                    using (Graphics gr = Graphics.FromImage(bm))
                    {
                        gr.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;
                        gr.CompositingQuality = System.Drawing.Drawing2D.CompositingQuality.HighQuality;
                        if (UseWatermark)
                        {
                            SizeF s = gr.MeasureString(Watermark,new Font(WatermarkFont, WatermarkFontSize));
                            float midW = (bm.Width / 2) - (s.Width / 2);
                            float midH = (bm.Height / 2) - (s.Height / 2);
                            gr.DrawString(Watermark, new Font(WatermarkFont, WatermarkFontSize), new SolidBrush(System.Drawing.ColorTranslator.FromHtml(WatermarkColor)), new PointF(midW, midH));
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
