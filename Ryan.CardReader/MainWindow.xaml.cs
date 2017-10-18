using MTLIB;
using MTSCRANET;
using Ryan.CardReader.Data;
using Ryan.CardReader.Models;
using Ryan.CardReader.Services;
using Ryan.CardReader.ViewModels;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Ryan.CardReader
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {

        #region Fields

        MainPageViewModel _mainPageViewModel;
        List<Attendee> _attendees;

        private MTSCRA m_SCRA;
        private MTConnectionState m_connectionState;
        private MTConnectionType m_connectionType;
        private List<MTLIB.MTDeviceInformation> _devices;

        #endregion

        public MainWindow()
        {
            InitializeComponent();

            var attendeesXml = UserContext.GetAttendees();
            _attendees = attendeesXml.Descendants("Attendee")
                                     .Select(x => new Attendee
                                     {
                                         FirstName = (string)x.Element("FirstName"),
                                         LastName = (string)x.Element("LastName"),
                                         FullName = (string)x.Element("FullName"),
                                         Email = (string)x.Element("Email"),
                                         Phone = (string)x.Element("Phone"),
                                         CompanyName = (string)x.Element("CompanyName"),
                                         Address = new Address
                                         {
                                             AddressLine1 = (string)x.Element("Address").Element("AddressLine1"),
                                             City = (string)x.Element("Address").Element("City"),
                                             State = (string)x.Element("Address").Element("State"),
                                             Zip = (string)x.Element("Address").Element("Zip")
                                         }
                                     })
                                     .ToList();

            //_attendeeNames = _attendees.Select(x => x.FullName);

            _mainPageViewModel = new MainPageViewModel()
            {
                Title = "DataMaster Registration",
                Attendees = _attendees
            };
            _mainPageViewModel.SelectedProduct = _mainPageViewModel.ProductList.FirstOrDefault(x => x.ProductId == 7);
            this.DataContext = _mainPageViewModel;

            m_SCRA = new MTSCRA();
            m_SCRA.OnDeviceList += OnDeviceList;
            m_SCRA.OnDeviceConnectionStateChanged += OnDeviceConnectionStateChanged;
            //m_SCRA.OnCardDataState += OnCardDataStateChanged;
            m_SCRA.OnDataReceived += OnDataReceived;

            ConnectSwiper();

            //Name.KeyUp += HandleNameKeyUp;

            //var cardNumber = "3456123434561234";
            //var masked = MaskCardForDisplay(cardNumber);
        }


        #region Methods

        #region MagTek Methods

        protected void OnDeviceList(object sender, MTLIB.MTConnectionType connectionType, List<MTLIB.MTDeviceInformation> deviceList)
        {
            updateDeviceList(deviceList);
        }

        protected void updateDeviceList(List<MTLIB.MTDeviceInformation> deviceList)
        {
            try
            {
                foreach (var device in deviceList)
                {
                    if (_devices == null) _devices = new List<MTDeviceInformation>();
                    _devices.Add(device);
                }

                //if (DeviceAddressCB.Dispatcher.CheckAccess())
                //{
                //    DeviceAddressCB.Items.Clear();

                //    if (deviceList.Count > 0)
                //    {
                //        foreach (var device in deviceList)
                //        {
                //            DeviceAddressCB.Items.Add(device);
                //        }

                //        DeviceAddressCB.Visibility = Visibility.Visible;

                //        DeviceAddressCB.SelectedIndex = 0;
                //    }

                //    DeviceAddressCB.IsEnabled = true;
                //}
                //else
                //{
                //    OutputTextBox.Dispatcher.BeginInvoke(new deviceListDispatcher(updateDeviceList),
                //                                            System.Windows.Threading.DispatcherPriority.Normal,
                //                                            new object[] { deviceList });
                //}
            }
            catch (Exception ex)
            {
            }

        }

        protected void OnDeviceConnectionStateChanged(object sender, MTLIB.MTConnectionState state)
        {
            UpdateState(state);
        }

        private void UpdateState(MTConnectionState state)
        {
            m_connectionState = state;

            try
            {
                //if (OutputTextBox.Dispatcher.CheckAccess())
                //{
                switch (state)
                {
                    case MTConnectionState.Connecting:
                        // TODO: Do something to indicate that it is connecting
                        //sendToDisplay("[Connecting....]");
                        //displayDeviceInformation();
                        Console.WriteLine("Connecting...");
                        break;
                    case MTConnectionState.Connected:
                        // TODO: Do something to indicate that it is connected
                        //sendToDisplay("[Connected]");
                        //requestEMVMessageFormat();
                        Console.WriteLine("Connected...");
                        break;
                    case MTConnectionState.Disconnecting:
                        // TODO: Do something to indicate that it is disconnecting
                        //sendToDisplay("[Disconnecting....]");
                        Console.WriteLine("Disconnecting...");
                        break;
                    case MTConnectionState.Disconnected:
                        // TODO: Do something to indicate that it is disconnected
                        //sendToDisplay("[Disconnected]");
                        Console.WriteLine("Disconnected...");
                        break;
                }
                //}
                //else
                //{
                //    OutputTextBox.Dispatcher.BeginInvoke(new updateStateDispatcher(updateState),
                //                                    System.Windows.Threading.DispatcherPriority.Normal,
                //                                    new object[] { state });
                //}
            }
            catch (Exception ex)
            {
            }

        }

        protected void OnDataReceived(object sender, IMTCardData cardData)
        {
            //clearDisplay();

            //sendToDisplay("[Raw Data]");
            //sendToDisplay(m_SCRA.getResponseData());
            var responseData = m_SCRA.getResponseData();

            //sendToDisplay("[Card Data]");
            //sendToDisplay(getCardInfo());
            var cardInfo = getCardInfo();
            var trackOne = m_SCRA.getTrack1();
            ParseCardData(trackOne);

            //sendToDisplay("[TLV Payload]");
            //sendToDisplay(cardData.getTLVPayload());
            var tlvPayload = cardData.getTLVPayload();
        }

        public string getCardInfo()
        {
            string cardData = "";

            cardData += string.Format("SDK.Version={0}\n", m_SCRA.getSDKVersion());

            cardData += formatStringIfNotEmpty("TLV.Version={0}\n", m_SCRA.getTLVVersion());

            cardData += formatStringIfNotEmpty("Response.Type={0}\n", m_SCRA.getResponseType());

            cardData += string.Format("Tracks.Masked={0}\n", m_SCRA.getMaskedTracks());
            cardData += string.Format("Track1.Encrypted={0}\n", m_SCRA.getTrack1());
            cardData += string.Format("Track2.Encrypted={0}\n", m_SCRA.getTrack2());
            cardData += string.Format("Track3.Encrypted={0}\n", m_SCRA.getTrack3());
            cardData += string.Format("Track1.Masked={0}\n", m_SCRA.getTrack1Masked());
            cardData += string.Format("Track2.Masked={0}\n", m_SCRA.getTrack2Masked());
            cardData += string.Format("Track3.Masked={0}\n", m_SCRA.getTrack3Masked());
            string mpData = m_SCRA.getMagnePrint();
            cardData += string.Format("MagnePrint.Encrypted={0}\n", mpData);
            cardData += string.Format("MagnePrint.Length={0} bytes\n", (mpData.Length / 2));
            cardData += string.Format("MagnePrint.Status={0}\n", m_SCRA.getMagnePrintStatus());
            cardData += string.Format("Device.Serial={0}\n", m_SCRA.getDeviceSerial());
            cardData += string.Format("Session.ID={0}\n", m_SCRA.getSessionID());
            cardData += string.Format("KSN={0}\n", m_SCRA.getKSN());

            if (m_SCRA.getSwipeCount() >= 0)
            {
                cardData += string.Format("Swipe.Count={0}\n", m_SCRA.getSwipeCount());
            }

            cardData += formatStringIfNotEmpty("Cap.MagnePrint={0}\n", m_SCRA.getCapMagnePrint());
            cardData += formatStringIfNotEmpty("Cap.MagnePrintEncryption={0}\n", m_SCRA.getCapMagnePrintEncryption());

            cardData += formatStringIfNotEmpty("Cap.MagStripeEncryption={0}\n", m_SCRA.getCapMagStripeEncryption());
            cardData += formatStringIfNotEmpty("Cap.MSR={0}\n", m_SCRA.getCapMSR());

            cardData += string.Format("Card.Data.CRC={0}\n", m_SCRA.getCardDataCRC());
            cardData += string.Format("Card.Exp.Date={0}\n", m_SCRA.getCardExpDate());
            cardData += string.Format("Card.IIN={0}\n", m_SCRA.getCardIIN());
            cardData += string.Format("Card.Last4={0}\n", m_SCRA.getCardLast4());
            cardData += string.Format("Card.Name={0}\n", m_SCRA.getCardName());
            cardData += string.Format("Card.PAN={0}\n", m_SCRA.getCardPAN());
            cardData += string.Format("Card.PAN.Length={0}\n", m_SCRA.getCardPANLength());
            cardData += string.Format("Card.Service.Code={0}\n", m_SCRA.getCardServiceCode());

            cardData += formatStringIfNotEmpty("Card.Status={0}\n", m_SCRA.getCardStatus());
            cardData += formatStringIfNotEmpty("Card.EncodeType={0}\n", m_SCRA.getCardEncodeType());

            cardData += formatStringIfNotEmpty("HashCode={0}\n", m_SCRA.getHashCode());

            if (m_SCRA.getDataFieldCount() != 0)
            {
                cardData += string.Format("Data.Field.Count={0}\n", m_SCRA.getDataFieldCount());
            }

            cardData += string.Format("Encryption.Status={0}\n", m_SCRA.getEncryptionStatus());

            cardData += formatStringIfNotEmpty("MagTek.Device.Serial={0}\n", m_SCRA.getMagTekDeviceSerial());

            cardData += string.Format("Track.Decode.Status={0}\n", m_SCRA.getTrackDecodeStatus());
            string tkStatus = m_SCRA.getTrackDecodeStatus();

            string tk1Status = "01";
            string tk2Status = "01";
            string tk3Status = "01";

            if (tkStatus.Length >= 6)
            {
                tk1Status = tkStatus.Substring(0, 2);
                tk2Status = tkStatus.Substring(2, 2);
                tk3Status = tkStatus.Substring(4, 2);

                cardData += string.Format("Track1.Status={0}\n", tk1Status);
                cardData += string.Format("Track2.Status={0}\n", tk2Status);
                cardData += string.Format("Track3.Status={0}\n", tk3Status);
            }

            return cardData;
        }

        /// <summary>
        ///     Parses out card info from Track 1 data feed
        ///     Track one data feed example:  %B4815111122223333^LIFFERTH/RYAN^2107201000000000815700157000000?
        /// </summary>
        /// <param name="track1">Assumed to be Track1.Encrypted</param>
        private void ParseCardData(string cardData)
        {
            if (cardData == "") return;
            var cardNumber = cardData.Substring(2, cardData.IndexOf('^') - 2);
            var name = cardData.Substring(cardData.IndexOf('^') + 1, cardData.LastIndexOf('^') - cardData.IndexOf('^') - 1);
            //var expDate = cardData.Substring(cardData.LastIndexOf('^') + 1, cardData.Length - cardData.LastIndexOf('^') - 2);
            var expDate = cardData.Substring(cardData.LastIndexOf('^') + 1, 4);

            _mainPageViewModel.CreditCardNumberDisplay = MaskCardForDisplay(cardNumber.Trim());
            _mainPageViewModel.CreditCardNumber = cardNumber.Trim();
            _mainPageViewModel.ExpMonth = expDate.Substring(2, 2);
            _mainPageViewModel.ExpYear = expDate.Substring(0, 2);
        }

        private string formatStringIfNotEmpty(string format, string data)
        {
            string result = "";

            if (!string.IsNullOrEmpty(data))
            {
                result = string.Format(format, data);
            }

            return result;
        }


        private void ConnectSwiper()
        {
            if (m_connectionState == MTConnectionState.Disconnected)
            {
                string address = getAddress();

                if (!string.IsNullOrEmpty(address))
                {
                    connect();
                }
                else
                {
                    MessageBoxResult result = MessageBox.Show("Please make sure a device is plugged in and selected before connecting.",
                                            "No Device Selected", MessageBoxButton.OK);
                    if (result != MessageBoxResult.OK)
                    {
                        return;
                    }
                }
            }
        }

        private string getAddress()
        {
            string address = "";

            //MTDeviceInformation devInfo = (MTDeviceInformation)DeviceAddressCB.SelectedValue;
            if (_devices == null || _devices.Count() < 1) m_SCRA.requestDeviceList(MTConnectionType.USB);
            if (_devices == null) return null;
            MTDeviceInformation devInfo = _devices.Any(x => x.Name == "B1A3D3D") ? _devices.FirstOrDefault(x => x.Name == "B1A3D3D") : _devices.FirstOrDefault();

            if (devInfo != null)
            {
                address = devInfo.Address;
            }

            return address;
        }

        private void connect()
        {
            if (m_connectionState == MTConnectionState.Connected)
            {
                return;
            }

            m_connectionType = getConnectionType();

            m_SCRA.setConnectionType(m_connectionType);

            string address = getAddress();

            m_SCRA.setAddress(address);

            resetStates();

            m_uartDataReceived = null;

            m_SCRA.openDevice();
        }

        private MTLIB.MTConnectionType getConnectionType()
        {
            int connectionTypeIndex = 0;

            MTLIB.MTConnectionType connectionType = MTLIB.MTConnectionType.USB;

            switch (connectionTypeIndex)
            {
                case 0:
                    connectionType = MTLIB.MTConnectionType.USB;
                    break;
            }

            return connectionType;
        }

        private enum STATE
        {
            STARTUP,
            GET_EMV_MESSSAGE_FORMAT,
            SETUP_TIME,
            READY
        }

        private STATE mState;
        private bool m_startTransactionActionPending;
        private bool m_turnOffLEDPending;
        private byte[] m_uartDataReceived;


        private void resetStates()
        {
            mState = STATE.STARTUP;

            m_startTransactionActionPending = false;
            m_turnOffLEDPending = false;

            //m_spiStatusRequestPending = false;
            //m_spiDataRequestPending = false;
            //m_spiDataReceived = null;
            //m_spiHeadData = null;
            //m_spiHeadDataLength = 0;
        }

        #endregion

        #endregion

        #region AutoFill Methods

        //private void HandleNameKeyUp(object sender, KeyEventArgs e)
        //{
        //    //var border = (ResultStack.Parent as ScrollViewer).Parent as Border;
        //    //TextBlock b = sender as TextBlock;
        //    //b.Background = Brushes.PeachPuff;

        //    TextBlock currentTextBlock = null;
        //    var childrenCount = ResultStack.Children.Count;
        //    var border = (ResultStack.Parent as ScrollViewer).Parent as Border;

        //    switch (e.Key)
        //    {
        //        case Key.Down:
        //            // See if there is a textblock below in the border/stackpanel
        //            if (_textBlockIndex + 1 == childrenCount)
        //            {
        //                currentTextBlock = ResultStack.Children[_textBlockIndex] as TextBlock;
        //                currentTextBlock.Background = Brushes.PeachPuff;
        //                return;
        //            }

        //            foreach (TextBlock child in ResultStack.Children)
        //            {
        //                var childItem = child as TextBlock;
        //                childItem.Background = Brushes.Transparent;
        //            }

        //            currentTextBlock = ResultStack.Children[_textBlockIndex + 1] as TextBlock;
        //            currentTextBlock.Background = Brushes.PeachPuff;

        //            _textBlockIndex++;
        //            break;
        //        case Key.Up:
        //            // See if there is a textblock below in the border/stackpanel
        //            if (_textBlockIndex == 0)
        //            {
        //                currentTextBlock = ResultStack.Children[_textBlockIndex] as TextBlock;
        //                currentTextBlock.Background = Brushes.PeachPuff;
        //                return;
        //            }

        //            foreach (TextBlock child in ResultStack.Children)
        //            {
        //                var childItem = child as TextBlock;
        //                childItem.Background = Brushes.Transparent;
        //            }

        //            currentTextBlock = ResultStack.Children[_textBlockIndex - 1] as TextBlock;
        //            currentTextBlock.Background = Brushes.PeachPuff;

        //            _textBlockIndex--;
        //            break;
        //        case Key.Enter:
        //            currentTextBlock = ResultStack.Children[_textBlockIndex] as TextBlock;
        //            Name.Text = currentTextBlock.Text;

        //            border.Visibility = Visibility.Collapsed;
        //            SetSelectedAttendee(Name.Text);
        //            ResultStack.Children.Clear();
        //            break;
        //        case Key.Escape:
        //            border.Visibility = Visibility.Collapsed;
        //            ResultStack.Children.Clear();
        //            break;
        //    }
        //}

        //private void TextBox_KeyUp(object sender, KeyEventArgs e)
        //{
        //    bool found = false;
        //    var border = (ResultStack.Parent as ScrollViewer).Parent as Border;

        //    string query = (sender as TextBox).Text;

        //    if (query.Length == 0)
        //    {
        //        // Clear 
        //        ResultStack.Children.Clear();
        //        border.Visibility = Visibility.Collapsed;
        //    }
        //    else
        //    {
        //        border.Visibility = Visibility.Visible;
        //    }

        //    // Clear the list 
        //    ResultStack.Children.Clear();

        //    // Add the result 
        //    foreach (var name in _attendeeNames)
        //    {
        //        if (name.ToLower().StartsWith(query.ToLower()))
        //        {
        //            // The word starts with this... Autocomplete must work 
        //            AddItem(name);
        //            found = true;
        //        }
        //    }

        //    if (!found)
        //    {
        //        ResultStack.Children.Add(new TextBlock() { Text = "No results found." });
        //    }
        //}

        //private void AddItem(string text)
        //{
        //    TextBlock block = new TextBlock();

        //    // Add the text 
        //    block.Text = text;

        //    // A little style... 
        //    block.Margin = new Thickness(2, 3, 2, 3);
        //    block.Cursor = Cursors.Hand;

        //    // Mouse events 
        //    block.MouseLeftButtonUp += (sender, e) =>
        //    {
        //        Name.Text = (sender as TextBlock).Text;
        //        var border = (ResultStack.Parent as ScrollViewer).Parent as Border;
        //        border.Visibility = Visibility.Collapsed;
        //        SetSelectedAttendee(Name.Text);
        //    };

        //    block.MouseEnter += (sender, e) =>
        //    {
        //        TextBlock b = sender as TextBlock;
        //        b.Background = Brushes.PeachPuff;
        //    };

        //    block.MouseLeave += (sender, e) =>
        //    {
        //        TextBlock b = sender as TextBlock;
        //        b.Background = Brushes.Transparent;
        //    };

        //    // Add to the panel 
        //    ResultStack.Children.Add(block);
        //}

        //private void SetSelectedAttendee(string fullName)
        //{
        //    _mainPageViewModel.SelectedAttendee = _attendees.FirstOrDefault(x => x.FullName.ToLowerInvariant() == fullName.ToLowerInvariant());
        //}

        #endregion

        private void NewUser_Click(object sender, RoutedEventArgs e)
        {
            if (AttendeeDropdown.Visibility == Visibility.Visible)
            {
                AttendeeDropdownLabel.Visibility = Visibility.Collapsed;
                AttendeeDropdown.Visibility = Visibility.Collapsed;
                NewUserNameTextBoxes.Visibility = Visibility.Visible;
                ((Button)sender).Content = "<< Go back to list";
                ClearUserData();
            }
            else
            {
                AttendeeDropdownLabel.Visibility = Visibility.Visible;
                AttendeeDropdown.Visibility = Visibility.Visible;
                NewUserNameTextBoxes.Visibility = Visibility.Collapsed;
                ((Button)sender).Content = "User Not in List";
                _mainPageViewModel.SelectedAttendee = null;
            }
        }

        private void ClearUserData_Click(object sender, RoutedEventArgs e)
        {
            ClearUserData();   
        }

        private void ClearUserData()
        {
            _mainPageViewModel.SelectedAttendee = new Attendee();
            _mainPageViewModel.SelectedAttendee.Address = new Address();
            _mainPageViewModel.CreditCardNumber = string.Empty;
            _mainPageViewModel.CreditCardNumberDisplay = string.Empty;
            _mainPageViewModel.ExpMonth = string.Empty;
            _mainPageViewModel.ExpYear = string.Empty;
            _mainPageViewModel.SecurityCode = string.Empty;
        }

        private void CreditCardDisplay_KeyUp(object sender, KeyEventArgs e)
        {
            _mainPageViewModel.CreditCardNumber = ((TextBox)sender).Text;
        }

        private string MaskCardForDisplay(string cardNumber)
        {
            //var cardNumber = "3456123434561234";

            var firstDigits = cardNumber.Substring(0, 6);
            var lastDigits = cardNumber.Substring(cardNumber.Length - 4, 4);

            var requiredMask = new String('X', cardNumber.Length - lastDigits.Length);

            //var maskedString = string.Concat(firstDigits, requiredMask, lastDigits);
            var maskedString = string.Concat(requiredMask, lastDigits);
            var maskedCardNumberWithSpaces = Regex.Replace(maskedString, ".{4}", "$0 ");

            return maskedCardNumberWithSpaces;
        }

    }
}
