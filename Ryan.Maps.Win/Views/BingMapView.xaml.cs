using Microsoft.Maps.MapControl.WPF;
using Microsoft.Maps.MapControl.WPF.Design;
using Ryan.AddressUtility.Models;
using Ryan.Maps.Win.ViewModels;
using System.Collections.Generic;
using System.Device.Location;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Media;

namespace Ryan.Maps.Win.Views
{
    /// <summary>
    /// Interaction logic for BingMapView.xaml
    /// </summary>
    public partial class BingMapView : UserControl
    {

        #region Fields
        private LocationConverter _locationConverter = new LocationConverter();
        private BingMapViewModel _bingMapViewModel;
        #endregion

        public BingMapView()
        {
            InitializeComponent();
            bingMap.Loaded += BingMap_Loaded;

            _bingMapViewModel = new BingMapViewModel
            {
                SubjectAddress = new Address
                {
                    AddressLine1 = "325 E Gordon Ave",
                    City = "Layton",
                    State = "UT",
                    Zip = "84045"
                },
                Comp1 = new Ryan.Maps.Win.Models.Comp
                {
                    CompNumber = "1",
                    Address = new Address
                    {
                        AddressLine1 = "765 E Gordon Ave",
                        City = "Layton",
                        State = "UT",
                        Zip = "84045"
                    }
                },
                Comp2 = new Ryan.Maps.Win.Models.Comp
                {
                    CompNumber = "2",
                    Address = new Address
                    {
                        AddressLine1 = "929 E Arbor Way",
                        City = "Layton",
                        State = "UT",
                        Zip = "84045"
                    }
                },
                Comp3 = new Ryan.Maps.Win.Models.Comp
                {
                    CompNumber = "3",
                    Address = new Address
                    {
                        AddressLine1 = "5848 Dartmouth Dr",
                        City = "Mountain Green",
                        State = "UT",
                        Zip = "84050"
                    }
                },
                ShowDetailedMapPushpin = false
            };

            this.DataContext = _bingMapViewModel;

        }

        private async void BingMap_Loaded(object sender, RoutedEventArgs e)
        {
            // Set Default location of map
            SetMapDefaults();
            AddEiffelTowerPin();

            var distance = new AddressUtility.Utilities.StraightLineDistance();

            var comps = new List<Destination>
            {
                new Destination { Address = _bingMapViewModel.Comp1.Address },
                new Destination { Address = _bingMapViewModel.Comp2.Address },
                new Destination { Address = _bingMapViewModel.Comp3.Address }
            };

            var geoCodeRepo = new AddressUtility.Repositories.BingGeoCodeAddressRepository();
            var distances = await Task.Run(() => distance.GetProximitiesByDestination(_bingMapViewModel.SubjectAddress, comps, geoCodeRepo));

            // Update distances 
            if (_bingMapViewModel.SubjectAddress != null)
            {
                this._bingMapViewModel.Comp1.DistanceFromSubject = distances.Destinations[0].DistanceFromOrigin.Substring(0, distances.Destinations[0].DistanceFromOrigin.IndexOf("mile")) + "mi.";
                this._bingMapViewModel.Comp2.DistanceFromSubject = distances.Destinations[1].DistanceFromOrigin.Substring(0, distances.Destinations[1].DistanceFromOrigin.IndexOf("mile")) + "mi.";
                this._bingMapViewModel.Comp3.DistanceFromSubject = distances.Destinations[2].DistanceFromOrigin.Substring(0, distances.Destinations[2].DistanceFromOrigin.IndexOf("mile")) + "mi.";
            }
            var locations = new List<Location>();

            // Add comp pins
            var location = new Location(double.Parse(distances.Destinations[0].GeocodedAddress.Latitude), double.Parse(distances.Destinations[0].GeocodedAddress.Longitude));
            AddPropertyPushpin(location, nameof(_bingMapViewModel.Comp1));
            locations.Add(location);

            location = new Location(double.Parse(distances.Destinations[1].GeocodedAddress.Latitude), double.Parse(distances.Destinations[1].GeocodedAddress.Longitude));
            AddPropertyPushpin(location, nameof(_bingMapViewModel.Comp2));
            locations.Add(location);

            location = new Location(double.Parse(distances.Destinations[2].GeocodedAddress.Latitude), double.Parse(distances.Destinations[2].GeocodedAddress.Longitude));
            AddPropertyPushpin(location, nameof(_bingMapViewModel.Comp3));
            locations.Add(location);

            // Add subject pin
            var subjectLoc = new Location(double.Parse(distances.GeocodedOriginAddress.Latitude), double.Parse(distances.GeocodedOriginAddress.Longitude));
            AddPropertyPushpin(subjectLoc, nameof(_bingMapViewModel.SubjectAddress), "Subject");
            locations.Add(subjectLoc);

            //var rectangle = new LocationRect(locations);    // Get the best view for all locations (https://social.msdn.microsoft.com/Forums/en-US/05f3ff75-cc48-4012-b70e-5465098c8839/bing-map-control-on-wpf-window-how-to-make-map-size-so-that-all-pushpins-display?forum=bingmapsservices)
            //bingMap.SetView(rectangle);

            bingMap.SetView(locations, new Thickness(5, 50, 80, 5), bingMap.Heading); // set view with padding (thickness to allow for pushpins) - see http://stackoverflow.com/questions/21681596/wpf-microsoft-bing-control-setview-using-a-bounding-rectangle
            //bingMap.SetView(subjectLoc, 16.0); // This maintains Bing animation
        }

        private void SetMapDefaults()
        {
            //var center = new GeoCoordinate() { Latitude = 48.8530, Longitude = 2.3498 };  // Paris
            var center = new GeoCoordinate() { Latitude = 39.3683, Longitude = -95.2734 };  // North America
            bingMap.ZoomLevel = 4.0;

            bingMap.Center = new Microsoft.Maps.MapControl.WPF.Location { Longitude = center.Longitude, Latitude = center.Latitude };
            bingMap.AnimationLevel = AnimationLevel.Full;
            //bingMap.Mode = new Microsoft.Maps.MapControl.WPF.AerialMode() { Labels = true };
            bingMap.Mode = new Microsoft.Maps.MapControl.WPF.RoadMode();
            //bingMap.Mode = new Microsoft.Maps.MapControl.WPF.AerialMode(true);


            var pin = new Pushpin
            {
                //ContentTemplate = (DataTemplate)this.FindResource("PropertyPushpin" + templateType + "DataTemplate" + pushpinTemplateSize),
                Location = new Location { Latitude = 48.8530, Longitude = 2.3498 },
                //Style = (Style)this.FindResource("AddressPushpin"), //TODO:  Maybe change this
                PositionOrigin = PositionOrigin.Center,
                Template = (ControlTemplate)this.FindResource("MarketConditionsPushpinTemplate"),
                Tag = "MC",
                ToolTip = "[Address Goes Here]",
                Height = 10,
                Width = 10,
                Background = System.Windows.Media.Brushes.Aqua,
                BorderBrush = System.Windows.Media.Brushes.Blue
            };
            //pin.SetBinding(Pushpin.ContentProperty, new Binding
            //{
            //    //Path = new PropertyPath("SubjectAddress"),  // This is the name of the property to bind to
            //    Path = new PropertyPath(propertyName),      // This is the name of the property to bind to
            //    Source = this._bingMapViewModel             // This is the source of data item to bind to
            //});

            this.bingMap.Children.Add(pin);

        }

        private void AddPropertyPushpin(Location location, string propertyName, string templateType = "Comp")
        {
            var pushpinTemplateSize = _bingMapViewModel.ShowDetailedMapPushpin ? "" : "Small";
            var pin = new Pushpin
            {
                ContentTemplate = (DataTemplate)this.FindResource("PropertyPushpin" + templateType + "DataTemplate" + pushpinTemplateSize),
                Location = location,
                Style = (Style)this.FindResource("AddressPushpin"),
                PositionOrigin = PositionOrigin.BottomLeft,
                Template = (ControlTemplate)this.FindResource(templateType + "PushpinTemplate"),
                Tag = templateType
            };
            pin.SetBinding(Pushpin.ContentProperty, new Binding
            {
                //Path = new PropertyPath("SubjectAddress"),  // This is the name of the property to bind to
                Path = new PropertyPath(propertyName),      // This is the name of the property to bind to
                Source = this._bingMapViewModel             // This is the source of data item to bind to
            });

            this.bingMap.Children.Add(pin);
        }


        /// <summary>
        /// Example/POC for adding custom pin with templates
        /// </summary>
        private void AddEiffelTowerPin()
        {
            // Add pin on the Eiffel Tower as a samples
            // Template (on XAML and this code taken from http://dennis.bloggingabout.net/2012/10/17/bing-maps-on-wpf-and-custom-pushpin-tutorial-for-pixelsense/)
            var pin = new Pushpin
            {
                //Background = System.Windows.Media.Brushes.DarkBlue
                //BorderBrush = System.Windows.Media.Brushes.DarkRed,
                //Content = "Eiffel Tower",
                ContentTemplate = (DataTemplate)this.FindResource("PropertyPushpinSubjectDataTemplate"),
                Location = new Location(48.858093, 2.294694),
                Style = (Style)this.FindResource("AddressPushpin"),
                PositionOrigin = PositionOrigin.BottomLeft,
                Template = (ControlTemplate)this.FindResource("SubjectPushpinTemplate")
            };
            pin.SetBinding(Pushpin.ContentProperty, new Binding
            {
                Path = new PropertyPath("SubjectAddress"),  // This is the name of the property to bind to
                Source = this._bingMapViewModel             // This is the source of data item to bind to
            });

            bingMap.Children.Add(pin);
        }

        /// <summary>
        /// ChangeMapView event handler (for buttons below the map)
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ChangeMapView_Click(object sender, RoutedEventArgs e)
        {
            // Parse the information of the button's Tag property
            string[] tagInfo = ((Button)sender).Tag.ToString().Split(' ');
            Location center = (Location)_locationConverter.ConvertFrom(tagInfo[0]);
            double zoom = System.Convert.ToDouble(tagInfo[1]);

            // Set the map view
            bingMap.SetView(center, zoom);

        }

        /// <summary>
        /// MapZoom button click handlers
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void MapZoom_Click(object sender, RoutedEventArgs e)
        {
            var button = (Button)sender;
            var incrementValue = double.Parse(button.Tag.ToString());
            var newZoomLevel = bingMap.ZoomLevel + incrementValue;
            bingMap.SetView(newZoomLevel, 0); // This maintains Bing animation
        }

        /// <summary>
        ///     Changes the map type from Road to Aerial and back
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void MapType_Click(object sender, RoutedEventArgs e)
        {
            var button = (Button)sender;
            var mapType = button.Tag.ToString();

            switch (mapType)
            {
                case "RoadMode":
                    {
                        bingMap.Mode = new Microsoft.Maps.MapControl.WPF.AerialMode(true);
                        mapTypeImage.Style = (Style)this.FindResource("MapTypeButtonImageRoad");
                        button.ToolTip = "Change map type to Road Mode";
                        button.Tag = "AerialMode";
                        mapTypeName.Text = "Road";
                        mapTypeName.Foreground = new SolidColorBrush(Colors.Black);
                        break;
                    }
                default:
                    {
                        bingMap.Mode = new Microsoft.Maps.MapControl.WPF.RoadMode();
                        mapTypeImage.Style = (Style)this.FindResource("MapTypeButtonImageAerial");
                        button.ToolTip = "Change map type to Aerial/Satellite Mode";
                        button.Tag = "RoadMode";
                        mapTypeName.Text = "Aerial";
                        mapTypeName.Foreground = new SolidColorBrush(Colors.White);
                        break;
                    }
            }

        }

        private void popupTest_Click(object sender, RoutedEventArgs e)
        {
            MyPopup.IsOpen = false;
            MyPopup.PlacementTarget = sender as UIElement;
            MyPopup.IsOpen = true;

        }

        private void ChangePushpins_Click(object sender, RoutedEventArgs e)
        {
            //var push = bingMap.Children[1] as Pushpin;
            //push.Width = 10;
            //((Pushpin)bingMap.Children[1]).Location = new Location(((Pushpin)bingMap.Children[0]).Location.Latitude, push.Location.Longitude);
            //((Pushpin)bingMap.Children[1]).Width = 300;
            //bingMap.Children[0] = push;

            ((Pushpin)bingMap.Children[0]).ContentTemplate = (DataTemplate)this.FindResource("PropertyPushpinSubjectDataTemplateSmall");
            ((Pushpin)bingMap.Children[1]).ContentTemplate = (DataTemplate)this.FindResource("PropertyPushpinCompDataTemplateSmall");
            ((Pushpin)bingMap.Children[2]).ContentTemplate = (DataTemplate)this.FindResource("PropertyPushpinCompDataTemplateSmall");
            ((Pushpin)bingMap.Children[3]).ContentTemplate = (DataTemplate)this.FindResource("PropertyPushpinCompDataTemplateSmall");
            ((Pushpin)bingMap.Children[4]).ContentTemplate = (DataTemplate)this.FindResource("PropertyPushpinSubjectDataTemplateSmall");
            bingMap.SetView(bingMap.Center, bingMap.ZoomLevel);
        }

        private void ToggleDetaildPushpin_Toggle(object sender, RoutedEventArgs e)
        {
            var button = (ToggleButton)sender;
            var templateSize = button.IsChecked == true ? "" : "Small";

            foreach (var child in bingMap.Children)
            {
                var childPushpin = (Pushpin)child;
                var templateType = childPushpin.Tag?.ToString().ToLower() == "subject" ? "Subject" : "Comp";

                childPushpin.ContentTemplate = (DataTemplate)this.FindResource("PropertyPushpin" + templateType + "DataTemplate" + templateSize);
            }


            //if (button.IsChecked == true)
            //{
            //    ((Pushpin)bingMap.Children[0]).ContentTemplate = (DataTemplate)this.FindResource("PropertyPushpinSubjectDataTemplate");
            //    ((Pushpin)bingMap.Children[1]).ContentTemplate = (DataTemplate)this.FindResource("PropertyPushpinCompDataTemplate");
            //    ((Pushpin)bingMap.Children[2]).ContentTemplate = (DataTemplate)this.FindResource("PropertyPushpinCompDataTemplate");
            //    ((Pushpin)bingMap.Children[3]).ContentTemplate = (DataTemplate)this.FindResource("PropertyPushpinCompDataTemplate");
            //    ((Pushpin)bingMap.Children[4]).ContentTemplate = (DataTemplate)this.FindResource("PropertyPushpinSubjectDataTemplate");
            //}
            //else
            //{
            //    ((Pushpin)bingMap.Children[0]).ContentTemplate = (DataTemplate)this.FindResource("PropertyPushpinSubjectDataTemplateSmall");
            //    ((Pushpin)bingMap.Children[1]).ContentTemplate = (DataTemplate)this.FindResource("PropertyPushpinCompDataTemplateSmall");
            //    ((Pushpin)bingMap.Children[2]).ContentTemplate = (DataTemplate)this.FindResource("PropertyPushpinCompDataTemplateSmall");
            //    ((Pushpin)bingMap.Children[3]).ContentTemplate = (DataTemplate)this.FindResource("PropertyPushpinCompDataTemplateSmall");
            //    ((Pushpin)bingMap.Children[4]).ContentTemplate = (DataTemplate)this.FindResource("PropertyPushpinSubjectDataTemplateSmall");
            //}

            //bingMap.SetView(bingMap.Center, bingMap.ZoomLevel);
            _bingMapViewModel.ShowDetailedMapPushpin = button.IsChecked ?? false;
        }

        private void ExpandPushpin_Click(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            //((Pushpin)((ContentPresenter)((TextBlock)sender).TemplatedParent).TemplatedParent).ContentTemplate = (DataTemplate)this.FindResource("PropertyPushpinSubjectDataTemplate");

            var textBlock = sender as TextBlock;
            var pushpinContentPresenter = textBlock?.TemplatedParent as ContentPresenter;
            var pushpin = pushpinContentPresenter?.TemplatedParent as Pushpin;
            var pushpinType = string.IsNullOrEmpty(textBlock.Tag.ToString()) ? "Comp" : textBlock.Tag.ToString();
            pushpin.ContentTemplate = (DataTemplate)this.FindResource("PropertyPushpin"+ pushpinType + "DataTemplate");

        }
    }
}
