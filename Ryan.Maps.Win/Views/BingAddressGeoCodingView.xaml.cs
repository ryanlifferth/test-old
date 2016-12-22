using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using Ryan.Maps.Win.ViewModels;
using Ryan.AddressUtility.Models;

namespace Ryan.Maps.Win.Views
{
    /// <summary>
    /// Interaction logic for BingAddressGeoCoding.xaml
    /// </summary>
    public partial class BingAddressGeoCodingView : UserControl
    {
        #region Fields

        private BingAddressGeoCodingViewModel _addressViewModel;

        #endregion

        #region Properties
        #endregion

        #region Constructors
        public BingAddressGeoCodingView()
        {
            InitializeComponent();
            this.Loaded += BingAddressGeoCodingView_Loaded;
            this.DataContextChanged += BingAddressGeoCodingView_DataContextChanged;
        }
        #endregion

        #region Event Handlers

        private void BingAddressGeoCodingView_Loaded(object sender, RoutedEventArgs e)
        {
            Window.GetWindow(this).KeyUp += BingAddressGeoCodingView_KeyUp;
        }

        private void BingAddressGeoCodingView_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Escape)
            {
                AddressDecisionPopup.IsOpen = false;
            }
        }

        private void Search_Click(object sender, RoutedEventArgs e)
        {
            _addressViewModel.GeoCodeResponse = new GeoCodeResponse();  // Clear out any old data

            if (string.IsNullOrEmpty(addressInput.Text))
            {
                addressInput.Text = "549 Muskmelon Way, Saratoga Springs, UT";
            }

            if (!string.IsNullOrEmpty(addressInput.Text))
            {
                ParseOriginalPropertyAddressForDisplay();
                var geoCodeRepo = new AddressUtility.Repositories.BingGeoCodeAddressRepository();
                var geoCodeResponse = geoCodeRepo.GeoCodeAddressWithDecisionInfo(new Address { FullAddress = addressInput.Text });
                _addressViewModel.GeoCodeResponse = geoCodeResponse;
                

                // Check to see what the responses are
                if (geoCodeResponse.MatchConfidence == AddressUtility.Models.MatchDecision.GoodMatch &&
                    geoCodeResponse.GeoCodedAddresses.Count == 1)
                {
                    // Good match
                    if (geoCodeResponse.Address.FullAddress.Length != geoCodeResponse.GeoCodedAddresses.FirstOrDefault().FullAddress.Length)
                    {
                        // Formatting difference, ask the user which one they want
                        _addressViewModel.DecisionText = "There is a slight difference between the address you entered and the USPS formatted address found.  Which would you like to use?";
                    }
                    else
                    {
                        // Basically an exact match - update the address object
                        _addressViewModel.PropertyAddress = _addressViewModel.GeoCodeResponse.GeoCodedAddresses.FirstOrDefault();
                        return;  // Don't show the popup
                    }
                }
                else
                {
                    if (geoCodeResponse.GeoCodedAddresses.Count > 1)
                    {
                        _addressViewModel.DecisionText = "We did not find an exact match.  Are any of these properties correct?";
                    }
                    else
                    {
                        _addressViewModel.DecisionText = "We did not find any properties.";
                    }
                }

                ShowPopup();
            }
        }

        private void BingAddressGeoCodingView_DataContextChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            _addressViewModel = (BingAddressGeoCodingViewModel)this.DataContext;
        }

        private void TestPopup_Click(object sender, RoutedEventArgs e)
        {
            ShowPopup();
        }

        private void GeoCodedAddress_Click(object sender, RoutedEventArgs e)
        {
            var buttonDataContext = ((Button)sender).DataContext;
            if (buttonDataContext.GetType() == typeof(Address))
            {
                _addressViewModel.PropertyAddress = _addressViewModel.GeoCodeResponse.GeoCodedAddresses.Where(x => x == buttonDataContext).FirstOrDefault();
                addressInput.Text = _addressViewModel.PropertyAddress.FullAddress;
                AddressDecisionPopup.IsOpen = false;
            }
        }

        private void OriginalAddress_Click(object sender, RoutedEventArgs e)
        {
            AddressDecisionPopup.IsOpen = false;
        }

        #endregion

        #region Methods

        private void ShowPopup()
        {
            AddressDecisionPopup.IsOpen = false;
            AddressDecisionPopup.PlacementTarget = Application.Current.MainWindow;
            AddressDecisionPopup.Placement = System.Windows.Controls.Primitives.PlacementMode.AbsolutePoint;
            AddressDecisionPopup.VerticalOffset = Application.Current.MainWindow.Top + (Application.Current.MainWindow.ActualHeight * .2);
            AddressDecisionPopup.IsOpen = true;

            var popupContainerWidth = ((Border)AddressDecisionPopup.FindName("AddressPopupContainer")).RenderSize.Width > 0 ? ((Border)AddressDecisionPopup.FindName("AddressPopupContainer")).RenderSize.Width : 100;
            AddressDecisionPopup.HorizontalOffset = Application.Current.MainWindow.Left + // start at left of application window
                                                    (Application.Current.MainWindow.ActualWidth * .5) + // middle of the window
                                                    popupContainerWidth * .5 + // half the width of the popup window
                                                    3 + // margin of popup (margin allows for dropshadow of popup border....it's a hack :))
                                                    5;  // horizontal padding (on each side)
            PopupHeader.MaxWidth = AddressPopupGrid.ActualWidth;
        }

        private void ParseOriginalPropertyAddressForDisplay()
        {
            var originalAddress = addressInput.Text.Split(',');
            _addressViewModel.PropertyAddressLine1 = originalAddress.FirstOrDefault();

            switch (originalAddress.Length)
            {
                case 2:
                    _addressViewModel.PropertyAddressLine2 = originalAddress[1].TrimStart();
                    break;
                case 3:
                    _addressViewModel.PropertyAddressLine2 = originalAddress[1].TrimStart() + "," + originalAddress[2];
                    break;
                case 4:
                    long outZip;
                    bool isZip = long.TryParse(originalAddress[3].Replace("-", "").Trim(), out outZip);
                    if (isZip)
                    {
                        _addressViewModel.PropertyAddressLine1 = originalAddress[0];
                        _addressViewModel.PropertyAddressLine2 = originalAddress[1].TrimStart() + "," + originalAddress[2] + " " + originalAddress[3];
                    }
                    else
                    {
                        _addressViewModel.PropertyAddressLine1 = originalAddress[0] + "," + originalAddress[1];
                        _addressViewModel.PropertyAddressLine2 = originalAddress[2].TrimStart() + "," + originalAddress[3];
                    }
                    break;
                case 5:
                    _addressViewModel.PropertyAddressLine1 = originalAddress[0] + "," + originalAddress[1];
                    _addressViewModel.PropertyAddressLine2 = originalAddress[2].TrimStart() + "," + originalAddress[3] + " " + originalAddress[4];
                    break;
                default:
                    _addressViewModel.PropertyAddressLine1 = addressInput.Text;
                    break;
            }

        }

        #endregion

    }
}
