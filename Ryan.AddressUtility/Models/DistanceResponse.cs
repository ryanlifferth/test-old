using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ryan.AddressUtility.Models
{
    public class DistanceResponse
    {

        public string OriginAddress { get; set; }

        public List<Destination> Destinations { get; set; }

    }
}
