using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Threading;
using FotosDaPiteca.Models;

namespace FotosDaPiteca.ViewModel
{
    class ViewModelBase : INotifyPropertyChanged
    {
        //basic ViewModelBase
        internal void RaisePropertyChanged(string prop)
        {
            if (PropertyChanged != null) { PropertyChanged(this, new PropertyChangedEventArgs(prop)); }
        }
        public event PropertyChangedEventHandler PropertyChanged;

        //Extra Stuff, shows why a base ViewModel is useful
        bool? _CloseWindowFlag;
        public bool? CloseWindowFlag
        {
            get { return _CloseWindowFlag; }
            set
            {
                _CloseWindowFlag = value;
                RaisePropertyChanged("CloseWindowFlag");
            }
        }

        public virtual void CloseWindow(bool? result = true)
        {
            Application.Current.Dispatcher.BeginInvoke(DispatcherPriority.Background, new Action(() =>
            {
                CloseWindowFlag = CloseWindowFlag == null
                    ? true
                    : !CloseWindowFlag;
            }));
        }

        public ViewModelBase()
        {

        }


        double _ScaleX = 1;
        public double ScaleX
        {
            get { return _ScaleX; }
            set
            {
                if (_ScaleX != value)
                {
                    _ScaleX = value;
                    RaisePropertyChanged("ScaleX");
                }
            }
        }
        double _ScaleY = 1;
        public double ScaleY
        {
            get { return _ScaleY; }
            set
            {
                if (_ScaleY != value)
                {
                    _ScaleY = value;
                    RaisePropertyChanged("ScaleY");
                }
            }
        }

    }
}
