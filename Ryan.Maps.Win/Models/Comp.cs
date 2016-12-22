using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ryan.Maps.Win.Models
{
    public class Comp : INotifyPropertyChanged
    {

        #region Fields

        private Ryan.AddressUtility.Models.Address _address;
        private string _distanceFromSubject;
        private string _compNumber;

        #endregion

        #region Properties



        public Ryan.AddressUtility.Models.Address Address
        {
            get { return _address; }
            set
            {
                if (value == _address) return;

                _address = value;
                OnPropertyChanged("Address");
            }
        }

        public string DistanceFromSubject
        {
            get { return _distanceFromSubject; }
            set
            {
                if (value == _distanceFromSubject) return;

                _distanceFromSubject = value;
                OnPropertyChanged("DistanceFromSubject");
            }
        }

        public string CompNumber
        {
            get { return _compNumber; }
            set
            {
                if (value == _compNumber) return;

                _compNumber = value;
                OnPropertyChanged("CompNumber");
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
