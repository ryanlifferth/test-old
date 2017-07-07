using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
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

namespace Ryan.Maps.Win.Views
{
    /// <summary>
    /// Interaction logic for BingStreetsideView.xaml
    /// </summary>
    [System.Runtime.InteropServices.ComVisible(true)]
    public partial class BingStreetsideView : UserControl
    {
        public BingStreetsideView()
        {
            InitializeComponent();
            mapViewport.Loaded += MapViewport_Loaded;

        }

        private void MapViewport_Loaded(object sender, RoutedEventArgs e)
        {
            //mapViewport.NavigateToString()
            //Value="/Ryan.Maps.Win;component/Resources/Images/zoomOut.png"
            //"Pack://application:,,,/Resources/Images/DM Logo Blue.ico", UriKind.RelativeOrAbsolute
            var uri = new Uri(@"pack://application:,,,/Resources/Html/Streetside.html");

            var source = Application.GetContentStream(uri).Stream;
            mapViewport.NavigateToStream(source);
            //mapViewport.Navigate("http://www.google.com");

            //mapViewport.ObjectForScripting = new ObjectForScriptingHelper(this);
        }


        //dynamic _streetside;
        public dynamic _streetside { get; set; }
        bool _firstLoad = true;
        private void MyButton_Click(object sender, RoutedEventArgs e)
        {
            /*
            dynamic apiObject = mapViewport.InvokeScript("testApi");
            var testProperty = apiObject.property;
            MessageBox.Show(testProperty);
            apiObject.Method1("Hi Ryan");
            MessageBox.Show(apiObject.Method2(""));  // for some reason when calling a method, the caller in C# won't work unless you have a parameter.  So apiObject.Method2("") works while apiObject.Method2() does not.
            MessageBox.Show(apiObject.Method3("Ryan"));
            */


            //dynamic streetSide = mapViewport.InvokeScript("streetSide");
            //streetSide.loadMapScenario("");

            if (string.IsNullOrEmpty(Address.Text))
            {
                //MessageBox.Show("Please enter an address.", "Validation Error", MessageBoxButton.OK, MessageBoxImage.Warning);
                //return;
                Address.Text = "549 Muskmelon Way, Saratoga Springs, UT 84045";
            }

            var geoCodeRepo = new AddressUtility.Repositories.BingGeocodeAddressRepository();
            //var address = geoCodeRepo.GeoCodeAddress(new AddressUtility.Models.Address { AddressLine1 = "549 Muskmelon Way", City = "Saratoga Springs", State = "Utah", Zip = "84045" });
            //var address = geoCodeRepo.GeocodeAddress(new AddressUtility.Models.Address { FullAddress = "549 Muskmelon Way, Saratoga Springs, UT 84045" });
            var propertyAddress = geoCodeRepo.GeocodeAddress(new AddressUtility.Models.Address { FullAddress = Address.Text });

            if (string.IsNullOrEmpty(propertyAddress?.Longitude) == false && string.IsNullOrEmpty(propertyAddress?.Latitude) == false)
            {
                if (_firstLoad)
                {
                    _streetside = mapViewport.InvokeScript("streetsideMapBing");
                    _streetside.loadMapScenario(propertyAddress.Latitude, propertyAddress.Longitude);
                    _firstLoad = false;
                    //_streetside.loadMapScenario("40.4016203", "-111.9331935");
                }
                else
                {
                    _streetside.updateView(propertyAddress.Latitude, propertyAddress.Longitude);
                }
            }
            else
            {
                MessageBox.Show("Address not found.", "Error", MessageBoxButton.OK, MessageBoxImage.Asterisk);
            }
        }


        private void MapCenter_Click(object sender, RoutedEventArgs e)
        {
            if (_streetside != null)
            {
                MessageBox.Show(_streetside.map._options.center.latitude.ToString());
            }
        }

        private void TestError_Click(object sender, RoutedEventArgs e)
        {
            _streetside = mapViewport.InvokeScript("streetsideMapBing");
            _streetside.noStreetsideFound("");
        }
    }

    //[System.Runtime.InteropServices.ComVisible(true)]
    //public class ObjectForScriptingHelper
    //{
    //    BingStreetsideView _streetsideView;

    //    public ObjectForScriptingHelper(BingStreetsideView streetsideView)
    //    {
    //        _streetsideView = streetsideView;
    //    }

    //    public void InvokeMeFromJavascript()
    //    {
    //        //_streetsideView.mapViewport.Refresh();
    //        if (_streetsideView?._streetside?.lat != null &&
    //            _streetsideView?._streetside?.lon != null)
    //        {
    //            //_streetsideView._streetside.updateView(_streetsideView._streetside.lat, _streetsideView._streetside.lon);
    //            _streetsideView._streetside = _streetsideView.mapViewport.InvokeScript("streetsideMapBing");
    //            _streetsideView._streetside.loadMapScenario(40.4017665, -111.9331024);

    //        }
            
    //    }

    //}

}
