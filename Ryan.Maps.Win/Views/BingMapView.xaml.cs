using Microsoft.Maps.MapControl.WPF;
using Microsoft.Maps.MapControl.WPF.Design;
using Ryan.AddressUtility.Models;
using Ryan.Maps.Win.ViewModels;
using System;
using System.Collections.Generic;
using System.Device.Location;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Shapes;

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

        private List<Pushpin> _modifiedPushpins = new List<Pushpin>();
        Location _originalLocation;

        #endregion

        public BingMapView()
        {
            InitializeComponent();
            bingMap.Loaded += BingMap_Loaded;
            bingMap.ViewChangeEnd += BingMap_ViewChangeEnd;

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

            //_originalLocation = RyanTestPin.Location;
            //return;
            AddEiffelTowerPin();

            var distance = new AddressUtility.Utilities.StraightLineDistance();

            var comps = new List<Destination>
            {
                new Destination { Address = _bingMapViewModel.Comp1.Address },
                new Destination { Address = _bingMapViewModel.Comp2.Address },
                new Destination { Address = _bingMapViewModel.Comp3.Address }
            };

            var geoCodeRepo = new AddressUtility.Repositories.BingGeocodeAddressRepository();
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

            //bingMap.SetView(new Location(48.8530, 2.3498), 10.0);
            //return;

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
                PositionOrigin = PositionOrigin.TopLeft,
                Template = (ControlTemplate)this.FindResource(templateType + "PushpinTemplate"),
                Tag = templateType,
                AllowDrop = true
            };
            pin.MouseDown += MoveMouseDown_Handler;
            pin.Loaded += (sender, e) => Pin_Loaded(sender, e, location);
            pin.SetBinding(Pushpin.ContentProperty, new Binding
            {
                //Path = new PropertyPath("SubjectAddress"),  // This is the name of the property to bind to
                Path = new PropertyPath(propertyName),      // This is the name of the property to bind to
                Source = this._bingMapViewModel            // This is the source of data item to bind to
            });
            
            this.bingMap.Children.Add(pin);
        }

        /// <summary>
        ///     Set the actual location of the property in the PathPointer tag
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        /// <param name="location"></param>
        private void Pin_Loaded(object sender, RoutedEventArgs e, Location location)
        {
            var pin = sender as Pushpin;
            var pointer = pin.Template.FindName("PathPointer", pin) as Path;
            if (pointer != null)
            {
                pointer.Tag = location;
            }
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

        private void BingMap_ViewChangeEnd(object sender, MapEventArgs e)
        {
            // Resize pointers on pushpins that have been moved
            foreach (var pin in _modifiedPushpins)
            {
                DrawPointerForNewLocation(bingMap, pin);
            }
            Console.WriteLine("View changed");
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

        Vector _mouseToMarker;
        Pushpin _pushpin;
        bool _dragPin;

        private void ExpandPushpin_Click(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            var textBlock = sender as TextBlock;
            var pushpinContentPresenter = textBlock?.TemplatedParent as ContentPresenter;
            var pushpin = pushpinContentPresenter?.TemplatedParent as Pushpin;
            //var pushpinType = string.IsNullOrEmpty(textBlock.Tag.ToString()) ? "Comp" : textBlock.Tag.ToString();
            //pushpin.ContentTemplate = (DataTemplate)this.FindResource("PropertyPushpin" + pushpinType + "DataTemplate");

            e.Handled = true;
            _pushpin = pushpin;
            _dragPin = true;

        }

        private void MoveMouseDown_Handler(object sender, MouseButtonEventArgs e)
        {
            var pushpin = sender.GetType() == typeof(Pushpin) ? sender as Pushpin : (sender as Grid)?.TemplatedParent as Pushpin;

            e.Handled = true;
            _pushpin = pushpin;
            _dragPin = true;
            //_mouseToMarker = Point.Subtract(ParentContainer.LocationToViewportPoint(pushpin.Location), e.GetPosition(ParentContainer));
            if (_modifiedPushpins == null) _modifiedPushpins = new List<Pushpin>();
            if (_modifiedPushpins.Exists(x => x == _pushpin) == false) _modifiedPushpins.Add(_pushpin);

            //Console.WriteLine($"Starting Location: {_pushpin.Location.Latitude}, { _pushpin.Location.Longitude}");
            //var viewportPoint = bingMap.LocationToViewportPoint(_pushpin.Location);
            //Console.WriteLine($"End Viewport Point: {viewportPoint.X}, { viewportPoint.Y}");
        }

        private void MoveMouseUp_Handler(object sender, MouseButtonEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Released)
            {
                if (_dragPin && _pushpin != null)
                {
                    //Console.WriteLine($"End Location: {_pushpin.Location.Latitude}, { _pushpin.Location.Longitude}");
                    //var viewportPoint = bingMap.LocationToViewportPoint(_pushpin.Location);
                    //Console.WriteLine($"End Viewport Point: {viewportPoint.X}, { viewportPoint.Y}");
                }
            }

            _dragPin = false;
            _pushpin = null;
        }

        private void MoveMouse_Handler(object sender, System.Windows.Input.MouseEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Pressed)
            {
                if (_dragPin && _pushpin != null)
                {
                    _pushpin.Location = bingMap.ViewportPointToLocation(Point.Add(e.GetPosition(bingMap), _mouseToMarker));
                    
                    //var difference = Point.Subtract(bingMap.LocationToViewportPoint(_originalLocation), bingMap.LocationToViewportPoint(_pushpin.Location));
                    //var pathPointer = (Path)_pushpin.Template.FindName("PathPointer", _pushpin);
                    //pathPointer.Data = Geometry.Parse($"M3,15 L{difference.X},{difference.Y} L12,3");
                    DrawPointerForNewLocation(bingMap, _pushpin);

                    // IDEA:  Instead of moving the location - just mess with the margin or triagle (pointer) size
                    //_pushpin.Margin = new Thickness(20, 0, 0, 0);

                    e.Handled = true;
                    //Console.WriteLine($"{_pushpin}, { _pushpin.PositionOrigin.Y}");
                }
            }
        }

        private void DrawPointerForNewLocation(Map map, Pushpin pushpin)
        {
            if (map == null || pushpin == null) return;
            
            // Get the pointer from the pushpin template
            var pathPointer = pushpin.Template.FindName("PathPointer", pushpin) as Path;

            var actualLocation = pathPointer.Tag as Location;

            // Get the distance between the original location and the new location in Points
            var pinDistance = Point.Subtract(map.LocationToViewportPoint(actualLocation), map.LocationToViewportPoint(pushpin.Location));


            if (pinDistance != null && pathPointer != null)
            {
                BuildInfoboxPointer(pushpin, pinDistance, pathPointer);
            }
        }

        /// <summary>
        ///     Moves/builds the pointer to the appropriate corner of the 
        ///     infobox (i.e, top-left, bottom-left, top-right, or bottom-right) 
        ///     depending on location
        /// </summary>
        /// <param name="pushpin"></param>
        /// <param name="pinDistance"></param>
        /// <param name="pathPointer"></param>
        private static void BuildInfoboxPointer(Pushpin pushpin, Vector pinDistance, Path pathPointer)
        {
            // Draw a triangle using path.  Starting point is upper right-hand corner of the pushpin infobox which is 0,0
            // - UPPER-LEFT corner pointer, points are:  1,10 - original pin location/point - 6,1

            // - LOWER-LEFT corner pointer, points are:  1,{infoboxHeight - 9} - original pin location/point - 6,{infoboxHeight - 2}
            //   - For example an infobox that is 47px high, points are 1,38 - original pin location/point - 6,45

            // - UPPER-RIGHT corner pointer, points are: {infoboxWidth - 6},1 - original pin location/point - {infoboxWidth - 2},10
            //   - For example an infobox that is 107px wide, points are 101,1 - original pin location/point - 105,10

            // - LOWER-RIGHT corner pointer, points are: {infoboxWidth - 2},{infoboxHeight - 9} - original pin location/point - {infoboxWidth - 6},{infoboxHieght - 2}
            //   - For example an infobox that is 107px wide x 47px high, points are 105,41 - original pin location/point - 101,45

            // Get an instance of the infobox (which is actually a rectangle around some text)
            var infobox = pushpin.Template.FindName("Infobox", pushpin) as Rectangle;
            // Get the height and width of the infobox
            double infoboxHeight = infobox != null || infobox.ActualHeight != double.NaN ? infobox.ActualHeight : 0;
            double infoboxWidth = infobox != null || infobox.ActualWidth != double.NaN ? infobox.ActualWidth : 0;
            // Set the default values for the path (default values are for upper-right corner of the infobox)
            double x1 = 1,
                   y1 = 10,
                   x2 = pinDistance.X,  // -4
                   y2 = pinDistance.Y,  // -2
                   x3 = 6,
                   y3 = 1;

            // Set the values for lower-left
            if (pinDistance.Y > infoboxHeight / 2 && pinDistance.X < infoboxWidth)
            {
                //pathPointer.Data = Geometry.Parse($"M4,{infoboxHeight - 14} L{pinDistance.X},{pinDistance.Y} L30,{infoboxHeight - 1}");
                x1 = 1;
                y1 = infoboxHeight - 9;
                x3 = 6;
                y3 = infoboxHeight - 2;
            }

            // Set the values for upper-right
            if (pinDistance.Y < infoboxHeight && pinDistance.X > infoboxWidth / 2)
            {
                x1 = infoboxWidth - 6;
                y1 = 1;
                x3 = infoboxWidth - 2;
                y3 = 10;
            }

            // Set the values for lower-right
            if (pinDistance.Y > infoboxHeight / 2 && pinDistance.X > infoboxWidth / 2)
            {
                x1 = infoboxWidth - 2;
                y1 = infoboxHeight - 9;
                x3 = infoboxWidth - 6;
                y3 = infoboxHeight - 2;
            }

            // Last, build and set the actual path (pointer)
            pathPointer.Data = Geometry.Parse($"M{x1},{y1} L{x2},{y2} L{x3},{y3}");
        }

    }
}
