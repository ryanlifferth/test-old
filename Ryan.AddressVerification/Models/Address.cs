using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ryan.AddressVerification.Models
{
    public class Address
    {

        #region Properties

        public string FullAddress { get; set; }
        public string AddressLine1 { get; set; }
        public string AddressLine2 { get; set; }

        public string HouseNumber { get; set; }
        public string StreetPreDirection { get; set; }
        public string StreetName { get; set; }
        public string StreetPostDirection { get; set; }
        public string StreetSuffix { get; set; }
        public string UnitNumber { get; set; }

        public string City { get; set; }
        public string State { get; set; }
        public string Zip { get; set; }
        public string County { get; set; }

        public string Latitude { get; set; }
        public string Longitude { get; set; }

        #endregion

    }
}
