using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.Drawing;
using System.IO;
using System.Drawing.Drawing2D;
using FotosDaPiteca.Effects;
using System.Drawing.Imaging;
using System.Timers;
using System.ComponentModel;

namespace FotosDaPiteca.Classes
{
    class DrawHelper
    {
        public System.Windows.Controls.Image _img;
        public Models.Photo _Foto;
        public ViewModel.MainWindowViewModel _vm;
        Bitmap bmFull;
        Bitmap bmRender;
        float zoomFactor = 0;
        float InvertedZoomFactor = 0;
        PointF LastPt;
        PointF CurrPt;

        List<Tuple<PointF, PointF>> lstPts;

        public void Destroy() {
            lstPts.Clear();
            bmFull.Dispose();
            bmRender.Dispose();
        }

        public DrawHelper(System.Windows.Controls.Image Img, FotosDaPiteca.Models.Photo Foto, ViewModel.MainWindowViewModel vm)
        {
            _img = Img;
            _Foto = Foto;
            _vm = vm;
            _img.MouseDown += _img_MouseDown;
            _img.MouseUp += _img_MouseUp;
            _img.MouseMove += _img_MouseMove;
        }


        private void _img_MouseDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            if (bmRender == null)
            {
                DrawRenderImage();
            }

            
            CurrPt = new PointF();
            LastPt = ConvertionHelpers.PointConverter(e.GetPosition((System.Windows.Controls.Image)sender), (float)_vm.ScaleX, (float)_vm.ScaleY);

            lstPts = new List<Tuple<PointF, PointF>>();
        }



        private void _img_MouseMove(object sender, System.Windows.Input.MouseEventArgs e)
        {
            if (e.LeftButton == System.Windows.Input.MouseButtonState.Pressed)
            {
                if (CurrPt != new PointF())
                {
                    LastPt = CurrPt;
                }


                CurrPt = ConvertionHelpers.PointConverter(e.GetPosition((System.Windows.Controls.Image)sender), (float)_vm.ScaleX, (float)_vm.ScaleY);

                int Step = 2;
                ProcessPointsFull(LastPt, CurrPt, bmRender, true, zoomFactor, (float)_vm.ScaleX, (float)_vm.ScaleY, Step);

                PointF LstPonto = new PointF((LastPt.X / zoomFactor), (LastPt.Y / zoomFactor));
                PointF CurPonto = new PointF((CurrPt.X / zoomFactor), (CurrPt.Y / zoomFactor));

                lstPts.Add(new Tuple<PointF, PointF>(LstPonto, CurPonto));
                if (zoomFactor >= 0.8)
                {
                    ProcessPointsFull(LstPonto, CurPonto, bmFull, false, 1, (float)_vm.ScaleX, (float)_vm.ScaleY, Step);
                }
                else
                {
                    ProcessPointsFull(LstPonto, CurPonto, bmFull, false, 1, (float)_vm.ScaleX, (float)_vm.ScaleY, Step * 2);
                }

            }
        }



        private void _img_MouseUp(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {

            //for (int i = 0; i <= lstPts.Count - 1; i++)
            //{
            //    ProcessPointsFull(lstPts[i].Item1, lstPts[i].Item2, bmFull, false, 1, (float)_vm.ScaleX, (float)_vm.ScaleY, 4);
            //}
            using (MemoryStream msSave = new MemoryStream())
            {
                bmFull.Save(msSave, System.Drawing.Imaging.ImageFormat.Bmp);
                _Foto.Image = msSave.ToArray();
            }
        }

        private bool ProcessPointsFull(PointF LastPoint, PointF CurrPoint, Bitmap BmUse, bool Render, float zoom, float scaleX, float scaleY, int Step)
        {
            PointF LstPonto = ConvertionHelpers.CircleCenter(LastPoint, (_vm.ToolSize * zoom) * scaleX);

            PointF CurPonto = ConvertionHelpers.CircleCenter(CurrPoint, (_vm.ToolSize * zoom) * scaleX);

            using (Graphics gr = Graphics.FromImage(BmUse))
            {
                gr.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.None;
                gr.CompositingQuality = System.Drawing.Drawing2D.CompositingQuality.AssumeLinear;
                gr.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.NearestNeighbor;


                List<PointF> ls = ConvertionHelpers.BresenhamLinePlotter(LstPonto, CurPonto);
                for (int i = Step; i <= ls.Count - 1; i += Step)
                {
                    using (Bitmap bm = CaptureCircle2(ls[i - Step], BmUse, zoom, scaleX, scaleY))
                    {
                        gr.DrawImage(Opacity.SetImageOpacity(bm, ((float)_vm.ToolHardness / 100)), ls[i]);
                    }
                }
            }
            if (Render)
            {
                _Foto.RenderedImage = PhotoHelper.ConvertToBitmapSource((Bitmap)BmUse.GetThumbnailImage(_Foto.RenderedImageSize.Width, _Foto.RenderedImageSize.Height, null, IntPtr.Zero));
            }
            return true;
        }

        private Bitmap CaptureCircle2(PointF Ponto, Bitmap bmUse, float zoom, float scaleX, float scaleY)
        {
            Bitmap bmCrop2 = null;
            Graphics grCrop2;
            RectangleF CropRect2 = new RectangleF(Ponto.X, Ponto.Y, (_vm.ToolSize * zoom) * scaleX, (_vm.ToolSize * zoom) * scaleY);
      
                bmCrop2 = new Bitmap((int)(CropRect2.Width), (int)(CropRect2.Width));
                grCrop2 = Graphics.FromImage(bmCrop2);
                grCrop2.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.None;
                grCrop2.CompositingQuality = System.Drawing.Drawing2D.CompositingQuality.Invalid;
                grCrop2.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.NearestNeighbor;
                GraphicsPath path = new GraphicsPath();
                path.AddEllipse(0, 0, CropRect2.Width, CropRect2.Height);
                grCrop2.SetClip(path);
   
            grCrop2.DrawImage(bmUse, -Ponto.X, -Ponto.Y);
            return (Bitmap)bmCrop2.Clone();


        }

        public void UpdateZoom() {
            zoomFactor = ConvertionHelpers.ZoomFactor(new SizeF(_Foto.RenderedImageSize.Width, _Foto.RenderedImageSize.Height), new SizeF(_Foto.ImageSize.Width, _Foto.ImageSize.Height));

            InvertedZoomFactor = ConvertionHelpers.InvertedZoomFactor(new SizeF(_Foto.RenderedImageSize.Width, _Foto.RenderedImageSize.Height), new SizeF(_Foto.ImageSize.Width, _Foto.ImageSize.Height));
        }

        public void DrawRenderImage()
        {
            using (MemoryStream ms = new MemoryStream(_Foto.Image))
            {
                using (Bitmap bm = new Bitmap(ms))
                {

                    UpdateZoom();
                    SizeF ZoomedSize = new SizeF(bm.Width * zoomFactor, bm.Height * zoomFactor);


                    bmRender = new Bitmap((int)ZoomedSize.Width, (int)ZoomedSize.Height);

                    using (Graphics gr = Graphics.FromImage(bmRender))
                    {
                        gr.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.HighSpeed;
                        gr.CompositingQuality = System.Drawing.Drawing2D.CompositingQuality.HighSpeed;
                        gr.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.Low;
                        gr.DrawImage(bm, 0, 0, ZoomedSize.Width, ZoomedSize.Height);
                    }

                    bmFull = new Bitmap((int)bm.Width, (int)bm.Height);

                    using (Graphics gr = Graphics.FromImage(bmFull))
                    {
                        gr.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.HighSpeed;
                        gr.CompositingQuality = System.Drawing.Drawing2D.CompositingQuality.HighSpeed;
                        gr.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.Low;
                        gr.DrawImage(bm, 0, 0, bm.Width, bm.Height);
                    }
                }

            }
        }

        private void DrawPoint(PointF Ponto, Brush br)
        {

            using (Graphics gr = Graphics.FromImage(bmRender))
            {
                gr.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.HighSpeed;
                gr.CompositingQuality = System.Drawing.Drawing2D.CompositingQuality.HighSpeed;
                gr.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.Low;

                PointF PontoCentro = ConvertionHelpers.CircleCenter(Ponto, _vm.ToolSize * (float)_vm.ScaleX);
                gr.FillEllipse(br, PontoCentro.X, PontoCentro.Y, _vm.ToolSize * (float)_vm.ScaleX, _vm.ToolSize * (float)_vm.ScaleX);

            }

            _Foto.RenderedImage = PhotoHelper.ConvertToBitmapSource(bmRender);
            //using (MemoryStream msSave = new MemoryStream())
            //{
            //    bmRender.Save(msSave, System.Drawing.Imaging.ImageFormat.Bmp);
            //    //_Foto.Image = msSave.ToArray();
            //    //_Foto.RenderImage();
            //    _Foto.RenderedImage = msSave.ToArray();
            //}
        }
    }
    class ConvertionHelpers
    {

        public static PointF PointConverter(System.Windows.Point Ponto, float ScaleX, float ScaleY)
        {
            return new PointF((float)Ponto.X * ScaleX, (float)Ponto.Y * ScaleY);
        }

        public static PointF PointConverter(PointF Ponto, float ScaleX, float ScaleY)
        {
            return new PointF((float)Ponto.X * ScaleX, (float)Ponto.Y * ScaleY);
        }

        public static float ZoomFactor(SizeF TamanhoDraw, SizeF TamanhoOriginal)
        {
            if (TamanhoDraw.Width == 0) TamanhoDraw.Width = 1;
            if (TamanhoDraw.Height == 0) TamanhoDraw.Height = 1;

            float zoomFactor = Math.Min(TamanhoDraw.Width / TamanhoOriginal.Width, TamanhoDraw.Height / TamanhoOriginal.Height);
            return zoomFactor;
        }

        public static float InvertedZoomFactor(SizeF TamanhoDraw, SizeF TamanhoOriginal)
        {
            if (TamanhoDraw.Width == 0) TamanhoDraw.Width = 1;
            if (TamanhoDraw.Height == 0) TamanhoDraw.Height = 1;

            float zoomFactor = Math.Max(TamanhoOriginal.Width / TamanhoDraw.Width, TamanhoOriginal.Height / TamanhoDraw.Height);
            return zoomFactor;
        }

        public static PointF CircleCenter(PointF Ponto, float CircleSize)
        {
            return new PointF(Ponto.X - (CircleSize / 2), Ponto.Y - (CircleSize / 2));
        }

        public static PointF PointTranslator(PointF Ponto, float Zoom)
        {
            //float Zoom = ZoomFactor(new SizeF(_Foto.RenderedImageSize.Width, _Foto.RenderedImageSize.Width), new SizeF(_Foto.ImageSize.Width, _Foto.ImageSize.Width));
            return new PointF(Ponto.X * Zoom, Ponto.Y * Zoom);
        }

        public static List<PointF> BresenhamLinePlotter(PointF Pt1, PointF Pt2)
        {
            //Implementação completa do algoritmo de Bresenham para desenhar linhas
            int x = (int)Pt1.X;
            int y = (int)Pt1.Y;
            int x2 = (int)Pt2.X;
            int y2 = (int)Pt2.Y;

            int w = x2 - x;
            int h = y2 - y;
            int dx1 = 0, dy1 = 0, dx2 = 0, dy2 = 0;
            if (w < 0) dx1 = -1; else if (w > 0) dx1 = 1;
            if (h < 0) dy1 = -1; else if (h > 0) dy1 = 1;
            if (w < 0) dx2 = -1; else if (w > 0) dx2 = 1;
            int longest = Math.Abs(w);
            int shortest = Math.Abs(h);
            if (!(longest > shortest))
            {
                longest = Math.Abs(h);
                shortest = Math.Abs(w);
                if (h < 0) dy2 = -1; else if (h > 0) dy2 = 1;
                dx2 = 0;
            }
            int numerator = longest >> 1;
            List<PointF> LstReturn = new List<PointF>();
            for (int i = 0; i <= longest; i++)
            {
                LstReturn.Add(new PointF(x,y));
                numerator += shortest;
                if (!(numerator < longest))
                {
                    numerator -= longest;
                    x += dx1;
                    y += dy1;
                }
                else
                {
                    x += dx2;
                    y += dy2;
                }
            }
            return LstReturn;
        }

    }
}
