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

        public MainWindow()
        {
            InitializeComponent();
            vm = (ViewModel.MainWindowViewModel)DataContext;

        }

        private void SairButton_Click(object sender, RoutedEventArgs e)
        {
            Environment.Exit(1);
        }

        private void ImgBig_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            
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
            //ViewModel.MainWindowViewModel vm = (ViewModel.MainWindowViewModel)DataContext;
            if (vm.SelectedToolIndex == 1)
            {
                var point = e.GetPosition((Image)sender);

                float ZoomFactor = Math.Min((float)vm.FotoSelecionada.RenderedImageSize.Width / (float)vm.FotoSelecionada.ImageSize.Width, (float)vm.FotoSelecionada.RenderedImageSize.Height / (float)vm.FotoSelecionada.ImageSize.Height);

                vm.CurrPoint = point;
                vm.CircleCenter = new Point(point.X - (vm.ToolSizeDisplay / 2), point.Y - (vm.ToolSizeDisplay / 2));
            }
            
        }

        private void ImgBig_MouseEnter(object sender, MouseEventArgs e)
        {
            //ViewModel.MainWindowViewModel vm = (ViewModel.MainWindowViewModel)DataContext;
            if (vm.SelectedToolIndex == 1)
            {
                Mouse.OverrideCursor = Cursors.Hand;
                vm.ShowTool = Visibility.Visible;
            }
            else
            {
                Mouse.OverrideCursor = Cursors.Arrow;
                vm.ShowTool = Visibility.Collapsed;
            }
        }

        private void ImgBig_MouseLeave(object sender, MouseEventArgs e)
        {
            Mouse.OverrideCursor = Cursors.Arrow;
            vm.ShowTool = Visibility.Collapsed;
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
    }
}
