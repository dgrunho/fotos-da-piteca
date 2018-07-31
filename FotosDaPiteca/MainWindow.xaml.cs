using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace FotosDaPiteca
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : MahApps.Metro.Controls.MetroWindow
    {
        ViewModel.MainWindowViewModel vm;
        Border currSquare;

        public MainWindow()
        {
            InitializeComponent();
            vm = (ViewModel.MainWindowViewModel)DataContext;

        }

        private void SairButton_Click(object sender, RoutedEventArgs e)
        {
            Environment.Exit(1);
        }

        private void MetroWindow_Loaded(object sender, RoutedEventArgs e)
        {
            //Tenho de medir o scaling do windows senão as contas da imagem ficam mal
            //ViewModel.MainWindowViewModel vm = (ViewModel.MainWindowViewModel)DataContext;
            PresentationSource source = PresentationSource.FromVisual(this);
            if (source != null)
            {
                vm.ScaleX = source.CompositionTarget.TransformToDevice.M11;
                vm.ScaleY = source.CompositionTarget.TransformToDevice.M22;
            }

            vm.ImagemDraw = ImgBig;
        }

        private void ApagarFotoButton_Click(object sender, RoutedEventArgs e)
        {
            if (MessageBox.Show("Tem a certeza que deseja apagar a Foto","Apagar Foto",MessageBoxButton.YesNo) == MessageBoxResult.Yes)
            {
                //ViewModel.MainWindowViewModel vm = (ViewModel.MainWindowViewModel)DataContext;
                Button btn = (Button)sender;
                if (vm.FotoSelecionada == (FotosDaPiteca.Models.Photo)btn.DataContext)
                {
                    vm.FotoSelecionada = null;
                }
                vm.Fotos.Remove((FotosDaPiteca.Models.Photo)btn.DataContext);
            }
        }

        private void ImgBig_MouseMove(object sender, MouseEventArgs e)
        {
            var point = e.GetPosition((IInputElement)sender);
            float ZoomFactor = Math.Min((float)vm.FotoSelecionada.RenderedImageSize.Width / (float)vm.FotoSelecionada.ImageSize.Width, (float)vm.FotoSelecionada.RenderedImageSize.Height / (float)vm.FotoSelecionada.ImageSize.Height);

            vm.CurrPoint = point;

            switch (vm.SelectedToolIndex)
            {
                case (int)FotosDaPiteca.ViewModel.MainWindowViewModel.Tools.Smudge:
                    vm.CircleCenter = new Point(point.X - (vm.ToolSizeDisplay / 2), point.Y - (vm.ToolSizeDisplay / 2));
                    break;
                case (int)FotosDaPiteca.ViewModel.MainWindowViewModel.Tools.Crop:
                    if (e.LeftButton == MouseButtonState.Pressed)
                    {
                        Point PointTL = vm.RectangleTool.TopLeft;
                        Point PointBR = vm.RectangleTool.BottomRight;
                        if (currSquare == brdTopLeft)
                        {
                            vm.RectangleTool = new Rect(new Point(point.X, point.Y), new Point(PointBR.X, PointBR.Y));
                        }

                        if (currSquare == brdTopRight)
                        {
                            vm.RectangleTool = new Rect(new Point(PointTL.X, point.Y), new Point(point.X, PointBR.Y));
                        }
                        if (currSquare == brdBottomRight)
                        {
                            vm.RectangleTool = new Rect(new Point(PointTL.X, PointTL.Y), new Point(point.X, point.Y));
                        }
                        if (currSquare == brdBottomLeft)
                        {
                            vm.RectangleTool = new Rect(new Point(point.X, PointTL.Y), new Point(PointBR.X, point.Y));
                        }
                    }
                    else currSquare = null;
                    break;
                default:
                    break;
            }
        }

        private void ImgBig_MouseEnter(object sender, MouseEventArgs e)
        {
            //ViewModel.MainWindowViewModel vm = (ViewModel.MainWindowViewModel)DataContext;


            switch (vm.SelectedToolIndex)
            {
                case (int)FotosDaPiteca.ViewModel.MainWindowViewModel.Tools.Smudge:
                    Mouse.OverrideCursor = Cursors.Hand;
                    vm.ShowCircleTool = Visibility.Visible;
                    break;
                case (int)FotosDaPiteca.ViewModel.MainWindowViewModel.Tools.Crop:
                    //Mouse.OverrideCursor = Cursors.Hand;
                    //vm.ShowRectangleTool = Visibility.Visible;
                    break;
                default:
                    Mouse.OverrideCursor = Cursors.Arrow;
                    vm.ShowCircleTool = Visibility.Collapsed;
                    vm.ShowRectangleTool = Visibility.Collapsed;
                    break;
            }
        }

        private void ImgBig_MouseLeave(object sender, MouseEventArgs e)
        {
            Mouse.OverrideCursor = Cursors.Arrow;
            vm.ShowCircleTool = Visibility.Collapsed;
            //vm.ShowRectangleTool = Visibility.Collapsed;
        }

        private void ImgBig_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Pressed)
            {
                vm.MouseLeftButtonIsDown = true;
            }
        }

        private void ImgBig_MouseUp(object sender, MouseButtonEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Released)
            {
                vm.MouseLeftButtonIsDown = false;
            }
        }

        private void Card_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {

        }



        private void brdTopLeft_MouseEnter(object sender, MouseEventArgs e)
        {
            currSquare = (Border)sender;
        }




        //Point ptInit;
        //private void Border_MouseDown(object sender, MouseButtonEventArgs e)
        //{
        //    if (e.LeftButton == MouseButtonState.Pressed)
        //    {
        //       ptInit = e.GetPosition((IInputElement)sender);
        //    }
        //}

        //private void Border_MouseMove(object sender, MouseEventArgs e)
        //{
        //    if (e.LeftButton == MouseButtonState.Pressed)
        //    {
        //        var point = e.GetPosition((IInputElement)sender);
        //        var pointdiff = new Point(ptInit.X - point.X, ptInit.Y - point.Y);
        //        Rect r = vm.RectangleTool;
        //        vm.RectangleTool = new Rect(new Point(r.X - pointdiff.X, r.Y - pointdiff.Y), r.BottomRight);
        //    }
        //}

        //private void Border_MouseUp(object sender, MouseButtonEventArgs e)
        //{

        //}
    }
}
