using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ryan.CardReader.Models
{
    public class Attendee : ModelBase, IDataErrorInfo
    {

        private string _firstName;
        private string _lastName;
        private string _fullName;
        private string _phone;
        private string _email;
        private string _companyName;
        private Address _address;


        public string FirstName
        {
            get { return _firstName; }
            set
            {
                _firstName = value;
                OnPropertyChanged("FirstName");
            }
        }
        public string LastName
        {
            get { return _lastName; }
            set
            {
                _lastName = value;
                OnPropertyChanged("LastName");
            }
        }
        public string FullName
        {
            get { return _fullName; }
            set
            {
                _fullName = value;
                OnPropertyChanged("FullName");
            }
        }
        public string Phone
        {
            get { return _phone; }
            set
            {
                _phone = value;
                OnPropertyChanged("Phone");
            }
        }
        public string Email
        {
            get { return _email; }
            set
            {
                _email = value;
                OnPropertyChanged("Email");
            }
        }
        public string CompanyName
        {
            get { return _companyName; }
            set
            {
                _companyName = value;
                OnPropertyChanged("CopmanyName");
            }
        }
        public Address Address
        {
            get { return _address; }
            set
            {
                _address = value;
                OnPropertyChanged("Address");
            }
        }


        public string Error { get; }

        public bool IsValid { get; set; }

        public string this[string propertyName]
        {
            get
            {
                if (propertyName == "FullName") return string.Empty;

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
