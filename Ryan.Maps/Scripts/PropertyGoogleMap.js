function PropertyGoogleMap() {
    'use strict';
    ///<summary>
    /// The summary goes here
    ///</summary>
    var self = this;

    /* PROPERTIES */
    self.subjectAddress = "";   // subject address
    self.addresses = [];   // array of addresses to be used in the mapping
    self.mapCanvasId = "";   // map canvas element id name
    self.mapCanvas = {};   // map canvas element
    self.addSubjectInfoContentWindow = true;  // Add info content popup for subject
    self.addSubjectStreetMapView = false;   // Add a street map view
    self.streetMapCanvasId = "";    // street map canvas element id name
    self.streetMapCanvas = {};      // street map canvas element

    /* FIELDS */
    var mapSubject = false;         // map subject bool (is the subject property going to be mapped?)
    var subjectLocation;            // google geocode response json json.results[0].geometry.location
    var subjectLatLng;              // google.maps.LatLng object
    var myOptions;                  // google maps marker options object
    var map;                        // google.maps map object 
    var bounds = new google.maps.LatLngBounds();    // google.map.bounds object used for centering the map

    /* CONSTRUCTOR */
    /// The constructor for this class.  This function MUST be called for the code to execute (e.g., propertyMap.init();)
    self.init = function () {
        // Set initial properties....

        if (self.mapCanvasId !== "") {
            self.mapCanvas = $("#" + self.mapCanvasId);
        }

        if (self.streetMapCanvasId !== "") {
            self.streetMapCanvas = $("#" + self.streetMapCanvasId);
        }

        mapSubject = (self.subjectAddress !== "") ? true : false;

        if (mapSubject) {
            // Map the subject property if it exists
            mapPropertyFromGoogleJson(self.subjectAddress, self.mapSubjectProperty);
        }

        if (self.addresses.length > 0) {
            // Loop through all properties and map each one
            for (var i = 0; i < self.addresses.length; i++) {
                mapPropertyFromGoogleJson(self.addresses[i], self.mapPropertyAddresses);
            }
        }

        // hide the canvas if set to false
        if (self.addSubjectStreetMapView === false) {
            $(self.streetMapCanvas).hide();
        }

    };

    /* METHODS */
    /* PUBLIC METHODS */
    /// Maps the subject property and creates the map on the map canvas.  Also adds 1 and 2 mile radius circles
    self.mapSubjectProperty = function (addressJson) {

        if (addressJson !== undefined) {
            subjectLocation = addressJson.results[0].geometry.location;
            subjectLatLng = new google.maps.LatLng(subjectLocation.lat, subjectLocation.lng);
        } else {
            self.mapSubjectProperty = false;
            return;
        }
        var mapOptions = {
            zoom: 17,
            center: new google.maps.LatLng(subjectLocation.lat, subjectLocation.lng),
            mapTypeId: google.maps.MapTypeId.ROADMAP
        };

        map = new google.maps.Map(self.mapCanvas[0], mapOptions);

        var marker = addMarkerToMap(subjectLatLng, "S", "Subject\n" + self.subjectAddress);

        if (self.addSubjectInfoContentWindow) {
            var infowindow = new google.maps.InfoWindow({
                content: getInfoWindowContent(buildAddressFromComponent(addressJson.results[0].address_components), "")
            });
            marker.addListener('click', function () {
                infowindow.open(map, marker);
            });
        }

        if (self.addresses.length > 0) {
            // Add 2 mile radius circle overlay and bind to marker
            addCircleToMap(marker, 3220, "#0000FF", 0.4, '#0069AB', 0.03);

            // Add 1 mile radius circle overlay and bind to marker
            addCircleToMap(marker, 1610, "#FF0000", 0.4, '#AA0000', 0.05);

            //  And increase the bounds to take this point
            bounds.extend(marker.position);
            map.fitBounds(bounds);
        }

        if (self.addSubjectStreetMapView) {

            var subjectPoint = new google.maps.LatLng(subjectLocation.lat, subjectLocation.lng);
            var streetViewService = new google.maps.StreetViewService();
            //var panorama = spaces.map.map.getStreetView();
            var panorama = new google.maps.StreetViewPanorama(self.streetMapCanvas[0]);

            streetViewService.getPanoramaByLocation(subjectPoint, 100, function (streetViewPanoramaData, status) {

                if (status === google.maps.StreetViewStatus.OK) {

                    var oldSubjectPoint = subjectPoint;
                    subjectPoint = streetViewPanoramaData.location.latLng;
                    var heading = google.maps.geometry.spherical.computeHeading(subjectPoint, oldSubjectPoint);

                    panorama.setPosition(subjectPoint);
                    panorama.setPov({
                        heading: heading,
                        zoom: 1,
                        pitch: 0
                    });
                    panorama.setVisible(true);
                    /*
                    var panorama = new google.maps.StreetViewPanorama(
                        self.streetMapCanvas[0], {
                            position: new google.maps.LatLng(subjectLocation.lat, subjectLocation.lng),
                            pov: {
                                heading: 0,
                                pitch: 5,
                                zoom: 1
                            }
                        });*/
                    map.setStreetView(panorama);
                } else {
                    $this.text("Sorry! Street View is not available.");
                    // Hide the street view canvas
                }
            });
        }
    };

    /// Maps the comp/MC properties on the map.
    /// If there is a subject, straight line distance info is added
    self.mapPropertyAddresses = function (propertyJson) {

        //for (var i = 0; i < self.addresses.length; i++) {
        //var propertyJson = getPropertyJsonFromGoogle(self.addresses[i]);
        if (propertyJson === undefined || propertyJson.status != "OK") {
            return;
        }

        var propertyLocation = propertyJson.results[0].geometry.location
        var propertyLatLng = new google.maps.LatLng(propertyLocation.lat, propertyLocation.lng);
        var distanceText = "";

        if (map === undefined) {
            map = new google.maps.Map(self.mapCanvas[0], { zoom: 13, center: propertyLatLng, mapTypeId: google.maps.MapTypeId.ROADMAP });
        }

        if (mapSubject) {
            var distanceFromSubject = google.maps.geometry.spherical.computeDistanceBetween(subjectLatLng, propertyLatLng);
            var heading = google.maps.geometry.spherical.computeHeading(subjectLatLng, propertyLatLng);
            var headingText = GetHeadingText(Math.round(heading));
            distanceFromSubject = Math.round((distanceFromSubject * 0.000621371192) * 10) / 10; // convert to miles and round to one decimal place
            distanceText = (distanceFromSubject === undefined || distanceFromSubject === null) ? "" : distanceFromSubject == 1.0 ? distanceFromSubject + " mile" : distanceFromSubject + " miles";
            distanceText += " " + headingText;
        }


        var marker = addMarkerToMap(propertyLatLng, "C", "Comparable\n" + propertyJson.results[0].formatted_address);

        attachInfoWindow(marker, getInfoWindowContent(buildAddressFromComponent(propertyJson.results[0].address_components), distanceText));

        //  And increase the bounds to take this point
        bounds.extend(marker.position);
        map.fitBounds(bounds);
        //}

    };

    var attachInfoWindow = function (marker, message) {
        var infowindow = new google.maps.InfoWindow({
            content: message
        });
        marker.addListener('click', function () {
            infowindow.open(map, marker);
        });
    };

    /* PRIVATE METHODS */
    /// Adds the property marker (pin) to the map
    var addMarkerToMap = function (markerLatLng, label, title) {
        var marker = new google.maps.Marker({
            position: markerLatLng,
            label: label,
            title: title,
            map: map,
            draggable: false
            //,icon: 'http://chart.apis.google.com/chart?chst=d_map_pin_letter&chld=A|FF0000|000000'
            //, icon: 'http://maps.google.com/mapfiles/ms/icons/blue.png'
        });

        return marker;
    };

    /// Adds a circle to the map (e.g., add a circle around the subject property)
    var addCircleToMap = function (marker, radiusInMeters, strokeColor, strokeOpacity, fillColor, fillOpacity) {
        var circle = new google.maps.Circle({
            map: map,
            radius: radiusInMeters,         // in meters
            strokeColor: strokeColor,
            strokeOpacity: strokeOpacity,
            strokeWeight: 1,
            fillColor: fillColor,
            fillOpacity: fillOpacity,
        });
        circle.bindTo('center', marker, 'position');
    };

    /// Adds property info to the marker (pin) onClick
    var getInfoWindowContent = function (address, distance) {
        var content = '<div class="property-info">' +
                      '<h2>Comparable Property</h2>' +
                      '<div class="property-photo"></div>' +
                      '<address>' + address + '</address>' +
                      '<div class="row">' +
                      '<label>MLS Number:</label>' +
                      '<span>1232</span>' +
                      '</div>' +
                      '<div class="row">' +
                      '<label>Tax ID:</label>' +
                      '<span>2132</span>' +
                      '</div>';
        if (distance != "") {
            content += '<div class="row">' +
                       '<label>' + distance + '</label>' +
                       '<span> from subject</span>' +
                       '</div>';
        }
        content += '</div>';

        return content;
    }

    /// Connect to Google Maps API and get the GeoCode info (in JSON format) on the address requested
    var mapPropertyFromGoogleJson = function (address, callback) {
        var googleGeoCodeUrl = 'http://maps.googleapis.com/maps/api/geocode/json?address=' + address + '&sensor=false';
        var propertyJson = {};

        var jqXHR = $.ajax({
            type: 'GET',
            dataType: 'json',
            url: googleGeoCodeUrl
            //success: function(data) { callback(data); }
        });

        jqXHR.done(function (data) {
            //propertyJson = data;
            callback(data); // maps the data
        });

    };

    /// Builds out the address into a presentable format from the Google GeoCode info
    var buildAddressFromComponent = function (components) {

        var streetNumber = getAddressComponent(components, "street_number"),
            street = getAddressComponent(components, "route"),
            city = getAddressComponent(components, "locality"),
            stateCode = getAddressComponent(components, "administrative_area_level_1"),
            postalCode = getAddressComponent(components, "postal_code"),
            postalCodeSuffix = getAddressComponent(components, "postal_code_suffix");
        if (!isNaN(postalCode) && !isNaN(postalCodeSuffix)) postalCode += "-" + postalCodeSuffix;

        return streetNumber + " " + street + ", " + city + ", " + stateCode + " " + postalCode;
    }

    /// Gets an address component from a Google GeoCode component by name
    var getAddressComponent = function (components, typeName) {

        for (var i = 0; i < components.length; i++) {
            if (components[i].types[0] === typeName) {
                return components[i].short_name;
                break;
            }
        }

        return "";
    }

    /// Gets the cardinal direction from heading
    var GetHeadingText = function (heading) {
        var headingText = "";

        if (heading === 0.0) headingText = "N";
        if (heading > 0 && heading < 90) headingText = "NE";
        if (heading === 90.0) headingText = "E";
        if (heading > 90 && heading < 180) headingText = "SE";
        if (heading === 180.0) headingText = "S";
        if (heading < 0 && heading > -90) headingText = "NW";
        if (heading === -90.0) headingText = "W";
        if (heading < -90 && heading > -180) headingText = "SW";

        return headingText;
    }

    return self;
}
