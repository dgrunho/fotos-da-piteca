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

namespace FotosDaPiteca.Classes
{
    class DrawHelper
    {
        public System.Windows.Controls.Image _img;
        public Models.Photo _Foto;
        public ViewModel.MainWindowViewModel _vm;
        Bitmap bmFull;
        Bitmap bmRender;
        Bitmap bmRenderPreview;
        IntPtr handleBmRender;
        IntPtr handleBmRenderPreview;
        float zoomFactor = 0;
        PointF LastPt;
        PointF CurrPt;
        List<PointF> lstPoints = new List<PointF>();

        List<List<PointF>> Movimentos = new List<List<PointF>>();

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
                bmRenderPreview = (Bitmap)bmRender.Clone();
                handleBmRender = bmRender.GetHbitmap();
                handleBmRenderPreview = bmRenderPreview.GetHbitmap();
            }

            
            CurrPt = new PointF();
            LastPt = ConvertionHelpers.PointConverter(e.GetPosition((System.Windows.Controls.Image)sender), (float)_vm.ScaleX, (float)_vm.ScaleY);

 
            lstPoints = new List<PointF>();
            lstPoints.Add(LastPt);
            //Bitmap bm = CaptureCircle(ConvertionHelpers.PointConverter(point, (float)_vm.ScaleX, (float)_vm.ScaleY));
            //bm.Save("bm.bmp");
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
                lstPoints.Add(CurrPt);
                ProcessPoints(LastPt, CurrPt, bmRender, true, zoomFactor);


                PointF LstPonto = ConvertionHelpers.CircleCenter(new PointF(LastPt.X / zoomFactor, LastPt.Y / zoomFactor), (_vm.ToolSize));
                PointF CurPonto = ConvertionHelpers.CircleCenter(new PointF(CurrPt.X / zoomFactor, CurrPt.Y / zoomFactor), (_vm.ToolSize));
                ProcessPoints(LstPonto, CurPonto, bmFull, false, 1);


                //ProcessPointsFull(LastPt, CurrPt, bmFull, true);
            }
        }

        private void _img_MouseUp(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            List<PointF> lstTmp = (new List<PointF>());
            lstTmp.AddRange(lstPoints);
            Movimentos.Add(lstTmp);

            using (MemoryStream msSave = new MemoryStream())
            {
                bmFull.Save(msSave, System.Drawing.Imaging.ImageFormat.Bmp);
                _Foto.Image = msSave.ToArray();
            }

            //List<PointF> lstBig = new List<PointF>();
            //for (int i = 1; i <= lstPoints.Count - 1; i++)
            //{
            //    lstBig.Add(ConvertionHelpers.PointTranslator(lstPoints[i], _Foto));
            //    //ProcessPoints(lstPoints[i - 1], lstPoints[i], bmRender, false);
            //}
            ////_Foto.RenderedImage = PhotoHelper.ConvertToBitmapSource(bmRender);

            ////using (MemoryStream msSave = new MemoryStream())
            ////{
            ////    bmRender.Save(msSave, System.Drawing.Imaging.ImageFormat.Bmp);
            ////    _Foto.RenderedImage = msSave.ToArray();
            ////}
        }


        private bool ProcessPoints(PointF LastPoint, PointF CurrPoint, Bitmap BmUse, bool Render, float zoom)
        {
            PointF LstPonto = ConvertionHelpers.CircleCenter(LastPoint, (_vm.ToolSize * zoom) * (float)_vm.ScaleX);
            PointF CurPonto = ConvertionHelpers.CircleCenter(CurrPoint, (_vm.ToolSize * zoom) * (float)_vm.ScaleX);
            using (Graphics gr = Graphics.FromImage(BmUse))
            {
                gr.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.None;
                gr.CompositingQuality = System.Drawing.Drawing2D.CompositingQuality.AssumeLinear;
                gr.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.NearestNeighbor;


                List<PointF> ls = ConvertionHelpers.BresenhamLinePlotter(LstPonto, CurPonto);
                for (int i = 2; i <= ls.Count - 1; i += 2)
                {
                    using (Bitmap bm = CaptureCircle(ls[i - 2], BmUse, zoom))
                    {
                        gr.DrawImage(Opacity.SetImageOpacity(bm, ((float)_vm.ToolHardness / 100)), ls[i]);
                    }
                }
            }
            if (Render) {
                _Foto.RenderedImage = PhotoHelper.ConvertToBitmapSource(BmUse);
            }
            
            //using (MemoryStream msSave = new MemoryStream())
            //{
            //    bmRender.Save(msSave, System.Drawing.Imaging.ImageFormat.Jpeg);
            //    _Foto.RenderedImage = msSave.ToArray();
            //}

            return true;
        }

        //private bool ProcessPointsFull(PointF LastPoint, PointF CurrPoint, Bitmap BmUse, bool Render)
        //{


        //    PointF LstPonto = ConvertionHelpers.CircleCenter(new PointF(LastPoint.X / zoomFactor, LastPoint.Y / zoomFactor), (_vm.ToolSize));
        //    PointF CurPonto = ConvertionHelpers.CircleCenter(new PointF(CurrPoint.X / zoomFactor, CurrPoint.Y / zoomFactor), (_vm.ToolSize)); 
        //    using (Graphics gr = Graphics.FromImage(BmUse))
        //    {
        //        gr.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.None;
        //        gr.CompositingQuality = System.Drawing.Drawing2D.CompositingQuality.AssumeLinear;
        //        gr.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.NearestNeighbor;


        //        List<PointF> ls = ConvertionHelpers.BresenhamLinePlotter(LstPonto, CurPonto);
        //        for (int i = 2; i <= ls.Count - 1; i += 2)
        //        {
        //            using (Bitmap bm = CaptureCircle(ls[i - 2], BmUse))
        //            {
        //                gr.DrawImage(Opacity.SetImageOpacity(bm, ((float)_vm.ToolHardness / 100)), ls[i]);
        //            }
        //        }
        //    }
        //    if (Render)
        //    {
        //        _Foto.RenderedImage = PhotoHelper.ConvertToBitmapSource((Bitmap)BmUse.GetThumbnailImage(_Foto.RenderedImageSize.Width, _Foto.RenderedImageSize.Height, null, IntPtr.Zero));
        //    }

        //    //using (MemoryStream msSave = new MemoryStream())
        //    //{
        //    //    bmRender.Save(msSave, System.Drawing.Imaging.ImageFormat.Jpeg);
        //    //    _Foto.RenderedImage = msSave.ToArray();
        //    //}

        //    return true;
        //}


        Bitmap bmCrop;
        Graphics grCrop;

        private Bitmap CaptureCircle(PointF Ponto, Bitmap bmUse, float zoom)
        {
            RectangleF CropRect = new RectangleF(Ponto.X, Ponto.Y, (_vm.ToolSize * zoom) * (float)_vm.ScaleX, (_vm.ToolSize * zoom) * (float)_vm.ScaleX);
            if (bmCrop == null)
            {
                bmCrop = new Bitmap((int)((_vm.ToolSize * zoom) * (float)_vm.ScaleX), (int)((_vm.ToolSize * zoom) * (float)_vm.ScaleX));
                grCrop = Graphics.FromImage(bmCrop);
                grCrop.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.None;
                grCrop.CompositingQuality = System.Drawing.Drawing2D.CompositingQuality.Invalid;
                grCrop.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.NearestNeighbor;
                GraphicsPath path = new GraphicsPath();
                path.AddEllipse(0, 0, CropRect.Width, CropRect.Height);
                grCrop.SetClip(path);
            }
            grCrop.DrawImage(bmUse, -Ponto.X, -Ponto.Y);
            return (Bitmap)bmCrop.Clone();


        }

        public void UpdateZoom() {
            zoomFactor = ConvertionHelpers.ZoomFactor(new SizeF(_Foto.RenderedImageSize.Width, _Foto.RenderedImageSize.Height), new SizeF(_Foto.ImageSize.Width, _Foto.ImageSize.Height));
            //DrawRenderImage();
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

                    //foreach (List<PointF> lst in Movimentos) {
                    //    for (int i = 1; i <= lst.Count - 1; i++)
                    //    {
                    //        ProcessPoints(lstPoints[i - 1], lstPoints[i], bmRender, false);
                    //    }
                    //}

                    bmFull = (Bitmap)bm.Clone();
                    bmCrop = null;
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

        public void Save() {
           
            List<PointF> lstBig = new List<PointF>();
            for (int i = 1; i <= lstPoints.Count - 1; i++)
            {
                lstBig.Add(ConvertionHelpers.PointTranslator(lstPoints[i], zoomFactor));
                //ProcessPoints(lstPoints[i - 1], lstPoints[i], bmFull, false);
            }

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
