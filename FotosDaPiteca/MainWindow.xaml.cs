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
        public MainWindow()
        {
            InitializeComponent();
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
            ViewModel.MainWindowViewModel vm = (ViewModel.MainWindowViewModel)DataContext;
            PresentationSource source = PresentationSource.FromVisual(this);
            if (source != null)
            {
                vm.ScaleX = source.CompositionTarget.TransformToDevice.M11;
                vm.ScaleY = source.CompositionTarget.TransformToDevice.M22;
            }
        }

        private void ApagarFotoButton_Click(object sender, RoutedEventArgs e)
        {
            if (MessageBox.Show("Tem a certeza que deseja apagar a Foto","Apagar Foto",MessageBoxButton.YesNo) == MessageBoxResult.Yes)
            {
                ViewModel.MainWindowViewModel vm = (ViewModel.MainWindowViewModel)DataContext;
                Button btn = (Button)sender;
                if (vm.FotoSelecionada == (FotosDaPiteca.Models.Photo)btn.DataContext)
                {
                    vm.FotoSelecionada = null;
                }
                vm.Fotos.Remove((FotosDaPiteca.Models.Photo)btn.DataContext);

            }

        }
    }
}
