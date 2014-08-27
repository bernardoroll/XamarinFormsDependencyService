using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel;

using Xamarin.Forms.Maps;
using Xamarin.Forms;

using XamFormsGPSDependency.DependencySvcGps;


namespace XamFormsGPSDependency.ViewModel
{
    public class LocationViewModel : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;
        private Command getLocationCommand;
        
        IGeoLocation geo;
        private Position pos;
        private string address;
        private string displayLoc;
        private bool isBusy;
        private int intLocCount;


        public LocationViewModel()
        {
            geo = DependencyService.Get<IGeoLocation>();
            pos = new Position(39.8282, -98.5795);
            address = "Waiting for GPS signal...";
            displayLoc = "Waiting for GPS signal...";

            isBusy = false;
            intLocCount = 0;

            MessagingCenter.Subscribe<IGeoLocation, Position>(this, "LocationChanged", (sender, arg) =>
            {
                this.LocationChanged((Position)arg);
            });
        }


        public Position Location
        {
            get
            {
                return pos;
            }
            set
            {
                pos = value;
                OnPropertyChanged("Location");
            }
        }


        public string DisplayLocation
        {
            get
            {
                return displayLoc;
            }
            set
            {
                displayLoc = value;
                OnPropertyChanged("DisplayLocation");
            }
        }

        public string Address
        {
            get
            {
                return address;
            }
            set
            {
                address = value;
                OnPropertyChanged("Address");
            }
        }


        private async Task GetLocation()
        {
            if (isBusy) return;
            isBusy = true;

            geo.InitLocationService();
        }

        
        private async Task LocationChanged(Position p)
        {
            intLocCount++;

            if (intLocCount < 2)
            {
                this.Location = p;
                this.DisplayLocation = String.Format("Lat: {0}, Long: {1}", p.Latitude.ToString(), p.Longitude.ToString());

                Geocoder gc = new Geocoder();
                this.Address = (await gc.GetAddressesForPositionAsync(p)).First().Replace(Environment.NewLine, ", ");
            }

            geo.StopLocationService();

            isBusy = false;
        }


        public Command GetLocationCommand
        {
            get
            {
                return getLocationCommand ?? (getLocationCommand = new Command(async () => await GetLocation()));
            }
        }


        protected void OnPropertyChanged(string propertyName)
        {
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
        }

    } 
}
