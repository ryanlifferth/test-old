using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace Ryan.CardReader.Data
{
    public class UserContext
    {

        #region Fields
        #endregion

        #region Properties
        #endregion

        #region Constructors
        #endregion

        #region Methods

        public static XDocument GetAttendees()
        {
            //var fileLoc = Package.Current.InstalledLocation.Path + @"\Data\Attendees.xml";
            var fileLoc = System.AppDomain.CurrentDomain.BaseDirectory + @"Data\Attendees.xml";

            return XDocument.Load(fileLoc);
        }

        #endregion

    }
}
