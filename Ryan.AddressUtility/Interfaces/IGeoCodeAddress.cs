using Ryan.AddressUtility.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ryan.AddressUtility.Interfaces
{
    public interface IGeocodeAddress
    {

        /// <summary>
        ///     Standardizes an address to USPS address format with address parts
        /// </summary>
        /// <param name="address"></param>
        /// <returns></returns>
        Address GeocodeAddress(Address address);

        /// <summary>
        ///     Standardizes and address to USPS and lat/lon, but returns decision information
        ///     and confidence to allow user input on response
        /// </summary>
        /// <param name="address"></param>
        /// <returns></returns>
        GeoCodeResponse GeocodeAddressWithDecisionInfo(Address address);

        /// <summary>
        ///     Standardizes a list of addresses to USPS address format with address parts
        /// </summary>
        /// <param name="addresses"></param>
        /// <returns></returns>
        List<Address> GeocodeAddresses(List<Address> addresses);


    }
}
