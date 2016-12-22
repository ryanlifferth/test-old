using Ryan.AddressUtility.Interfaces;
using Ryan.AddressUtility.Models;
using Ryan.Maps.Win.ViewModels;
using System;
using System.Collections.Generic;
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

namespace Ryan.Maps.Win.Views
{
    /// <summary>
    /// Interaction logic for ProximityView.xaml
    /// </summary>
    public partial class ProximityView : UserControl
    {

        #region Fields
        private ProximityViewModel _proximityViewModel;
        #endregion

        #region Properties

        #endregion

        #region Constructors
        public ProximityView()
        {
            InitializeComponent();

            _proximityViewModel = new ProximityViewModel
            {
                SubjectAddress = new Address
                {
                    AddressLine1 = "325 E Gordon Ave",
                    City = "Layton",
                    State = "UT",
                    Zip = "84045"                    
                },
                Comp1Address = new Address
                {
                    AddressLine1 = "765 E Gordon Ave",
                    City = "Layton",
                    State = "UT",
                    Zip = "84045"
                },
                Comp2Address = new Address
                {
                    AddressLine1 = "929 E Arbor Way",
                    City = "Layton",
                    State = "UT",
                    Zip = "84045"
                },
                ShowSpinner = false
            };
            _proximityViewModel.SubjectAddress.FullAddress = AddressUtility.Utilities.AddressFormatting.BuildFullAddress(_proximityViewModel.SubjectAddress);
            _proximityViewModel.Comp1Address.FullAddress = AddressUtility.Utilities.AddressFormatting.BuildFullAddress(_proximityViewModel.Comp1Address);
            _proximityViewModel.Comp2Address.FullAddress = AddressUtility.Utilities.AddressFormatting.BuildFullAddress(_proximityViewModel.Comp2Address);

            this.DataContext = _proximityViewModel;
        }
        #endregion

        #region Methods
        public async void ProximityView_Loaded(object sender, RoutedEventArgs e)
        {
            _proximityViewModel.ShowSpinner = true;

            var subjectAddress = Ryan.AddressUtility.Utilities.AddressFormatting.BuildAddressStringFromParts(_proximityViewModel.SubjectAddress).FullAddress;
            var compAddresses = new List<Destination>
            {
                new Destination { Address = _proximityViewModel.Comp1Address },
                new Destination { Address = _proximityViewModel.Comp2Address }
            };

            //var distance = new Ryan.AddressUtility.Repositories.GoogleDistanceCalculationRepository();
            var distance = new Ryan.AddressUtility.Repositories.BingDrivingDistanceCalculationRepository();
            var distances = await GetDistanceFromSubjectAsync(subjectAddress, compAddresses, distance);

            await Task.Delay(3000);

            // Now display the results
            _proximityViewModel.Comp1DistanceFromSubject = distances.Destinations[0].DistanceFromOrigin;
            _proximityViewModel.Comp2DistanceFromSubject = distances.Destinations[1].DistanceFromOrigin;
            _proximityViewModel.ShowSpinner = false;
        }

        private async Task<DistanceResponse> GetDistanceFromSubjectAsync(string subjectAddress, List<Destination> destinationAddresses, IDrivingDistanceCalculation distance)
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
}
