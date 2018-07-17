using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.Drawing;
using System.IO;
using System.Drawing.Drawing2D;

namespace FotosDaPiteca.Classes
{
    class DrawHelper
    {
        System.Windows.Controls.Image _img;
        Models.Photo _Foto;
        ViewModel.MainWindowViewModel _vm;
        Bitmap bmRender;
        float zoomFactor = 0;

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
                using (MemoryStream ms = new MemoryStream(_Foto.Image))
                {
                    DrawRenderImage();
                }
            }
        }

        private void _img_MouseMove(object sender, System.Windows.Input.MouseEventArgs e)
        {
            if (e.LeftButton == System.Windows.Input.MouseButtonState.Pressed)
            {
                var point = e.GetPosition((System.Windows.Controls.Image)sender);
                DrawPoint(ConvertionHelpers.PointConverter(point, (float)_vm.ScaleX, (float)_vm.ScaleY));
            }
        }

        private void _img_MouseUp(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            
        }

        private void DrawRenderImage()
        {

            using (MemoryStream ms = new MemoryStream(_Foto.Image))
            {
                using (Bitmap bm = new Bitmap(ms))
                {
                    zoomFactor = ConvertionHelpers.ZoomFactor(new SizeF(_Foto.RenderedImageSize.Width, _Foto.RenderedImageSize.Height), new SizeF(bm.Width, bm.Height));

                    SizeF ZoomedSize = new SizeF(bm.Width * zoomFactor, bm.Height * zoomFactor);


                    bmRender = new Bitmap((int)ZoomedSize.Width, (int)ZoomedSize.Height);

                    using (Graphics gr = Graphics.FromImage(bmRender))
                    {
                        gr.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.HighSpeed;
                        gr.CompositingQuality = System.Drawing.Drawing2D.CompositingQuality.HighSpeed;
                        gr.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.Low;
                        gr.DrawImage(bm, 0, 0, ZoomedSize.Width, ZoomedSize.Height);
                    }

                }
            }
        }

        private void DrawPoint(PointF Ponto)
        {
 
            using (Graphics gr = Graphics.FromImage(bmRender))
            {
                gr.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.HighSpeed;
                gr.CompositingQuality = System.Drawing.Drawing2D.CompositingQuality.HighSpeed;
                gr.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.Low;
  
                PointF PontoCentro = ConvertionHelpers.CircleCenter(Ponto, _vm.CircleSize * (float)_vm.ScaleX);
                gr.FillEllipse(Brushes.Green, PontoCentro.X, PontoCentro.Y, _vm.CircleSize * (float)_vm.ScaleX, _vm.CircleSize * (float)_vm.ScaleX);

            }
            using (MemoryStream msSave = new MemoryStream())
            {
                bmRender.Save(msSave, System.Drawing.Imaging.ImageFormat.Bmp);
                //_Foto.Image = msSave.ToArray();
                //_Foto.RenderImage();
                _Foto.RenderedImage = msSave.ToArray();
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

        public static PointF PointTranslator(PointF Ponto)
        {


            return new PointF(-1, -1);
        }
    }
}
