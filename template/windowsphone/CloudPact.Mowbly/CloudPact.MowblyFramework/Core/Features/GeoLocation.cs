//-----------------------------------------------------------------------------------------
// <copyright file="GeoLocation.cs" company="CloudPact Software Technologies (P) Ltd.">
//     Copyright (c) 2011-2014 CloudPact. All rights reserved.
// </copyright>
// <author>Sathish(sathish@cloudpact.com)</author>
// <website>http://www.mowbly.com</website>
//-----------------------------------------------------------------------------------------

using CloudPact.MowblyFramework.Core.Log;
using Microsoft.Phone.Maps.Services;
using System;
using System.Collections.Generic;
using System.Device.Location;
using Windows.Devices.Geolocation;
using Newtonsoft.Json;

namespace CloudPact.MowblyFramework.Core.Features
{
    class GeoLocation : Feature
    {
        #region Feature

        /// <summary>
        /// Name of the feature
        /// </summary>
        internal override string Name { get { return Constants.FEATURE_GEOLOCATION; } }

        /// <summary>
        /// Invoke the method specified by the 
        /// <see cref="CloudPact.MowblyFramework.Core.Features.JSMessage">JSMessage</see> object
        /// </summary>
        /// <param name="message">
        /// <see cref="CloudPact.MowblyFramework.Core.Features.JSMessage">JSMessage</see> object
        /// </param>
        internal async override void InvokeAsync(JSMessage message)
        {
            string callbackId = message.CallbackId;
            switch (message.Method)
            {
                case "getCurrentPosition":

                    LocationOptions options =
                        JsonConvert.DeserializeObject<LocationOptions>(
                        message.Args[message.Args.Length - 1].ToString());

                    // Configure Geolocator
                    Geolocator geolocator = new Geolocator();
                    if (options.Accuracy)
                    {
                        geolocator.DesiredAccuracy = PositionAccuracy.High;
                        geolocator.MovementThreshold = 100;
                    }
                    else
                    {
                        geolocator.MovementThreshold = 1000;
                    }
                    
                    // Request for position
                    Geoposition position = await geolocator.GetGeopositionAsync(
                        new TimeSpan(0, 0, 0, 50),
                        timeout:TimeSpan.FromMilliseconds(options.Timeout));
                    Geocoordinate coordinate = position.Coordinate;

                    // Send result to JS layer
                    InvokeCallbackJavascript(message.CallbackId, new MethodResult 
                    { 
                        Result = GeocoordinateToMowblyCoordinate(coordinate) 
                    });

                    break;

                default:
                    Logger.Error("Feature " + Name + " does not support method " + message.Method);
                    break;
            }
        }

        #endregion

        #region GeoLocation

        private Dictionary<string, object> GeocoordinateToMowblyCoordinate(Geocoordinate coordinate)
        {
            Dictionary<string, object> result = new Dictionary<string, object>();

            // Read coordinate values
            double latitude = Convert.ToDouble(coordinate.Latitude);
            double longitude = Convert.ToDouble(coordinate.Longitude);
            double altitude = Convert.ToDouble(coordinate.Altitude);
            double accuracy = Convert.ToDouble(coordinate.Accuracy);
            double altitudeAccuracy = Convert.ToDouble(coordinate.AltitudeAccuracy);
            double heading = Convert.ToDouble(coordinate.Heading);
            double speed = Convert.ToDouble(coordinate.Speed);

            // Read timestamp
            DateTimeOffset locTime = coordinate.Timestamp;
            DateTime Jan1970 = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);
            TimeSpan ts = locTime - Jan1970;
            double timestamp = ts.TotalMilliseconds;

            // Create coords
            Dictionary<string, double> coords = new Dictionary<string, double>();
            if (!Double.IsNaN(latitude))
                coords.Add("latitude", latitude);
            if (!Double.IsNaN(longitude))
                coords.Add("longitude", longitude);
            if (!Double.IsNaN(altitude))
                coords.Add("altitude", altitude);
            if (!Double.IsNaN(accuracy))
                coords.Add("accuracy", accuracy);
            if (!Double.IsNaN(altitudeAccuracy))
                coords.Add("altitudeAccuracy", altitudeAccuracy);
            if (!Double.IsNaN(heading))
                coords.Add("heading", heading);
            if (!Double.IsNaN(speed))
                coords.Add("speed", speed);

            // Create result
            Dictionary<string, object> position = new Dictionary<string, object>();
            position.Add("coords", coords);
            position.Add("timestamp", timestamp);
            result.Add("position", position);

            return result;
        }
        
        #endregion
    }

    #region LocationOptions

    [JsonObject(MemberSerialization.OptIn)]
    public sealed class LocationOptions
    {
        const string KEY_ACCURACY = "enableHighAccuracy";
        const string KEY_TIMEOUT = "timeout";

        [JsonProperty(
            PropertyName = KEY_ACCURACY,
            Required = Required.Default)]
        public bool Accuracy { get; set; }

        [JsonProperty(
            PropertyName = KEY_TIMEOUT,
            Required = Required.Default)]
        public int Timeout { get; set; }
    }

    #endregion
}