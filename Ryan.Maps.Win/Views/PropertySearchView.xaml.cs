using Ryan.AddressUtility.Models;
using Ryan.Maps.Win.Models;
using Ryan.Maps.Win.ViewModels;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Device.Location;
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

namespace Ryan.Maps.Win.Views
{
    /// <summary>
    /// Interaction logic for PropertySearchView.xaml
    /// </summary>
    public partial class PropertySearchView : UserControl
    {
        private GeoCoordinateWatcher Watcher = null;


        public PropertySearchView()
        {
            InitializeComponent();

            PartialAddressSearchInput.Visibility = Visibility.Collapsed;

            // Create the watcher.
            Watcher = new GeoCoordinateWatcher();
            // Catch the StatusChanged event.
            Watcher.StatusChanged += Watcher_StatusChanged;
            // Start the watcher.
            Watcher.Start();

            var states = new string[] { "Utah", "Oregon", "California", "Washington" };
            var federalStateList = new FederalState().Get().Where(a => states.Contains(a.StateName)).ToList();
            this.DataContext = new PropertySearchViewModel
            {
                Title = "Ryan's Data Source",
                StandardizedParcelList = CreateStandardizedParcelList(),
                FederalStateList = federalStateList,
                SelectedFederalState = federalStateList.FirstOrDefault(a => a.StateAbbreviation == "UT")
            };
            //AddressSearchState.SelectedItem = propertySearchViewModel.FederalStateList.FirstOrDefault(s => s.StateAbbreviation == "UT");


        }

        private List<StandardizedParcel> CreateStandardizedParcelList()
        {
            return new List<StandardizedParcel> {
                new StandardizedParcel { Apn = "123456", MlsNumber = "654654", AddressLine1 = "2939 N 725 W", AddressLine2 = "", City = "Layton", County = "Davis", State = "UT", Zip = "84041" },
                new StandardizedParcel { Apn = "654321", MlsNumber = "321321", AddressLine1 = "549 W Muskmelon Way", AddressLine2 = "", City = "Saratoga Springs", County = "Utah", State = "UT", Zip = "84045" },
                new StandardizedParcel { Apn = "987654", MlsNumber = "987987", AddressLine1 = "765 E Gordon Ave", AddressLine2 = "Suite 300", City = "Layton", County = "Davis", State = "UT", Zip = "84041" },
                new StandardizedParcel { Apn = "456789", MlsNumber = "321654", AddressLine1 = "929 S Arbor Way", AddressLine2 = "", City = "Layton", County = "Davis", State = "UT", Zip = "84041" },
                new StandardizedParcel { Apn = "654789", MlsNumber = "789321", AddressLine1 = "123 Somehwere Ave", AddressLine2 = "Unit B", City = "Kaysville", County = "Davis", State = "UT", Zip = "84040" },

                /*
                new StandardizedParcel { Apn = "123456", MlsNumber = "654654", AddressLine1 = "2939 N 725 W", AddressLine2 = "", City = "Layton", County = "Davis", State = "UT", Zip = "84041" },
                new StandardizedParcel { Apn = "654321", MlsNumber = "321321", AddressLine1 = "549 W Muskmelon Way", AddressLine2 = "", City = "Saratoga Springs", County = "Utah", State = "UT", Zip = "84045" },
                new StandardizedParcel { Apn = "987654", MlsNumber = "987987", AddressLine1 = "765 E Gordon Ave", AddressLine2 = "Suite 300", City = "Layton", County = "Davis", State = "UT", Zip = "84041" },
                new StandardizedParcel { Apn = "456789", MlsNumber = "321654", AddressLine1 = "929 S Arbor Way", AddressLine2 = "", City = "Layton", County = "Davis", State = "UT", Zip = "84041" },
                new StandardizedParcel { Apn = "654789", MlsNumber = "789321", AddressLine1 = "123 Somehwere Ave", AddressLine2 = "Unit B", City = "Kaysville", County = "Davis", State = "UT", Zip = "84040" },
                new StandardizedParcel { Apn = "123456", MlsNumber = "654654", AddressLine1 = "2939 N 725 W", AddressLine2 = "", City = "Layton", County = "Davis", State = "UT", Zip = "84041" },
                new StandardizedParcel { Apn = "654321", MlsNumber = "321321", AddressLine1 = "549 W Muskmelon Way", AddressLine2 = "", City = "Saratoga Springs", County = "Utah", State = "UT", Zip = "84045" },
                new StandardizedParcel { Apn = "987654", MlsNumber = "987987", AddressLine1 = "765 E Gordon Ave", AddressLine2 = "Suite 300", City = "Layton", County = "Davis", State = "UT", Zip = "84041" },
                new StandardizedParcel { Apn = "456789", MlsNumber = "321654", AddressLine1 = "929 S Arbor Way", AddressLine2 = "", City = "Layton", County = "Davis", State = "UT", Zip = "84041" },
                new StandardizedParcel { Apn = "654789", MlsNumber = "789321", AddressLine1 = "123 Somehwere Ave", AddressLine2 = "Unit B", City = "Kaysville", County = "Davis", State = "UT", Zip = "84040" },
                new StandardizedParcel { Apn = "123456", MlsNumber = "654654", AddressLine1 = "2939 N 725 W", AddressLine2 = "", City = "Layton", County = "Davis", State = "UT", Zip = "84041" },
                new StandardizedParcel { Apn = "654321", MlsNumber = "321321", AddressLine1 = "549 W Muskmelon Way", AddressLine2 = "", City = "Saratoga Springs", County = "Utah", State = "UT", Zip = "84045" },
                new StandardizedParcel { Apn = "987654", MlsNumber = "987987", AddressLine1 = "765 E Gordon Ave", AddressLine2 = "Suite 300", City = "Layton", County = "Davis", State = "UT", Zip = "84041" },
                new StandardizedParcel { Apn = "456789", MlsNumber = "321654", AddressLine1 = "929 S Arbor Way", AddressLine2 = "", City = "Layton", County = "Davis", State = "UT", Zip = "84041" },
                new StandardizedParcel { Apn = "654789", MlsNumber = "789321", AddressLine1 = "123 Somehwere Ave", AddressLine2 = "Unit B", City = "Kaysville", County = "Davis", State = "UT", Zip = "84040" },
                */

                new StandardizedParcel { Apn = "555444", MlsNumber = "333222", AddressLine1 = "365 E Gordon Ave", AddressLine2 = "#21", City = "Layton", County = "Davis", State = "UT", Zip = "84041" }
            };
        }


        // The watcher's status has change. See if it is ready.
        private void Watcher_StatusChanged(object sender, GeoPositionStatusChangedEventArgs e)
        {
            if (e.Status == GeoPositionStatus.Ready)
            {
                // Display the latitude and longitude.
                if (Watcher.Position.Location.IsUnknown)
                {
                    Console.WriteLine("Cannot find location data");
                }
                else
                {
                    GeoCoordinate location = Watcher.Position.Location;
                    Console.WriteLine(location.Latitude.ToString() + " " + location.Longitude.ToString());
                    var geocodeRepo = new AddressUtility.Repositories.BingGeocodeAddressRepository();
                    var address = geocodeRepo.GeocodeByPoint(location);
                    var county = address.County;
                    var stateCode = address.State;
                    //State.SelectedValue
                    //var lItem = State.Items
                    //                 .Cast<string>()
                    //                 .Where(item => String.Compare(item.ToString(), "abc") == 0);

                    //cmbBudgetYear.SelectedValue = "2009";
                }
            }
        }

        private void ShowPartialAddressSearch_Click(object sender, RoutedEventArgs e)
        {
            var thisButton = (Button)sender;
            var ryan = thisButton.Tag;

            if (thisButton.Tag.ToString() == "NormalAddressSearch")
            {
                thisButton.Tag = "PartialAddressSearch";
                thisButton.Content = "normal search";
                AddressSearchInput.Visibility = Visibility.Collapsed;
                PartialAddressSearchInput.Visibility = Visibility.Visible;
            }
            else
            {
                thisButton.Tag = "NormalAddressSearch";
                thisButton.Content = "partial search";
                PartialAddressSearchInput.Visibility = Visibility.Collapsed;
                AddressSearchInput.Visibility = Visibility.Visible;
            }
        }
    }


    public class StandardizedParcel
    {
        public string Apn { get; set; }
        public string MlsNumber { get; set; }
        public string AddressLine1 { get; set; }
        public string AddressLine2 { get; set; }
        public string City { get; set; }
        public string County { get; set; }
        public string State { get; set; }
        public string Zip { get; set; }
    }

}
