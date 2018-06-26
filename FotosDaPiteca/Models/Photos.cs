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
    [Serializable()]
    public class Photo : INotifyPropertyChanged
    {
        public Photo(FileInfo FilePath)
        {
            Name = FilePath.Name;
            OriginalPath = FilePath.FullName;
            Image = File.ReadAllBytes(FilePath.FullName);
        }
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

        int _WaterMarkSize = 24;
        public int WaterMarkSize
        {
            get { return _WaterMarkSize; }
            set
            {
                if (_WaterMarkSize != value)
                {
                    _WaterMarkSize = value;
                    RaisePropertyChanged("WaterMarkSize");
                    if (UseWaterMark)
                    {
                        RenderImage();
                    }
                }
            }
        }

        string _WaterMarkPosition = "Center";
        public string WaterMarkPosition
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

        bool _UseWaterMark;
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

        void RenderImage()
        {
            if (Image != null)
            {
                
                RenderedImage = PhotoHelper.RenderFinal(Image, UseWaterMark, WaterMark, WaterMarkPosition, WaterMarkColor, WaterMarkFont);
                RenderedThumb = PhotoHelper.RenderThumb(RenderedImage, 196, 140);
            }
            
        }

        void RaisePropertyChanged(string prop)
        {
            if (PropertyChanged != null) { PropertyChanged(this, new PropertyChangedEventArgs(prop)); }
        }
        public event PropertyChangedEventHandler PropertyChanged;
    }

}
