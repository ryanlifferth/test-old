using System;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using Ryan.AddressUtility.Interfaces;
using Newtonsoft.Json.Linq;
using Ryan.AddressUtility.Models;

namespace Ryan.AddressUtility.Repositories
{
    /// <summary>
    ///     SmartyStreets Verify Address API.  
    ///     Subscription based service based on daily usage (see https://smartystreets.com/pricing for pricing)
    ///     See https://smartystreets.com/docs/address for more information
    /// </summary>
    public class SmartyStreetsAddressVerificationRepository : IAddressVerification
    {

        #region Methods

        #region IAddressVerification Implementation

        /// <summary>
        ///     Uses SmartyStreets' address verification REST service to validate the address
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
            // Validate that the minimum input params have been set (street + city + state OR street + zip)
            if (ValidateSearchCriteria(addressLine1, city, stateCode, zip) == false)
            {
                throw new ArgumentException("Missing minimum requirements for Smarty Streets Address Validation search service.  Minimum requirements are street + city + state OR street + zipcode.");
            }

            var searchParams = FormatAddressStringForSearch(addressLine1, city, stateCode, zip);
            return SmartyStreetAddressVerification(searchParams);
        }

        #endregion

        #region Private

        /// <summary>
        ///     Validates the search/input paramaters for the Smarty Streets Verify Address service.
        ///     The minimum input params are: street + city + state OR street + zip
        /// </summary>
        /// <param name="addressLine1"></param>
        /// <param name="city"></param>
        /// <param name="stateCode"></param>
        /// <param name="zip"></param>
        /// <returns></returns>
        private bool ValidateSearchCriteria(string addressLine1, string city, string stateCode, string zip)
        {
            if ((string.IsNullOrEmpty(addressLine1) == false && string.IsNullOrEmpty(city) == false && string.IsNullOrEmpty(stateCode) == false) ||
                (string.IsNullOrEmpty(addressLine1) == false))
            {
                return true;
            }
            return false;
        }

        /// <summary>
        ///     Formats the input params to match SmartyStreets query format
        ///     example:  street=549+Muskmelon+Way&city=Saratoga+Springs&state=UT&zipcode=84045
        /// </summary>
        /// <param name="addressLine1"></param>
        /// <param name="city"></param>
        /// <param name="stateCode"></param>
        /// <param name="zip"></param>
        /// <returns></returns>
        private string FormatAddressStringForSearch(string addressLine1, string city, string stateCode, string zip)
        {
            var addressSearchString = new StringBuilder("?street=");
            addressSearchString.Append(addressLine1.Replace(" ", "+"));
            if (!string.IsNullOrEmpty(city))
            {
                addressSearchString.Append("&city=").Append(city.Replace(" ", "+"));
            }
            if (!string.IsNullOrEmpty(city))
            {
                addressSearchString.Append("&state=").Append(stateCode.Replace(" ", "+"));
            }
            if (!string.IsNullOrEmpty(zip))
            {
                addressSearchString.Append("&zipcode=").Append(stateCode.Replace(" ", "+"));
            }
            addressSearchString.Append("&candidates=5"); // Max results in case search returns multiple results

            return addressSearchString.ToString();
        }

        /// <summary>
        ///     Call SmartyStreets' address verification REST api
        ///     - Calls the service (WebRequest)
        ///     - Get the result back
        ///     - Format the result to ADDRESS data type
        ///     TODO: Refactor to break out into individual methods
        /// </summary>
        /// <param name="searchParams"></param>
        /// <returns></returns>
        private Address SmartyStreetAddressVerification(string searchParams)
        {
            Address address = null;
            string authID = "&auth-id=9a100f46-6b49-474f-9997-47858ce3471b";
            string authToken = "&auth-token=3PCtopLWf2dACjALQszU";
            string url = "https://api.smartystreets.com/street-address" + searchParams + authID + authToken;
            // Sample SmartyStreets GET request - https://api.smartystreets.com/street-address?auth-id=9a100f46-6b49-474f-9997-47858ce3471b&auth-token=3PCtopLWf2dACjALQszU&street=549+Muskmelon+Way&city=Saratoga+Springs&state=UT&zipcode=84045&candidates=10

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

                if (!string.IsNullOrEmpty(resultString) && resultString != "[]\n")
                {
                    var resultJson = JArray.Parse(resultString);
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
        private Address ConvertJsonToAddress(JArray addressJson)
        {
            var address = new Address();
            var addressComponents = addressJson[0].Children<JProperty>().FirstOrDefault(x => x.Name == "components").Value;

            address.HouseNumber = GetValueFromAddressComponent(addressComponents, "primary_number");
            address.StreetPreDirection = GetValueFromAddressComponent(addressComponents, "street_predirection");
            address.StreetName = GetValueFromAddressComponent(addressComponents, "street_name");
            address.StreetSuffix = GetValueFromAddressComponent(addressComponents, "street_postdirection");
            address.City = GetValueFromAddressComponent(addressComponents, "city_name");
            address.State = GetValueFromAddressComponent(addressComponents, "state_abbreviation");
            var zipSuffix = GetValueFromAddressComponent(addressComponents, "plus4_code");
            address.Zip = string.IsNullOrEmpty(zipSuffix) ? GetValueFromAddressComponent(addressComponents, "zipcode") : GetValueFromAddressComponent(addressComponents, "zipcode") + "-" + zipSuffix;


            address.AddressLine1 = addressJson[0].Children<JProperty>().FirstOrDefault(x => x.Name == "delivery_line_1").Value.ToString();

            address.Latitude = addressJson[0].Children<JProperty>()
                                            .FirstOrDefault(x => x.Name == "metadata").Value
                                            .Children<JProperty>()
                                            .FirstOrDefault(x => x.Name == "latitude").Value.ToString();
            address.Longitude = addressJson[0].Children<JProperty>()
                                            .FirstOrDefault(x => x.Name == "metadata").Value
                                            .Children<JProperty>()
                                            .FirstOrDefault(x => x.Name == "longitude").Value.ToString();
            address.County = addressJson[0].Children<JProperty>()
                                            .FirstOrDefault(x => x.Name == "metadata").Value
                                            .Children<JProperty>()
                                            .FirstOrDefault(x => x.Name == "county_name").Value.ToString();
            var fipsCode = addressJson[0].Children<JProperty>()
                                            .FirstOrDefault(x => x.Name == "metadata").Value
                                            .Children<JProperty>()
                                            .FirstOrDefault(x => x.Name == "county_fips").Value.ToString();
            var timeZone = addressJson[0].Children<JProperty>()
                                            .FirstOrDefault(x => x.Name == "metadata").Value
                                            .Children<JProperty>()
                                            .FirstOrDefault(x => x.Name == "time_zone").Value.ToString();
            var dst = addressJson[0].Children<JProperty>()
                                            .FirstOrDefault(x => x.Name == "metadata").Value
                                            .Children<JProperty>()
                                            .FirstOrDefault(x => x.Name == "dst").Value.ToString();

            return address;
        }

        /// <summary>
        ///     Helper method to extract a data item from the JSON results based on the type name
        /// </summary>
        /// <param name="addressComponents"></param>
        /// <param name="typeName"></param>
        /// <returns></returns>
        private string GetValueFromAddressComponent(JToken addressComponents, string typeName)
        {
            return addressComponents.Children<JProperty>().FirstOrDefault(x => x.Name == typeName).Value.ToString();
        }

        #endregion

        #endregion 

    }
}
