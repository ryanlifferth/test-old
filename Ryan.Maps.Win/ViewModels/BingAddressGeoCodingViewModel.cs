using Ryan.AddressUtility.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ryan.Maps.Win.ViewModels
{
    public class BingAddressGeocodingViewModel : ViewModelBase
    {

        #region Fields

        private Address _propertyAddress;
        private GeoCodeResponse _geoCodeResponse;
        private string _decisionText;
        private string _propertyAddressLine1;
        private string _propertyAddressLine2;

        #endregion

        #region Properties

        public Address PropertyAddress
        {
            get { return _propertyAddress; }
            set
            {
                if (value == _propertyAddress) return;

                _propertyAddress = value;
                OnPropertyChanged("PropertyAddress");
            }
        }


        public string PropertyAddressLine1
        {
            get { return _propertyAddressLine1; }
            set
            {
                _propertyAddressLine1 = value;
                OnPropertyChanged("PropertyAddressLine1");
            }
        }
        
        public string PropertyAddressLine2
        {
            get { return _propertyAddressLine2; }
            set
            {
                _propertyAddressLine2 = value;
                OnPropertyChanged("PropertyAddressLine2");
            }
        }
        
        public GeoCodeResponse GeoCodeResponse
        {
            get { return _geoCodeResponse; }
            set
            {
                _geoCodeResponse = value;
                OnPropertyChanged("GeoCodeResponse");
            }
        }

        public string DecisionText
        {
            get { return _decisionText; }
            set
            {
                _decisionText = value;
                OnPropertyChanged("DecisionText");
            }
        }


        #endregion

        #region Constructors
        #endregion

        #region Methods
        #endregion

    }
}
