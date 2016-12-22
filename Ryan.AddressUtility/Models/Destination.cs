using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ryan.AddressUtility.Models
{
    public class Destination
    {

        public Address Address { get; set; }

        public Address GeocodedAddress { get; set; }

        public string DistanceFromOrigin { get; set; }

        public string Apn { get; set; }

        public string MlsNumber { get; set; }

    }
}
