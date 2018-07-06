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

        }

        public Photo(FileInfo FilePath)
        {
            Name = FilePath.Name;
            OriginalPath = FilePath.FullName;
            Image = File.ReadAllBytes(FilePath.FullName);
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

        Size _ImageSize = new Size(800, 600);
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
                if (_RenderedImageSize != value)
                {
                    _RenderedImageSize = value;
                    RaisePropertyChanged("RenderedImageSize");
                    RenderImage();
                }
            }
        }

        byte[] _RenderedImage;
        public byte[] RenderedImage
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
        byte[] _RenderedThumb;
        public byte[] RenderedThumb
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



        string _WaterMark;
        public string WaterMark
        {
            get { return _WaterMark; }
            set
            {
                if (_WaterMark != value)
                {
                    _WaterMark = value;
                    RaisePropertyChanged("WaterMark");
                    if (UseWaterMark)
                    {
                        RenderImage();
                    }
                }
            }
        }

        string _WaterMarkFont = "Segoe UI";
        public string WaterMarkFont
        {
            get { return _WaterMarkFont; }
            set
            {
                if (_WaterMarkFont != value)
                {
                    _WaterMarkFont = value;
                    RaisePropertyChanged("WaterMarkFont");
                    if (UseWaterMark)
                    {
                        RenderImage();
                    }
                }
            }
        }

        string _WaterMarkColor = "#FF000000";
        public string WaterMarkColor
        {
            get { return _WaterMarkColor; }
            set
            {
                if (_WaterMarkColor != value)
                {
                    _WaterMarkColor = value;
                    RaisePropertyChanged("WaterMarkColor");
                    if (UseWaterMark)
                    {
                        RenderImage();
                    }
                }
            }
        }

        int _WaterMarkFontSize = 120;
        public int WaterMarkFontSize
        {
            get { return _WaterMarkFontSize; }
            set
            {
                if (_WaterMarkFontSize != value)
                {
                    _WaterMarkFontSize = value;
                    RaisePropertyChanged("WaterMarkFontSize");
                    if (UseWaterMark)
                    {
                        RenderImage();
                    }
                }
            }
        }

        Posicoes _WaterMarkPosition = Posicoes.Centro;
        public Posicoes WaterMarkPosition
        {
            get { return _WaterMarkPosition; }
            set
            {
                if (_WaterMarkPosition != value)
                {
                    _WaterMarkPosition = value;
                    RaisePropertyChanged("WaterMarkPosition");
                    if (UseWaterMark)
                    {
                        RenderImage();
                    }
                }
            }
        }

        bool _UseWaterMark = true;
        public bool UseWaterMark
        {
            get { return _UseWaterMark; }
            set
            {
                if (_UseWaterMark != value)
                {
                    _UseWaterMark = value;
                    RaisePropertyChanged("UseWaterMark");
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

        async void RenderImage()
        {
            if (Image != null)
            {
                IsLoading = true;
                await Task.Factory.StartNew(() =>
                {
                    RenderedImage = PhotoHelper.RenderFinal(this, RenderedImageSize.ToDrawingSize());
                    RenderedThumb = PhotoHelper.RenderThumb(RenderedImage, RenderedThumbSize.ToDrawingSize());
                });
                IsLoading = false;
            }

        }

        public void RenderImage(string FilePath)
        {

            File.WriteAllBytes(FilePath, PhotoHelper.RenderFinal(this, ImageSize.ToDrawingSize()));
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
