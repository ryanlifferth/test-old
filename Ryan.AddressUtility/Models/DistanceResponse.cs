using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ryan.AddressUtility.Models
{
    public class DistanceResponse
    {
        public int StatusCode { get; set; }

        public string StatusMessage { get; set; }

        public Address OriginAddress { get; set; }

        public Address GeocodedOriginAddress { get; set; }

        public string OriginApn { get; set; }

        public string OriginMlsNumber { get; set; }

        public List<Destination> Destinations { get; set; }

    }
}
