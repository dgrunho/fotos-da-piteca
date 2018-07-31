using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;
using System.IO;
using System.Drawing.Drawing2D;
using FotosDaPiteca.Effects;
using System.Runtime.InteropServices;

namespace FotosDaPiteca.Classes
{
     class PhotoHelper
    {

        public static System.Windows.Media.Imaging.BitmapSource RenderThumb(byte[] Original, Size Tamanho)
        {
            using (MemoryStream ms = new MemoryStream(Original))
            {
                using (Bitmap bmOriginal = new Bitmap(ms))
                {
                    return ConvertToBitmapSource(bmOriginal);
                    //using (MemoryStream msSave = new MemoryStream())
                    //{
                    //    bmOriginal.GetThumbnailImage(Tamanho.Width, Tamanho.Height, null, IntPtr.Zero).Save(msSave, System.Drawing.Imaging.ImageFormat.Jpeg);
                    //    return msSave.ToArray();
                    //}
                }
            }
        }

        public static System.Windows.Media.Imaging.BitmapSource RenderFinal(FotosDaPiteca.Models.Photo Foto, SizeF Tamanho)
        {
            
            using (MemoryStream ms = new MemoryStream(Foto.Image))
            {
                using (Bitmap bmOriginal = new Bitmap(ms))
                {
                    Bitmap bm = (Bitmap)bmOriginal.Clone();
                    if (Tamanho.Width == 0) Tamanho.Width = 1;
                    if (Tamanho.Height == 0) Tamanho.Height = 1;

                    float ZoomFactor = Math.Min(Tamanho.Width / bm.Width, Tamanho.Height / bm.Height);

                    SizeF ZoomedSize = new SizeF(bm.Width * ZoomFactor, bm.Height * ZoomFactor);


                    if (Foto.RedBalance != 255 || Foto.GreenBalance != 255 || Foto.BlueBalance != 255)
                    {
                        bm = Effects.ColorBalance.SetImageColorBalance(bm, Foto.RedBalance, Foto.GreenBalance, Foto.BlueBalance);
                    }

                    bm = Effects.ColorCorrections.SetImageBrightnessContrastGamma(bm, ((float)(Foto.Brightness) / 100) + 1f, ((float)(Foto.Contrast) / 100) + 1f, ((float)(Foto.Gamma) / 100) + 1f);
                        
                    if (Foto.Normalize)
                    {
                        bm = Effects.ColorCorrections.NormalizeImageBrightness(bm);
                        //bm = Effects.Contrast.SetImageContrast(bm, Foto.Contrast);
                    }
                    
                    using (Bitmap bmFinal = new Bitmap((int)ZoomedSize.Width, (int)ZoomedSize.Height))
                    {
                        using (Graphics gr = Graphics.FromImage(bmFinal))
                        {
                            gr.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.HighSpeed;
                            gr.CompositingQuality = System.Drawing.Drawing2D.CompositingQuality.HighSpeed;
                            gr.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.Low;
                            gr.DrawImage(bm, 0, 0, ZoomedSize.Width, ZoomedSize.Height);




                            if (Foto.UseWatermark)
                            {
                                gr.TextRenderingHint = System.Drawing.Text.TextRenderingHint.AntiAlias;
                                int RenderFontSize = Foto.WatermarkFontSize;
                                RenderFontSize = Convert.ToInt32((Convert.ToDouble(Foto.WatermarkFontSize) / Convert.ToDouble(bm.Height)) * Convert.ToDouble(ZoomedSize.Height));
                                SizeF s = gr.MeasureString(Foto.Watermark, new Font(Foto.WatermarkFont, RenderFontSize));



                                PointF Posicao = GetTextDrawPosition((int)Foto.WatermarkPosition, new SizeF(bmFinal.Size.Width, bmFinal.Size.Height), s);

                                if (Foto.AddShadow) {
                                    if (Foto.RenderedImageSize.Width == 0) Foto.RenderedImageSize.Width = 1;
                                    if (Foto.RenderedImageSize.Height == 0) Foto.RenderedImageSize.Height = 1;

                                    float zoomFactor = Math.Min(Foto.RenderedImageSize.Width / Foto.ImageSize.Width, Foto.RenderedImageSize.Height / Foto.ImageSize.Height);

                                    using (Bitmap Blur = GetWatermarkShadow(Foto.Watermark, s, Color.FromArgb(100, Color.Black), new Font(Foto.WatermarkFont, RenderFontSize), Foto.ShadowRadius, zoomFactor))
                                    {
                                        if (Blur != null)
                                        {
                                            gr.DrawImage(Blur, Posicao.X - 5, Posicao.Y - 5);
                                        }

                                    }

                                }
                                

                                gr.DrawString(Foto.Watermark, new Font(Foto.WatermarkFont, RenderFontSize), new SolidBrush(System.Drawing.ColorTranslator.FromHtml(Foto.WatermarkColor)), Posicao);
                            }

                        }
                        return ConvertToBitmapSource(bmFinal);
                        //using (MemoryStream msSave = new MemoryStream())
                        //{
                        //    bmFinal.Save(msSave, System.Drawing.Imaging.ImageFormat.Bmp);
                        //    return msSave.ToArray();
                        //}
                    }
                    
                }
            }
        }

        private static Bitmap GetWatermarkShadow(string Watermark, SizeF ShadowSize, Color WatermarkColor, Font WatermarkFont, int ShadowRadious, float zoom)
        {
            if (ShadowSize.Width > 0 && ShadowSize.Height > 0)
            {
                using (Bitmap bmFinal = new Bitmap((int)ShadowSize.Width + 10, (int)ShadowSize.Height + 10))
                {
                    using (Graphics gr = Graphics.FromImage(bmFinal))
                    {
                        gr.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.HighSpeed;
                        gr.CompositingQuality = System.Drawing.Drawing2D.CompositingQuality.HighSpeed;
                        gr.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.Low;
                        gr.TextRenderingHint = System.Drawing.Text.TextRenderingHint.AntiAlias;
                        gr.Clear(Color.Transparent);
                        gr.DrawString(Watermark, WatermarkFont, new SolidBrush(WatermarkColor), new PointF(5, 5));

                        GraphicsPath gp = new GraphicsPath();
                        float emSize = gr.DpiY * WatermarkFont.SizeInPoints / 72;
                        gp.AddString(Watermark, WatermarkFont.FontFamily, (int)WatermarkFont.Style, emSize, new RectangleF(5, 5, (float)((ShadowSize.Width - 5) * 5.25), ShadowSize.Height - 5), StringFormat.GenericDefault);
                        gr.DrawPath(new Pen(WatermarkColor, 15 * zoom), gp);
                    }
                    var blur = new GaussianBlur(bmFinal as Bitmap);
                    Bitmap result = blur.Process((int)((float)ShadowRadious * zoom));
                    return result;
                }
            }
            return null;

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

        public static byte[] CropImage(FotosDaPiteca.Models.Photo Foto, RectangleF rectangle)
        {

            using (MemoryStream ms = new MemoryStream(Foto.Image))
            {
                using (Bitmap bmOriginal = new Bitmap(ms))
                {
                    Bitmap bm = (Bitmap)bmOriginal.Clone();

                    float ZoomFactor = Math.Min((float)Foto.RenderedImageSize.Width / (float)bm.Width, (float)Foto.RenderedImageSize.Height / (float)bm.Height);

                    RectangleF ZoomedRectangle = new RectangleF(rectangle.X / ZoomFactor, rectangle.Y / ZoomFactor, rectangle.Width / ZoomFactor, rectangle.Height / ZoomFactor);



                    using (Bitmap bmFinal = new Bitmap((int)ZoomedRectangle.Width, (int)ZoomedRectangle.Height))
                    {
                        using (Graphics gr = Graphics.FromImage(bmFinal))
                        {
                            gr.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;
                            gr.CompositingQuality = System.Drawing.Drawing2D.CompositingQuality.HighQuality;
                            gr.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.HighQualityBicubic;
                            gr.DrawImage(bm, -ZoomedRectangle.X , -ZoomedRectangle.Y, bm.Width, bm.Height);
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


        [DllImport("gdi32.dll", EntryPoint = "DeleteObject")]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool DeleteObject([In] IntPtr hObject);
        public static System.Windows.Media.Imaging.BitmapSource ConvertToBitmapSource(Bitmap bitmap)
        {
            var handle = bitmap.GetHbitmap();
            try
            {
                System.Windows.Media.Imaging.BitmapSource bms = System.Windows.Interop.Imaging.CreateBitmapSourceFromHBitmap(handle, IntPtr.Zero, System.Windows.Int32Rect.Empty, System.Windows.Media.Imaging.BitmapSizeOptions.FromEmptyOptions());
                bms.Freeze();
                return bms;
            }
            finally { DeleteObject(handle); }
        }

        public static Bitmap ConvertFromBitmapSource(System.Windows.Media.Imaging.BitmapSource bitmapsource)
        {
            Bitmap bitmap;
            using (var outStream = new MemoryStream())
            {
                System.Windows.Media.Imaging.BitmapEncoder enc = new System.Windows.Media.Imaging.BmpBitmapEncoder();
                enc.Frames.Add(System.Windows.Media.Imaging.BitmapFrame.Create(bitmapsource));
                enc.Save(outStream);
                bitmap = new Bitmap(outStream);
           
            }
            return bitmap;
        }

        public static byte[] ConvertFromBitmapSourceBytes(System.Windows.Media.Imaging.BitmapSource bitmapsource)
        {
            //Bitmap bitmap;
            using (var outStream = new MemoryStream())
            {
                System.Windows.Media.Imaging.BitmapEncoder enc = new System.Windows.Media.Imaging.BmpBitmapEncoder();
                enc.Frames.Add(System.Windows.Media.Imaging.BitmapFrame.Create(bitmapsource));
                enc.Save(outStream);
                //bitmap = new Bitmap(outStream);
                return outStream.ToArray();
            }
         
        }
    }
}
