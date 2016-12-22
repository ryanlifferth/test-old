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
            this.LoadBingAddress();

            // Hook up Commands to associated methods
            this.LoadProximityCommand = new DelegateCommand(o => this.LoadProximity());
            this.LoadBingMapCommand = new DelegateCommand(o => this.LoadBingMap());
            this.LoadBingStreetsideCommand = new DelegateCommand(o => this.LoadBingStreetside());
            this.LoadBingAddressCommand = new DelegateCommand(o => this.LoadBingAddress());
        }
        #endregion

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
            CurrentViewModel = new BingAddressGeoCodingViewModel() { ViewTitle = "Bing Address View" };
        }




    }
}
