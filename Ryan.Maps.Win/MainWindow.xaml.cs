using Newtonsoft.Json.Linq;
using Ryan.AddressUtility.Interfaces;
using Ryan.AddressUtility.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Ryan.Maps.Win.ViewModels;
using Ryan.Maps.Win.Models;
using System.Collections.ObjectModel;

namespace Ryan.Maps.Win
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// MVVM pattern taken from http://stackoverflow.com/questions/15936428/binding-contentcontrol-content-for-dynamic-content
    /// </summary>
    public partial class MainWindow : Window
    {

        #region Fields
        
        #endregion

        #region Properties
        
        #endregion

        #region Constructors
        public MainWindow()
        {
            InitializeComponent();

            var mainWindowViewModel = new MainWindowViewModel();
            this.DataContext = mainWindowViewModel;
            mainWindowViewModel.PropertyChanged += viewModel_PropertyChanged;
        }
        
        #endregion

        #region Methods

        private void viewModel_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            // Reset all styles to default style
            this.bingMapButton.Style = (Style)FindResource("NavButtons");
            this.proximityButton.Style = (Style)FindResource("NavButtons");
            this.bingStreetside.Style = (Style)FindResource("NavButtons");
            this.bingAddress.Style = (Style)FindResource("NavButtons");
            this.propertySearch.Style = (Style)FindResource("NavButtons");
            this.deeds.Style = (Style)FindResource("NavButtons");
            this.hyperlinks.Style = (Style)FindResource("NavButtons");
            this.printableMap.Style = (Style)FindResource("NavButtons");
            

            var vm = sender as MainWindowViewModel;

            switch (vm?.CurrentViewModel?.ViewTitle)
            {
                case "Proximity View":
                    this.proximityButton.Style = (Style)FindResource("NavButtonsSelected");
                    break;
                case "Bing Maps View":
                    this.bingMapButton.Style = (Style)FindResource("NavButtonsSelected");
                    break;
                case "Bing Streetside View":
                    this.bingStreetside.Style = (Style)FindResource("NavButtonsSelected");
                    break;
                case "Bing Address View":
                    this.bingAddress.Style = (Style)FindResource("NavButtonsSelected");
                    break;
                case "Property Search View":
                    this.propertySearch.Style = (Style)FindResource("NavButtonsSelected");
                    break;
                case "Deeds View":
                    this.deeds.Style = (Style)FindResource("NavButtonsSelected");
                    break;
                case "Hyperlinks View":
                    this.hyperlinks.Style = (Style)FindResource("NavButtonsSelected");
                    break;
                case "Printable Map View":
                    this.printableMap.Style = (Style)FindResource("NavButtonsSelected");
                    break;
                case "Crop Image":
                    this.printableMap.Style = (Style)FindResource("NavButtonsSelected");
                    break;
            }

            
        }

        #endregion

    }

}
