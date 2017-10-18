using Ryan.CardReader.Commands;
using Ryan.CardReader.Models;
using Ryan.CardReader.Services;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using System.Xml;
using System.Xml.Linq;
using System.Xml.Serialization;

namespace Ryan.CardReader.ViewModels
{
    public class MainPageViewModel : ViewModelBase, IDataErrorInfo
    {

        #region Fields

        private string _title;
        private Attendee _selectedAttendee;
        private List<Attendee> _attendees;
        private string _creditCardNumberDisplay;
        private string _creditCardNumber;
        private string _expMonth;
        private string _expYear;
        List<Product> _productList;
        private Product _selectedProduct;
        private List<string> _stateList;
        private List<int> _monthList;
        private List<int> _yearList;

        #endregion

        #region Properties

        public string Title
        {
            get { return _title; }
            set
            {
                _title = value;
                OnPropertyChanged("Title");
            }
        }

        public Attendee SelectedAttendee
        {
            get { return _selectedAttendee; }
            set
            {
                _selectedAttendee = value;
                OnPropertyChanged("SelectedAttendee");
            }
        }

        public List<Attendee> Attendees
        {
            get { return _attendees; }
            set
            {
                _attendees = value;
                OnPropertyChanged("Attendees");
            }
        }

        public List<Product> ProductList
        {
            get { return _productList; }
            set
            {
                _productList = value;
                OnPropertyChanged("ProductList");
            }
        }

        public Product SelectedProduct
        {
            get { return _selectedProduct; }
            set
            {
                _selectedProduct = value;
                OnPropertyChanged("SelectedProduct");
            }
        }


        private IEnumerable<string> _attendeeNames;

        public IEnumerable<string> AttendeeNames
        {
            get { return _attendeeNames; }
            set { _attendeeNames = value; }
        }


        public string CreditCardNumberDisplay
        {
            get { return _creditCardNumberDisplay; }
            set
            {
                _creditCardNumberDisplay = value;
                OnPropertyChanged("CreditCardNumberDisplay");
            }
        }

        public string CreditCardNumber
        {
            get { return _creditCardNumber; }
            set
            {
                _creditCardNumber = value;
                OnPropertyChanged("CreditCardNumber");
            }
        }

        public string ExpMonth
        {
            get { return _expMonth; }
            set
            {
                _expMonth = value;
                OnPropertyChanged("ExpMonth");
            }
        }

        private string _securityCode;

        public string SecurityCode
        {
            get { return _securityCode; }
            set
            {
                _securityCode = value;
                OnPropertyChanged("SecurityCode");
            }
        }


        public string ExpYear
        {
            get { return _expYear; }
            set
            {
                _expYear = value;
                OnPropertyChanged("ExpYear");
            }
        }

        public bool IsValid { get; set; }

        public List<string> StateList
        {
            get { return _stateList; }
            set
            {
                _stateList = value;
                OnPropertyChanged("StateList");
            }
        }

        public List<int> MonthList
        {
            get { return _monthList; }
            set
            {
                _monthList = value;
                OnPropertyChanged("MonthList");
            }
        }

        public List<int> YearList
        {
            get { return _yearList; }
            set
            {
                _yearList = value;
                OnPropertyChanged("YearList");
            }
        }





        public string Error { get; }

        public string this[string propertyName]
        {
            get
            {
                switch (propertyName)
                {
                    case "SelectedAttendee":
                        if (SelectedAttendee == null)
                        {
                            IsValid = false;
                            return "No user data entered.";
                        }
                        break;
                    case "Title":
                    case "AttendeeNames":
                        // do nothing/not required
                        break;
                    default:
                        if (propertyName.GetType() == typeof(string))
                        {
                            if (string.IsNullOrEmpty(this.GetType().GetProperty(propertyName).GetValue(this, null)?.ToString()))
                            {
                                IsValid = false;
                                return propertyName + " cannot be empty.";
                            }
                        }
                        break;
                }
                IsValid = true;
                return string.Empty;
            }
        }

        #endregion

        #region Constructor
        public MainPageViewModel()
        {
            _productList = ProductService.GetProductList().OrderBy(x => x.ProductName).ToList();
            _stateList = DateAndStateService.GetStateCodes();
            _monthList = DateAndStateService.GetMonthNums();
            _yearList = DateAndStateService.GetYears();
        }

        #endregion

        #region Methods

        private ICommand _registerClicked;
        public ICommand RegisterClicked
        {
            get
            {
                return _registerClicked ?? (_registerClicked = new RelayCommand(param => RegisterClickedExecute(param)));
            }
        }


        private void RegisterClickedExecute(object param)
        {

            //AddressBox.GetBindingExpression(TextBox.TextProperty).UpdateSource();
            if (SelectedAttendee.IsValid && SelectedAttendee.Address.IsValid && this.IsValid)
            {
                // create new payment dynamic object
                var cardInfo = new PaymentInfo
                {
                    CreditCardNumber = this.CreditCardNumber,
                    ExpMonth = this.ExpMonth,
                    ExpYear = this.ExpYear,
                    SecurityCode = this.SecurityCode
                };
                var paymentXml = Serialize<PaymentInfo>(cardInfo);
                // Encrypt the credit card info first
                var encryptedPaymentXml = EncryptionUtility.Encrypt(paymentXml);
                var decryptedPaymentXml = EncryptionUtility.Decrypt(encryptedPaymentXml);

                // Serialize the Attendee into XML
                var attendeeXml = Serialize<Attendee>(SelectedAttendee, new List<string> { "IsValid" });

                AddNewUserToFile(attendeeXml, encryptedPaymentXml);

                MessageBox.Show(SelectedAttendee.FullName + " has been successfully added as a new DataMaster customer", "SUCSSESS!", MessageBoxButton.OK, MessageBoxImage.Information);
                SelectedAttendee = null;
                CreditCardNumber = null;
                CreditCardNumberDisplay = null;
                ExpMonth = null;
                ExpYear = null;
                SecurityCode = null;
                SelectedProduct = ProductList.FirstOrDefault(x => x.ProductId == 7);

            }
        }

        private void AddNewUserToFile(string attendeeXml, string encryptedPaymentXml)
        {
            var fileLoc = System.AppDomain.CurrentDomain.BaseDirectory + @"Data\NewUsers.xml";
            var newUsersDoc = XDocument.Load(fileLoc);
            if (newUsersDoc != null)
            {
                //https://stackoverflow.com/questions/2948255/xml-file-creation-using-xdocument-in-c-sharp
                var newUser = new XElement("NewUser",
                                XDocument.Parse(attendeeXml).Descendants("Attendee").Elements(),
                                new XElement("ProductId", SelectedProduct.ProductId),
                                new XElement("MarketingSource", "6"),
                                new XElement("MarketingSourceOther", "ValuationExpo - Oct17"),
                                new XElement("PaymentInfo", encryptedPaymentXml)
                            );
                newUsersDoc.Root.Add(newUser);

                // Save the changes to the file
                newUsersDoc.Save(fileLoc);

            }
        }

        private string Serialize<T>(T objectToSerialize, List<string> nodesToRemove = null)
        {
            string xml = string.Empty;
            using (var sw = new StringWriter())
            {
                using (var tw = new XmlTextWriter(sw))
                {
                    var ns = new XmlSerializerNamespaces();
                    ns.Add(string.Empty, string.Empty);
                    var serializer = new System.Xml.Serialization.XmlSerializer(objectToSerialize.GetType());
                    serializer.Serialize(tw, objectToSerialize, ns);
                }
                xml = sw.ToString();
            }

            if (nodesToRemove != null)
            {
                var doc = XDocument.Parse(xml);
                nodesToRemove.ForEach(x => doc.Descendants(x).Remove());
                xml = doc.ToString();
            }

            return xml;
        }


        #endregion

    }
}
