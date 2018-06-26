using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using FotosDaPiteca.Models;
using System.Threading;
using System.Windows.Input;
using MaterialDesignThemes.Wpf;
using FotosDaPiteca.Helpers;
using Microsoft.Win32;
using System.IO;

namespace FotosDaPiteca.ViewModel
{
    class MainWindowViewModel : ViewModelBase

    {
        #region "Properties"

        string _Titulo = "Fotos da Piteca";
        public string Titulo
        {
            get
            {
                return _Titulo;
            }
            set
            {
                if (_Titulo != value)
                {
                    _Titulo = value;
                    RaisePropertyChanged("Titulo");
                }
            }
        }

        ObservableCollection<Photo> _Fotos = new ObservableCollection<Photo>();
        public ObservableCollection<Photo> Fotos
        {
            get
            {
                return _Fotos;
            }
            set
            {
                if (_Fotos != value)
                {
                    _Fotos = value;
                    RaisePropertyChanged("Fotos");
                }
            }
        }

        Photo _FotoSelecionada ;
        public Photo FotoSelecionada
        {
            get
            {
                return _FotoSelecionada;
            }
            set
            {
                if (_FotoSelecionada != value)
                {
                    _FotoSelecionada = value;
                    RaisePropertyChanged("FotoSelecionada");
                }
            }
        }

        bool _IsLoading = false;
        public bool IsLoading
        {
            get
            {
                return _IsLoading;
            }
            set
            {
                if (_IsLoading != value)
                {
                    _IsLoading = value;
                    RaisePropertyChanged("IsLoading");
                }
            }
        }

        Visibility _ShowProgress = Visibility.Collapsed;
        public Visibility ShowProgress
        {
            get
            {
                return _ShowProgress;
            }
            set
            {
                if (_ShowProgress != value)
                {
                    _ShowProgress = value;
                    RaisePropertyChanged("ShowProgress");
                }
            }
        }

        #endregion

        #region "Initialization"

        public MainWindowViewModel()
        {
            if (!DesignerProperties.GetIsInDesignMode(new DependencyObject()))
            {
                //load();
            }
            else
            {
                //Pessoas = new ObservableCollection<Pessoa>
                //{
                //    new Pessoa { ID = 1, Nome = "Teste", ContactosAbreviados = "EML: eml@eml.com" }
                //};
            }
        }

        #endregion

        #region "Commands"
        public RelayCommand ExecutaCommando => new RelayCommand(ExecuteRunDialog);

        private async void ExecuteRunDialog(object o)
        {
            
            //PessoaSelecionada = (Pessoa)o;
            //var view = new PessoaEditor
            //{
            //    DataContext = new PessoaEditorViewModel() { PessoaSelecionada = PessoaSelecionada}
            //};

            ////show the dialog
            //var result = await DialogHost.Show(view, "RootDialog", ClosingEventHandler);

            ////check the result...
            //Console.WriteLine("Dialog was closed, the CommandParameter used to close it was: " + (result ?? "NULL"));
        }


        public RelayCommand cmdNewProject => new RelayCommand(ExecuteNewProject);
        private void ExecuteNewProject(object o)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.Multiselect = true;
            openFileDialog.Filter = "Imagens|*.jpg;*.bmp;*.gif;*.png|Outros Ficheiros|*.*";
            if (openFileDialog.ShowDialog() == true)
            {
                List<FileInfo> files = new List<FileInfo>();
                foreach (string file in openFileDialog.FileNames)
                {
                    files.Add(new FileInfo(file));
                }
                load(files);
            }
                
        }

        #endregion

        #region "Events"

        private void ClosingEventHandler(object sender, DialogClosingEventArgs eventArgs)
        {
            Console.WriteLine("You can intercept the closing event, and cancel here.");
        }

        #endregion

        #region "Methods"

        async void load(List<FileInfo> files)
        {
            IsLoading = true;
            ShowProgress = Visibility.Visible;
            await Task.Factory.StartNew(() =>
            {
                ObservableCollection<Photo> _fotos = new ObservableCollection<Photo>();
                foreach (FileInfo fs in files)
                {
                    //_fotos.Add(new Photo(fs));
                    Photo Foto = new Photo(fs);
                    App.Current.Dispatcher.Invoke((Action)delegate
                    {
                        Fotos.Add(Foto);
                    });
                }
                //App.Current.Dispatcher.Invoke((Action)delegate
                //{
                //    Fotos = _fotos;
                //});
            });
            IsLoading = false;
            ShowProgress = Visibility.Collapsed;
        }

        #endregion







    }
}
