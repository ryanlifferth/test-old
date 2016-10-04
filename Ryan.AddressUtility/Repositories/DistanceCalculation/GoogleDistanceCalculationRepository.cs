using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Ryan.AddressUtility.Interfaces;
using Ryan.AddressUtility.Models;
using System.Net;
using System.IO;
using Newtonsoft.Json.Linq;

namespace Ryan.AddressUtility.Repositories
{
    public class GoogleDistanceCalculationRepository : IDistanceCalculation
    {

        #region Interface Implementations (IDistanceCalculation)

        public DistanceResponse CalculateDistances(string originAddress, List<string> destinationAddresses)
        {
            var googleResponseJson = GetDistanceFromSubjectGoogle(originAddress, FormatAddressesForGoogleDistance(destinationAddresses));

            dynamic elements = (JObject.Parse(googleResponseJson) as dynamic).rows[0].elements;

            var destinations = new List<Destination>();
            foreach (var element in elements)
            {
                var address = (JObject.Parse(googleResponseJson) as dynamic).destination_addresses;
                destinations.Add(new Destination
                {
                    Address = address[elements.IndexOf(element)],
                    DistanceFromOrigin = element.distance.text.Value
                });
            }

            return new DistanceResponse
            {
                OriginAddress = (JObject.Parse(googleResponseJson) as dynamic).origin_addresses[0],
                Destinations = destinations
            };
        }

        #endregion

        #region Methods

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

                }
            }
            return string.Empty;

        }

        private string FormatAddressesForGoogleDistance(List<string> addresses)
        {
            var sb = new StringBuilder();
            foreach (var address in addresses)
            {
                if (addresses.IndexOf(address) != 0)
                {
                    sb.Append("|");
                }
                sb.Append(address.Replace(" ", "+"));
            }


            return sb.ToString();
        }

        #endregion

    }
}
