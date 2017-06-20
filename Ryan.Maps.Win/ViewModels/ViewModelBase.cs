using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ryan.Maps.Win.ViewModels
{
    public abstract class ViewModelBase : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;
        private string _viewTitle;

        public string ViewTitle
        {
            get
            {
                return _viewTitle;
            }
            set
            {
                _viewTitle = value;
                this.OnPropertyChanged(value);
            }
        }

        protected void OnPropertyChanged(string propertyName)
        {
            this.OnPropertyChanged(new PropertyChangedEventArgs(propertyName));
        }

        protected virtual void OnPropertyChanged(PropertyChangedEventArgs e)
        {
            var handler = this.PropertyChanged;
            if (handler != null)
            {
                handler(this, e);
            }
        }

        internal void CallPropertyChangedOnAll()
        {
            this.PropertyChanged(this, new PropertyChangedEventArgs(string.Empty));
        }


    }

}
