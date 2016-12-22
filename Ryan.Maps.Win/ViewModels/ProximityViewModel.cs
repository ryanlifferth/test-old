using Ryan.AddressUtility.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ryan.Maps.Win.ViewModels
{
    public class ProximityViewModel : ViewModelBase
    {

        #region Fields

        private Address _subjectAddress;
        private Address _comp1Address;
        private Address _comp2Address;
        private string _comp1DistanceFromSubject;
        private string _comp2DistanceFromSubject;
        private bool _showSpinner;
        
        #endregion

        #region Properties

        public Address SubjectAddress
        {
            get { return _subjectAddress; }
            set
            {
                if (value == _subjectAddress) return;

                _subjectAddress = value;
                OnPropertyChanged("SubjectAddress");
            }
        }

        public Address Comp1Address
        {
            get { return _comp1Address; }
            set
            {
                if (value == _comp1Address) return;

                _comp1Address = value;
                OnPropertyChanged("Comp1Address");
            }
        }

        public Address Comp2Address
        {
            get { return _comp2Address; }
            set
            {
                if (value == _comp2Address) return;

                _comp2Address = value;
                OnPropertyChanged("Comp2Address");
            }
        }

        public string Comp1DistanceFromSubject
        {
            get { return _comp1DistanceFromSubject; }
            set
            {
                if (value == _comp1DistanceFromSubject) return;

                _comp1DistanceFromSubject = value;
                OnPropertyChanged("Comp1DistanceFromSubject");
            }
        }

        public string Comp2DistanceFromSubject
        {
            get { return _comp2DistanceFromSubject; }
            set
            {
                if (value == _comp2DistanceFromSubject) return;

                _comp2DistanceFromSubject = value;
                OnPropertyChanged("Comp2DistanceFromSubject");
            }
        }

        public bool ShowSpinner
        {
            get { return _showSpinner; }
            set
            {
                if (value == _showSpinner) return;

                _showSpinner = value;
                OnPropertyChanged("ShowSpinner");
            }
        }

        #endregion

    }

}
