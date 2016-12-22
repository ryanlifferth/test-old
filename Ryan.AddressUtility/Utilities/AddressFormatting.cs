using Newtonsoft.Json.Linq;
using Ryan.AddressUtility.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;

namespace Ryan.AddressUtility.Utilities
{
    public static class AddressFormatting
    {

        #region Fields
        #endregion

        #region Properties
        #endregion

        #region Methods
        public static Address BuildAddressStringFromParts(Address parcel)
        {
            // Make sure object isn't null and critical pieces of the address exist.
            if (string.IsNullOrEmpty(parcel?.AddressLine1) || string.IsNullOrEmpty(parcel.City) || string.IsNullOrEmpty(parcel.State))
            {
                return null;
            }

            var parcelAddress = new Address
            {
                AddressLine1 = parcel.AddressLine1.Trim(),
                AddressLine2 = parcel.AddressLine2,
                City = parcel.City.Replace(", " + parcel.State, ""),
                State = parcel.State,
                Zip = parcel.Zip
            };

            parcelAddress.FullAddress = BuildFullAddress(parcelAddress);

            return parcelAddress;
        }

        public static string BuildFullAddress(Address parcelAddress)
        {
            if (parcelAddress == null) return null;
            var address = new StringBuilder(parcelAddress.AddressLine1);
            AddItemToAddressStringIfExists(address, parcelAddress.AddressLine2);
            AddItemToAddressStringIfExists(address, parcelAddress.City, ", ");
            AddItemToAddressStringIfExists(address, parcelAddress.State, ", ");
            AddItemToAddressStringIfExists(address, parcelAddress.Zip);

            return address.ToString();
        }

        private static void AddItemToAddressStringIfExists(StringBuilder address, string addressItem, string prefixDelimiter = " ")
        {
            if (!string.IsNullOrEmpty(addressItem))
            {
                address.Append(prefixDelimiter).Append(addressItem.Trim());
            }
        }
        #endregion

    }
}
