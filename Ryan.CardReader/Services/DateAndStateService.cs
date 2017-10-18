using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ryan.CardReader.Services
{
    public class DateAndStateService
    {

        public static List<State> GetStates()
        {
            return new List<State>
            {
                new State { StateCode = "AL", StateName ="Alabama" },
                new State { StateCode = "AK", StateName ="Alaska" },
                new State { StateCode = "AZ", StateName ="Arizona" },
                new State { StateCode = "AR", StateName ="Arkansas" },
                new State { StateCode = "CA", StateName ="California" },
                new State { StateCode = "CO", StateName ="Colorado" },
                new State { StateCode = "CT", StateName ="Connecticut" },
                new State { StateCode = "DE", StateName ="Delaware" },
                new State { StateCode = "DC", StateName ="District of Columbia" },
                new State { StateCode = "FL", StateName ="Florida" },
                new State { StateCode = "GA", StateName ="Georgia" },
                new State { StateCode = "HI", StateName ="Hawaii" },
                new State { StateCode = "ID", StateName ="Idaho" },
                new State { StateCode = "IL", StateName ="Illinois" },
                new State { StateCode = "IN", StateName ="Indiana" },
                new State { StateCode = "IA", StateName ="Iowa" },
                new State { StateCode = "KS", StateName ="Kansas" },
                new State { StateCode = "KY", StateName ="Kentucky" },
                new State { StateCode = "LA", StateName ="Louisiana" },
                new State { StateCode = "ME", StateName ="Maine" },
                new State { StateCode = "MD", StateName ="Maryland" },
                new State { StateCode = "MA", StateName ="Massachusetts" },
                new State { StateCode = "MI", StateName ="Michigan" },
                new State { StateCode = "MN", StateName ="Minnesota" },
                new State { StateCode = "MS", StateName ="Mississippi" },
                new State { StateCode = "MO", StateName ="Missouri" },
                new State { StateCode = "MT", StateName ="Montana" },
                new State { StateCode = "NE", StateName ="Nebraska" },
                new State { StateCode = "NV", StateName ="Nevada" },
                new State { StateCode = "NH", StateName ="New Hampshire" },
                new State { StateCode = "NJ", StateName ="New Jersey" },
                new State { StateCode = "NM", StateName ="New Mexico" },
                new State { StateCode = "NY", StateName ="New York" },
                new State { StateCode = "NC", StateName ="North Carolina" },
                new State { StateCode = "ND", StateName ="North Dakota" },
                new State { StateCode = "OH", StateName ="Ohio" },
                new State { StateCode = "OK", StateName ="Oklahoma" },
                new State { StateCode = "OR", StateName ="Oregon" },
                new State { StateCode = "PA", StateName ="Pennsylvania" },
                new State { StateCode = "RI", StateName ="Rhode Island" },
                new State { StateCode = "SC", StateName ="South Carolina" },
                new State { StateCode = "SD", StateName ="South Dakota" },
                new State { StateCode = "TN", StateName ="Tennessee" },
                new State { StateCode = "TX", StateName ="Texas" },
                new State { StateCode = "UT", StateName ="Utah" },
                new State { StateCode = "VT", StateName ="Vermont" },
                new State { StateCode = "VA", StateName ="Virginia" },
                new State { StateCode = "WA", StateName ="Washington" },
                new State { StateCode = "WV", StateName ="West Virginia" },
                new State { StateCode = "WI", StateName ="Wisconsin" },
                new State { StateCode = "WY", StateName = "Wyoming" }
            };
        }

        public static List<string> GetStateCodes()
        {
            return new List<string>
            {
                "AL",
                "AK",
                "AZ",
                "AR",
                "CA",
                "CO",
                "CT",
                "DE",
                "DC",
                "FL",
                "GA",
                "HI",
                "ID",
                "IL",
                "IN",
                "IA",
                "KS",
                "KY",
                "LA",
                "ME",
                "MD",
                "MA",
                "MI",
                "MN",
                "MS",
                "MO",
                "MT",
                "NE",
                "NV",
                "NH",
                "NJ",
                "NM",
                "NY",
                "NC",
                "ND",
                "OH",
                "OK",
                "OR",
                "PA",
                "RI",
                "SC",
                "SD",
                "TN",
                "TX",
                "UT",
                "VT",
                "VA",
                "WA",
                "WV",
                "WI",
                "WY"
            };
        }

        public static List<Month> GetMonths()
        {
            return new List<Month>
            {
                new Month { Value = 1, MonthName = "1 - Jan" },
                new Month { Value = 2, MonthName = "2 - Feb" },
                new Month { Value = 3, MonthName = "3 - Mar" },
                new Month { Value = 4, MonthName = "4 - Apr" },
                new Month { Value = 5, MonthName = "5 - May" },
                new Month { Value = 6, MonthName = "6 - Jun" },
                new Month { Value = 7, MonthName = "7 - Jul" },
                new Month { Value = 8, MonthName = "8 - Aug" },
                new Month { Value = 9, MonthName = "9 - Sep" },
                new Month { Value = 10, MonthName = "10 - Oct" },
                new Month { Value = 11, MonthName = "11 - Nov" },
                new Month { Value = 12, MonthName = "12 - Dec" }
            };
        }

        public static List<int> GetMonthNums()
        {
            return new List<int>
            {
                1,2,3,4,5,6,7,8,9,10,11,12
            };
        }

        public static List<int> GetYears()
        {
            return new List<int>
            {
                2017,
                2018,
                2019,
                2020,
                2021,
                2022,
                2023,
                2024,
                2025,
                2026,
                2027
            };
        }

    }

    public class State
    {
        public string StateCode { get; set; }
        public string StateName { get; set; }
    }

    public class Month
    {
        public int Value { get; set; }
        public string MonthName { get; set; }
    }

}
