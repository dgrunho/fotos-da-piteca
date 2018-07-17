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
using WpfColorFontDialog;
using ColorPickerWPF;
using System.Windows.Media;
using System.Drawing.Text;
using Microsoft.WindowsAPICodePack.Dialogs;
using System.Xml.Serialization;
using System.Xml;

namespace FotosDaPiteca.ViewModel
{
    class MainWindowViewModel : ViewModelBase

    {
        #region "Variables"
        Classes.DrawHelper dh;
        #endregion
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
                    if (_Fotos == null)
                    {
                        IsTrueOnNew = true;
                        IsFalseOnNew = false;
                    }
                    else
                    {
                        IsTrueOnNew = false;
                        IsFalseOnNew = true;
                    }

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

        Photo _FotoSelecionada = new Photo();
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

        bool _IsTrueOnNew = true;
        public bool IsTrueOnNew
        {
            get
            {
                return _IsTrueOnNew;
            }
            set
            {
                if (_IsTrueOnNew != value)
                {
                    _IsTrueOnNew = value;
                    RaisePropertyChanged("IsTrueOnNew");
                }
            }
        }

        bool _IsFalseOnNew = false;
        public bool IsFalseOnNew
        {
            get
            {
                return _IsFalseOnNew;
            }
            set
            {
                if (_IsFalseOnNew != value)
                {
                    _IsFalseOnNew = value;
                    RaisePropertyChanged("IsFalseOnNew");
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

        SnackbarMessageQueue _PropertiesMessageQueue = new SnackbarMessageQueue();
        public SnackbarMessageQueue PropertiesMessageQueue
        {
            get
            {
                return _PropertiesMessageQueue;
            }
            set
            {
                if (_PropertiesMessageQueue != value)
                {
                    _PropertiesMessageQueue = value;
                    RaisePropertyChanged("PropertiesMessageQueue");
                }
            }
        }

        System.Windows.Controls.Image _ImagemDraw;
        public System.Windows.Controls.Image ImagemDraw
        {
            get
            {
                return _ImagemDraw;
            }
            set
            {
                if (_ImagemDraw != value)
                {
                    _ImagemDraw = value;
                    RaisePropertyChanged("ImagemDraw");
                }
            }
        }

        int _SelectedToolIndex = 0;
        public int SelectedToolIndex
        {
            get
            {
                return _SelectedToolIndex;
            }
            set
            {
                if (_SelectedToolIndex != value)
                {
                    _SelectedToolIndex = value;
                    
                    RaisePropertyChanged("SelectedToolIndex");
                    if (_SelectedToolIndex == 1) {
                        if (dh == null) {
                            dh = new Classes.DrawHelper(ImagemDraw, FotoSelecionada, this);
                        }
                    }
                }
            }
        }

        Point _CurrPoint;
        public Point CurrPoint
        {
            get
            {
                return _CurrPoint;
            }
            set
            {
                if (_CurrPoint != value)
                {
                    _CurrPoint = value;
                    RaisePropertyChanged("CurrPoint");
                }
            }
        }

        Point _CircleCenter;
        public Point CircleCenter
        {
            get
            {
                return _CircleCenter;
            }
            set
            {
                if (_CircleCenter != value)
                {
                    _CircleCenter = value;
                    RaisePropertyChanged("CircleCenter");
                }
            }
        }

        int _CircleSize = 50;
        public int CircleSize
        {
            get
            {
                return _CircleSize;
            }
            set
            {
                if (_CircleSize != value)
                {
                    _CircleSize = value;
                    RaisePropertyChanged("CircleSize");
                }
            }
        }

        bool _MouseLeftButtonIsDown = false;
        public bool MouseLeftButtonIsDown
        {
            get
            {
                return _MouseLeftButtonIsDown;
            }
            set
            {
                if (_MouseLeftButtonIsDown != value)
                {
                    _MouseLeftButtonIsDown = value;
                    RaisePropertyChanged("MouseLeftButtonIsDown");
                }
            }
        }

        Visibility _ShowTool = Visibility.Collapsed;
        public Visibility ShowTool
        {
            get
            {
                return _ShowTool;
            }
            set
            {
                if (_ShowTool != value)
                {
                    _ShowTool = value;
                    RaisePropertyChanged("ShowTool");
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
                //Fotos = new ObservableCollection<Photo>();
                //Fotos.Add(new Photo(new FileInfo("C:\\Users\\0101410\\Desktop\\Photos\\1 (1).jpg")));
                //Fotos.Add(new Photo(new FileInfo("C:\\Users\\0101410\\Desktop\\Photos\\1 (2).jpg")));
                //Fotos.Add(new Photo(new FileInfo("C:\\Users\\0101410\\Desktop\\Photos\\1 (3).jpg")));

            }
            else
            {
                //Fotos = new ObservableCollection<Photo>();
                //Fotos.Add(new Photo(new FileInfo("C:\\Users\\0101410\\Desktop\\Photos\\1 (1).jpg")));
                //Fotos.Add(new Photo(new FileInfo("C:\\Users\\0101410\\Desktop\\Photos\\1 (2).jpg")));
                //Fotos.Add(new Photo(new FileInfo("C:\\Users\\0101410\\Desktop\\Photos\\1 (3).jpg")));
                
            }
        }

        #endregion

        #region "Commands"
        public RelayCommand SelecionarFont => new RelayCommand(delegate (object o)
        {

            Color color;

            if (ColorPickerWindow.ShowDialog(out color) == true)
            {
                FotoSelecionada.WatermarkColor = new ColorConverter().ConvertToString(color);
            }

            ////We can pass a bool to choose if we preview the font directly in the list of fonts.
            //bool previewFontInFontList = true;
            ////True to allow user to input arbitrary font sizes. False to only allow predtermined sizes
            //bool allowArbitraryFontSizes = true;


            //ColorFontDialog dialog = new ColorFontDialog(previewFontInFontList, allowArbitraryFontSizes);
            //if (FotoSelecionada.WatermarkFont == null)
            //{
            //    FotoSelecionada.WatermarkFont = "Segoe UI";
            //}
            //FontInfo fi = new FontInfo(new System.Windows.Media.FontFamily(FotoSelecionada.WatermarkFont), 18, FontStyles.Normal, FontStretches.Normal, FontWeights.Normal, System.Windows.Media.Brushes.Black);
            //dialog.Font = fi;

            ////Optional custom allowed size range
            //dialog.FontSizes = new int[] { 10, 12, 14, 16, 18, 20, 22 };

            //if (dialog.ShowDialog() == true)
            //{
            //    FontInfo font = dialog.Font;
            //    if (font != null)
            //    {
            //        FotoSelecionada.WatermarkFont = font.Family.ToString();
            //        //FontInfo.ApplyFont(MyTextBox, font);
            //    }
            //}
        });

        public RelayCommand AplicarTodos => new RelayCommand(delegate (object o)
        {

            foreach (Photo p in Fotos)
            {
                p.Watermark = FotoSelecionada.Watermark;
                p.WatermarkFont = FotoSelecionada.WatermarkFont;
                p.WatermarkFontSize = FotoSelecionada.WatermarkFontSize;
                p.WatermarkPosition = FotoSelecionada.WatermarkPosition;
                p.WatermarkColor = FotoSelecionada.WatermarkColor;
                p.UseWatermark = FotoSelecionada.UseWatermark;
                p.AddShadow = FotoSelecionada.AddShadow;
            }
            Task.Factory.StartNew(() => PropertiesMessageQueue.Enqueue("Marca de água aplicada a todas as fotos"));
        });

        public RelayCommand Memorizar => new RelayCommand(delegate (object o)
        {

            Properties.Settings.Default.UseWatermark = FotoSelecionada.UseWatermark;
            Properties.Settings.Default.Watermark = FotoSelecionada.Watermark;
            Properties.Settings.Default.WatermarkFont = FotoSelecionada.WatermarkFont;
            Properties.Settings.Default.WatermarkSize = FotoSelecionada.WatermarkFontSize;
            Properties.Settings.Default.WatermarkPosition = (int)FotoSelecionada.WatermarkPosition;
            Properties.Settings.Default.AddShadow = FotoSelecionada.AddShadow;
            Properties.Settings.Default.WatermarkColor = FotoSelecionada.WatermarkColor;
            Properties.Settings.Default.Save();
            Task.Factory.StartNew(() => PropertiesMessageQueue.Enqueue("Dados memorizados"));
        });


        public RelayCommand cmdNewProject => new RelayCommand(delegate (object o)
        {
            Fotos = new ObservableCollection<Photo>();
            FotoSelecionada = new Photo();
            Importar();
        });

        public RelayCommand cmdAbrir => new RelayCommand(delegate (object o)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.Multiselect = true;
            openFileDialog.Filter = "Projeto Fotos da Piteca|*.fdp;*.png|Outros Ficheiros|*.*";
            if (openFileDialog.ShowDialog() == true)
            {
                using (FileStream fileStream = new FileStream(openFileDialog.FileName, FileMode.Open))
                {
                    XmlSerializer serializer = new XmlSerializer(typeof(ObservableCollection<Photo>));
                    Fotos = (ObservableCollection<Photo>)serializer.Deserialize(fileStream);
                }
            }
        });

        public RelayCommand cmdGuardar => new RelayCommand(delegate (object o)
        {
            Guardar();
        });


        public RelayCommand cmdImportar => new RelayCommand(delegate (object o)
        {
            Importar();

        });

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
                IsLoading = true;
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
                IsLoading = false;
            }
        }

        async void Guardar()
        {
            SaveFileDialog saveFileDialog = new SaveFileDialog();
            saveFileDialog.Filter = "Projeto Fotos da Piteca|*.fdp;*.png|Outros Ficheiros|*.*";
            if (saveFileDialog.ShowDialog() == true)
            {
                IsLoading = true;
                await Task.Factory.StartNew(() =>
                {
                    XmlSerializer xsSubmit = new XmlSerializer(typeof(ObservableCollection<Photo>));
                    var xml = "";

                    using (var sww = new StringWriter())
                    {
                        using (XmlWriter writer = XmlWriter.Create(sww))
                        {
                            xsSubmit.Serialize(writer, Fotos);
                            xml = sww.ToString();
                        }
                    }
                    File.WriteAllText(saveFileDialog.FileName, xml);
                });
                IsLoading = false;
            }
        }

        async void Importar()
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.Multiselect = true;
            openFileDialog.Filter = "Imagens|*.jpg;*.bmp;*.gif;*.png|Outros Ficheiros|*.*";
            if (openFileDialog.ShowDialog() == true)
            {
                IsLoading = true;
                List<FileInfo> files = new List<FileInfo>();
                foreach (string file in openFileDialog.FileNames)
                {
                    files.Add(new FileInfo(file));
                }

                await Task.Factory.StartNew(() =>
                {
                    ObservableCollection<Photo> FotosExport = new ObservableCollection<Photo>();
                    foreach (FileInfo fs in files)
                    {
                        Console.WriteLine(fs.FullName);

                        FotosExport.Add(new Photo(fs));
                    }
                    App.Current.Dispatcher.Invoke((Action)delegate
                                    {
                                        foreach (Photo P in FotosExport)
                                        {
                                            Fotos.Add(P);
                                        }

                                    });
                });
                IsLoading = false;
            }


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
