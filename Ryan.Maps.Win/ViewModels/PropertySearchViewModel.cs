using Ryan.Maps.Win.Models;
using Ryan.Maps.Win.Views;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ryan.Maps.Win.ViewModels
{
    public class PropertySearchViewModel : ViewModelBase
    {

        #region Fields
        private StandardizedParcel _selectedParcel;
        private List<StandardizedParcel> _standardizedParcelList;
        private string _title;
        private List<FederalState> _federalStateList;
        private List<County> _countyList;
        #endregion

        #region Properties


        public string Title
        {
            get { return _title; }
            set
            {
                _title = value;
                OnPropertyChanged("Title");
            }
        }


        public List<StandardizedParcel> StandardizedParcelList
        {
            get { return _standardizedParcelList; }
            set
            {
                _standardizedParcelList = value;
                OnPropertyChanged("StandardizedParcelList");
            }
        }



        public StandardizedParcel SelectedParcel
        {
            get { return _selectedParcel; }
            set
            {
                _selectedParcel = value;
                OnPropertyChanged("SelectedParcel");
            }
        }

        public List<FederalState> FederalStateList
        {
            get { return _federalStateList; }
            set
            {
                _federalStateList = value;
                OnPropertyChanged("FederalStateList");
            }
        }

        public List<County> CountyList
        {
            get { return _countyList; }
            set
            {
                _countyList = value;
                OnPropertyChanged("CountyList");
            }
        }

        private FederalState _selectedFederalState;

        public FederalState SelectedFederalState
        {
            get { return _selectedFederalState; }
            set
            {
                _selectedFederalState = value;
                OnPropertyChanged("SelectedFederalState");
            }
        }

        #endregion


        #region Constructors

        #endregion

    }
}
