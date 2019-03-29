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
        #region Enums
        public enum Tools
        {
            [Description("Select")]
            Select = 0,
            [Description("Smudge")]
            Smudge = 1,
            [Description("Contrast, Brightness & Gamma")]
            ContrastBrightnessGamma = 2,
            [Description("Color Levels")]
            ColorLevels = 3,
            [Description("Crop")]
            Crop = 4
        }
        #endregion

        #region "Variables"
        Classes.DrawTools.SmudgeDrawHelper sdh;
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

        public List<int> TamanhosTool
        {
            get
            {
                List<int> _TamanhosTool = new List<int>();
                _TamanhosTool.AddRange(new int[] { 20, 40, 60, 80, 100, 120, 140, 160, 180, 200, 220, 240 });
                return _TamanhosTool;
            }

        }

        public List<int> OpacidadeTool
        {
            get
            {
                List<int> _OpacidadeTool = new List<int>();
                for (int i = 1; i <= 100; i++) {
                    _OpacidadeTool.Add(i);
                }
                return _OpacidadeTool;
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
                    SelectedToolIndex = (int)Tools.Select;
                    
                    if (FotoSelecionada != null) {
                        
                        FotoSelecionada.RenderedImageSize = new Models.Size((int)ImagemWidth, (int)ImagemHeight);
                        float ZoomFactor = Math.Min(FotoSelecionada.RenderedImageSize.Width / FotoSelecionada.ImageSize.Width, FotoSelecionada.RenderedImageSize.Height / FotoSelecionada.ImageSize.Height);
                        ToolSizeDisplay = _ToolSize * ZoomFactor;
                    }
                    
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
                    Update();


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
                    Update();
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
                    switch (_SelectedToolIndex)
                    {
                        case (int)Tools.Smudge: 
                            sdh = null;
                            Update();
                            ShowRectangleTool = Visibility.Collapsed;
                            break;
                        case (int)Tools.Crop:
                            RectangleTop = new Point(0, 0);
                            RectangleBottom = new Point(ImagemDraw.ActualWidth, ImagemDraw.ActualHeight);

                            RectangleTool = new Rect(RectangleTop, RectangleBottom);
                            ShowRectangleTool = Visibility.Visible;
                            break;
                        default:
                            ShowRectangleTool = Visibility.Collapsed;
                            if (sdh != null) sdh.Save();
                            break;
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

        int _ToolSize = 40;
        public int ToolSize
        {
            get
            {
                return _ToolSize;
            }
            set
            {
                if (_ToolSize != value)
                {
                    _ToolSize = value;
                    RaisePropertyChanged("ToolSize");
                    float ZoomFactor = Math.Min((float)FotoSelecionada.RenderedImageSize.Width / (float)FotoSelecionada.ImageSize.Width, (float)FotoSelecionada.RenderedImageSize.Height / (float)FotoSelecionada.ImageSize.Height);
                    ToolSizeDisplay = _ToolSize * ZoomFactor;
                }
            }
        }

        float _ToolSizeDisplay = 40;
        public float ToolSizeDisplay
        {
            get
            {
                return _ToolSizeDisplay;
            }
            set
            {
                if (_ToolSizeDisplay != value)
                {
                    _ToolSizeDisplay = value;
                    RaisePropertyChanged("ToolSizeDisplay");
                }
            }
        }

        int _ToolHardness = 30;
        public int ToolHardness
        {
            get
            {
                return _ToolHardness;
            }
            set
            {
                if (_ToolHardness != value)
                {
                    _ToolHardness = value;
                    RaisePropertyChanged("ToolHardness");
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

        Visibility _ShowCircleTool = Visibility.Collapsed;
        public Visibility ShowCircleTool
        {
            get
            {
                return _ShowCircleTool;
            }
            set
            {
                if (_ShowCircleTool != value)
                {
                    _ShowCircleTool = value;
                    RaisePropertyChanged("ShowCircleTool");
                }
            }
        }

        Rect _RectangleTool;
        public Rect RectangleTool
        {
            get
            {
                return _RectangleTool;
            }
            set
            {
                if (_RectangleTool != value)
                {
                    _RectangleTool = value;
                    RaisePropertyChanged("RectangleTool");
                }
            }
        }

        Point _RectangleTop;
        public Point RectangleTop
        {
            get
            {
                return _RectangleTop;
            }
            set
            {
                if (_RectangleTop != value)
                {
                    _RectangleTop = value;
                    RaisePropertyChanged("RectangleTop");
                }
            }
        }

        Point _RectangleBottom;
        public Point RectangleBottom
        {
            get
            {
                return _RectangleBottom;
            }
            set
            {
                if (_RectangleBottom != value)
                {
                    _RectangleBottom = value;
                    RaisePropertyChanged("RectangleBottom");
                }
            }
        }

        Visibility _ShowRectangleTool = Visibility.Collapsed;
        public Visibility ShowRectangleTool
        {
            get
            {
                return _ShowRectangleTool;
            }
            set
            {
                if (_ShowRectangleTool != value)
                {
                    _ShowRectangleTool = value;
                    RaisePropertyChanged("ShowRectangleTool");
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
            aplicarTodos();
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

        public RelayCommand cmdSaveImage => new RelayCommand(delegate (object o)
        {
            Exportar();
        });

        public RelayCommand CropRectangle => new RelayCommand(delegate (object o)
        {
            FotoSelecionada.CropImage(RectangleTool, (float)ScaleX, (float)ScaleY);
            
            Update();
            RectangleTop = new Point(0, 0);
            RectangleBottom = new Point(FotoSelecionada.RenderedImageSize.Width, FotoSelecionada.RenderedImageSize.Height);
            RectangleTool = new Rect(RectangleTop, RectangleBottom);
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

                        Photo tmpPhoto = new Photo(fs);
                        while (tmpPhoto.IsLoading == true)
                        {
                            string espera = "Espera";
                        }
                        FotosExport.Add(tmpPhoto);
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

        async void aplicarTodos()
        {

                IsLoading = true;
            await Task.Factory.StartNew(() =>
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

                    while (p.IsLoading == true)
                    {
                        string espera = "Espera";
                    }
                }
                Task.Factory.StartNew(() => PropertiesMessageQueue.Enqueue("Marca de água aplicada a todas as fotos"));
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

        void Update()
        {
            float ZoomFactor = Math.Min((float)ImagemWidth / (float)FotoSelecionada.ImageSize.Width, (float)ImagemHeight / (float)FotoSelecionada.ImageSize.Height);
            FotoSelecionada.RenderedImageSize = new Models.Size((int)(FotoSelecionada.ImageSize.Width * ZoomFactor), (int)(FotoSelecionada.ImageSize.Height * ZoomFactor));
            ToolSizeDisplay = ToolSize * ZoomFactor;

            if (SelectedToolIndex == (int)Tools.Smudge)
            {
                if (sdh == null)
                {
                    sdh = new Classes.DrawTools.SmudgeDrawHelper(ImagemDraw, FotoSelecionada, this);
                }
                else {

                    //await Task.Factory.StartNew(() =>
                    //{
                    //    sdh.Save();
                    //});
                    //sdh = new Classes.DrawTools.SmudgeDrawHelper(ImagemDraw, FotoSelecionada, this);
                    sdh.DrawRenderImage();
                }
            }
        }

        #endregion
    }
}
