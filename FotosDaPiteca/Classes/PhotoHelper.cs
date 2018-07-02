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
        public static byte[] RenderThumb(byte[] Original, Size Tamanho)
        {
            using (MemoryStream ms = new MemoryStream(Original))
            {
                using (Bitmap bmOriginal = new Bitmap(ms))
                {
                    using (MemoryStream msSave = new MemoryStream())
                    {
                        bmOriginal.GetThumbnailImage(Tamanho.Width, Tamanho.Height, null, IntPtr.Zero).Save(msSave, System.Drawing.Imaging.ImageFormat.Jpeg);
                        return msSave.ToArray();
                    }
                }
            }
        }

        public static byte[] RenderFinal(byte[] Original, Size Tamanho, bool UseWatermark, string Watermark, int WatermarkPosition, string WatermarkColor, string WatermarkFont, int WatermarkFontSize)
        {
            using (MemoryStream ms = new MemoryStream(Original))
            {
                using (Bitmap bm = new Bitmap(ms))
                {
                    using (Bitmap bmFinal = new Bitmap(Tamanho.Width, Tamanho.Height))
                    {
                        using (Graphics gr = Graphics.FromImage(bmFinal))
                        {
                            gr.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;
                            gr.CompositingQuality = System.Drawing.Drawing2D.CompositingQuality.HighQuality;
                            gr.DrawImage(bm, 0, 0, Tamanho.Width, Tamanho.Height);
                            if (UseWatermark)
                            {
                                int RenderFontSize = WatermarkFontSize;
                                RenderFontSize = Convert.ToInt32((Convert.ToDouble(WatermarkFontSize) / Convert.ToDouble(bm.Height)) * Convert.ToDouble(Tamanho.Height));
                                SizeF s = gr.MeasureString(Watermark, new Font(WatermarkFont, RenderFontSize));

                                PointF Posicao = GetTextDrawPosition(WatermarkPosition, new SizeF(bmFinal.Size.Width, bmFinal.Size.Height), s);
                                //float midW = (bmFinal.Width / 2) - (s.Width / 2);
                                //float midH = (bmFinal.Height / 2) - (s.Height / 2);
                                gr.DrawString(Watermark, new Font(WatermarkFont, RenderFontSize), new SolidBrush(System.Drawing.ColorTranslator.FromHtml(WatermarkColor)), Posicao);
                            }

                        }
                        using (MemoryStream msSave = new MemoryStream())
                        {
                            bmFinal.Save(msSave, System.Drawing.Imaging.ImageFormat.Bmp);
                            return msSave.ToArray();
                        }
                    }
                    
                }
            }
        }

        public static SizeF GetImageSize(byte[] Original)
        {
            using (MemoryStream ms = new MemoryStream(Original))
            {
                using (Bitmap bm = new Bitmap(ms))
                {
                    return new SizeF(bm.Width, bm.Height);

                }
            }
        }

        private static PointF GetTextDrawPosition(int Position, SizeF ImageSize, SizeF TextSize)
        {
            int Margin = Convert.ToInt32(ImageSize.Width * 0.01);
            switch (Position)
            {
                case 1:
                    return new PointF(Margin, Margin);
                case 2:
                    return new PointF(ImageSize.Width - TextSize.Width - Margin, Margin);
                case 3:
                    float midW = (ImageSize.Width / 2) - (TextSize.Width / 2);
                    float midH = (ImageSize.Height / 2) - (TextSize.Height / 2);
                    return new PointF(midW, midH);
                case 4:
                    return new PointF(Margin, ImageSize.Height - TextSize.Height - Margin);
                case 5:
                    return new PointF(ImageSize.Width - TextSize.Width - Margin, ImageSize.Height - TextSize.Height - Margin);
                default:
                    return new Point(Margin, Margin);
            }

        }
    }
}
