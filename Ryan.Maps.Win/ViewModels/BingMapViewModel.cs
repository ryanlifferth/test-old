using Ryan.AddressUtility.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ryan.Maps.Win.ViewModels
{
    public class BingMapViewModel : ViewModelBase
    {

        #region Fields

        private Address _subjectAddress;
        private Ryan.Maps.Win.Models.Comp _comp1;
        private Ryan.Maps.Win.Models.Comp _comp2;
        private Ryan.Maps.Win.Models.Comp _comp3;
        private bool _showDetailedMapPushpin;

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



        public Ryan.Maps.Win.Models.Comp Comp1
        {
            get { return _comp1; }
            set
            {
                if (value == _comp1) return;

                _comp1 = value;
                OnPropertyChanged("Comp1");
            }
        }

        public Ryan.Maps.Win.Models.Comp Comp2
        {
            get { return _comp2; }
            set
            {
                if (value == _comp2) return;

                _comp2 = value;
                OnPropertyChanged("Comp2");
            }
        }

        public Ryan.Maps.Win.Models.Comp Comp3
        {
            get { return _comp3; }
            set
            {
                if (value == _comp3) return;

                _comp3 = value;
                OnPropertyChanged("Comp3");
            }
        }

        public bool ShowDetailedMapPushpin
        {
            get { return _showDetailedMapPushpin; }
            set
            {
                if (value == _showDetailedMapPushpin) return;

                _showDetailedMapPushpin = value;
                OnPropertyChanged("ShowDetailedMapPushpin");
            }
        }


        #endregion

        #region Constructors
        #endregion

        #region Methods
        #endregion

    }
}
