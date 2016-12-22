using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Ryan.AddressUtility.Models;

namespace Ryan.Maps.Win.ViewModels
{
    public class BingStreetsideViewModel : ViewModelBase
    {

        #region Fields

        private Address _propertyAddress;

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

        #endregion

        #region Constructors
        #endregion

        #region Methods
        #endregion

    }
}
