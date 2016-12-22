using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Device.Location;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace Ryan.AddressUtility.MSLocation
{
    class Program
    {
        public GeoCoordinate Subject { get; set; }
        public GeoCoordinate Comp { get; set; }

        static void Main(string[] args)
        {
            var subject = GetGeoCoordinateFromAddress("765 E Gordon Ave, Layton, UT 84041");
            //var comp = GetGeoCoordinateFromAddress("929 S Arbor Way, Layton, UT 84041");
            var comp = GetGeoCoordinateFromAddress("2939 N 725 W, Layton, UT 84041");

            //var direction = GetBearing(subject, comp);
            //Console.WriteLine("bearing: " + bearing);
            var direction = GetDirectionFromCoordinates(subject, comp);
            
            //GetDistance();
            var distance = GetDistanceBetweenProperties(subject, comp);
            var metersText = distance == 1.0 ? "meter" : "meters";
            Console.WriteLine(distance + " " + metersText + " " + direction);

            var distanceInMiles = ConvertMetersToMiles(distance);
            var milesText = distanceInMiles == 1.0 ? "mile" : "miles";
            Console.WriteLine(distanceInMiles + " " + milesText + " " + direction);


            // So the app doesn't close down in debug mode
            Console.WriteLine("Press any key to stop...");
            Console.ReadKey();

        }

        private static double ConvertMetersToMiles(double meters)
        {
            var distance = (meters / 1609.344);
            return Math.Round(distance, 2);
        }

        private static double GetDistanceBetweenProperties(GeoCoordinate subject, GeoCoordinate comp)
        {
            return subject.GetDistanceTo(comp);
        }

        private static GeoCoordinate GetGeoCoordinateFromAddress(string address)
        {
            var jsonResponse = GetGeoCodeFromBing(address.Replace(" ", "%20"));

            // Parse the response
            dynamic geo = (JObject.Parse(jsonResponse) as dynamic).resourceSets[0].resources[0];
            
            return new GeoCoordinate(geo.point.coordinates[0].Value, geo.point.coordinates[1].Value);
        }


        private static string GetGeoCodeFromBing(string address)
        {
            string apiKey = "Alt3gPG8fTsQ4-zl8x68BRF-nNPx9s4ho-U-oAU-tuo7jnKfkZYt_Cx-La0T533b";
            string url = "http://dev.virtualearth.net/REST/v1/Locations?" +
                                            "&q=" + address +
                                            "&maxResults=1" +
                                            "&key=" + apiKey;
            var request = WebRequest.Create(url) as HttpWebRequest;
            using (var response = request.GetResponse() as HttpWebResponse)
            {
                if (response.StatusCode != HttpStatusCode.OK)
                {
                    throw new Exception(String.Format(
                                    "Server error (HTTP {0}: {1}).",
                                    response.StatusCode,
                                    response.StatusDescription));
                }

                var resultJson = new StreamReader(response.GetResponseStream()).ReadToEnd();

                if (!string.IsNullOrEmpty(resultJson))
                {
                    return resultJson;

                }
            }
            return string.Empty;
        }

        private static void GetDistance()
        {
            var subjectLoc = new GeoCoordinate();
            var compLoc = new GeoCoordinate();

            subjectLoc.Latitude = -29.83245;
            subjectLoc.Longitude = 31.04034;

            compLoc.Latitude = -51.39792;
            compLoc.Longitude = -0.12084;

            var distance = subjectLoc.GetDistanceTo(compLoc);
        }

        private static string GetDirectionFromCoordinates(GeoCoordinate subject, GeoCoordinate comp)
        {
            var bearing = GetBearing(subject, comp);
            return DegreesToCardinal(bearing);
        }

        /// <summary>
        ///     Taken from http://stackoverflow.com/questions/2042599/direction-between-2-latitude-longitude-points-in-c-sharp
        /// </summary>
        /// <param name="subject"></param>
        /// <param name="comp"></param>
        /// <returns></returns>
        private static double GetBearing(GeoCoordinate subject, GeoCoordinate comp)
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
        private static string DegreesToCardinal(double degrees)
        {
            string[] caridnals = { "N", "NE", "E", "SE", "S", "SW", "W", "NW", "N" };
            return caridnals[(int)Math.Round(((double)degrees % 360) / 45)];
        }


        private static double ToRad(double degrees)
        {
            return degrees * (Math.PI / 180);
        }

        private static double ToDegrees(double radians)
        {
            return radians * 180 / Math.PI;
        }

        private static double ToBearing(double radians)
        {
            // convert radians to degrees (as bearing: 0...360)
            return (ToDegrees(radians) + 360) % 360;
        }



    }
}
