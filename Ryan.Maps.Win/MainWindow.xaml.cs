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

namespace Ryan.Maps.Win
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {

        #region Fields
        private MapViewModel _mapViewModel;
        #endregion

        #region Properties

        #endregion

        #region Constructors
        public MainWindow()
        {
            InitializeComponent();

            _mapViewModel = new MapViewModel
            {
                SubjectAddress = new Address
                {
                    AddressLine1 = "325 E Gordon Ave",
                    AddressLine2 = "Layton, UT 84041"
                },
                Comp1Address = new Address
                {
                    AddressLine1 = "765 E Gordon Ave",
                    AddressLine2 = "Layton, UT 84041"
                },
                Comp2Address = new Address
                {
                    AddressLine1 = "929 E Arbor Way",
                    AddressLine2 = "Layton, UT 84041"
                },
                ShowSpinner = false
            };

            //GetDistanceFromSubjectGoogle(FormatAddressesForGoogleDistance(new List<Address> { _mapViewModel.SubjectAddress }), 
            //                             FormatAddressesForGoogleDistance(new List<Address> { _mapViewModel.Comp1Address, _mapViewModel.Comp2Address }));

            this.DataContext = _mapViewModel;
        }



        #endregion

        #region Methods
        public async void MainWindow_Loaded(object sender, RoutedEventArgs e)
        {
            _mapViewModel.ShowSpinner = true;

            string subjectAddress = FormatAddressesForGoogleDistance(new List<Address> { _mapViewModel.SubjectAddress });
            List<string> compAddresses = new List<string> {
                FormatAddressesForGoogleDistance(new List<Address> { _mapViewModel.Comp1Address }),
                FormatAddressesForGoogleDistance(new List<Address> { _mapViewModel.Comp2Address })
            };
            var distance = new Ryan.AddressUtility.Repositories.GoogleDistanceCalculationRepository();
            //var distance = new Ryan.AddressUtility.Repositories.BingDistanceCalculationRepository();
            var distances = await GetDistanceFromSubjectAsync(subjectAddress, compAddresses, distance);

            await Task.Delay(3000);

            // Now display the results
            _mapViewModel.Comp1DistanceFromSubject = distances.Destinations[0].DistanceFromOrigin;
            _mapViewModel.Comp2DistanceFromSubject = distances.Destinations[1].DistanceFromOrigin;
            _mapViewModel.ShowSpinner = false;
        }

        private async Task<DistanceResponse> GetDistanceFromSubjectAsync(string subjectAddress, List<string> destinationAddresses, IDistanceCalculation distance)
        {
            return await Task.Run(() => distance.CalculateDistances(subjectAddress, destinationAddresses));
        }

        #region OLD METHOD
        //public async void MainWindow_Loaded_OLD(object sender, RoutedEventArgs e)
        //{
        //    _mapViewModel.ShowSpinner = true;

        //    string subjectAddress = FormatAddressesForGoogleDistance(new List<Address> { _mapViewModel.SubjectAddress });
        //    string compAddresses = FormatAddressesForGoogleDistance(new List<Address> { _mapViewModel.Comp1Address, _mapViewModel.Comp2Address });
        //    var s = await GetDistanceFromSubjectGoogleAsync(subjectAddress, compAddresses);

        //    await Task.Delay(3000);

        //    // Now do some stuff
        //    dynamic elements = (JObject.Parse(s) as dynamic).rows[0].elements;

        //    var distances = new List<string>();
        //    foreach (var element in elements)
        //    {
        //        distances.Add(element.distance.text.Value);
        //    }
        //    _mapViewModel.Comp1DistanceFromSubject = distances[0];
        //    _mapViewModel.Comp2DistanceFromSubject = distances[1];
        //    _mapViewModel.ShowSpinner = false;
        //}

        //private async Task<string> GetDistanceFromSubjectGoogleAsync(string subjectAddress, string destinationAddresses)
        //{
        //    return await Task.Run(() => GetDistanceFromSubjectGoogle(subjectAddress, destinationAddresses));
        //}

        /// <summary>
        ///     Gets the distance between origin and destination
        ///     Sample Google GET request - https://maps.googleapis.com/maps/api/distancematrix/json?origins=549+Muskmelon+Way,+Saratoga+Springs+UT+84045&destinations=325+E+Gordon+Ave,+Layton,+UT+84041|765+E+Gordon+Ave,+Layton,+UT+84041&units=imperial&key=AIzaSyCPLKxIElAG_cjDfeR2n_2EPrFzvTPPs70
        /// </summary>
        /// <param name="subjectAddress">Should be formatted for querystring (e.g., 549+Muskmelon+Way,+Saratoga+Springs+UT+84045)
        /// </param>
        /// <param name="destinationAddresses">Should be formatted for querystring (e.g., 765+E+Gordon+Ave,+Layton,+UT+84041).  Multiple addresses delimited by pipe (e.g., 325+E+Gordon+Ave,+Layton,+UT+84041|765+E+Gordon+Ave,+Layton,+UT+84041)</param>
        /// <returns></returns>
        private string GetDistanceFromSubjectGoogle(string subjectAddress, string destinationAddresses)
        {
            string apiKey = "AIzaSyCPLKxIElAG_cjDfeR2n_2EPrFzvTPPs70";
            string url = "https://maps.googleapis.com/maps/api/distancematrix/json?units=imperial" +
                                                                        "&origins=" + subjectAddress +
                                                                        "&destinations=" + destinationAddresses +
                                                                        "&key=" + apiKey;

            var request = WebRequest.Create(url) as HttpWebRequest;
            using (var response = request.GetResponse() as HttpWebResponse)
            {
                if (response.StatusCode != HttpStatusCode.OK)
                {
                    throw new Exception(String.Format(
                                    "Server error (HTTP {0}: {1}).",
                                    response.StatusCode,
                                    response.StatusDescription));
                }

                var resultJson = new StreamReader(response.GetResponseStream()).ReadToEnd();

                if (!string.IsNullOrEmpty(resultJson))
                {
                    return resultJson;
                    ////var resultObject = JObject.Parse(resultString);
                    ////_mapViewModel.Comp1DistanceFromSubject = resultObject["rows"].FirstOrDefault()["elements"][0]["distance"]["text"].Value<string>();
                    ////_mapViewModel.Comp2DistanceFromSubject = resultObject["rows"][0]["elements"][1]["distance"]["text"].Value<string>();

                    ////var items = new List<string>();
                    ////foreach (var item in resultObject["rows"].FirstOrDefault()["elements"])
                    ////{
                    ////    items.Add(item["distance"]["text"].Value<string>());
                    ////}

                    //dynamic elements = (JObject.Parse(resultJson) as dynamic).rows[0].elements;

                    //var distances = new List<string>();
                    //foreach(var element in elements)
                    //{
                    //    distances.Add(element.distance.text.Value);
                    //}
                    //_mapViewModel.Comp1DistanceFromSubject = distances[0];
                    //_mapViewModel.Comp2DistanceFromSubject = distances[1];
                }
            }
            return string.Empty;

        }

        private string FormatAddressesForGoogleDistance(List<Address> addresses)
        {
            var sb = new StringBuilder();
            foreach (var address in addresses)
            {
                if (addresses.IndexOf(address) != 0)
                {
                    sb.Append("|");
                }
                sb.Append(address.AddressLine1.Replace(" ", "+")).Append(",");
                sb.Append(address.AddressLine2.Replace(" ", "+"));
            }


            return sb.ToString();
        }
        #endregion

        //private string GetDistanceFromSubjectBing(string subjectAddress, string destinationAddress)
        //{

        //}

        #endregion

    }

    public class MapViewModel : INotifyPropertyChanged
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

        #region Interface Implementation

        public event PropertyChangedEventHandler PropertyChanged;

        protected void OnPropertyChanged(string propertyName)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }
        }

        #endregion

    }

    public class Address : INotifyPropertyChanged
    {

        #region Fields

        private string _addressLine1;
        private string _addressLine2;

        #endregion

        #region Properties

        public string AddressLine1
        {
            get { return _addressLine1; }
            set
            {
                if (value == _addressLine1) return;

                _addressLine1 = value;
                OnPropertyChanged("AddressLine1");
            }
        }

        public string AddressLine2
        {
            get { return _addressLine2; }
            set
            {
                if (value == _addressLine2) return;

                _addressLine2 = value;
                OnPropertyChanged("AddressLine2");
            }
        }

        #endregion

        #region Interface Implementation

        public event PropertyChangedEventHandler PropertyChanged;

        protected void OnPropertyChanged(string propertyName)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }
        }

        #endregion

    }

}
