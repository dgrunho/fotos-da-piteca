using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;
using System.IO;
using FotosDaPiteca.Classes;
using System.Windows.Media.Imaging;

namespace FotosDaPiteca.Models
{
    public enum Posicoes
    {
        [Description("Superior Esquerda")]
        SuperiorEsquerda = 1,
        [Description("Superior Direita")]
        SuperiorDireita = 2,
        [Description("Centro")]
        Centro = 3,
        [Description("Inferior Esquerda")]
        InferiorEsquerda = 4,
        [Description("Inferior Direita")]
        InferiorDireita = 5
    }

    [Serializable()]
    public class Photo : INotifyPropertyChanged
    {
        #region Enumeracoes



        #endregion

        #region Construtores

        public Photo()
        {
            try
            {
                UseWatermark = Properties.Settings.Default.UseWatermark;
                Watermark = Properties.Settings.Default.Watermark;
                WatermarkFont = Properties.Settings.Default.WatermarkFont;
                WatermarkFontSize = Properties.Settings.Default.WatermarkSize;
                WatermarkColor = Properties.Settings.Default.WatermarkColor;
                WatermarkPosition = (Posicoes)Properties.Settings.Default.WatermarkPosition;
                AddShadow = Properties.Settings.Default.AddShadow;
            }
            catch
            {
            }
        }

        public Photo(FileInfo FilePath)
        {
            Name = FilePath.Name;
            OriginalPath = FilePath.FullName;
            Image = File.ReadAllBytes(FilePath.FullName);
            try {
                UseWatermark = Properties.Settings.Default.UseWatermark;
                Watermark = Properties.Settings.Default.Watermark;
                WatermarkFont = Properties.Settings.Default.WatermarkFont ;
                WatermarkFontSize = Properties.Settings.Default.WatermarkSize ;
                WatermarkColor = Properties.Settings.Default.WatermarkColor;
                WatermarkPosition = (Posicoes)Properties.Settings.Default.WatermarkPosition;
                AddShadow = Properties.Settings.Default.AddShadow;
            } catch {
            }
        }

        #endregion

        #region Propriedades

        string _Name;
        public string Name
        {
            get { return _Name; }
            set
            {
                if (_Name != value)
                {
                    _Name = value;
                    RaisePropertyChanged("Name");
                }
            }
        }

        string _OriginalPath;
        public string OriginalPath
        {
            get { return _OriginalPath; }
            set
            {
                if (_OriginalPath != value)
                {
                    _OriginalPath = value;
                    RaisePropertyChanged("OriginalPath");
                }
            }
        }

        byte[] _Image;
        public byte[] Image
        {
            get { return _Image; }
            set
            {
                if (_Image != value)
                {
                    _Image = value;
                    RaisePropertyChanged("Image");
                    RenderImage();
                    System.Drawing.SizeF s = PhotoHelper.GetImageSize(Image);
                    ImageSize = new Size((int)s.Width, (int)s.Height);
                }
            }
        }

        Size _ImageSize = new Size(320, 240);
        public Size ImageSize
        {
            get { return _ImageSize; }
            set
            {
                if (_ImageSize != value)
                {
                    _ImageSize = value;
                    RaisePropertyChanged("ImageSize");
                }
            }
        }


        Size _RenderedImageSize = new Size(800, 600);
        public Size RenderedImageSize
        {
            get { return _RenderedImageSize; }
            set
            {
                if (_RenderedImageSize.Width != value.Width || _RenderedImageSize.Height != value.Height)
                {
                    _RenderedImageSize = value;
                    RaisePropertyChanged("RenderedImageSize");
                    RenderImage();
                }
            }
        }

        BitmapSource _RenderedImage;
        public BitmapSource RenderedImage
        {
            get { return _RenderedImage; }
            set
            {
                if (_RenderedImage != value)
                {
                    _RenderedImage = value;
                    RaisePropertyChanged("RenderedImage");
                }
            }
        }


        Size _RenderedThumbSize = new Size(196, 140);
        public Size RenderedThumbSize
        {
            get { return _RenderedThumbSize; }
            set
            {
                if (_RenderedThumbSize != value)
                {
                    _RenderedThumbSize = value;
                    RaisePropertyChanged("RenderedThumbSize");
                    RenderImage();
                }
            }
        }
        BitmapSource _RenderedThumb;
        public BitmapSource RenderedThumb
        {
            get { return _RenderedThumb; }
            set
            {
                if (_RenderedThumb != value)
                {
                    _RenderedThumb = value;
                    RaisePropertyChanged("RenderedThumb");
                }
            }
        }



        string _Watermark;
        public string Watermark
        {
            get { return _Watermark; }
            set
            {
                if (_Watermark != value)
                {
                    _Watermark = value;
                    RaisePropertyChanged("Watermark");
                    if (UseWatermark)
                    {
                        RenderImage();
                    }
                }
            }
        }

        string _WatermarkFont = "Segoe UI";
        public string WatermarkFont
        {
            get { return _WatermarkFont; }
            set
            {
                if (_WatermarkFont != value)
                {
                    _WatermarkFont = value;
                    RaisePropertyChanged("WatermarkFont");
                    if (UseWatermark)
                    {
                        RenderImage();
                    }
                }
            }
        }

        string _WatermarkColor = "#FF000000";
        public string WatermarkColor
        {
            get { return _WatermarkColor; }
            set
            {
                if (_WatermarkColor != value)
                {
                    _WatermarkColor = value;
                    RaisePropertyChanged("WatermarkColor");
                    if (UseWatermark)
                    {
                        RenderImage();
                    }
                }
            }
        }

        int _WatermarkFontSize = 120;
        public int WatermarkFontSize
        {
            get { return _WatermarkFontSize; }
            set
            {
                if (_WatermarkFontSize != value)
                {
                    _WatermarkFontSize = value;
                    RaisePropertyChanged("WatermarkFontSize");
                    if (UseWatermark)
                    {
                        RenderImage();
                    }
                }
            }
        }

        Posicoes _WatermarkPosition = Posicoes.Centro;
        public Posicoes WatermarkPosition
        {
            get { return _WatermarkPosition; }
            set
            {
                if (_WatermarkPosition != value)
                {
                    _WatermarkPosition = value;
                    RaisePropertyChanged("WatermarkPosition");
                    if (UseWatermark)
                    {
                        RenderImage();
                    }
                }
            }
        }

        bool _UseWatermark = true;
        public bool UseWatermark
        {
            get { return _UseWatermark; }
            set
            {
                if (_UseWatermark != value)
                {
                    _UseWatermark = value;
                    RaisePropertyChanged("UseWatermark");
                    RenderImage();
                }
            }
        }

        bool _AddShadow = true;
        public bool AddShadow
        {
            get { return _AddShadow; }
            set
            {
                if (_AddShadow != value)
                {
                    _AddShadow = value;
                    RaisePropertyChanged("AddShadow");
                    RenderImage();
                }
            }
        }

        int _ShadowRadius = 7;
        public int ShadowRadius
        {
            get { return _ShadowRadius; }
            set
            {
                if (_ShadowRadius != value)
                {
                    _ShadowRadius = value;
                    RaisePropertyChanged("ShadowRadius");
                    if (AddShadow)
                    {
                        RenderImage();
                    }
                }
            }
        }

        string _ShadowColor = "#FF000000";
        public string ShadowColor
        {
            get { return _ShadowColor; }
            set
            {
                if (_ShadowColor != value)
                {
                    _ShadowColor = value;
                    RaisePropertyChanged("ShadowColor");
                    if (AddShadow)
                    {
                        RenderImage();
                    }
                }
            }
        }

        int _Contrast = 0;
        public int Contrast
        {
            get
            {
                return _Contrast;
            }
            set
            {
                if (_Contrast != value)
                {
                    _Contrast = value;
                    RaisePropertyChanged("Contrast");
                    RenderImage();
                }
            }
        }

        int _Brightness = 0;
        public int Brightness
        {
            get
            {
                return _Brightness;
            }
            set
            {
                if (_Brightness != value)
                {
                    _Brightness = value;
                    RaisePropertyChanged("Brightness");
                    RenderImage();
                }
            }
        }

        int _Gamma = 0;
        public int Gamma
        {
            get
            {
                return _Gamma;
            }
            set
            {
                if (_Gamma != value)
                {
                    _Gamma = value;
                    RaisePropertyChanged("Gamma");
                    RenderImage();
                }
            }
        }

        bool _Normalize = false;
        public bool Normalize
        {
            get
            {
                return _Normalize;
            }
            set
            {
                if (_Normalize != value)
                {
                    _Normalize = value;
                    RaisePropertyChanged("Normalize");
                    RenderImage();
                }
            }
        }


        byte _RedBalance = 255;
        public byte RedBalance
        {
            get
            {
                return _RedBalance;
            }
            set
            {
                if (_RedBalance != value)
                {
                    _RedBalance = value;
                    RaisePropertyChanged("RedBalance");
                    RenderImage();
                }
            }
        }

        byte _GreenBalance = 255;
        public byte GreenBalance
        {
            get
            {
                return _GreenBalance;
            }
            set
            {
                if (_GreenBalance != value)
                {
                    _GreenBalance = value;
                    RaisePropertyChanged("GreenBalance");
                    RenderImage();
                }
            }
        }

        byte _BlueBalance = 255;
        public byte BlueBalance
        {
            get
            {
                return _BlueBalance;
            }
            set
            {
                if (_BlueBalance != value)
                {
                    _BlueBalance = value;
                    RaisePropertyChanged("BlueBalance");
                    RenderImage();
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
                    if (_IsLoading)
                    {
                        ShowProgress = System.Windows.Visibility.Visible;
                    }
                    else
                    {
                        ShowProgress = System.Windows.Visibility.Collapsed;
                    }
                }
            }
        }

        System.Windows.Visibility _ShowProgress = System.Windows.Visibility.Collapsed;
        public System.Windows.Visibility ShowProgress
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

        #region Metodos e Eventos

        async public void RenderImage()
        {
            if (Image != null)
            {
                IsLoading = true;
                await Task.Factory.StartNew(() =>
                {
                    RenderedImage = PhotoHelper.RenderFinal(this, RenderedImageSize.ToDrawingSize());
                    RenderedThumb = RenderedImage; //PhotoHelper.RenderThumb(RenderedImage, RenderedThumbSize.ToDrawingSize());
                });
                IsLoading = false;
            }

        }

        public void RenderImage(string FilePath)
        {

            //File.WriteAllBytes(FilePath, PhotoHelper.RenderFinal(this, ImageSize.ToDrawingSize()));
        }

        void RaisePropertyChanged(string prop)
        {
            if (PropertyChanged != null) { PropertyChanged(this, new PropertyChangedEventArgs(prop)); }
        }
        public event PropertyChangedEventHandler PropertyChanged;

        #endregion


    }

    public class Size : INotifyPropertyChanged
    {
        #region Construtores
        public Size()
        {
            Width = 0;
            Height = 0;
        }

        public Size(int width, int height)
        {
            Width = width;
            Height = height;
        }

        #endregion

        #region Propriedades

        int _Width;
        public int Width
        {
            get { return _Width; }
            set
            {
                if (_Width != value)
                {
                    _Width = value;
                    RaisePropertyChanged("Width");
                }
            }
        }

        int _Height;
        public int Height
        {
            get { return _Height; }
            set
            {
                if (_Height != value)
                {
                    _Height = value;
                    RaisePropertyChanged("Height");
                }
            }
        }

        #endregion

        #region Metodos e Eventos

        public System.Drawing.Size ToDrawingSize()
        {
            return new System.Drawing.Size(Width, Height);
        }

        void RaisePropertyChanged(string prop)
        {
            if (PropertyChanged != null) { PropertyChanged(this, new PropertyChangedEventArgs(prop)); }
        }
        public event PropertyChangedEventHandler PropertyChanged;

        #endregion
    }
}
