using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ryan.CardReader.Models
{
    public class Address : IDataErrorInfo
    {

        public string AddressLine1 { get; set; }
        public string AddressLine2 { get; set; }
        public string City { get; set; }
        public string State { get; set; }
        public string Zip { get; set; }


        public bool IsValid { get; set; }

        public string Error { get; }
        
        public string this[string propertyName]
        {
            get
            {
                if (propertyName == "AddressLine2") return string.Empty;

                if (propertyName.GetType() == typeof(string))
                {
                    if (string.IsNullOrEmpty(this.GetType().GetProperty(propertyName).GetValue(this, null)?.ToString()))
                    {
                        IsValid = false;
                        return propertyName + " cannot be empty.";
                    }
                }
                IsValid = true;
                return string.Empty;
            }
        }

    }
}
