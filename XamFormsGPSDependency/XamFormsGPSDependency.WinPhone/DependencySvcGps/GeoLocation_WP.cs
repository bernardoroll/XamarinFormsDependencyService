using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.Device.Location;
using Xamarin.Forms;
using Xamarin.Forms.Maps;

using XamFormsGPSDependency.DependencySvcGps;
using XamFormsGPSDependency.WinPhone.DependencySvcGps;

[assembly: Xamarin.Forms.Dependency(typeof(GeoLocation_WP))]


namespace XamFormsGPSDependency.WinPhone.DependencySvcGps
{
    public class GeoLocation_WP : IGeoLocation
    {
        Position pos;
        GeoCoordinateWatcher watcher;

        public GeoLocation_WP()
        {
            pos = new Position();
        }


        public Position GetCurrentPosition()
        {
            return pos;
        }


        public void InitLocationService()
        {
            watcher = new GeoCoordinateWatcher(GeoPositionAccuracy.High); // using high accuracy
            watcher.MovementThreshold = 20; // use MovementThreshold to ignore noise in the signal

            watcher.PositionChanged += watcher_PositionChanged;
            watcher.StatusChanged += watcher_StatusChanged;

            watcher.Start();
        }

        void watcher_StatusChanged(object sender, GeoPositionStatusChangedEventArgs e)
        {
            
        }

        void watcher_PositionChanged(object sender, GeoPositionChangedEventArgs<GeoCoordinate> e)
        {
            if ((pos.Latitude != e.Position.Location.Latitude) && (pos.Longitude != e.Position.Location.Longitude))
            {
                pos = new Position(e.Position.Location.Latitude, e.Position.Location.Longitude);
                MessagingCenter.Send<IGeoLocation, Position>(this, "LocationChanged", pos);
            } //end if
            
        }


        public void StopLocationService()
        {
            watcher.Stop();
        }

    }
}
