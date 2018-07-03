﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;
using System.IO;
using System.Drawing.Drawing2D;
using FotosDaPiteca.Effects;

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

        public static byte[] RenderFinal(byte[] Original, SizeF Tamanho, bool UseWatermark, string Watermark, int WatermarkPosition, string WatermarkColor, string WatermarkFont, int WatermarkFontSize)
        {
            using (MemoryStream ms = new MemoryStream(Original))
            {
                using (Bitmap bm = new Bitmap(ms))
                {
                    if (Tamanho.Width == 0) Tamanho.Width = 1;
                    if (Tamanho.Height == 0) Tamanho.Height = 1;

                    float ZoomFactor = Math.Min(Tamanho.Width / bm.Width, Tamanho.Height / bm.Height);

                    SizeF ZoomedSize = new SizeF(bm.Width * ZoomFactor, bm.Height * ZoomFactor);



                    using (Bitmap bmFinal = new Bitmap((int)ZoomedSize.Width, (int)ZoomedSize.Height))
                    {
                        using (Graphics gr = Graphics.FromImage(bmFinal))
                        {
                            gr.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;
                            gr.CompositingQuality = System.Drawing.Drawing2D.CompositingQuality.HighQuality;
                            gr.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.HighQualityBicubic;
                            gr.DrawImage(bm, 0, 0, ZoomedSize.Width, ZoomedSize.Height);
                            if (UseWatermark)
                            {
                                gr.TextRenderingHint = System.Drawing.Text.TextRenderingHint.AntiAlias;
                                int RenderFontSize = WatermarkFontSize;
                                RenderFontSize = Convert.ToInt32((Convert.ToDouble(WatermarkFontSize) / Convert.ToDouble(bm.Height)) * Convert.ToDouble(ZoomedSize.Height));
                                SizeF s = gr.MeasureString(Watermark, new Font(WatermarkFont, RenderFontSize));



                                PointF Posicao = GetTextDrawPosition(WatermarkPosition, new SizeF(bmFinal.Size.Width, bmFinal.Size.Height), s);

                                try
                                {
                                    using (Bitmap Blur = GetWaterMarkShadow(Watermark, s, Color.Black, new Font(WatermarkFont, RenderFontSize)))
                                    {
                                        gr.DrawImage(Blur, Posicao.X - 5, Posicao.Y - 5);
                                    }
                                }
                                catch (Exception e) { }





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

        private static Bitmap GetWaterMarkShadow(string Watermark, SizeF ShadowSize, Color WatermarkColor, Font WatermarkFont)
        {
            using (Bitmap bmFinal = new Bitmap((int)ShadowSize.Width + 10, (int)ShadowSize.Height + 10))
            {
                using (Graphics gr = Graphics.FromImage(bmFinal))
                {
                    gr.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;
                    gr.CompositingQuality = System.Drawing.Drawing2D.CompositingQuality.HighQuality;
                    gr.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.HighQualityBicubic;
                    gr.TextRenderingHint = System.Drawing.Text.TextRenderingHint.AntiAlias;
                    gr.Clear(Color.Transparent);
                    gr.DrawString(Watermark, WatermarkFont, new SolidBrush(WatermarkColor), new PointF(5, 5));
                }
                var blur = new GaussianBlur(bmFinal as Bitmap);
                Bitmap result = blur.Process(10);
                return result;
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
