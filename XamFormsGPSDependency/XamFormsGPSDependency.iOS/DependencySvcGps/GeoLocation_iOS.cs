using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;

using MonoTouch.Foundation;
using MonoTouch.UIKit;
using MonoTouch.CoreLocation;
using Xamarin.Forms;
using Xamarin.Forms.Maps;

using XamFormsGPSDependency.DependencySvcGps;
using XamFormsGPSDependency.iOS.DependencySvcGps;

[assembly: Xamarin.Forms.Dependency(typeof(GeoLocation_iOS))]

namespace XamFormsGPSDependency.iOS.DependencySvcGps
{
    public class GeoLocation_iOS : IGeoLocation
    {
        private CLLocationManager LocMgr;
        Position pos;

        public GeoLocation_iOS()
        {
            LocMgr = new CLLocationManager();
            pos = new Position();
        }


        public Position GetCurrentPosition()
        {
            return pos;
        }
        

        public void InitLocationService()
        {
            // We need the user's permission for our app to use the GPS in iOS. This is done either by the user accepting
            // the popover when the app is first launched, or by changing the permissions for the app in Settings

            if (CLLocationManager.LocationServicesEnabled)
            {

                LocMgr.DesiredAccuracy = 10; // sets the accuracy that we want in meters

                // Location updates are handled differently pre-iOS 6. If we want to support older versions of iOS,
                // we want to do perform this check and let our LocationManager know how to handle location updates.

                if (UIDevice.CurrentDevice.CheckSystemVersion(6, 0))
                {

                    LocMgr.LocationsUpdated += (object sender, CLLocationsUpdatedEventArgs e) =>
                    {
                        // fire our custom Location Updated event
                        //this.LocationUpdated(this, new LocationUpdatedEventArgs(e.Locations[e.Locations.Length - 1]));

                        CLLocation loc = e.Locations[e.Locations.Length - 1];

                        if ((loc.Coordinate.Latitude != pos.Latitude) && (loc.Coordinate.Longitude != pos.Longitude))
                        {
                            pos = new Position(loc.Coordinate.Latitude, loc.Coordinate.Longitude);
                            MessagingCenter.Send<IGeoLocation, Position>(this, "LocationChanged", pos);
                        }
                    };

                }
                else
                {

                    // this won't be called on iOS 6 (deprecated). We will get a warning here when we build.
                    LocMgr.UpdatedLocation += (object sender, CLLocationUpdatedEventArgs e) =>
                    {
                        //this.LocationUpdated(this, new LocationUpdatedEventArgs(e.NewLocation));

                        CLLocation loc = e.NewLocation;
                        if ((loc.Coordinate.Latitude != pos.Latitude) && (loc.Coordinate.Longitude != pos.Longitude))
                        {
                            pos = new Position(loc.Coordinate.Latitude, loc.Coordinate.Longitude);
                            MessagingCenter.Send<IGeoLocation, Position>(this, "LocationChanged", pos);
                        }
                    };
                }

                // Start our location updates
                LocMgr.StartUpdatingLocation();

                // Get some output from our manager in case of failure
                LocMgr.Failed += (object sender, NSErrorEventArgs e) =>
                {
                    Console.WriteLine(e.Error);
                };

            }
            else
            {

                //Let the user know that they need to enable LocationServices
                Console.WriteLine("Location services not enabled, please enable this in your Settings");

            }
        }


        public void StopLocationService()
        {
            LocMgr.StopUpdatingLocation();
        }


    }
}