using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Ryan.AddressUtility.Interfaces;
using Ryan.AddressUtility.Models;
using System;
using System.Collections.Generic;
using System.Device.Location;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace Ryan.AddressUtility.Repositories
{
    /// <summary>
    ///     see https://msdn.microsoft.com/en-us/library/ff701714.aspx for more information
    ///     TODO:  Get terms of use/service/limits/etc.
    /// </summary>
    public class BingGeocodeAddressRepository : IGeocodeAddress
    {

        #region Fields
        #endregion

        #region Properties
        #endregion

        #region IAddressVerification Implementation
        public Address GeocodeAddress(Address address)
        {
            ValidateAddress(address);   // Validate that the minimum input params have been set (street + city + state OR street + zip)

            var jsonResponse = GetGeoCodeResponseFromBing(BuildSearchUrl(address));

            dynamic resources = (JObject.Parse(jsonResponse) as dynamic).resourceSets[0].resources[0];
            var geoCodedAddress = new Address
            {
                FullAddress = resources.name.Value,
                Latitude = resources.point.coordinates[0].Value.ToString(),
                Longitude = resources.point.coordinates[1].Value.ToString(),
                AddressLine1 = resources.address.addressLine,
                City = resources.address.locality,
                State = resources.address.adminDistrict,
                Zip = resources.address.postalCode
            };

            return geoCodedAddress;
        }

        public Address GeocodeByPoint(GeoCoordinate geoCoordination)
        {

            var jsonResponse = GetGeoCodeResponseFromBing(BuildSearchUrl(geoCoordination.Latitude.ToString(), geoCoordination.Longitude.ToString()));

            dynamic resources = (JObject.Parse(jsonResponse) as dynamic).resourceSets[0].resources[0];
            var geoCodedAddress = new Address
            {
                FullAddress = resources.name.Value,
                Latitude = resources.point.coordinates[0].Value.ToString(),
                Longitude = resources.point.coordinates[1].Value.ToString(),
                AddressLine1 = resources.address.addressLine,
                City = resources.address.locality,
                State = resources.address.adminDistrict,
                County = string.IsNullOrEmpty(resources.address.adminDistrict2.ToString()) ? 
                                              string.Empty : 
                                              resources.address.adminDistrict2.ToString().Replace(" Co.", ""),
                Zip = resources.address.postalCode
            };

            return geoCodedAddress;
        }

        public GeoCodeResponse GeocodeAddressWithDecisionInfo(Address address)
        {
            ValidateAddress(address);       // Validate that the minimum input params have been set (street + city + state OR street + zip)

            var jsonResponse = GetGeoCodeResponseFromBing(BuildSearchUrl(address));
            dynamic parsedJson = JObject.Parse(jsonResponse);
            var resources = parsedJson.resourceSets[0].resources;

            var geoCodeResponse = new GeoCodeResponse { Address = address };
            if (resources.Count > 0)
            {
                geoCodeResponse.GeoCodedAddresses = new List<Address>();
                foreach (var resource in resources)
                {
                    geoCodeResponse.GeoCodedAddresses.Add(new Address
                    {
                        //FullAddress = resource.name.Value,
                        FullAddress = resource.address.formattedAddress,
                        Latitude = resource.point.coordinates[0].Value.ToString(),
                        Longitude = resource.point.coordinates[1].Value.ToString(),
                        AddressLine1 = resource.address.addressLine,
                        City = resource.address.locality,
                        County = resource.address.adminDistrict2,
                        State = resource.address.adminDistrict,
                        Zip = resource.address.postalCode
                    });
                }

                if (resources.Count == 1 && resources[0].confidence == "High" && resources[0].entityType == "Address")
                {
                    geoCodeResponse.Address.Latitude = resources[0].point.coordinates[0].Value.ToString();
                    geoCodeResponse.Address.Longitude = resources[0].point.coordinates[1].Value.ToString();
                    geoCodeResponse.MatchConfidence = MatchDecision.GoodMatch;
                }
                else
                {
                    if (resources.Count > 0 && resources[0].confidence != "High")
                    {
                        if (resources[0].confidence != "High" && resources[0].entityType == "Address")
                        {
                            if (resources.Count == 1)
                            {
                                geoCodeResponse.MatchConfidence = MatchDecision.LowConfidenceMatch;
                            }
                            else
                            {
                                geoCodeResponse.MatchConfidence = MatchDecision.NeedUserInput;
                            }
                        }
                        else
                        {
                            if (resources[0].confidence == "High" && resources[0].entityType == "CountryRegion")
                            {
                                geoCodeResponse.MatchConfidence = MatchDecision.NotFound;
                            }
                            else
                            {
                                geoCodeResponse.MatchConfidence = MatchDecision.LowConfidenceMatch;
                            }
                        }
                    }
                    else
                    {
                        geoCodeResponse.MatchConfidence = MatchDecision.NotFound;
                        geoCodeResponse.EntityType = resources[0].entityType;
                    }
                    geoCodeResponse.MatchConfidence = MatchDecision.Confused;
                }
            }
            else
            {
                geoCodeResponse.MatchConfidence = MatchDecision.NotFound;
            }

            return geoCodeResponse;
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

        private string GetGeoCodeResponseFromBing(string url)
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
            // http://dev.virtualearth.net/REST/v1/Locations?CountryRegion=US&adminDistrict=UT&locality=Saratoga%20Springs&postalCode=840456&addressLine=549%20Muskmelon%20Way&key=Alt3gPG8fTsQ4-zl8x68BRF-nNPx9s4ho-U-oAU-tuo7jnKfkZYt_Cx-La0T533b 
            // http://dev.virtualearth.net/REST/v1/Locations?q=549%20Muskmelon%20Way,%20Saratoga%20Springs,%20UT%2084045&key=Alt3gPG8fTsQ4-zl8x68BRF-nNPx9s4ho-U-oAU-tuo7jnKfkZYt_Cx-La0T533b 
            string apiKey = "Alt3gPG8fTsQ4-zl8x68BRF-nNPx9s4ho-U-oAU-tuo7jnKfkZYt_Cx-La0T533b";
            StringBuilder url = new StringBuilder("http://dev.virtualearth.net/REST/v1/Locations?");
            url.Append(BuildAddressParameter(address));
            url.Append("&maxResults=3");
            url.Append("&key=" + apiKey);

            return url.ToString();
        }

        private string BuildSearchUrl(string lat, string lon)
        {
            //http://dev.virtualearth.net/REST/v1/Locations/41.0755475801052,-111.952174819234?maxResults=3&key=Alt3gPG8fTsQ4-zl8x68BRF-nNPx9s4ho-U-oAU-tuo7jnKfkZYt_Cx-La0T533b
            string apiKey = "Alt3gPG8fTsQ4-zl8x68BRF-nNPx9s4ho-U-oAU-tuo7jnKfkZYt_Cx-La0T533b";
            StringBuilder url = new StringBuilder("http://dev.virtualearth.net/REST/v1/Locations/");
            url.Append(lat).Append(",").Append(lon).Append("?");
            url.Append("&maxResults=3");
            url.Append("&key=" + apiKey);

            return url.ToString();
        }

        private string BuildAddressParameter(Address address)
        {
            StringBuilder param = new StringBuilder();
            if ((string.IsNullOrEmpty(address.FullAddress) == false))
            {
                //param.Append("countryRegion=US");
                param.Append("q=").Append(address.FullAddress.Replace(" ", "%20"));
            }
            else
            {
                param.Append("countryRegion=US");
                if (!string.IsNullOrEmpty(address.State)) param.Append("&adminDistrict=").Append(address.State);
                if (!string.IsNullOrEmpty(address.City)) param.Append("&locality=").Append(address.City.Replace(" ", "%20"));
                if (!string.IsNullOrEmpty(address.Zip)) param.Append("&postalCode=").Append(address.Zip.Replace(" ", "%20"));
                if (!string.IsNullOrEmpty(address.AddressLine1)) param.Append("&addressLine=").Append(address.AddressLine1.Replace(" ", "%20"));
            }

            return param.ToString();
        }

        /// <summary>
        ///     Validates the search/input paramaters for the Address service.
        ///     The minimum input params are: street + city + state OR street + zip
        /// </summary>
        /// <param name="addressLine1"></param>
        /// <param name="city"></param>
        /// <param name="stateCode"></param>
        /// <param name="zip"></param>
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

        /// <summary>
        ///     Validates address, but throws exception if not passed
        /// </summary>
        /// <param name="address"></param>
        private void ValidateAddress(Address address)
        {
            // Validate that the minimum input params have been set (street + city + state OR street + zip)
            if (ValidateMinimumAddressParts(address) == false)
            {
                throw new ArgumentException("Missing minimum requirements for Bing GeoLocation search service.  Minimum requirements are full address OR street + city + state OR street + zipcode.");
            }
        }

        #endregion

    }

}
