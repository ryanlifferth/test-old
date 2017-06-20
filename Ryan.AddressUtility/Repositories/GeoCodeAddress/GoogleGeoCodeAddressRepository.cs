using Newtonsoft.Json.Linq;
using Ryan.AddressUtility.Interfaces;
using Ryan.AddressUtility.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace Ryan.AddressUtility.Repositories
{
    /// <summary>
    ///     Google Maps Geocoding API.  
    ///     Daily transaction limit, but can only use this service if we are also using their maps as part 
    ///     of this transaction.  So for our purposes - we are technically violating their terms of use.
    ///     See https://developers.google.com/maps/documentation/geocoding/ for more information
    /// </summary>
    public class GoogleGeocodeAddressRepository : IGeocodeAddress
    {

        #region Fields
        #endregion

        #region Properties
        #endregion

        #region IAddressVerification Implementation
        public Address GeocodeAddress(Address address)
        {
            // Validate that the minimum input params have been set (street + city + state OR street + zip)
            if (ValidateMinimumAddressParts(address) == false)
            {
                throw new ArgumentException("Missing minimum requirements for Google GeoLocation search service.  Minimum requirements are full address OR street + city + state OR street + zipcode.");
            }

            var jsonResponse = GetGeoCodeResponseFromGoogle(BuildSearchUrl(address));
            var addressJson = JObject.Parse(jsonResponse);
            var geoCodedAddress = new Address();
            var addressComponents = addressJson["results"][0]["address_components"];
            //var addressline1 = addressJson.SelectToken("$..address_components[?(@.long_name == '549')]");

            geoCodedAddress.HouseNumber = GetValueFromAddressComponent(addressComponents, "street_number");
            geoCodedAddress.StreetName = GetValueFromAddressComponent(addressComponents, "route");
            geoCodedAddress.City = GetValueFromAddressComponent(addressComponents, "locality");
            geoCodedAddress.State = GetValueFromAddressComponent(addressComponents, "administrative_area_level_1");
            geoCodedAddress.County = GetValueFromAddressComponent(addressComponents, "administrative_area_level_2");
            var zipSuffix = GetValueFromAddressComponent(addressComponents, "postal_code_suffix");
            geoCodedAddress.Zip = string.IsNullOrEmpty(zipSuffix) ? GetValueFromAddressComponent(addressComponents, "postal_code") : GetValueFromAddressComponent(addressComponents, "postal_code") + "-" + zipSuffix;

            if (!string.IsNullOrEmpty(geoCodedAddress.HouseNumber) &&
                !string.IsNullOrEmpty(geoCodedAddress.StreetName))
            {
                geoCodedAddress.AddressLine1 = geoCodedAddress.HouseNumber + " " + geoCodedAddress.StreetName;
            }

            geoCodedAddress.Latitude = (string)addressJson["results"][0]["geometry"]["location"]["lat"];
            geoCodedAddress.Longitude = (string)addressJson["results"][0]["geometry"]["location"]["lng"];


            return geoCodedAddress;
        }
        
        public GeoCodeResponse GeocodeAddressWithDecisionInfo(Address address)
        {
            throw new NotImplementedException();
        }
        
        public List<Address> GeocodeAddresses(List<Address> addresses)
        {
            var geoCodedAddresses = new List<Address>();
            foreach (var address in addresses)
            {
                geoCodedAddresses.Add(GeocodeAddress(address));
            }
            return geoCodedAddresses;
        }
        #endregion

        #region Methods

        private string GetGeoCodeResponseFromGoogle(string url)
        {
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

        private string BuildSearchUrl(Address address)
        {
            // Examples
            // Sample Google GET request - https://maps.googleapis.com/maps/api/geocode/json?address=549+Muskmelon+Way,+Saratoga+Springs,+UT&key=AIzaSyCPLKxIElAG_cjDfeR2n_2EPrFzvTPPs70
            string apiKey = "AIzaSyCPLKxIElAG_cjDfeR2n_2EPrFzvTPPs70";
            StringBuilder url = new StringBuilder("https://maps.googleapis.com/maps/api/geocode/json?");
            url.Append(BuildAddressParameter(address));
            url.Append("&key=" + apiKey);

            return url.ToString();
        }

        private string BuildAddressParameter(Address address)
        {
            StringBuilder param = new StringBuilder("address=");
            if ((string.IsNullOrEmpty(address.FullAddress) == false))
            {
                param.Append(address.FullAddress.Replace(" ", "+"));
            }
            else
            {
                if (!string.IsNullOrEmpty(address.AddressLine1)) param.Append(address.AddressLine1.Replace(" ", "+")).Append(",+");
                if (!string.IsNullOrEmpty(address.City)) param.Append(address.City.Replace(" ", "+")).Append(",+");
                if (!string.IsNullOrEmpty(address.State)) param.Append(address.State.Replace(" ", "+")).Append(",+");
                if (!string.IsNullOrEmpty(address.Zip)) param.Append(address.Zip.Replace(" ", "+"));

                // TODO: remove any hanging ",+" -> which only occurs if there is no address.Zip value
            }

            return param.ToString();
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

        /// <summary>
        ///     Validates the search/input paramaters for the Address service.
        ///     The minimum input params are: street + city + state OR street + zip
        /// </summary>
        /// <param name="address"></param>
        /// <returns></returns>
        private bool ValidateMinimumAddressParts(Address address)
        {
            if ((string.IsNullOrEmpty(address.FullAddress) == false))
            {
                return true;
            }
            else if ((string.IsNullOrEmpty(address.AddressLine1) == false && string.IsNullOrEmpty(address.City) == false && string.IsNullOrEmpty(address.State) == false))
            {
                return true;
            }
            else if ((string.IsNullOrEmpty(address.AddressLine1) == false && string.IsNullOrEmpty(address.Zip) == false))
            {
                return true;
            }
            return false;
        }

        #endregion

    }
}
