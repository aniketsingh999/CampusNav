using CommunityToolkit.Mvvm.ComponentModel;
using Realms.Sync;
using Realms;
using CampusNav.Models;
using Microsoft.Maui.Maps;
using Microsoft.Maui.Controls.Maps;

namespace CampusNav.ViewModels
{
    [QueryProperty(nameof(User), "Data")]
    public partial class HomeViewModel : BaseViewModel
    {
        private Realm realm;
        private FlexibleSyncConfiguration config;
        private readonly Microsoft.Maui.ApplicationModel.IMap ext_map;
        private readonly IGeolocation geolocation;
        private readonly IConnectivity connectivity;

        public HomeViewModel(Microsoft.Maui.ApplicationModel.IMap map, IGeolocation geolocation, IConnectivity connectivity)
        {
            this.ext_map = map;
            this.geolocation = geolocation;
            this.connectivity = connectivity;
            Endlocation = "NA";
        }

        [ObservableProperty]
        UserInfo user;

        [ObservableProperty]
        string currentlocation;

        [ObservableProperty]
        string endlocation;

        [ObservableProperty]
        Location startlocation;

        [ObservableProperty]
        Location destination;

        [ObservableProperty]
        string locationInput;
        public async Task InitialiseRealm()
        {
            var cUser = App.RealmApp.CurrentUser;
            if (cUser == null)
            {
                await Application.Current.MainPage.DisplayAlert("Logged out", "User not logged in", "Close");
                return;
            }
            config = new FlexibleSyncConfiguration(cUser);
            realm = await Realm.GetInstanceAsync(config);

            realm.Subscriptions.Update(() =>
            {
                var currentUser = realm.All<UserInfo>().Where(t => t.Email == User.Email);
                realm.Subscriptions.Add(currentUser);
            });

            await realm.Subscriptions.WaitForSynchronizationAsync();

        }

        public async Task AddUser()
        {
            await InitialiseRealm();

            try
            {
                var oldUser = realm.All<UserInfo>().Where(t => t.Email == User.Email);
                if (!oldUser.Any())
                {
                    var newUser = new UserInfo
                    {
                        Name = User.Name,
                        Email = User.Email,
                        Password = User.Password,
                    };
                    realm.Write(() =>
                    {
                        realm.Add(newUser);
                    });

                }

            }
            catch (Exception ex)
            {
                await Application.Current.MainPage.DisplayAlert("Error", ex.Message, "Close");

            }

        }

        public async Task CheckLocation(Microsoft.Maui.Controls.Maps.Map map)
        {
            if (connectivity.NetworkAccess != NetworkAccess.Internet)
            {
                await Application.Current.MainPage.DisplayAlert("No Internet", "Oops! You seem to be offline", "Close");
                return;
            }

            var location = await geolocation.GetLastKnownLocationAsync();

            if (location == null)
            {
                location = await geolocation.GetLocationAsync(new GeolocationRequest
                {
                    DesiredAccuracy = GeolocationAccuracy.Best,
                    Timeout = TimeSpan.FromSeconds(10),
                    RequestFullAccuracy = true,
                });
            }

            if (location == null)
            {
                await Application.Current.MainPage.DisplayAlert("Error", "Failed to fetch location", "Close");
                return;
            }

            Startlocation = location;
            Currentlocation = await GetGeocodeReverseData(location.Latitude, location.Longitude);
            map.MoveToRegion(MapSpan.FromCenterAndRadius(location, Microsoft.Maui.Maps.Distance.FromMeters(100)));

            // Add pin for current location
            Pin currentPin = new()
            {
                Label = Currentlocation,
                Address = "You are here",
                Type = PinType.Generic,
                Location = location
            };
            map.Pins.Add(currentPin);
        }

        private async Task<string> GetGeocodeReverseData(double latitude, double longitude)
        {
            IEnumerable<Placemark> placemarks = await Geocoding.Default.GetPlacemarksAsync(latitude, longitude);

            Placemark place = placemarks?.FirstOrDefault();

            if (place != null)
            {
                return $"{place.FeatureName}, {place.SubLocality}, {place.Locality}, {place.AdminArea}, {place.CountryName} - {place.PostalCode}";

            }

            return "Location info not found";
        }

        public async Task HandleMapClick(Microsoft.Maui.Controls.Maps.Map map, double latitude, double longitude)
        {
            Endlocation = await GetGeocodeReverseData(latitude, longitude);
            Destination = new Location(latitude, longitude);
            map.MoveToRegion(MapSpan.FromCenterAndRadius(map.VisibleRegion.Center,
                Microsoft.Maui.Maps.Distance.FromMeters(500)));

            map.Pins.Clear();
            map.MapElements.Clear();

            // Add pin for current location
            Pin currentPin = new()
            {
                Label = Endlocation,
                Address = "Your final stop",
                Type = PinType.SearchResult,
                Location = Destination
            };
            map.Pins.Add(currentPin);
        }

        public async Task ShowDirections(Microsoft.Maui.Controls.Maps.Map map)
        {
            if (map == null || Currentlocation == "")
            {
                await Application.Current.MainPage.DisplayAlert("Location service failed!", "Oops! Failed to get location", "Close");
                return;
            }

            if (Endlocation == "")
            {
                await Application.Current.MainPage.DisplayAlert("Destination not selected", "Click on map to start journey", "Close");
                return;
            }

            Polyline polyline = new()
            {
                StrokeColor = Colors.Green,
                StrokeWidth = 12,
                Geopath =
                {
                    Startlocation,
                    Destination
                    
                }
            };

            // clear older lines
            map.MapElements.Clear();

            await ext_map.OpenAsync(Destination.Latitude, Destination.Longitude, new MapLaunchOptions
            {
                Name = "Destination",
                NavigationMode = NavigationMode.Walking
            });

        }
    }
}
