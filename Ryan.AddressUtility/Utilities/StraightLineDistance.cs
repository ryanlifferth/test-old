using System;
using System.Collections.Generic;
using System.Device.Location;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Ryan.AddressUtility.Models;
using Ryan.AddressUtility.Interfaces;

namespace Ryan.AddressUtility.Utilities
{
    public class StraightLineDistance
    {

        #region Constructors
        #endregion

        #region Methods

        public DistanceResponse GetProximitiesByAddress(Address subjectAddress, List<Address> destinationAddresses, IGeocodeAddress geoCodingRepository)
        {
            if (subjectAddress == null || !destinationAddresses.Any())
            {
                return new DistanceResponse { StatusCode = 1, StatusMessage = "Origin Address and/or Destination Address(es) are missing." };
            }

            var response = new DistanceResponse
            {
                OriginAddress = subjectAddress,
                GeocodedOriginAddress = AddressHasGeoCoordinates(subjectAddress) ?
                                            subjectAddress :
                                            geoCodingRepository.GeocodeAddress(subjectAddress) // Only geocode if lat/long ARE NOT present
            };

            var originGeoCoordinate = new GeoCoordinate(Double.Parse(response.GeocodedOriginAddress.Latitude, CultureInfo.InvariantCulture), Double.Parse(response.GeocodedOriginAddress.Longitude, CultureInfo.InvariantCulture));
            foreach (var compAddress in destinationAddresses)
            {
                if (compAddress == null) continue;

                var destination = new Destination
                {
                    Address = compAddress,
                    GeocodedAddress = AddressHasGeoCoordinates(compAddress) ?
                                compAddress :
                                geoCodingRepository.GeocodeAddress(compAddress)
                };

                var compGeoCoordinate = new GeoCoordinate(Double.Parse(destination.GeocodedAddress.Latitude, CultureInfo.InvariantCulture), Double.Parse(destination.GeocodedAddress.Longitude, CultureInfo.InvariantCulture));
                var bearing = GetBearing(originGeoCoordinate, compGeoCoordinate);
                destination.DistanceFromOrigin = DegreesToCardinal(bearing);
            }

            return new DistanceResponse();
        }

        public DistanceResponse GetProximitiesByDestination(Address subjectAddress, List<Destination> destinationProperties, IGeocodeAddress geoCodingRepository)
        {
            if (subjectAddress == null || destinationProperties == null)
            {
                return new DistanceResponse { StatusCode = 1, StatusMessage = "Origin Address and/or Destination Address(es) are missing." };
            };

            var response = new DistanceResponse
            {
                OriginAddress = subjectAddress,
                GeocodedOriginAddress = AddressHasGeoCoordinates(subjectAddress) ?
                                            subjectAddress :
                                            geoCodingRepository.GeocodeAddress(subjectAddress) // Only geocode if lat/long ARE NOT present
            };

            var originGeoCoordinate = new GeoCoordinate(Double.Parse(response.GeocodedOriginAddress.Latitude, CultureInfo.InvariantCulture), Double.Parse(response.GeocodedOriginAddress.Longitude, CultureInfo.InvariantCulture));
            var destinations = new List<Destination>();
            foreach (var comp in destinationProperties)
            {
                if (comp == null || comp.Address == null) continue;

                var destination = new Destination
                {
                    Address = comp.Address,
                    GeocodedAddress = AddressHasGeoCoordinates(comp.Address) ?
                                comp.Address :
                                geoCodingRepository.GeocodeAddress(comp.Address),
                    Apn = comp.Apn,
                    MlsNumber = comp.MlsNumber
                };

                var compGeoCoordinate = new GeoCoordinate(Double.Parse(destination.GeocodedAddress.Latitude, CultureInfo.InvariantCulture), Double.Parse(destination.GeocodedAddress.Longitude, CultureInfo.InvariantCulture));
                destination.DistanceFromOrigin = GetDistanceBetweenProperties(originGeoCoordinate, compGeoCoordinate);

                destinations.Add(destination);
            }
            response.Destinations = destinations;

            return response;
        }

        public DistanceResponse GetProximitiesByGeoCoordinate(GeoCoordinate subjectCoordinate, List<GeoCoordinate> destinationCoordinates)
        {
            return new DistanceResponse();
        }

        #region Private Methods

        private bool AddressHasGeoCoordinates(Address address)
        {
            if (string.IsNullOrEmpty(address.Latitude) && string.IsNullOrEmpty(address.Longitude))
            {
                return false;
            }
            else
            {
                return true;
            }
        }

        #region Compass Bearing Methods (TODO:  Extract to individual class)
        /// <summary>
        ///     Taken from http://stackoverflow.com/questions/2042599/direction-between-2-latitude-longitude-points-in-c-sharp
        /// </summary>
        /// <param name="subject"></param>
        /// <param name="comp"></param>
        /// <returns></returns>
        private double GetBearing(GeoCoordinate subject, GeoCoordinate comp)
        {
            var dLon = ToRad(comp.Longitude - subject.Longitude);
            var dPhi = Math.Log(
                Math.Tan(ToRad(comp.Latitude) / 2 + Math.PI / 4) / Math.Tan(ToRad(subject.Latitude) / 2 + Math.PI / 4));
            if (Math.Abs(dLon) > Math.PI)
                dLon = dLon > 0 ? -(2 * Math.PI - dLon) : (2 * Math.PI + dLon);
            return ToBearing(Math.Atan2(dLon, dPhi));
        }

        /// <summary>
        ///     Taken from http://www.themethodology.net/2013/12/how-to-convert-degrees-to-cardinal.html
        /// </summary>
        /// <param name="degrees"></param>
        /// <returns></returns>
        private string DegreesToCardinal(double degrees)
        {
            string[] caridnals = { "N", "NE", "E", "SE", "S", "SW", "W", "NW", "N" };
            return caridnals[(int)Math.Round(((double)degrees % 360) / 45)];
        }

        private double ToRad(double degrees)
        {
            return degrees * (Math.PI / 180);
        }

        private double ToDegrees(double radians)
        {
            return radians * 180 / Math.PI;
        }

        private double ToBearing(double radians)
        {
            // convert radians to degrees (as bearing: 0...360)
            return (ToDegrees(radians) + 360) % 360;
        }

        #endregion

        #region Straight Line Distance Methods (TODO:  Extract to individual class)

        private string GetDistanceBetweenProperties(GeoCoordinate origin, GeoCoordinate destination)
        {
            // Get the compas bearing
            var bearingValue = GetBearing(origin, destination);
            var compassBearing = DegreesToCardinal(bearingValue);

            // Get the actual distance
            var distanceInMeters = CalculateDistanceBetweenProperties(origin, destination);
            var distanceInMiles = ConvertMetersToMiles(distanceInMeters);
            var milesText = distanceInMiles == 1.0 ? "mile" : "miles";

            return $"{distanceInMiles} {milesText} {compassBearing}";
        }

        private double CalculateDistanceBetweenProperties(GeoCoordinate subject, GeoCoordinate comp)
        {
            return subject.GetDistanceTo(comp);
        }

        private double ConvertMetersToMiles(double meters)
        {
            var distance = (meters / 1609.344);
            return Math.Round(distance, 2);
        }


        #endregion

        #endregion

        #endregion

    }

}
