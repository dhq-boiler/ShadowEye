

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ShadowEye.Utils
{
    [Serializable]
    public abstract class Notifier : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        protected bool SetProperty<T>(ref T storage, T value, string propertyName = null)
        {
            if (object.Equals(storage, value))
            {
                return false;
            }
            storage = value;
            this.OnPropertyChanged(propertyName);
            return true;
        }

        protected bool SetProperty<T>(ref T storage, T value, params string[] propertyNames)
        {
            if (object.Equals(storage, value))
            {
                return false;
            }
            storage = value;
            foreach (var propertyName in propertyNames)
            {
                this.OnPropertyChanged(propertyName);
            }
            return true;
        }

        protected void OnPropertyChanged(string propertyName = null)
        {
            PropertyChangedEventHandler propertyChanged = this.PropertyChanged;
            if (propertyChanged != null)
            {
                propertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }
        }

        protected void OnPropertyChanged(params string[] propertyNames)
        {
            PropertyChangedEventHandler propertyChanged = this.PropertyChanged;
            if (propertyChanged != null)
            {
                foreach (var propertyName in propertyNames)
                {
                    propertyChanged(this, new PropertyChangedEventArgs(propertyName));
                }
            }
        }
    }
}
