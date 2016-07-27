using Ryan.AddressVerification;
using Ryan.AddressVerification.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace Ryan.DragAndDrop.Controllers
{
    public class AddressVerificationServiceController : ApiController
    {

        [HttpGet]
        [Route("api/addressverificationservice/{address}")]
        public Address Get(string address)
        {
            var addressService = new AddressVerification.AddressVerificationRepositories.SmartyStreetsAddressRepository();
            var responseAddress = addressService.VerifyAddress(address, null, null, null, null, null);

            return responseAddress;
        }

    }
}
