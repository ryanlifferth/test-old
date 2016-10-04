
using Ryan.AddressUtility.Models;

namespace Ryan.AddressUtility.Interfaces
{
    public interface IAddressVerification
    {

        /// <summary>
        ///     Standardizes an address to USPS address format with address parts
        /// </summary>
        /// <param name="addressLine1"></param>
        /// <param name="addressLine2"></param>
        /// <param name="city"></param>
        /// <param name="stateCode"></param>
        /// <param name="zip"></param>
        /// <param name="county"></param>
        /// <returns></returns>
        Address VerifyAddress(string addressLine1, string addressLine2, string city, string stateCode, string zip, string county);

    }
}
