using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ryan.Maps.Win.Models
{

    public class Address : INotifyPropertyChanged
    {

        #region Fields

        private string _addressLine1;
        private string _addressLine2;
        private double _latitude;

        #endregion

        #region Properties

        public string AddressLine1
        {
            get { return _addressLine1; }
            set
            {
                if (value == _addressLine1) return;

                _addressLine1 = value;
                OnPropertyChanged("AddressLine1");
            }
        }

        public string AddressLine2
        {
            get { return _addressLine2; }
            set
            {
                if (value == _addressLine2) return;

                _addressLine2 = value;
                OnPropertyChanged("AddressLine2");
            }
        }

        public double Latitude
        {
            get { return _latitude; }
            set
            {
                if (value == _latitude) return;

                _latitude = value;
                OnPropertyChanged("Latitude");
            }
        }

        #endregion

        #region Interface Implementation

        public event PropertyChangedEventHandler PropertyChanged;

        protected void OnPropertyChanged(string propertyName)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }
        }

        #endregion

    }
}
