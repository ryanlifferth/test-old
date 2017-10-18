using Ryan.AddressUtility.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ryan.Maps.Win.ViewModels
{
    public class PrintableMapViewModel : ViewModelBase
    {

        #region Fields

        private Address _subject;
        private List<Models.Comp> _comps;
        private bool _showDetailedMapPushpin;
        private bool _showMarketConditionsOnMap;

        #endregion

        #region Properties

        public Address Subject
        {
            get { return _subject; }
            set
            {
                if (value == _subject) return;

                _subject = value;
                OnPropertyChanged("Subject");
            }
        }

        public List<Models.Comp> Comps
        {
            get { return _comps; }
            set
            {
                if (value == _comps) return;

                _comps = value;
                OnPropertyChanged("Comps");
            }
        }

        public bool ShowDetailedMapPushpin
        {
            get { return _showDetailedMapPushpin; }
            set
            {
                _showDetailedMapPushpin = value;
                OnPropertyChanged("ShowDetailedMapPushpin");
            }
        }

        public bool ShowMarketConditionsOnMap
        {
            get { return _showMarketConditionsOnMap; }
            set
            {
                _showMarketConditionsOnMap = value;
                OnPropertyChanged("ShowMarketConditionsOnMap");
            }
        }

        #endregion

        #region Constructors

        public PrintableMapViewModel()
        {
            ShowDetailedMapPushpin = true;
            ShowMarketConditionsOnMap = false;
        }

        #endregion

    }
}
