using System;
using System.IO;
using System.Net;
using System.Text;
using Ryan.AddressUtility.Interfaces;
using Newtonsoft.Json.Linq;
using Ryan.AddressUtility.Models;

namespace Ryan.AddressUtility.Repositories
{
    /// <summary>
    ///     YAddress.net Address verification.  Per-transaction use (see http://www.yaddress.net/Home/Pricing for pricing info)
    ///     API info:  http://www.yaddress.net/Home/WebApi
    ///     REST API - Querystring based for request, returns either JSON or XML
    /// </summary>
    public class YAddressAddressVerificationRepository : IAddressVerification
    {

        #region Methods

        #region IAddressVerification Implementation

        /// <summary>
        ///     Uses YAddress.net's address verification REST service to validate the address
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
            var searchParams = FormatAddressForSearch(addressLine1, city, stateCode, zip);
            return CallYaddressService(searchParams);
        }
        
        #endregion

        #region Private

        /// <summary>
        ///     Formats the input params to match YAddress.net query format
        ///     example:  AddressLine1=549+Muskmelon+Way&AddressLine2=Saratoga+Springs+UT
        /// </summary>
        /// <param name="addressLine1"></param>
        /// <param name="city"></param>
        /// <param name="stateCode"></param>
        /// <param name="zip"></param>
        /// <returns></returns>
        private string FormatAddressForSearch(string addressLine1, string city, string stateCode, string zip)
        {
            var addressSearchString = new StringBuilder("?AddressLine1=");
            addressSearchString.Append(addressLine1.Replace(" ", "+"));
            addressSearchString.Append("&AddressLine2=");
            addressSearchString.Append(city.Replace(" ", "+"));
            addressSearchString.Append("+");
            addressSearchString.Append(stateCode.Replace(" ", "+"));
            if (!string.IsNullOrEmpty(zip))
            {
                addressSearchString.Append("+");
                addressSearchString.Append(zip.Replace(" ", "+"));
            }

            return addressSearchString.ToString();
        }

        /// <summary>
        ///     Call YAddress' address verification REST api
        ///     - Calls the service (WebRequest)
        ///     - Get the result back
        ///     - Format the result to ADDRESS data type
        ///     TODO: Refactor to break out into individual methods
        /// </summary>
        /// <param name="searchParameters"></param>
        /// <returns></returns>
        private Address CallYaddressService(string searchParameters)
        {
            Address address = null;
            string url = "http://www.yaddress.net/api/address" + searchParameters;
            // Sample SmartyStreets GET request - http://www.yaddress.net/api/address?AddressLine1=549+Muskmelon+Way&AddressLine2=Saratoga+Springs+UT

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
                //JObject googleSearch = JObject.Parse(googleSearchText);
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
            address.AddressLine1 = GetValueFromJson(addressJson, "AddressLine1");
            address.AddressLine2 = GetValueFromJson(addressJson, "AddressLine2");
            address.HouseNumber = GetValueFromJson(addressJson, "Number");
            address.StreetPreDirection = GetValueFromJson(addressJson, "PreDir");
            address.StreetName = GetValueFromJson(addressJson, "Street");
            address.StreetSuffix = GetValueFromJson(addressJson, "Suffix");
            address.City = GetValueFromJson(addressJson, "City");
            address.County = GetValueFromJson(addressJson, "County");
            address.State = GetValueFromJson(addressJson, "State");
            var zip4 = GetValueFromJson(addressJson, "Zip4");
            address.Zip = string.IsNullOrEmpty(zip4) == false ? GetValueFromJson(addressJson, "Zip") + "-" + zip4 : GetValueFromJson(addressJson, "Zip");
            address.Latitude = GetValueFromJson(addressJson, "Latitude");
            address.Longitude = GetValueFromJson(addressJson, "Longitude");

            return address;
        }

        /// <summary>
        ///     Helper method to extract a data item from the JSON results based on the type name
        /// </summary>
        /// <param name="addressJson"></param>
        /// <param name="name"></param>
        /// <returns></returns>
        private string GetValueFromJson(JObject addressJson, string name)
        {
            if (addressJson != null &&
                addressJson[name] != null &&
                addressJson[name].Type.ToString().ToLower() == "string" &&
                addressJson[name].ToString().Length > 0) { 
                return addressJson[name].ToString();
            }

            return null;
        }

        #endregion

        #endregion

    }
}
