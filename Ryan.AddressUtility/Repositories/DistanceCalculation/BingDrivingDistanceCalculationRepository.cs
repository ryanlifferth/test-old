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
using System.Device;
using System.Device.Location;

namespace Ryan.AddressUtility.Repositories
{
    /// <summary>
    ///     http://dev.virtualearth.net/REST/v1/Routes?wp.0=929%20S%20Arbor%20Way,%20Layton,%20Utah%2084041&wp.1=325%20E%20Gordon%20Ave,%20Layton,%20Utah%2084041&du=mi&ra=routeSummariesOnly&key=Alt3gPG8fTsQ4-zl8x68BRF-nNPx9s4ho-U-oAU-tuo7jnKfkZYt_Cx-La0T533b
    ///     https://msdn.microsoft.com/en-us/library/ff701717.aspx
    /// </summary>
    public class BingDrivingDistanceCalculationRepository : IDrivingDistanceCalculation
    {

        #region Interface Implementations (IDistanceCalculation)

        public DistanceResponse CalculateDistances(string originAddress, List<Destination> destinationAddresses)
        {
            if (string.IsNullOrEmpty(originAddress) || !destinationAddresses.Any())
            {
                return new DistanceResponse { StatusCode = 1, StatusMessage = "Origin Address and/or Destination Address(es) are missing." };
            }

            var destinations = new List<Destination>();

            foreach (var destination in destinationAddresses)
            {
                var jsonResponse = GetDistanceFromSubjectBing(originAddress.Replace(" ", "%20"), destination.Address.FullAddress.Replace(" ", "%20"));

                // Parse the response
                dynamic resources = (JObject.Parse(jsonResponse) as dynamic).resourceSets[0].resources[0];
                //DistanceFromOrigin = element.distance.text.Value
                destinations.Add(new Destination
                {
                    //TODO:Fix so that we put it into the correct destination address
                    Address = new Address { FullAddress = destination.Address.FullAddress },
                    DistanceFromOrigin = Math.Round(resources.travelDistance.Value, 1) == 1.0 ?
                                            Math.Round(resources.travelDistance.Value, 1) + " mile" :
                                            Math.Round(resources.travelDistance.Value, 1) + " miles"
                });
            }

            return new DistanceResponse
            {
                StatusCode = 0,
                StatusMessage = "Success",
                OriginAddress = new Address { FullAddress = originAddress },
                Destinations = destinations
            };
        }

        #endregion

        #region Methods

        private string GetDistanceFromSubjectBing(string subjectAddress, string destinationAddress)
        {
            string apiKey = "Alt3gPG8fTsQ4-zl8x68BRF-nNPx9s4ho-U-oAU-tuo7jnKfkZYt_Cx-La0T533b";
            string url = "http://dev.virtualearth.net/REST/v1/Routes?" +
                                            "&wp.0=" + subjectAddress +
                                            "&wp.1=" + destinationAddress +
                                            "&du=mi" + // request in miles
                                            "&ra=routeSummariesOnly" +  // summary info only in response
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

        #endregion

    }
}
