using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Android.Util;
using Android.Locations;

using Xamarin.Forms;
using Xamarin.Forms.Maps;

using XamFormsGPSDependency.DependencySvcGps;
using XamFormsGPSDependency.Droid.DependencySvcGps;

[assembly: Xamarin.Forms.Dependency(typeof(GeoLocation_Android))]

namespace XamFormsGPSDependency.Droid.DependencySvcGps
{
    public class GeoLocation_Android : Java.Lang.Object, ILocationListener, IGeoLocation
    {
        Position pos;

        public GeoLocation_Android()
        {
            pos = new Position();
        } //end ctor

        public Position GetCurrentPosition()
        {
            _currentLocation = _locationManager.GetLastKnownLocation(_locationProvider);

            if (_currentLocation != null)
            {
                pos = new Position(_currentLocation.Latitude, _currentLocation.Longitude);
            }

            return pos;
        }


        Location _currentLocation;
        LocationManager _locationManager;

        string _locationProvider;

        public void OnLocationChanged(Location location)
        {
            _currentLocation = location;
            if (_currentLocation == null)
            {
               
            }
            else
            {
                pos = new Position(_currentLocation.Latitude, _currentLocation.Longitude);
                MessagingCenter.Send<IGeoLocation, Position>(this, "LocationChanged", pos);
            }
        }

        public void OnProviderDisabled(string provider)
        {
        }

        public void OnProviderEnabled(string provider)
        {
        }

        public void OnStatusChanged(string provider, Availability status, Bundle extras)
        {
            Console.WriteLine("{0}, {1}", provider, status);
        }


        public void InitLocationService()
        {
            var wrapper = new Android.Content.ContextWrapper(Forms.Context);

            _locationManager = (LocationManager) wrapper.GetSystemService(Context.LocationService);
            Criteria criteriaForLocationService = new Criteria
            {
                Accuracy = Accuracy.Fine
            };
            IList<string> acceptableLocationProviders = _locationManager.GetProviders(criteriaForLocationService, true);

            if (acceptableLocationProviders.Any())
            {
                _locationProvider = acceptableLocationProviders.First();
                _locationManager.RequestLocationUpdates(_locationProvider, 2000, 10, this);
            }
            else
            {
                _locationProvider = String.Empty;
            }
            Console.WriteLine("Using " + _locationProvider + ".");
        }


        public void StopLocationService()
        {
            _locationManager.RemoveUpdates(this);
        }

        //protected override void OnResume()
        //{
        //    base.OnResume();
        //    _locationManager.RequestLocationUpdates(_locationProvider, 0, 0, this);
        //    Log.Debug(LogTag, "Listening for location updates using " + _locationProvider + ".");
        //}

        //protected override void OnPause()
        //{
        //    base.OnPause();
        //    _locationManager.RemoveUpdates(this);
        //    Log.Debug(LogTag, "No longer listening for location updates.");
        //}


    }
}