﻿using System;
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
using WpfColorFontDialog;
using ColorPickerWPF;
using System.Windows.Media;
using System.Drawing.Text;
using Microsoft.WindowsAPICodePack.Dialogs;

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

        ObservableCollection<string> _TiposLetra = new ObservableCollection<string>();
        public ObservableCollection<string> TiposLetra
        {
            get
            {
                return _TiposLetra;
            }
            set
            {
                if (_TiposLetra != value)
                {
                    _TiposLetra = value;
                    RaisePropertyChanged("TiposLetra");
                }
            }
        }

        public List<int> TamanhosLetra
        {
            get
            {
                List<int> _TamanhoLetra = new List<int>();
                _TamanhoLetra.AddRange(new int[] { 20, 40, 60, 80, 100, 120, 140, 160, 180, 200, 220, 240 });
                return _TamanhoLetra;
            }

        }

        Photo _FotoSelecionada = new Photo() ;
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
                    FotoSelecionada.RenderedImageSize = new Models.Size((int)ImagemWidth, (int)ImagemHeight);
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

        bool _IsExporting = false;
        public bool IsExporting
        {
            get
            {
                return _IsExporting;
            }
            set
            {
                if (_IsExporting != value)
                {
                    _IsExporting = value;
                    RaisePropertyChanged("IsExporting");
                }
            }
        }

        double _ImagemWidth;
        public double ImagemWidth
        {
            get
            {
                return _ImagemWidth;
            }
            set
            {
                if (_ImagemWidth != value * ScaleX)
                {
                    _ImagemWidth = value * ScaleX;
                   RaisePropertyChanged("ImagemWidth");
                   FotoSelecionada.RenderedImageSize = new Models.Size((int)_ImagemWidth / 2, (int)ImagemHeight / 2);
                }
            }
        }
        double _ImagemHeight;
        public double ImagemHeight
        {
            get
            {
                return _ImagemHeight;
            }
            set
            {
                if (_ImagemHeight != value * ScaleY)
                {
                    _ImagemHeight = value * ScaleY;
                    RaisePropertyChanged("ImagemHeight");
                }
            }
        }


        #endregion

        #region "Initialization"

        public MainWindowViewModel()
        {
            loadTiposLetra();
            if (!DesignerProperties.GetIsInDesignMode(new DependencyObject()))
            {
                List<FileInfo> files = new List<FileInfo>();
                files.Add(new FileInfo("C:\\Users\\0101410\\Desktop\\Photos\\1 (1).jpg"));
                files.Add(new FileInfo("C:\\Users\\0101410\\Desktop\\Photos\\1 (2).jpg"));
                files.Add(new FileInfo("C:\\Users\\0101410\\Desktop\\Photos\\1 (3).jpg"));
                files.Add(new FileInfo("C:\\Users\\0101410\\Desktop\\Photos\\1 (4).jpg"));
                files.Add(new FileInfo("C:\\Users\\0101410\\Desktop\\Photos\\1 (5).jpg"));
                load(files);
                //FotoSelecionada = new Photo(files[0]);
            }
            else
            {
                List<FileInfo> files = new List<FileInfo>();
                files.Add(new FileInfo("C:\\Users\\0101410\\Desktop\\Photos\\1 (1).jpg"));
                files.Add(new FileInfo("C:\\Users\\0101410\\Desktop\\Photos\\1 (2).jpg"));
                files.Add(new FileInfo("C:\\Users\\0101410\\Desktop\\Photos\\1 (3).jpg"));
                files.Add(new FileInfo("C:\\Users\\0101410\\Desktop\\Photos\\1 (4).jpg"));
                files.Add(new FileInfo("C:\\Users\\0101410\\Desktop\\Photos\\1 (5).jpg"));
                load(files);
                //FotoSelecionada = new Photo(files[0]);
            }
        }

        #endregion

        #region "Commands"
        public RelayCommand SelecionarFont => new RelayCommand(delegate(object o)
        {

            Color color;

            if (ColorPickerWindow.ShowDialog(out color) == true)
            {
                FotoSelecionada.WaterMarkColor = new ColorConverter().ConvertToString(color);
            }

            ////We can pass a bool to choose if we preview the font directly in the list of fonts.
            //bool previewFontInFontList = true;
            ////True to allow user to input arbitrary font sizes. False to only allow predtermined sizes
            //bool allowArbitraryFontSizes = true;


            //ColorFontDialog dialog = new ColorFontDialog(previewFontInFontList, allowArbitraryFontSizes);
            //if (FotoSelecionada.WaterMarkFont == null)
            //{
            //    FotoSelecionada.WaterMarkFont = "Segoe UI";
            //}
            //FontInfo fi = new FontInfo(new System.Windows.Media.FontFamily(FotoSelecionada.WaterMarkFont), 18, FontStyles.Normal, FontStretches.Normal, FontWeights.Normal, System.Windows.Media.Brushes.Black);
            //dialog.Font = fi;

            ////Optional custom allowed size range
            //dialog.FontSizes = new int[] { 10, 12, 14, 16, 18, 20, 22 };

            //if (dialog.ShowDialog() == true)
            //{
            //    FontInfo font = dialog.Font;
            //    if (font != null)
            //    {
            //        FotoSelecionada.WaterMarkFont = font.Family.ToString();
            //        //FontInfo.ApplyFont(MyTextBox, font);
            //    }
            //}
        });

        public RelayCommand AplicarTodos => new RelayCommand(delegate (object o)
        {

            foreach (Photo p in Fotos)
            {
                p.WaterMark = FotoSelecionada.WaterMark;
                p.WaterMarkFont = FotoSelecionada.WaterMarkFont;
                p.WaterMarkFontSize = FotoSelecionada.WaterMarkFontSize;
                p.WaterMarkPosition = FotoSelecionada.WaterMarkPosition;
                p.WaterMarkColor = FotoSelecionada.WaterMarkColor;
                p.UseWaterMark = FotoSelecionada.UseWaterMark;
            }
        });


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

        public RelayCommand cmdExport => new RelayCommand(delegate (object o)
        {
            Exportar();
        });

        #endregion

        #region "Events"

        private void ClosingEventHandler(object sender, DialogClosingEventArgs eventArgs)
        {
            Console.WriteLine("You can intercept the closing event, and cancel here.");
        }

        #endregion

        #region "Methods"

        async void Exportar()
        {
            var dialog = new CommonOpenFileDialog();
            dialog.IsFolderPicker = true;
            if (dialog.ShowDialog() == CommonFileDialogResult.Ok)
            {
                String FolderName = dialog.FileName;
                IsExporting = true;
                await Task.Factory.StartNew(() =>
                {
                    foreach (Photo P in Fotos)
                    {
                        if (P.Name.ToUpper().EndsWith(".JPG"))
                        {
                            P.RenderImage(FolderName + "\\" + P.Name);
                        }
                        else
                        {
                            P.RenderImage(FolderName + "\\" + P.Name + ".jpg");
                        }

                    }
                });
                IsExporting = false;
            }
        }

        async void load(List<FileInfo> files)
        {
            IsLoading = true;
            await Task.Factory.StartNew(() =>
            {
                foreach (FileInfo fs in files)
                {
                    Console.WriteLine(fs.FullName);
                    Photo Foto = new Photo(fs);
                    App.Current.Dispatcher.Invoke((Action)delegate
                    {
                        Fotos.Add(Foto);
                    });
                }
            });
            IsLoading = false;
        }

        async void loadTiposLetra()
        {
            await Task.Factory.StartNew(() =>
            {
                InstalledFontCollection installedFontCollection = new InstalledFontCollection();

                // Get the array of FontFamily objects.
                ObservableCollection<string> _tiposLetra = new ObservableCollection<string>();
                foreach (System.Drawing.FontFamily fs in installedFontCollection.Families)
                {
                    _tiposLetra.Add(fs.Name);
                }

                App.Current.Dispatcher.Invoke((Action)delegate
                {
                    TiposLetra = _tiposLetra;
                });
            });
        }

        #endregion







    }
}
