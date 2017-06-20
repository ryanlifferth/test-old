using Ryan.Maps.Win.Commands;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace Ryan.Maps.Win.ViewModels
{
    public class DeedsViewModel : ViewModelBase
    {

        #region Fields
        private string _title;
        private List<PublicRecordsDeedFacade> _publicRecordsDeedsList;
        private List<MlsListingHistory> _mlsListingHistoryList;
        private PublicRecordsDeedFacade _salesVerificationDeed;
        private ObservableCollection<PublicRecordsDeedFacade> _priorSalesDeedList;
        private PublicRecordsDeedFacade _selectedDeed;
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

        public List<PublicRecordsDeedFacade> PublicRecordsDeedsList
        {
            get { return _publicRecordsDeedsList; }
            set
            {
                _publicRecordsDeedsList = value;
                OnPropertyChanged("PublicRecordsDeedsList");
            }
        }

        public List<MlsListingHistory> MlsListingHistoryList
        {
            get { return _mlsListingHistoryList; }
            set
            {
                _mlsListingHistoryList = value;
                OnPropertyChanged("MlsListingHistoryList");
            }
        }

        public PublicRecordsDeedFacade SalesVerificationDeed
        {
            get { return _salesVerificationDeed; }
            set
            {
                _salesVerificationDeed = value;
                OnPropertyChanged("SalesVerificationDeed");
            }
        }


        public ObservableCollection<PublicRecordsDeedFacade> PriorSalesDeedList
        {
            get { return _priorSalesDeedList; }
            set
            {
                _priorSalesDeedList = value;
                OnPropertyChanged("PriorSalesDeedList");
            }
        }

        public PublicRecordsDeedFacade SelectedDeed
        {
            get { return _selectedDeed; }
            set
            {
                _selectedDeed = value;
                OnPropertyChanged("SelectedDeed");
            }
        }


        #endregion

        #region Constructors

        public DeedsViewModel()
        {
            //PublicRecordsDeedsList.OnPropertyChanged += OnDeedListPropertyChanged;
            
        }

        void OnDeedListPropertyChanged(object sender, PropertyChangedEventArgs e)
        {

        }

        #endregion


        private ICommand _selectedCommand;

        public ICommand SelectedCommand
        {
            get
            {
                return _selectedCommand ?? (_selectedCommand = new RelayCommand(param => SelectedCommandExecute(param)));
            }
        }

        private ICommand _checkboxClicked;
        public ICommand CheckboxClicked
        {
            get
            {
                return _checkboxClicked ?? (_checkboxClicked = new RelayCommand(param => CheckboxClickedExecute(param)));
            }
        }

        private void SelectedCommandExecute(object param)
        {
            //http://stackoverflow.com/questions/15327227/datagrid-bind-command-to-row-select
            // Do something
            var s = "";
            var r = "";

            SalesVerificationDeed.IsCurrentSale = true;

            //if (param != null)
            //{
            //    var selectedDeed = (PublicRecordsDeed)param;
            //    var gridData = (List<PublicRecordsDeed)param;
            //    selectedDeed.IsCurrentSale = false;
            //}

        }

        private void CheckboxClickedExecute(object param)
        {
            var checkboxType = (string)param ?? "SalesVerification";

            switch (checkboxType)
            {
                case "SalesVerification":
                    // Make sure it is checked == true
                    if (SelectedDeed.IsCurrentSale == true)
                    {
                        // "Unselect" all previous current sales items (if any)
                        PublicRecordsDeedsList.Where(d => d != SelectedDeed).ToList().ForEach(a => a.IsCurrentSale = false);
                        PublicRecordsDeedsList.First(d => d == SelectedDeed).IsPriorSale = false;
                        RemoveSelectedDeedFromPriorSalesDeedList();     // Removes the selected deed from the PriorSalesDeedList (if it is on that list)
                        SalesVerificationDeed = SelectedDeed;
                        // TODO:  1.  Add selected row style
                        //        2.  Add/Update selected deed display
                    }
                    else
                    {
                        SalesVerificationDeed = null;
                    }
                    break;
                case "PriorSale":
                    if (SelectedDeed.IsPriorSale == true)
                    {
                        // Set SalesVerificationDeed to null if IsCurrentSale
                        if (SelectedDeed.IsCurrentSale)
                        {
                            SalesVerificationDeed = null;
                        }
                        // Uncheck Sales Verification if it is selected
                        PublicRecordsDeedsList.First(d => d == SelectedDeed).IsCurrentSale = false;
                        // TODO:  1.  Add selected row style
                        //        2.  Add/Update selected deed display list
                        AddSelectedDeedToPriorSalesDeedList();
                    }
                    else
                    {
                        RemoveSelectedDeedFromPriorSalesDeedList();
                    }
                    break;
            }

        }

        private void RemoveSelectedDeedFromPriorSalesDeedList()
        {
            if (PriorSalesDeedList != null)
            {
                PriorSalesDeedList.Remove(SelectedDeed);
            }
        }

        private void AddSelectedDeedToPriorSalesDeedList()
        {
            if (PriorSalesDeedList != null)
            {
                PriorSalesDeedList.Add(SelectedDeed);
            }
            else
            {
                PriorSalesDeedList = new ObservableCollection<PublicRecordsDeedFacade>() { SelectedDeed };
            }
        }

    }

    public class PublicRecordsDeedFacade : ViewModelBase
    {
        private PublicRecordsDeed _publicRecordsDeed = new PublicRecordsDeed();

        public bool IsCurrentSale
        {
            get { return _publicRecordsDeed.IsCurrentSale; }
            set
            {
                _publicRecordsDeed.IsCurrentSale = value;
                OnPropertyChanged("IsCurrentSale");
            }
        }

        public bool IsPriorSale
        {
            get { return _publicRecordsDeed.IsPriorSale; }
            set
            {
                _publicRecordsDeed.IsPriorSale = value;
                OnPropertyChanged("IsPriorSale");
            }
        }

        public string DocNumber
        {
            get { return _publicRecordsDeed.DocNumber; }
            set { _publicRecordsDeed.DocNumber = value; }
        }
        public DateTime RecordDate
        {
            get { return _publicRecordsDeed.RecordDate; }
            set { _publicRecordsDeed.RecordDate = value; }
        }
        public string DeedType
        {
            get { return _publicRecordsDeed.DeedType; }
            set { _publicRecordsDeed.DeedType = value; }
        }
        public double Amount
        {
            get { return _publicRecordsDeed.Amount; }
            set { _publicRecordsDeed.Amount = value; }
        }
        public string Grantor
        {
            get { return _publicRecordsDeed.Grantor; }
            set { _publicRecordsDeed.Grantor = value; }
        }
        public string Grantee
        {
            get { return _publicRecordsDeed.Grantee; }
            set { _publicRecordsDeed.Grantee = value; }
        }

    }


    public class PublicRecordsDeed : INotifyPropertyChanged
    {

        bool _isCurrentSale;
        bool _isPriorSale;
        public bool IsCurrentSale
        {
            get { return _isCurrentSale; }
            set
            {
                _isCurrentSale = value;
                //OnPropertyChanged("IsCurrentSale");
            }
        }
        public bool IsPriorSale
        {
            get { return _isPriorSale; }
            set
            {
                _isPriorSale = value;
                //OnPropertyChanged("IsPriorSale");
            }
        }
        public string DocNumber { get; set; }
        public DateTime RecordDate { get; set; }
        public string DeedType { get; set; }
        public double Amount { get; set; }
        public string Grantor { get; set; }
        public string Grantee { get; set; }

        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged(string propertyName)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }
        }
    }

    public class MlsListingHistory
    {

        public string MlsNumber { get; set; }
        public DateTime ClosedDate { get; set; }
        public string Status { get; set; }
        public double SoldPrice { get; set; }
        public DateTime ListDate { get; set; }
        public double ListPrice { get; set; }
        public int DaysOnMarket { get; set; }

    }


}
