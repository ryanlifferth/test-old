using Ryan.AddressUtility.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ryan.Maps.Win.Models
{
    public class PropertyMap
    {

        #region Fields

        #endregion

        #region Properties

        public Address Address { get; set; }

        public int CompNumber { get; set; }

        public ComparableType CompType { get; set; }

        public string DistanceFromSubject { get; set; }

        #endregion

    }
}
