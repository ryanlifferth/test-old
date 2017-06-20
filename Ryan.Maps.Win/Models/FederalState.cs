using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ryan.Maps.Win.Models
{
    public class FederalState
    {
        public string StateName { get; set; }
        public string StateAbbreviation { get; set; }
        public bool IsNonDisclosure { get; set; }
        public List<County> Counties { get; set; }

        public List<FederalState> Get()
        {
            var federalStateList = new List<FederalState>
            {
                new FederalState { StateName = "Alabama", StateAbbreviation = "AL", IsNonDisclosure = false, Counties = GetCountiesByAbbreviation("AL") },
                new FederalState { StateName = "Alaska", StateAbbreviation = "AK", IsNonDisclosure = false, Counties = GetCountiesByAbbreviation("AK") },
                new FederalState { StateName = "Arizona", StateAbbreviation = "AZ", IsNonDisclosure = false, Counties = GetCountiesByAbbreviation("AZ") },
                new FederalState { StateName = "Arkansas", StateAbbreviation = "AR", IsNonDisclosure = false, Counties = GetCountiesByAbbreviation("AR") },
                new FederalState { StateName = "California", StateAbbreviation = "CA", IsNonDisclosure = false, Counties = GetCountiesByAbbreviation("CA") },
                new FederalState { StateName = "Colorado", StateAbbreviation = "CO", IsNonDisclosure = false, Counties = GetCountiesByAbbreviation("CO") },
                new FederalState { StateName = "Connecticut", StateAbbreviation = "CT", IsNonDisclosure = false, Counties = GetCountiesByAbbreviation("CT") },
                new FederalState { StateName = "Delaware", StateAbbreviation = "DE", IsNonDisclosure = false, Counties = GetCountiesByAbbreviation("DE") },
                new FederalState { StateName = "Florida", StateAbbreviation = "FL", IsNonDisclosure = false, Counties = GetCountiesByAbbreviation("FL") },
                new FederalState { StateName = "Georgia", StateAbbreviation = "GA", IsNonDisclosure = false, Counties = GetCountiesByAbbreviation("GA") },
                new FederalState { StateName = "Hawaii", StateAbbreviation = "HI", IsNonDisclosure = false, Counties = GetCountiesByAbbreviation("HI") },
                new FederalState { StateName = "Idaho", StateAbbreviation = "ID", IsNonDisclosure = false, Counties = GetCountiesByAbbreviation("ID") },
                new FederalState { StateName = "Illinois", StateAbbreviation = "IL", IsNonDisclosure = false, Counties = GetCountiesByAbbreviation("IL") },
                new FederalState { StateName = "Indiana", StateAbbreviation = "IN", IsNonDisclosure = false, Counties = GetCountiesByAbbreviation("IN") },
                new FederalState { StateName = "Iowa", StateAbbreviation = "IA", IsNonDisclosure = false, Counties = GetCountiesByAbbreviation("IA") },
                new FederalState { StateName = "Kansas", StateAbbreviation = "KS", IsNonDisclosure = false, Counties = GetCountiesByAbbreviation("KS") },
                new FederalState { StateName = "Kentucky", StateAbbreviation = "KY", IsNonDisclosure = false, Counties = GetCountiesByAbbreviation("KY") },
                new FederalState { StateName = "Louisiana", StateAbbreviation = "LA", IsNonDisclosure = false, Counties = GetCountiesByAbbreviation("LA") },
                new FederalState { StateName = "Maine", StateAbbreviation = "ME", IsNonDisclosure = false, Counties = GetCountiesByAbbreviation("ME") },
                new FederalState { StateName = "Maryland", StateAbbreviation = "MD", IsNonDisclosure = false, Counties = GetCountiesByAbbreviation("MD") },
                new FederalState { StateName = "Massachusetts", StateAbbreviation = "MA", IsNonDisclosure = false, Counties = GetCountiesByAbbreviation("MA") },
                new FederalState { StateName = "Michigan", StateAbbreviation = "MI", IsNonDisclosure = false, Counties = GetCountiesByAbbreviation("MI") },
                new FederalState { StateName = "Minnesota", StateAbbreviation = "MN", IsNonDisclosure = false, Counties = GetCountiesByAbbreviation("MN") },
                new FederalState { StateName = "Mississippi", StateAbbreviation = "MS", IsNonDisclosure = false, Counties = GetCountiesByAbbreviation("MS") },
                new FederalState { StateName = "Missouri", StateAbbreviation = "MO", IsNonDisclosure = false, Counties = GetCountiesByAbbreviation("MO") },
                new FederalState { StateName = "Montana", StateAbbreviation = "MT", IsNonDisclosure = false, Counties = GetCountiesByAbbreviation("MT") },
                new FederalState { StateName = "Nebraska", StateAbbreviation = "NE", IsNonDisclosure = false, Counties = GetCountiesByAbbreviation("NE") },
                new FederalState { StateName = "Nevada", StateAbbreviation = "NV", IsNonDisclosure = false, Counties = GetCountiesByAbbreviation("NV") },
                new FederalState { StateName = "New Hampshire", StateAbbreviation = "NH", IsNonDisclosure = false, Counties = GetCountiesByAbbreviation("NH") },
                new FederalState { StateName = "New Jersey", StateAbbreviation = "NJ", IsNonDisclosure = false, Counties = GetCountiesByAbbreviation("NJ") },
                new FederalState { StateName = "New Mexico", StateAbbreviation = "NM", IsNonDisclosure = false, Counties = GetCountiesByAbbreviation("NM") },
                new FederalState { StateName = "New York", StateAbbreviation = "NY", IsNonDisclosure = false, Counties = GetCountiesByAbbreviation("NY") },
                new FederalState { StateName = "North Carolina", StateAbbreviation = "NC", IsNonDisclosure = false, Counties = GetCountiesByAbbreviation("NC") },
                new FederalState { StateName = "North Dakota", StateAbbreviation = "ND", IsNonDisclosure = false, Counties = GetCountiesByAbbreviation("ND") },
                new FederalState { StateName = "Ohio", StateAbbreviation = "OH", IsNonDisclosure = false, Counties = GetCountiesByAbbreviation("OH") },
                new FederalState { StateName = "Oklahoma", StateAbbreviation = "OK", IsNonDisclosure = false, Counties = GetCountiesByAbbreviation("OK") },
                new FederalState { StateName = "Oregon", StateAbbreviation = "OR", IsNonDisclosure = false, Counties = GetCountiesByAbbreviation("OR") },
                new FederalState { StateName = "Pennsylvania", StateAbbreviation = "PA", IsNonDisclosure = false, Counties = GetCountiesByAbbreviation("PA") },
                new FederalState { StateName = "Rhode Island", StateAbbreviation = "RI", IsNonDisclosure = false, Counties = GetCountiesByAbbreviation("RI") },
                new FederalState { StateName = "South Carolina", StateAbbreviation = "SC", IsNonDisclosure = false, Counties = GetCountiesByAbbreviation("SC") },
                new FederalState { StateName = "South Dakota", StateAbbreviation = "SD", IsNonDisclosure = false, Counties = GetCountiesByAbbreviation("SD") },
                new FederalState { StateName = "Tennessee", StateAbbreviation = "TN", IsNonDisclosure = false, Counties = GetCountiesByAbbreviation("TN") },
                new FederalState { StateName = "Texas", StateAbbreviation = "TX", IsNonDisclosure = false, Counties = GetCountiesByAbbreviation("TX") },
                new FederalState { StateName = "Utah", StateAbbreviation = "UT", IsNonDisclosure = true, Counties = GetCountiesByAbbreviation("UT") },
                new FederalState { StateName = "Vermont", StateAbbreviation = "VT", IsNonDisclosure = false, Counties = GetCountiesByAbbreviation("VT") },
                new FederalState { StateName = "Virginia", StateAbbreviation = "VA", IsNonDisclosure = false, Counties = GetCountiesByAbbreviation("VA") },
                new FederalState { StateName = "Washington", StateAbbreviation = "WA", IsNonDisclosure = false, Counties = GetCountiesByAbbreviation("WA") },
                new FederalState { StateName = "West Virginia", StateAbbreviation = "WV", IsNonDisclosure = false, Counties = GetCountiesByAbbreviation("WV") },
                new FederalState { StateName = "Wisconsin", StateAbbreviation = "WI", IsNonDisclosure = false, Counties = GetCountiesByAbbreviation("WI") },
                new FederalState { StateName = "Wyoming", StateAbbreviation = "WY", IsNonDisclosure = false, Counties = GetCountiesByAbbreviation("WY") }
            };
            return federalStateList;
        }

        public List<County> GetCountiesById(int stateId)
        {
            return new List<County>();
        }

        public List<County> GetCountiesByAbbreviation(string stateAbbreviation)
        {
            var county = new County();
            return county.GetCountiesByStateAbbreviation(stateAbbreviation);
        }
    }
}