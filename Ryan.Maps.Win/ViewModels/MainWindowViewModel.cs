using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace Ryan.Maps.Win.ViewModels
{
    public class MainWindowViewModel : ViewModelBase
    {

        #region Fields
        public ICommand LoadProximityCommand { get; private set; }
        public ICommand LoadBingMapCommand { get; private set; }
        public ICommand LoadBingStreetsideCommand { get; private set; }
        public ICommand LoadBingAddressCommand { get; private set; }
        public ICommand LoadPropertySearchCommand { get; private set; }
        public ICommand LoadDeedsCommand { get; private set; }
        public ICommand LoadHyperlinkCommand { get; private set; }
        public ICommand LoadPrintableMapCommand { get; private set; }

        public ICommand LoadCropImageCommand { get; private set; }

        // ViewModel that is currently bound to the ContentControl
        private ViewModelBase _currentViewModel;
        #endregion

        #region Properties
        public ViewModelBase CurrentViewModel
        {
            get { return _currentViewModel; }
            set
            {
                _currentViewModel = value;
                this.OnPropertyChanged("CurrentViewModel");
            }
        }
        #endregion

        #region Constructor
        public MainWindowViewModel()
        {
            //this.LoadProximity();
            //this.LoadBingMap();
            //this.LoadBingStreetside();
            //this.LoadBingAddress();
            //this.LoadPropertySearch();
            //this.LoadDeeds();
            //this.LoadHyperlink();
            this.LoadPrintableMap();
            //this.LoadCropMap();

            // Hook up Commands to associated methods
            this.LoadProximityCommand = new DelegateCommand(o => this.LoadProximity());
            this.LoadBingMapCommand = new DelegateCommand(o => this.LoadBingMap());
            this.LoadBingStreetsideCommand = new DelegateCommand(o => this.LoadBingStreetside());
            this.LoadBingAddressCommand = new DelegateCommand(o => this.LoadBingAddress());
            this.LoadPropertySearchCommand = new DelegateCommand(o => this.LoadPropertySearch());
            this.LoadDeedsCommand = new DelegateCommand(o => this.LoadDeeds());
            this.LoadHyperlinkCommand = new DelegateCommand(o => this.LoadHyperlink());
            this.LoadPrintableMapCommand = new DelegateCommand(o => this.LoadPrintableMap());

            this.LoadCropImageCommand = new DelegateCommand(o => this.LoadCropMap());
        }
        #endregion

        #region Methods

        private void LoadProximity()
        {
            CurrentViewModel = new ProximityViewModel() { ViewTitle = "Proximity View" };
        }

        private void LoadBingMap()
        {
            CurrentViewModel = new BingMapViewModel() { ViewTitle = "Bing Maps View" };
        }

        private void LoadBingStreetside()
        {
            CurrentViewModel = new BingStreetsideViewModel() { ViewTitle = "Bing Streetside View" };
        }

        private void LoadBingAddress()
        {
            CurrentViewModel = new BingAddressGeocodingViewModel() { ViewTitle = "Bing Address View" };
        }

        private void LoadPropertySearch()
        {
            CurrentViewModel = new PropertySearchViewModel() { ViewTitle = "Property Search View" };
        }

        private void LoadDeeds()
        {
            CurrentViewModel = new DeedsViewModel() { ViewTitle = "Deeds View" };
        }

        private void LoadHyperlink()
        {
            CurrentViewModel = new HyperlinkViewModel() { ViewTitle = "Hyperlinks View" };
        }

        private void LoadPrintableMap()
        {
            CurrentViewModel = new PrintableMapViewModel() { ViewTitle = "Printable Map View" };
        }

        private void LoadCropMap()
        {
            CurrentViewModel = new CropMapViewModel() { ViewTitle = "Crop Image" };
        }

        #endregion

    }
}
