using Microsoft.Maps.MapControl.WPF;
using Microsoft.Maps.MapControl.WPF.Design;
using Ryan.AddressUtility.Models;
using Ryan.Maps.Win.Models;
using Ryan.Maps.Win.ViewModels;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using Brush = System.Windows.Media.Brush;
using Brushes = System.Windows.Media.Brushes;
using Image = System.Windows.Controls.Image;
using Path = System.Windows.Shapes.Path;
using Point = System.Windows.Point;
using Rectangle = System.Windows.Shapes.Rectangle;
using Size = System.Windows.Size;

namespace Ryan.Maps.Win.Views
{
    /// <summary>
    /// Interaction logic for PrintableMapView.xaml
    /// </summary>
    public partial class PrintableMapView : UserControl
    {

        #region Fields
        private LocationConverter _locationConverter = new LocationConverter();
        private PrintableMapViewModel _mapViewModel;
        private AddressUtility.Repositories.BingGeocodeAddressRepository _geoCodeRepo;

        Pushpin _pushpin;
        bool _dragPin;
        List<Pushpin> _modifiedPushpins = new List<Pushpin>();

        private string _backgroundColorSubject = "#ffffff";
        private string _backgroundColorComp = "#ffffff";

        private string _borderColorSubject = "#9E0B0F";
        private string _borderColorSatelliteViewSubject = "#D85852";
        private string _borderColorComp = "#005167";
        private string _borderColorSatelliteViewComp = "#239FFF";

        private Point startDrag;

        #endregion

        #region Constructors

        public PrintableMapView()
        {
            InitializeComponent();

            //SetMapDefaults();
            this.DataContext = new PrintableMapViewModel
            {
                Subject = new Address
                {
                    AddressLine1 = "325 E Gordon Ave",
                    City = "Layton",
                    State = "UT",
                    Zip = "84041"
                },
                Comps = new List<Comp>
                {
                    new Comp
                    {
                        CompNumber = "1",
                        Address = new Address
                        {
                            AddressLine1 = "929 S Arbor Way",
                            City = "Layton",
                            State = "UT",
                            Zip = "84041"
                        }
                    },
                    new Comp
                    {
                        CompNumber = "2",
                        Address = new Address
                        {
                            AddressLine1 = "765 E Gordon Ave",
                            City = "Layton",
                            State = "UT",
                            Zip = "84041"
                        }
                    },
                    new Comp
                    {
                        CompNumber = "3",
                        Address = new Address
                        {
                            AddressLine1 = "2939 N 725 W",
                            City = "Layton",
                            State = "UT",
                            Zip = "84041"
                        }
                    }
                }
            };

            DataContextChanged += ComparablesMapView_DataContextChanged;
            bingMap.Loaded += Map_Loaded;
            bingMap.ViewChangeEnd += BingMap_ViewChangeEnd;

            _geoCodeRepo = new AddressUtility.Repositories.BingGeocodeAddressRepository();

            //this.DataContext = _mapViewModel;

            canvas.MouseDown += new MouseButtonEventHandler(canvas_MouseDown);
            canvas.MouseUp += new MouseButtonEventHandler(canvas_MouseUp);
            canvas.MouseMove += new MouseEventHandler(canvas_MouseMove);

        }

        private async void Map_Loaded(object sender, RoutedEventArgs e)
        {
            _mapViewModel = DataContext as PrintableMapViewModel;
            bingMap.Measure(new Size(double.PositiveInfinity, double.PositiveInfinity));  // Set default bounds for the map

            MapLoadingStatus.Text = "Loading map...";
            try
            {
                MapLoadingStatus.Text = "Geocoding properties...";

                // Geocode subject
                var subjectAddress = await Task.Run(() => GeocodePropertiesToBeMapped(_mapViewModel.Subject));

                // Geocode and load all properties (loaded into list in order of least important to most important so that most important will be on top - e.g., comp pushpin will be on top of market condition pushpins when map is rendered)
                var propertiesToMap = new List<PropertyMap>();
                /*if (_mapViewModel?.ShowMarketConditionsOnMap == true)  // if market conditions toggled map market conditions
                {
                    propertiesToMap.AddRange(await Task.Run(() => AddAddressesToPropertyMap(DataMasterGlobal.Get().ChosenParcel.MarketConditions,
                                                                   GetComparableType(Enumeration.PropertyUsageType.MarketConditions),
                                                                   subjectAddress)));

                    propertiesToMap.AddRange(await Task.Run(() => AddAddressesToPropertyMap(DataMasterGlobal.Get().ChosenParcel.ProjectMarketConditions,
                                                                   GetComparableType(Enumeration.PropertyUsageType.ProjectMarketConditions),
                                                                   subjectAddress)));
                }
                propertiesToMap.AddRange(await Task.Run(() => AddAddressesToPropertyMap(DataMasterGlobal.Get().ChosenParcel.ReoListings,
                                                                   GetComparableType(Enumeration.PropertyUsageType.ReoListing),
                                                                   subjectAddress)));
                propertiesToMap.AddRange(await Task.Run(() => AddAddressesToPropertyMap(DataMasterGlobal.Get().ChosenParcel.Rentals,
                                                                   GetComparableType(Enumeration.PropertyUsageType.ComparableRental),
                                                                   subjectAddress)));
                propertiesToMap.AddRange(await Task.Run(() => AddAddressesToPropertyMap(DataMasterGlobal.Get().ChosenParcel.Listings,
                                                                   GetComparableType(Enumeration.PropertyUsageType.Listing),
                                                                   subjectAddress)));
                propertiesToMap.AddRange(await Task.Run(() => AddAddressesToPropertyMap(DataMasterGlobal.Get().ChosenParcel.ComparableRentals,
                                                                   GetComparableType(Enumeration.PropertyUsageType.ComparableRental),
                                                                   subjectAddress)));
                                                                   */
                propertiesToMap.AddRange(await Task.Run(() => AddAddressesToPropertyMap(_mapViewModel.Comps,
                                                                   GetComparableType(Enumeration.PropertyUsageType.Comparable),
                                                                   subjectAddress)));


                MapLoadingStatus.Text = "Adding property detail to map...";

                // Add property items to the map
                AddPropertyPushpinsToMap(subjectAddress, propertiesToMap);
                MapLoadingStatus.Text = "Loading complete...";
            }
            catch (Exception ex)
            {
                MapLoadingStatus.Text = "Error loading map...";
                await Task.Delay(2000);
            }
            finally
            {
                MapLoadingStatus.Text = "";
            }
        }

        private void ComparablesMapView_DataContextChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            _mapViewModel = (PrintableMapViewModel)DataContext;
        }

        #endregion

        #region Methods

        private void AddPropertyPushpinsToMap(Address subject, List<PropertyMap> properties)
        {
            var locations = new List<Location>();

            // Add comp/MC pushpins
            foreach (var property in properties)
            {
                var location = new Location(double.Parse(property.Address.Latitude), double.Parse(property.Address.Longitude));

                if (locations.Contains(location)) continue; // Make sure location isn't already in locations collection (i.e., property hasn't already been mapped)
                AddPropertyPushpin(location, property);
                locations.Add(location);
            }

            // Add subject pushpin
            if (subject != null)
            {
                var subjectLoc = new Location(double.Parse(subject.Latitude), double.Parse(subject.Longitude));
                AddPropertyPushpin(subjectLoc, new PropertyMap { Address = subject }, "Subject");
                locations.Add(subjectLoc);
            }

            if (locations.Count > 1)
            {
                bingMap.SetView(locations, new Thickness(5, 5, 80, 60), bingMap.Heading);
            }
            else if (locations.Count == 1)
            {
                bingMap.SetView(locations.FirstOrDefault(), 14.0);
            }
        }

        private void AddPropertyPushpin(Location location, PropertyMap propertyData, string templateType = "Comp")
        {

            Pushpin pin;
            if (propertyData.CompType?.PropertyType == Enumeration.PropertyUsageType.MarketConditions ||
                propertyData.CompType?.PropertyType == Enumeration.PropertyUsageType.ProjectMarketConditions)
            {
                pin = BuildPropertyPinMarketConditions(location, propertyData);
            }
            else
            {
                if (IsAlreadyMapped(propertyData)) return;
                pin = BuildPropertyPin(location, propertyData, templateType);
            }


            bingMap.Children.Add(pin);
        }

        private bool IsAlreadyMapped(PropertyMap propertyData)
        {
            /*
            foreach (var child in bingMap.Children)
            {
                if (child.GetType() == typeof(Pushpin))
                {

                    var childPin = child as Pushpin;
                    var propertyMap = childPin.Content as PropertyMap;
                    if (propertyMap.Address.Latitude == propertyData.Address.Latitude &&
                        propertyMap.Address.Longitude == propertyData.Address.Longitude)
                    {
                        var s = "";
                    }
                }

            }*/

            return bingMap.Children
                          .OfType<Pushpin>()
                          .Select(x => x.Content as PropertyMap)
                          .Any(p => p.Address.Longitude == propertyData.Address.Longitude &&
                                    p.Address.Latitude == propertyData.Address.Latitude);
        }

        private Pushpin BuildPropertyPin(Location location, PropertyMap propertyData, string templateType = "Comp")
        {
            var backgroundColor = GetBackgroundColor(templateType);
            var borderColor = GetBorderColor(templateType);

            // All properties EXCEPT Market Condition properties
            var pushpinTemplateSize = _mapViewModel?.ShowDetailedMapPushpin == true ? "" : "Small";
            var pin = new Pushpin
            {
                BorderBrush = (Brush)(new BrushConverter().ConvertFrom(borderColor)),
                Background = (Brush)(new BrushConverter().ConvertFrom(backgroundColor)),
                ContentTemplate = (DataTemplate)FindResource("PropertyPushpin" + templateType + "DataTemplate" + pushpinTemplateSize),
                Location = location,
                PositionOrigin = PositionOrigin.TopLeft,
                Style = (Style)FindResource("AddressPushpin"),
                Tag = templateType,
                Template = (ControlTemplate)FindResource(templateType + "PushpinTemplate"),
            };
            pin.SetBinding(ContentProperty, new Binding
            {
                //Path = new PropertyPath(nameof(address)),      // This is the name of the property to bind to
                Source = propertyData             // This is the source of data item to bind to
            });
            pin.MouseDown += MoveMouseDown_Handler;
            pin.Loaded += (sender, e) => Pin_Loaded(sender, e, location);

            return pin;
        }

        private Pushpin BuildPropertyPinMarketConditions(Location location, PropertyMap propertyData)
        {
            var borderColor = Brushes.Blue;
            var fillColor = propertyData.CompType.PropertyType == Enumeration.PropertyUsageType.ProjectMarketConditions ? Brushes.LightGoldenrodYellow : Brushes.LimeGreen;
            var pin = new Pushpin
            {
                Background = fillColor,
                BorderBrush = borderColor,
                Location = location,
                PositionOrigin = PositionOrigin.Center,
                Style = (Style)FindResource("MarketConditionsPushpin"),
                Tag = propertyData.CompType.PropertyTypeAbbreviation,
                Template = (ControlTemplate)FindResource("MarketConditionsPushpinTemplate"),
                ToolTip = propertyData.Address?.FullAddress
            };

            return pin;
        }

        private string GetBackgroundColor(string templateType)
        {
            return templateType.ToLowerInvariant() == "comp" ? _backgroundColorComp : _backgroundColorSubject;
        }

        private string GetBorderColor(string templateType)
        {
            return templateType.ToLowerInvariant() == "comp" ? _borderColorComp : _borderColorSubject;
        }

        private void SetMapDefaults()
        {
            var center = new Location() { Latitude = 39.3683, Longitude = -95.2734 };  // North America
            bingMap.ZoomLevel = 4.0;

            bingMap.Center = new Location { Longitude = center.Longitude, Latitude = center.Latitude };
            bingMap.AnimationLevel = AnimationLevel.Full;
            bingMap.Mode = new RoadMode();
        }

        private string GetDistanceFromSubject(Address subject, Address comp)
        {
            var distanceFromSubject = string.Empty;
            if (subject?.Latitude != null && subject?.Longitude != null && comp != null && comp?.Latitude != null && comp?.Longitude != null)
            {
                var distanceResponse = new AddressUtility.Utilities.StraightLineDistance().GetProximitiesByAddress(subject, new List<Address> { comp }, _geoCodeRepo);
                var distanceFromOrigin = distanceResponse?.Destinations?.FirstOrDefault().DistanceFromOrigin;
                distanceFromSubject = distanceFromOrigin.IndexOf("mile") > 0 ? distanceFromOrigin.Substring(0, distanceFromOrigin.IndexOf("mile")) + "mi." : distanceFromOrigin;
            }
            return distanceFromSubject;
        }

        private IEnumerable<PropertyMap> AddAddressesToPropertyMap(List<Comp> properties, ComparableType compType, Address subjectAddress = null)
        {
            foreach (var property in properties)
            {
                var propertyAddress = GeocodePropertiesToBeMapped(property.Address);
                if (propertyAddress == null) continue;  // if property could not be mapped - move next

                // Update parcel object (this is ESSENTIAL so that we don't call the geocoding services more than once per property - we get charged every time we call that service)
                property.Address.Latitude = propertyAddress.Latitude;
                property.Address.Longitude = propertyAddress.Longitude;

                var distanceFromSubject = GetDistanceFromSubject(subjectAddress, propertyAddress);
                yield return new PropertyMap
                {
                    Address = propertyAddress,
                    CompNumber = properties.IndexOf(property) + 1,
                    CompType = compType,
                    DistanceFromSubject = string.IsNullOrEmpty(distanceFromSubject) ? null : distanceFromSubject
                };
            }
        }

        private List<Address> GeocodePropertiesToBeMapped(List<Address> addresses)
        {
            var propertyAddressList = new List<Address>();
            foreach (var address in addresses)
            {
                propertyAddressList.Add(AddressUtility.Utilities.AddressFormatting.BuildAddressStringFromParts(address));
            }
            return _geoCodeRepo.GeocodeAddresses(propertyAddressList);
        }

        private Address GeocodePropertiesToBeMapped(Address address)
        {
            var propertyAddress = AddressUtility.Utilities.AddressFormatting.BuildAddressStringFromParts(address);
            return _geoCodeRepo.GeocodeAddress(propertyAddress);
        }

        private ComparableType GetComparableType(Enumeration.PropertyUsageType propertyUsageType)
        {
            switch (propertyUsageType)
            {
                case Enumeration.PropertyUsageType.Comparable:
                    return new ComparableType
                    {
                        PropertyType = Enumeration.PropertyUsageType.Comparable,
                        PropertyTypeAbbreviation = "C",
                        PropertyTypeNameShort = "Comp"
                    };
                case Enumeration.PropertyUsageType.ComparableRental:
                    return new ComparableType
                    {
                        PropertyType = Enumeration.PropertyUsageType.ComparableRental,
                        PropertyTypeAbbreviation = "R",
                        PropertyTypeNameShort = "Rent"
                    };
                case Enumeration.PropertyUsageType.Listing:
                    return new ComparableType
                    {
                        PropertyType = Enumeration.PropertyUsageType.Listing,
                        PropertyTypeAbbreviation = "L",
                        PropertyTypeNameShort = "List"
                    };
                case Enumeration.PropertyUsageType.Rental:
                    return new ComparableType
                    {
                        PropertyType = Enumeration.PropertyUsageType.Rental,
                        PropertyTypeAbbreviation = "R",
                        PropertyTypeNameShort = "Rent"
                    };
                case Enumeration.PropertyUsageType.ReoListing:
                    return new ComparableType
                    {
                        PropertyType = Enumeration.PropertyUsageType.ComparableRental,
                        PropertyTypeAbbreviation = "RE",
                        PropertyTypeNameShort = "REO"
                    };
                case Enumeration.PropertyUsageType.MarketConditions:
                    return new ComparableType
                    {
                        PropertyType = Enumeration.PropertyUsageType.MarketConditions,
                        PropertyTypeAbbreviation = "MC",
                        PropertyTypeNameShort = "MC"
                    };
                case Enumeration.PropertyUsageType.ProjectMarketConditions:
                    return new ComparableType
                    {
                        PropertyType = Enumeration.PropertyUsageType.ProjectMarketConditions,
                        PropertyTypeAbbreviation = "P",
                        PropertyTypeNameShort = "Proj"
                    };

            }
            return null;
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

        #region Event Handlers

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
                        ChangePinsToSatelliteModeFriendlyColor();
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
                        ChangePinsToDefaultColor();
                        break;
                    }
            }

        }

        /// <summary>
        ///     Changes all pins to be Satellite-mode friendly, meaning
        ///     the pins/pointers are easier to see in satellite/aerial mode
        /// </summary>
        private void ChangePinsToSatelliteModeFriendlyColor()
        {
            ChangePinColors(_borderColorSatelliteViewSubject, _borderColorSatelliteViewComp);
        }

        /// <summary>
        ///     Changes all pins to be default color
        /// </summary>
        private void ChangePinsToDefaultColor()
        {
            ChangePinColors(_borderColorSubject, _borderColorComp);
        }

        /// <summary>
        ///     Updates all pin border colors based on the colors passed in in the parameter
        /// </summary>
        /// <param name="subjectColor"></param>
        /// <param name="compColor"></param>
        private void ChangePinColors(string subjectColor, string compColor)
        {
            Brush borderBrush;

            foreach (var child in bingMap.Children)
            {
                var childPushpin = (Pushpin)child;
                var templateType = childPushpin.Tag?.ToString().ToLower() == "comp" ? "Comp" : "Subject";

                borderBrush = templateType == "Subject" ?
                                                (Brush)(new BrushConverter().ConvertFrom(subjectColor)) :
                                                (Brush)(new BrushConverter().ConvertFrom(compColor));
                childPushpin.BorderBrush = borderBrush;
            }
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

            //bingMap.SetView(bingMap.Center, bingMap.ZoomLevel);
            _mapViewModel.ShowDetailedMapPushpin = button.IsChecked ?? false;
        }

        private void ExpandPushpin_Click(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            //((Pushpin)((ContentPresenter)((TextBlock)sender).TemplatedParent).TemplatedParent).ContentTemplate = (DataTemplate)this.FindResource("PropertyPushpinSubjectDataTemplate");

            var textBlock = sender as TextBlock;
            var pushpinContentPresenter = textBlock?.TemplatedParent as ContentPresenter;
            var pushpin = pushpinContentPresenter?.TemplatedParent as Pushpin;
            var pushpinType = string.IsNullOrEmpty(textBlock.Tag.ToString()) ? "Comp" : textBlock.Tag.ToString();
            pushpin.ContentTemplate = (DataTemplate)this.FindResource("PropertyPushpin" + pushpinType + "DataTemplate");

        }

        private async void ToggleShowMC_Toggle(object sender, RoutedEventArgs e)
        {
            //var button = (ToggleButton)sender;
            //var showMC = button.IsChecked;
            //List<Location> locations = bingMap.Children.OfType<Pushpin>().Select(x => x.Location).ToList();
            //MapLoadingStatus.Text = "Begin loading Market Condition properties...";

            //try
            //{
            //    if (showMC == true)
            //    {
            //        // Load the MCs and recenter the map to include the MC properties
            //        var propertiesToMap = new List<PropertyMap>();
            //        MapLoadingStatus.Text = "GeoCoding Market Condition properties...";
            //        if (DataMasterGlobal.Get().ChosenParcel.MarketConditions.Count < 1 && DataMasterGlobal.Get().ChosenParcel.ProjectMarketConditions.Count < 1)
            //        {
            //            MapLoadingStatus.Text = "Market Conditions properties have not yet been loaded.  Go the Market Conditions tab and load properties.";
            //            return;
            //        }

            //        var subjectAddress = await Task.Run(() => GeocodePropertiesToBeMapped(DataMasterGlobal.Get().ChosenParcel.Subject));

            //        propertiesToMap.AddRange(await Task.Run(() => AddAddressesToPropertyMap(DataMasterGlobal.Get().ChosenParcel.MarketConditions,
            //                                                           GetComparableType(Enumeration.PropertyUsageType.MarketConditions),
            //                                                           subjectAddress)));

            //        propertiesToMap.AddRange(await Task.Run(() => AddAddressesToPropertyMap(DataMasterGlobal.Get().ChosenParcel.ProjectMarketConditions,
            //                                                       GetComparableType(Enumeration.PropertyUsageType.ProjectMarketConditions),
            //                                                       subjectAddress)));
            //        //AddPropertyPushpinsToMap(subjectAddress, propertiesToMap);

            //        MapLoadingStatus.Text = "Adding properties to map...";

            //        foreach (var property in propertiesToMap)
            //        {
            //            var location = new Location(double.Parse(property.Address.Latitude), double.Parse(property.Address.Longitude));

            //            if (!locations.Contains(location))  // Make sure loation isn't already in locations collection (i.e., property hasn't already been mapped)
            //            {
            //                AddPropertyPushpin(location, property);
            //                locations.Add(location);
            //            }
            //        }

            //        _mapViewModel.ShowMarketConditionsOnMap = true;
            //    }
            //    else
            //    {
            //        MapLoadingStatus.Text = "Removing properties from map...";

            //        // Load the MCs and recenter the map for just the subject and comps
            //        foreach (var child in bingMap.Children.OfType<Pushpin>().Where(x => x.Tag.ToString() == "MC" || x.Tag.ToString() == "P").ToList())
            //        {
            //            bingMap.Children.Remove(child);
            //            locations.Remove(child.Location);
            //        }
            //        _mapViewModel.ShowMarketConditionsOnMap = false;
            //    }

            //    MapLoadingStatus.Text = "Recentering map...";
            //    if (locations.Count > 1)
            //    {
            //        bingMap.SetView(locations, new Thickness(5, 50, 80, 5), bingMap.Heading);
            //    }
            //    else if (locations.Count == 1)
            //    {
            //        bingMap.SetView(locations.FirstOrDefault(), 14.0);
            //    }
            //    MapLoadingStatus.Text = "Loading complete...";
            //}
            //catch
            //{
            //    MapLoadingStatus.Text = "Error loading map...";
            //    await Task.Delay(2000);
            //}
            //finally
            //{
            //    MapLoadingStatus.Text = "";
            //}

        }

        private void MoveMouseDown_Handler(object sender, MouseButtonEventArgs e)
        {
            var pushpin = sender.GetType() == typeof(Pushpin) ? sender as Pushpin : (sender as Grid)?.TemplatedParent as Pushpin;

            e.Handled = true;
            _pushpin = pushpin;
            _dragPin = true;

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
                    _pushpin.Location = bingMap.ViewportPointToLocation(Point.Add(e.GetPosition(bingMap), new Vector()));

                    DrawPointerForNewLocation(bingMap, _pushpin);

                    e.Handled = true;
                    //Console.WriteLine($"{_pushpin}, { _pushpin.PositionOrigin.Y}");
                }
            }
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

        private void BingMap_ViewChangeEnd(object sender, MapEventArgs e)
        {
            // Resize pointers on pushpins that have been moved
            foreach (var pin in _modifiedPushpins)
            {
                DrawPointerForNewLocation(bingMap, pin);
            }
            Console.WriteLine("View changed");
        }


        #endregion

        #endregion

        #region Print Preview

        private enum HitType
        {
            None, Body, UL, UR, LR, LL, L, R, T, B
        };

        // True if a drag is in progress.
        private bool DragInProgress = false;

        // The drag's last point.
        private Point LastPoint;

        // The part of the rectangle under the mouse.
        HitType MouseHitType = HitType.None;

        // Return a HitType value to indicate what is at the point.
        private HitType SetHitType(Rectangle rect, Point point)
        {
            double left = Canvas.GetLeft(rectangle);
            double top = Canvas.GetTop(rectangle);
            double right = left + rectangle.Width;
            double bottom = top + rectangle.Height;
            if (point.X < left) return HitType.None;
            if (point.X > right) return HitType.None;
            if (point.Y < top) return HitType.None;
            if (point.Y > bottom) return HitType.None;

            const double GAP = 10;
            if (point.X - left < GAP)
            {
                // Left edge.
                if (point.Y - top < GAP) return HitType.UL;
                if (bottom - point.Y < GAP) return HitType.LL;
                return HitType.L;
            }
            if (right - point.X < GAP)
            {
                // Right edge.
                if (point.Y - top < GAP) return HitType.UR;
                if (bottom - point.Y < GAP) return HitType.LR;
                return HitType.R;
            }
            if (point.Y - top < GAP) return HitType.T;
            if (bottom - point.Y < GAP) return HitType.B;
            return HitType.Body;
        }

        // Set a mouse cursor appropriate for the current hit type.
        private void SetMouseCursor()
        {
            // See what cursor we should display.
            Cursor desired_cursor = Cursors.Arrow;
            switch (MouseHitType)
            {
                case HitType.None:
                    desired_cursor = Cursors.Arrow;
                    break;
                case HitType.Body:
                    desired_cursor = Cursors.ScrollAll;
                    break;
                case HitType.UL:
                case HitType.LR:
                    desired_cursor = Cursors.SizeNWSE;
                    break;
                case HitType.LL:
                case HitType.UR:
                    desired_cursor = Cursors.SizeNESW;
                    break;
                case HitType.T:
                case HitType.B:
                    desired_cursor = Cursors.SizeNS;
                    break;
                case HitType.L:
                case HitType.R:
                    desired_cursor = Cursors.SizeWE;
                    break;
            }

            // Display the desired cursor.
            if (Cursor != desired_cursor) Cursor = desired_cursor;
        }

        // Start dragging.
        private void canvas_MouseDown(object sender, MouseButtonEventArgs e)
        {
            MouseHitType = SetHitType(rectangle, Mouse.GetPosition(canvas));
            SetMouseCursor();
            if (MouseHitType == HitType.None) return;

            LastPoint = Mouse.GetPosition(canvas);
            DragInProgress = true;
        }

        // If a drag is in progress, continue the drag.
        // Otherwise display the correct cursor.
        private void canvas_MouseMove(object sender, MouseEventArgs e)
        {
            if (!DragInProgress)
            {
                MouseHitType = SetHitType(rectangle, Mouse.GetPosition(canvas));
                SetMouseCursor();
            }
            else
            {
                // See how much the mouse has moved.
                Point point = Mouse.GetPosition(canvas);
                double offset_x = point.X - LastPoint.X;
                double offset_y = point.Y - LastPoint.Y;

                // Get the rectangle's current position.
                double new_x = Canvas.GetLeft(rectangle);
                double new_y = Canvas.GetTop(rectangle);
                double new_width = rectangle.Width;
                double new_height = rectangle.Height;

                // Update the rectangle.
                switch (MouseHitType)
                {
                    case HitType.Body:
                        new_x += offset_x;
                        new_y += offset_y;
                        break;
                    case HitType.UL:
                        new_x += offset_x;
                        new_y += offset_y;
                        new_width -= offset_x;
                        new_height -= offset_y;
                        break;
                    case HitType.UR:
                        new_y += offset_y;
                        new_width += offset_x;
                        new_height -= offset_y;
                        break;
                    case HitType.LR:
                        new_width += offset_x;
                        new_height += offset_y;
                        break;
                    case HitType.LL:
                        new_x += offset_x;
                        new_width -= offset_x;
                        new_height += offset_y;
                        break;
                    case HitType.L:
                        new_x += offset_x;
                        new_width -= offset_x;
                        break;
                    case HitType.R:
                        new_width += offset_x;
                        break;
                    case HitType.B:
                        new_height += offset_y;
                        break;
                    case HitType.T:
                        new_y += offset_y;
                        new_height -= offset_y;
                        break;
                }

                // Don't use negative width or height.
                if ((new_width > 0) && (new_height > 0))
                {
                    // Update the rectangle.
                    Canvas.SetLeft(rectangle, new_x);
                    Canvas.SetTop(rectangle, new_y);
                    rectangle.Width = new_width;
                    rectangle.Height = new_height;

                    // Save the mouse's new location.
                    LastPoint = point;
                }
            }
        }

        // Stop dragging.
        private void canvas_MouseUp(object sender, MouseButtonEventArgs e)
        {
            DragInProgress = false;
        }

        #endregion

        #region Print Methods


        private void PrintPreview_Click(object sender, RoutedEventArgs e)
        {

            // copy image to  image control
            //https://stackoverflow.com/questions/24466482/how-to-take-a-screenshot-of-a-wpf-control
            var renderTargetBitmap = new RenderTargetBitmap(Convert.ToInt32(bingMap.ActualWidth), Convert.ToInt32(bingMap.ActualHeight), 96, 96, PixelFormats.Pbgra32);
            renderTargetBitmap.Render(bingMap);

            var pngImage = new JpegBitmapEncoder();
            pngImage.Frames.Add(BitmapFrame.Create(renderTargetBitmap));

            using (var stream = new System.IO.MemoryStream())
            {
                pngImage.Save(stream);
                stream.Seek(0, System.IO.SeekOrigin.Begin);

                var bitmap = new BitmapImage(); //(new Uri(@"C:\Users\Ron\Documents\DataMaster\testmap.jpeg"));
                bitmap.BeginInit();
                bitmap.CacheOption = BitmapCacheOption.OnLoad;
                bitmap.StreamSource = stream;
                bitmap.EndInit();


                MapImage.Source = bitmap;
            }

            //fix clipping rectangle 
            var centerx = MapImage.ActualWidth / 2;
            double printwidth = MapImage.ActualWidth < 820 ? MapImage.ActualWidth : 820;
            var topX = centerx - (printwidth / 2);

            var centery = MapImage.ActualHeight / 2;
            double printheight = MapImage.ActualHeight < 1024 ? MapImage.ActualHeight : 1024;
            var topy = centery - (printheight / 2);

            rectangle.Width = printwidth;
            rectangle.Height = printheight;
            Canvas.SetLeft(rectangle, topX);
            Canvas.SetTop(rectangle, topy);


            // hide controls
            MapButtons.Visibility = Visibility.Hidden;
            bingMap.Visibility = Visibility.Hidden;
            PrintPrevieButton.Visibility = Visibility.Hidden;
            // show controls
            canvas.Visibility = Visibility.Visible;
            PrintButton.Visibility = Visibility.Visible;


        }

        private void PrintButtonNew_Click(object sender, RoutedEventArgs e)
        {
            var printDialog = new PrintDialog();
            if (printDialog.ShowDialog() == true)
            {
                var image = CropImage();
                var doc = PrepareFlowDocument(printDialog, image);

                IDocumentPaginatorSource paginationSource = doc;
                printDialog.PrintDocument(paginationSource.DocumentPaginator, "Sample printing");
            }
        }

        private ImageSource CropImage()
        {
            ImageSource image = null;

            //https://stackoverflow.com/questions/24466482/how-to-take-a-screenshot-of-a-wpf-control
            var renderTargetBitmap = new RenderTargetBitmap(Convert.ToInt32(MapImage.ActualWidth), Convert.ToInt32(MapImage.ActualHeight), 96, 96, PixelFormats.Pbgra32);
            renderTargetBitmap.Render(MapImage);

            var pngImage = new JpegBitmapEncoder();
            pngImage.Frames.Add(BitmapFrame.Create(renderTargetBitmap));
            using (var stream = new System.IO.MemoryStream())
            {
                pngImage.Save(stream);
                stream.Seek(0, System.IO.SeekOrigin.Begin);

                var bitmap = new BitmapImage(); //(new Uri(@"C:\Users\Ron\Documents\DataMaster\testmap.jpeg"));
                bitmap.BeginInit();
                bitmap.CacheOption = BitmapCacheOption.OnLoad;
                bitmap.StreamSource = stream;
                bitmap.EndInit();


                UIElement container = VisualTreeHelper.GetParent(canvas) as UIElement;
                Point relativeLocation = rectangle.TranslatePoint(new Point(0, 0), container);

                image = new CroppedBitmap(bitmap,
                    new Int32Rect(Convert.ToInt32(relativeLocation.X), Convert.ToInt32(relativeLocation.Y),
                        Convert.ToInt32(rectangle.ActualWidth), Convert.ToInt32(rectangle.ActualHeight)));

            }

            return image;
        }
        private FlowDocument PrepareFlowDocument(PrintDialog printDialog, ImageSource image)
        {
            var doc = new FlowDocument();
            doc.FontFamily = new System.Windows.Media.FontFamily("Segoe UI");

            doc.PageWidth = printDialog.PrintableAreaWidth; // CentimeterToPoint(21.59);
            doc.PageHeight = printDialog.PrintableAreaHeight; // CentimeterToPoint(27.94);
            // margins (top, left
            doc.PagePadding = new Thickness(48, 72, 48, 72);

            doc.ColumnWidth = doc.PageWidth;
            doc.IsOptimalParagraphEnabled = true;

            var grid2 = new Grid();
            grid2.Margin = new Thickness(0);
            grid2.ColumnDefinitions.Add(new ColumnDefinition()); // { Width = new GridLength(Convert.ToInt32(doc.ColumnWidth), GridUnitType.Pixel)});
            grid2.RowDefinitions.Add(new RowDefinition());

            var mapImage2 = new Image();
            mapImage2.Source = image;
            mapImage2.Height = 750;
            mapImage2.Stretch = Stretch.Fill;
            //mapImage.VerticalAlignment = VerticalAlignment.Top;
            grid2.Children.Add(mapImage2);
            Grid.SetColumn(mapImage2, 0);
            Grid.SetRow(mapImage2, 0);


            doc.Blocks.Add(new BlockUIContainer(grid2) { BorderBrush = Brushes.Black, BorderThickness = new Thickness(1, 2, 1, 2) });

            var comps = _mapViewModel.Comps;

            var gridAdresses = new Grid();
            gridAdresses.Margin = new Thickness(0, 15, 0, 5);
            gridAdresses.ColumnDefinitions.Add(new ColumnDefinition() { Width = new GridLength(Convert.ToInt32(100), GridUnitType.Pixel) });
            gridAdresses.ColumnDefinitions.Add(new ColumnDefinition()
            {
                Width = new GridLength(Convert.ToInt32(600), GridUnitType.Pixel)
            });

            int row = 0;

            gridAdresses.RowDefinitions.Add(new RowDefinition() { Height = new GridLength(25, GridUnitType.Pixel) });
            var subject = new TextBlock(new Bold(new Run("SUBJECT")));
            gridAdresses.Children.Add(subject);
            Grid.SetColumn(subject, 0);
            Grid.SetRow(subject, row);

            string addressFormat = "{0} {1} {2}, {3} {4}";

            var address = new TextBlock();
            address.Inlines.Add(new Bold(new Run(string.Format("{0} {1}", _mapViewModel.Subject.AddressLine1, _mapViewModel.Subject.AddressLine2))));
            address.Inlines.Add(new Run(string.Format("{0}, {1} {2}", _mapViewModel.Subject.City, _mapViewModel.Subject.State, _mapViewModel.Subject.Zip)));

            //var address = new TextBlock(new Run(string.Format(addressFormat, _mapViewModel.Subject.AddressLine1, _mapViewModel.Subject.AddressLine2, _mapViewModel.Subject.City, _mapViewModel.Subject.State, _mapViewModel.Subject.Zip)));
            gridAdresses.Children.Add(address);
            Grid.SetColumn(address, 1);
            Grid.SetRow(address, row++);

            foreach (var comp in _mapViewModel.Comps)
            {
                gridAdresses.RowDefinitions.Add(new RowDefinition() { Height = new GridLength(25, GridUnitType.Pixel) });

                var compNumber = new TextBlock(new Bold(new Run("COMP " + comp.CompNumber)));
                gridAdresses.Children.Add(compNumber);
                Grid.SetColumn(compNumber, 0);
                Grid.SetRow(compNumber, row);

                address = new TextBlock();
                address.Inlines.Add(new Bold(new Run(string.Format("{0} {1}", comp.Address.AddressLine1, comp.Address.AddressLine2))));
                address.Inlines.Add(new Run(string.Format("{0}, {1} {2}", comp.Address.City, comp.Address.State, comp.Address.Zip)));

                //address = new TextBlock(new Run(string.Format(addressFormat, comp.Address.AddressLine1, comp.Address.AddressLine2, comp.Address.City, comp.Address.State, comp.Address.Zip)));
                gridAdresses.Children.Add(address);
                Grid.SetColumn(address, 1);
                Grid.SetRow(address, row);

                row++;
            }

            doc.Blocks.Add(new BlockUIContainer(gridAdresses));

            return doc;
        }

        private void PrintButton_Click(object sender, RoutedEventArgs e)
        {
            //MessageBox.Show("Coming soon...", "DataMaster", MessageBoxButton.OK, MessageBoxImage.Information);
            var printDialog = new PrintDialog();
            if (printDialog.ShowDialog() == true)
            {
                var doc = new FlowDocument();
                doc.FontFamily = new System.Windows.Media.FontFamily("Segoe UI");

                doc.PageWidth = printDialog.PrintableAreaWidth; // CentimeterToPoint(21.59);
                doc.PageHeight = printDialog.PrintableAreaHeight; // CentimeterToPoint(27.94);
                                                 // margins (top, left
                doc.PagePadding = new Thickness(48, 72, 48, 72);

                doc.ColumnWidth = doc.PageWidth;
                doc.IsOptimalParagraphEnabled = true;

                //https://stackoverflow.com/questions/24466482/how-to-take-a-screenshot-of-a-wpf-control
                var renderTargetBitmap = new RenderTargetBitmap(Convert.ToInt32(bingMap.ActualWidth), Convert.ToInt32(bingMap.ActualHeight), 96, 96, PixelFormats.Pbgra32);
                renderTargetBitmap.Render(bingMap);

                using (FileStream stream = File.Create(@"C:\Users\Ron\Documents\DataMaster\testmap.jpeg"))
                {
                    JpegBitmapEncoder encoder = new JpegBitmapEncoder();
                    encoder.QualityLevel = 90;
                    encoder.Frames.Add(BitmapFrame.Create(renderTargetBitmap));
                    encoder.Save(stream);
                    stream.Close();
                   
                }

              //  CutImage(@"C:\Users\Ron\Documents\DataMaster\testmap.jpeg");

                var pngImage = new JpegBitmapEncoder();
                pngImage.Frames.Add(BitmapFrame.Create(renderTargetBitmap));
                var mapImage = new Image(); // { Width = mapWidth, Height = mapHeight };


                //https://stackoverflow.com/questions/13987408/convert-rendertargetbitmap-to-bitmapimage
                using (var stream = new System.IO.MemoryStream())
                {
                    pngImage.Save(stream);
                    stream.Seek(0, System.IO.SeekOrigin.Begin);

                    var bitmap = new BitmapImage(); //(new Uri(@"C:\Users\Ron\Documents\DataMaster\testmap.jpeg"));
                    bitmap.BeginInit();
                    bitmap.CacheOption = BitmapCacheOption.OnLoad;
                    bitmap.StreamSource = stream;
                    bitmap.EndInit();
                    

                    mapImage.Source = bitmap;
                    //mapImage.VerticalAlignment = VerticalAlignment.Top;
                    //mapImage.Width = 1374;
                    //mapImage.Height = 534;
                    //mapImage.StretchDirection = StretchDirection.Both;
                    //mapImage.Stretch = Stretch.Fill;

                    var grid = new Grid();
                    grid.Margin = new Thickness(0);
                    grid.ColumnDefinitions.Add(new ColumnDefinition()); // { Width = new GridLength(Convert.ToInt32(doc.ColumnWidth), GridUnitType.Pixel)});
                    grid.RowDefinitions.Add(new RowDefinition());
                    grid.RowDefinitions.Add(new RowDefinition());
                    grid.ShowGridLines = true;

                    grid.Children.Add(mapImage);
                    Grid.SetColumn(mapImage, 0);
                    Grid.SetRow(mapImage, 0);


                    var mapImage2 = new Image();

                    var centerx = bingMap.ActualWidth/2;
                    
                    double printwidth = bingMap.ActualWidth < 820 ? bingMap.ActualWidth : 820 ;
                    var topX = centerx - (printwidth/2);

                    var centery = bingMap.ActualHeight / 2;
                    double printheight = bingMap.ActualHeight < 1024 ? bingMap.ActualHeight : 1024;
                    var topy = centery - (printheight/ 2);

                    var image = new CroppedBitmap(bitmap, new Int32Rect(Convert.ToInt32(topX), Convert.ToInt32(topy), Convert.ToInt32(printwidth), Convert.ToInt32(printheight)));

                    var grid2 = new Grid();
                    grid2.Margin = new Thickness(0);
                    grid2.ColumnDefinitions.Add(new ColumnDefinition()); // { Width = new GridLength(Convert.ToInt32(doc.ColumnWidth), GridUnitType.Pixel)});
                    grid2.RowDefinitions.Add(new RowDefinition());


                    mapImage2.Source = image;
                    mapImage2.Height = 750;
                    mapImage2.Stretch = Stretch.Fill;
                    //mapImage.VerticalAlignment = VerticalAlignment.Top;
                    grid2.Children.Add(mapImage2);
                    Grid.SetColumn(mapImage2, 0);
                    Grid.SetRow(mapImage2, 0);

                    //var text = new TextBlock();
                    //text.Inlines.Add(new Run(@"Lorem Ipsum is simply dummy text of the printing and typesetting industry. Lorem Ipsum has been the industry's standard dummy text ever since the 1500s, when an unknown printer took a galley "));
                    //grid.Children.Add(text);
                    //Grid.SetColumn(text, 0);
                    //Grid.SetRow(text, 1);

                    //doc.ColumnWidth = mapWidth;
                   // doc.Blocks.Add(new BlockUIContainer(grid) { BorderBrush = (Brush)(new BrushConverter().ConvertFromString("Blue")), BorderThickness = new Thickness(1), Background = Brushes.Cyan});
                    doc.Blocks.Add(new BlockUIContainer(grid2) { BorderBrush = Brushes.Black, BorderThickness = new Thickness(1, 2, 1, 2) });


                    var comps = _mapViewModel.Comps;

                    var gridAdresses = new Grid();
                    gridAdresses.Margin = new Thickness(0, 15, 0, 5);
                    gridAdresses.ColumnDefinitions.Add(new ColumnDefinition()  { Width = new GridLength(Convert.ToInt32(100), GridUnitType.Pixel)});
                    gridAdresses.ColumnDefinitions.Add(new ColumnDefinition()
                    {
                        Width = new GridLength(Convert.ToInt32(600), GridUnitType.Pixel)
                    });

                    int row = 0;

                    gridAdresses.RowDefinitions.Add(new RowDefinition() { Height = new GridLength(25, GridUnitType.Pixel) });
                    var subject = new TextBlock(new Bold(new Run("SUBJECT")));
                    gridAdresses.Children.Add(subject);
                    Grid.SetColumn(subject, 0);
                    Grid.SetRow(subject, row);

                    string addressFormat = "{0} {1} {2}, {3} {4}";

                    var address = new TextBlock();
                    address.Inlines.Add(new Bold(new Run(string.Format("{0} {1}", _mapViewModel.Subject.AddressLine1, _mapViewModel.Subject.AddressLine2))));
                    address.Inlines.Add(new Run(string.Format("{0}, {1} {2}", _mapViewModel.Subject.City, _mapViewModel.Subject.State, _mapViewModel.Subject.Zip)));

                    //var address = new TextBlock(new Run(string.Format(addressFormat, _mapViewModel.Subject.AddressLine1, _mapViewModel.Subject.AddressLine2, _mapViewModel.Subject.City, _mapViewModel.Subject.State, _mapViewModel.Subject.Zip)));
                    gridAdresses.Children.Add(address);
                    Grid.SetColumn(address, 1);
                    Grid.SetRow(address, row++);

                    foreach (var comp  in _mapViewModel.Comps)
                    {
                        gridAdresses.RowDefinitions.Add(new RowDefinition() {Height =  new GridLength(25, GridUnitType.Pixel)});

                        var compNumber = new TextBlock(new Bold(new Run("COMP "  + comp.CompNumber)));
                        gridAdresses.Children.Add(compNumber);
                        Grid.SetColumn(compNumber,0);
                        Grid.SetRow(compNumber, row);

                        address = new TextBlock();
                        address.Inlines.Add(new Bold(new Run(string.Format("{0} {1}", comp.Address.AddressLine1, comp.Address.AddressLine2))));
                        address.Inlines.Add(new Run(string.Format("{0}, {1} {2}", comp.Address.City, comp.Address.State, comp.Address.Zip)));

                        //address = new TextBlock(new Run(string.Format(addressFormat, comp.Address.AddressLine1, comp.Address.AddressLine2, comp.Address.City, comp.Address.State, comp.Address.Zip)));
                        gridAdresses.Children.Add(address);
                        Grid.SetColumn(address, 1);
                        Grid.SetRow(address, row);

                        row++;
                    }

                    doc.Blocks.Add(new BlockUIContainer(gridAdresses));
                }

                IDocumentPaginatorSource paginationSource = doc;

                printDialog.PrintDocument(paginationSource.DocumentPaginator, "Sample printing");

            }


        }

        #endregion

        private void CutImage(string img)
        {
            int count = 0;

            BitmapImage src = new BitmapImage();
            src.BeginInit();
            src.UriSource = new Uri(img, UriKind.Relative);
            src.CacheOption = BitmapCacheOption.OnLoad;
            src.EndInit();

            Image croppedImage = new Image();
            croppedImage.Width = 100;


            var image = new CroppedBitmap(src, new Int32Rect(100, 10, 500, 500));
            croppedImage.Source = image;

            FileStream stream = new FileStream(@"C:\Users\Ron\Documents\DataMaster\cropped.jpg", FileMode.Create);
            JpegBitmapEncoder encoder = new JpegBitmapEncoder();
            encoder.Frames.Add(BitmapFrame.Create(image));
            encoder.Save(stream);
        }

    }
}
