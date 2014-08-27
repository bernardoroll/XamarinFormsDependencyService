using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Maps;

using XamFormsGPSDependency.DependencySvcTts;
using XamFormsGPSDependency.DependencySvcGps;
using XamFormsGPSDependency.ViewModel;


namespace XamFormsGPSDependency.Pages
{
    public class WhereAmIPage : ContentPage
    {
        LocationViewModel viewModel;
        Slider slider;
        Map map;


        public WhereAmIPage()
        {
            this.Title = "Where Am I?";


            this.BindingContext = viewModel = new LocationViewModel();
            this.Content = this.SetContent();

            MessagingCenter.Subscribe<IGeoLocation, Position>(this, "LocationChanged", (sender, arg) =>
            {
                this.LocationChanged();
            });
        }


        public View SetContent()
        {
            Button btnGo = new Button() 
            { 
                Text = "Let's Find Out",
                HorizontalOptions = LayoutOptions.Center,
            };
            btnGo.Clicked += btnGo_Clicked;


            Frame frameMap = new Frame() { Padding = 1, VerticalOptions = LayoutOptions.FillAndExpand };
            map = new Map(
                MapSpan.FromCenterAndRadius(
                    viewModel.Location, Distance.FromMiles(2000)))
                    {
                        VerticalOptions = LayoutOptions.FillAndExpand,
                    };
            frameMap.Content = map;

            slider = new Slider(1, 18, 9);
            slider.ValueChanged += slider_ValueChanged;

            StackLayout stackGPS = this.GetGPSLayout();
            StackLayout stackAddr = this.GetAddrLayout();

            StackLayout stack = new StackLayout()
            {
                VerticalOptions = LayoutOptions.FillAndExpand,
                Padding = new Thickness(10, Device.OnPlatform(0, 0, 0), 10, 10),
                Spacing = 5,
                Orientation = StackOrientation.Vertical,
                Children =
                {
                   btnGo,
                   stackGPS,
                   stackAddr,
                   frameMap,
                   slider,
                },

            };

            return stack;
        }

        void slider_ValueChanged(object sender, ValueChangedEventArgs e)
        {
            if (map.VisibleRegion != null)
            {
                double dblZoom = e.NewValue;
                var lat = viewModel.Location.Latitude / (Math.Pow(2, dblZoom));
                var lon = viewModel.Location.Longitude / (Math.Pow(2, dblZoom));
                map.MoveToRegion(new MapSpan(map.VisibleRegion.Center, lat, lon));
            }

        }

        void btnGo_Clicked(object sender, EventArgs e)
        {
            DependencyService.Get<ITextToSpeech>().Speak("Starting the GPS service.");
            viewModel.GetLocationCommand.Execute(null);
        } //end SetContent

        void tap_Addr(object sender, EventArgs e)
        {
            DependencyService.Get<ITextToSpeech>().Speak(viewModel.Address);
        }

        void tap_GPS(object sender, EventArgs e)
        {
            DependencyService.Get<ITextToSpeech>().Speak(viewModel.DisplayLocation);
        }

        private void LocationChanged()
        {
            DependencyService.Get<ITextToSpeech>().Speak("Your location has been found.");

            this.PlotLocation();
        }


        private void PlotLocation()
        {
            Pin pin = new Pin()
            {
                Type = PinType.Place,
                Position = viewModel.Location,
                Label = "You're Here!",
            };
            map.Pins.Add(pin);

            MapSpan ms = MapSpan.FromCenterAndRadius(viewModel.Location, Distance.FromMiles(1));
            map.MoveToRegion(ms);

        }


        private StackLayout GetGPSLayout()
        {
            Image img = new Image() { Source = "gps.png" };
            TapGestureRecognizer tap = new TapGestureRecognizer();
            tap.Tapped += tap_GPS;
            img.GestureRecognizers.Add(tap);

            StackLayout stackLabel = this.GetGPSLabel();

            StackLayout stack = new StackLayout()
            {
                Orientation = StackOrientation.Horizontal,
                Children = 
                { 
                    img,
                    stackLabel,
                },
            };

            return stack;
        }

        private StackLayout GetGPSLabel()
        {
            var lblHeader = new Label
            {
                HorizontalOptions = LayoutOptions.FillAndExpand,
                Text = "GPS Location",
                TextColor = Color.Red,
                Font = Device.OnPlatform(
                    iOS: Font.OfSize("GillSans-Light", 16),
                    Android: Font.OfSize("Droid Sans", 16),
                    WinPhone: Font.OfSize("Segoe UI", 18)
                ),
            };

            Label lblGPS = new Label
            {
                HorizontalOptions = LayoutOptions.FillAndExpand,
                TextColor = Color.Gray,
                Font = Device.OnPlatform(
                    iOS: Font.OfSize("GillSans-Light", 12),
                    Android: Font.OfSize("Droid Sans Mono", 14),
                    WinPhone: Font.OfSize("Segoe UI", 14)
                ),
            };
            lblGPS.SetBinding(Label.TextProperty, "DisplayLocation");

            StackLayout stack = new StackLayout()
            {
                Children = 
                {
                    lblHeader,
                    lblGPS,
                },
                Spacing = 0,
            };

            return stack;
        }


        private StackLayout GetAddrLayout()
        {
            Image img = new Image() { Source = "address.png" };
            TapGestureRecognizer tap = new TapGestureRecognizer();
            tap.Tapped += tap_Addr;
            img.GestureRecognizers.Add(tap);

            StackLayout stackLabel = this.GetAddrLabel();

            StackLayout stack = new StackLayout()
            {
                Orientation = StackOrientation.Horizontal,
                Children = 
                { 
                    img,
                    stackLabel,
                },
            };

            return stack;
        }

        private StackLayout GetAddrLabel()
        {
            var lblHeader = new Label
            {
                HorizontalOptions = LayoutOptions.FillAndExpand,
                Text = "Address",
                TextColor = Color.Red,
                Font = Device.OnPlatform(
                    iOS: Font.OfSize("GillSans-Light", 16),
                    Android: Font.OfSize("Droid Sans", 16),
                    WinPhone: Font.OfSize("Segoe UI", 18)
                ),
            };

            Label lblAddr = new Label
            {
                HorizontalOptions = LayoutOptions.FillAndExpand,
                TextColor = Color.Gray,
                Font = Device.OnPlatform(
                    iOS: Font.OfSize("GillSans-Light", 12),
                    Android: Font.OfSize("Droid Sans Mono", 14),
                    WinPhone: Font.OfSize("Segoe UI", 14)
                ),
            };
            lblAddr.SetBinding(Label.TextProperty, "Address");

            StackLayout stack = new StackLayout()
            {
                Children = 
                {
                    lblHeader,
                    lblAddr,
                },
                Spacing = 0,
            };

            return stack;
        }


    }
}
