using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ryan.Reflection
{
    public class Address
    {

        #region Properties

        public string AddressLine1 { get; set; }
        public string AddressLine2 { get; set; }

        public string HouseNumber { get; set; }
        public string StreetPrefix { get; set; }
        public string StreetName { get; set; }
        public string StreetSuffix { get; set; }
        public string StreetType { get; set; }
        public string UnitNumber { get; set; }

        public string City { get; set; }
        public string State { get; set; }
        public string Zip { get; set; }
        public string County { get; set; }

        public string Latitude { get; set; }
        public string Longitude { get; set; }

        #endregion

    }

    public class Person
    {

        #region Properties

        public string FirstName { get; set; }
        public string LastName { get; set; }

        #endregion

    }

    public class User
    {

        #region Properties

        public string UserName { get; set; }
        public Person Person { get; set; }

        #endregion

    }

    public class StandardPropertyModel
    {

        #region Properties

        public string DataSourceName { get; set; }
        public string MlsNumber { get; set; }

        public Address Address { get; set; }

        public User User { get; set; }

        #endregion

    }

    public class DataSourceTranslationMap
    {

        #region Properties

        public string DataSourceColumnName { get; set; }
        public string StandardPropertyItemName { get; set; }
        public string TranslateToType { get; set; }
        public string Value { get; set; }

        #endregion

    }

}
