using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ryan.AddressUtility.Models
{
    public class GeoCodeResponse
    {

        public Address Address { get; set; }

        public List<Address> GeoCodedAddresses { get; set; }

        /// <summary>
        ///     - Bing returns Confidence (Low, Medium, and High) and MatchCodes (Good, Ambiguous, and UpHierarchy)
        ///     - Google returns a "Partial Match" bool - so that if not an exact match will return true.
        ///         -  In the case of Google - need to add some logic (e.g., if partialMatch = true and result count = 1 and type = "premise" or 
        ///            "subpremise" then confidence can be high)
        ///    In both cases the response from either will set the MatchConfidence enum
        /// </summary>
        public MatchDecision MatchConfidence { get; set; }

        public String EntityType { get; set; }
    }

    //public enum MatchConfidence
    //{
    //    High, 
    //    Medium,
    //    Low
    //}

    public enum MatchDecision
    {
        GoodMatch,
        LowConfidenceMatch,
        NeedUserInput,
        NotFound,
        Confused
    }
}
