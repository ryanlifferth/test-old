using System;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using Ryan.AddressVerification.Interfaces;
using Newtonsoft.Json.Linq;
using Ryan.AddressVerification.Models;

namespace Ryan.AddressVerification.AddressVerificationRepositories
{

    /// <summary>
    ///     Google Maps Geocoding API.  
    ///     Daily transaction limit, but can only use this service if we are also using their maps as part 
    ///     of this transaction.  So for our purposes - we are technically violating their terms of use.
    ///     See https://developers.google.com/maps/documentation/geocoding/ for more information
    /// </summary>
    public class GoogleAddressRepository : IAddressVerification
    {

        #region Methods

        #region IAddressVerification Implementation

        /// <summary>
        ///     Uses Google's GeoCoding REST service to validate the address
        /// </summary>
        /// <param name="addressLine1"></param>
        /// <param name="addressLine2"></param>
        /// <param name="city"></param>
        /// <param name="stateCode"></param>
        /// <param name="zip"></param>
        /// <param name="county"></param>
        /// <returns></returns>
        public Address VerifyAddress(string addressLine1, string addressLine2, string city, string stateCode, string zip, string county)
        {
            var searchParams = FormatAddressStringForSearch(addressLine1, city, stateCode);
            return CallGoogleAddressVerification(searchParams);
        }

        #endregion

        #region Private

        /// <summary>
        ///     Formats the input params to match Google's query format
        ///     example:  address=549+Muskmelon+Way,+Saratoga+Springs,+UT
        /// </summary>
        /// <param name="addressLine1"></param>
        /// <param name="city"></param>
        /// <param name="stateCode"></param>
        /// <returns></returns>
        private string FormatAddressStringForSearch(string addressLine1, string city, string stateCode)
        {
            var addressSearchString = new StringBuilder("?address=");
            addressSearchString.Append(addressLine1.Replace(" ", "+"));
            addressSearchString.Append(",+");
            addressSearchString.Append(city.Replace(" ", "+"));
            addressSearchString.Append(",+");
            addressSearchString.Append(stateCode.Replace(" ", "+"));

            return addressSearchString.ToString();
        }

        /// <summary>
        ///     Call Google's validation/geocode api
        ///     - Calls the service
        ///     - Get the result back
        ///     - Format the result to ADDRESS data type
        ///     TODO: Refactor to break out into individual methods
        /// </summary>
        /// <param name="searchParams"></param>
        /// <returns></returns>
        private Address CallGoogleAddressVerification(string searchParams)
        {
            Address address = null;
            string apiKey = "&key=AIzaSyCPLKxIElAG_cjDfeR2n_2EPrFzvTPPs70";
            string url = "https://maps.googleapis.com/maps/api/geocode/json" + searchParams + apiKey;
            // Sample Google GET request - https://maps.googleapis.com/maps/api/geocode/json?address=549+Muskmelon+Way,+Saratoga+Springs,+UT&key=AIzaSyCPLKxIElAG_cjDfeR2n_2EPrFzvTPPs70

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

                var resultString = new StreamReader(response.GetResponseStream()).ReadToEnd();

                if (!string.IsNullOrEmpty(resultString))
                {
                    var resultJson = JObject.Parse(resultString);
                    address = ConvertJsonToAddress(resultJson);
                }

            }

            return address;
        }

        /// <summary>
        ///     Populates the Address object form the json response (using JSON.net aka Newtonsoft.Json)
        /// </summary>
        /// <param name="addressJson"></param>
        /// <returns></returns>
        private Address ConvertJsonToAddress(JObject addressJson)
        {
            var address = new Address();
            var addressComponents = addressJson["results"][0]["address_components"];
            //var addressline1 = addressJson.SelectToken("$..address_components[?(@.long_name == '549')]");

            address.HouseNumber = GetValueFromAddressComponent(addressComponents, "street_number");
            address.StreetName = GetValueFromAddressComponent(addressComponents, "route");
            address.City = GetValueFromAddressComponent(addressComponents, "locality");
            address.State = GetValueFromAddressComponent(addressComponents, "administrative_area_level_1");
            var zipSuffix = GetValueFromAddressComponent(addressComponents, "postal_code_suffix");
            address.Zip = string.IsNullOrEmpty(zipSuffix) ? GetValueFromAddressComponent(addressComponents, "postal_code") : GetValueFromAddressComponent(addressComponents, "postal_code") + "-" + zipSuffix;

            if (!string.IsNullOrEmpty(address.HouseNumber) &&
                !string.IsNullOrEmpty(address.StreetName))
            {
                address.AddressLine1 = address.HouseNumber + " " + address.StreetName;
            }

            address.Latitude = (string)addressJson["results"][0]["geometry"]["location"]["lat"];
            address.Longitude = (string)addressJson["results"][0]["geometry"]["location"]["lng"];


            return address;
        }

        /// <summary>
        ///     Helper method to extract a data item from the JSON results based on the type name
        ///     Googles response is a bit different as all items are individual nodes
        /// </summary>
        /// <param name="addressComponents"></param>
        /// <param name="typeName"></param>
        /// <returns></returns>
        private string GetValueFromAddressComponent(JToken addressComponents, string typeName)
        {
            // Example nodes:
            /*CITY:
                {
                   "long_name" : "Saratoga Springs",
                   "short_name" : "Saratoga Springs",
                   "types" : [ "locality", "political" ]
                }
                STREET NUMBER
                {
                   "long_name" : "549",
                   "short_name" : "549",
                   "types" : [ "street_number" ]
                }
            */

            if (addressComponents.FirstOrDefault(x => x["types"].Values<string>().Contains(typeName)) == null)
            {
                return string.Empty;
            }

            return addressComponents.FirstOrDefault(x => x["types"].Values<string>().Contains(typeName))["short_name"].Value<string>();
        }

        #endregion

        #endregion

    }
}
