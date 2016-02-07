using WindowsStore.Common;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;
using WindowsStore.Utils;
using Common;
using Common.Tables.Disaster;
using Windows.UI.Popups;
using Bing.Maps;
using Windows.Devices.Geolocation;

// The Basic Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=234237

namespace WindowsStore.Views
{
    /// <summary>
    /// A basic page that provides characteristics common to most applications.
    /// </summary>
    public sealed partial class MainPage : Page
    {

        Geolocator geolocator;

        private NavigationHelper navigationHelper;
        private ObservableDictionary defaultViewModel = new ObservableDictionary();

        /// <summary>
        /// This can be changed to a strongly typed view model.
        /// </summary>
        public ObservableDictionary DefaultViewModel
        {
            get { return this.defaultViewModel; }
        }

        /// <summary>
        /// NavigationHelper is used on each page to aid in navigation and 
        /// process lifetime management
        /// </summary>
        public NavigationHelper NavigationHelper
        {
            get { return this.navigationHelper; }
        }

        MapLayer pinLayer;

        public MainPage()
        {
            this.InitializeComponent();
            this.navigationHelper = new NavigationHelper(this);
            this.navigationHelper.LoadState += navigationHelper_LoadState;
            this.navigationHelper.SaveState += navigationHelper_SaveState;
            pinLayer = new MapLayer();
            map.Children.Add(pinLayer);
        }
        private async void navigationHelper_LoadState(object sender, LoadStateEventArgs e)
        {
            var error = false;
            pageTitle.Text = "Hi, " + App.User.Fullname;
            try
            {
                var existingDisasters = await Service.GetTable<Disaster>()
                    .Where(t => t.Active == DisasterStatusType.Active).ToListAsync();
                lstDisasters.ItemsSource = existingDisasters;
                foreach(var item in existingDisasters)
                {
                    var pin = new Pushpin();
                    pin.Text = item.Description;
                    map.Children.Add(pin);
                    MapLayer.SetPosition(pin, new Location(item.Latitude, item.Longitude));
                }
            }
            catch (Exception ex)
            {
                error = true;
            }

            if(error)
            {
                await new MessageDialog("Something went wrong!").ShowAsync();
                return;
            }
            geolocator = new Geolocator();
            geolocator.DesiredAccuracy = PositionAccuracy.High;
            geolocator.DesiredAccuracyInMeters = 100;
            var location = await geolocator.GetGeopositionAsync();
            map.Center = new Location(location.Coordinate.Latitude, location.Coordinate.Longitude);
        }

        private void navigationHelper_SaveState(object sender, SaveStateEventArgs e)
        {
        }

        #region NavigationHelper registration

        /// The methods provided in this section are simply used to allow
        /// NavigationHelper to respond to the page's navigation methods.
        /// 
        /// Page specific logic should be placed in event handlers for the  
        /// <see cref="GridCS.Common.NavigationHelper.LoadState"/>
        /// and <see cref="GridCS.Common.NavigationHelper.SaveState"/>.
        /// The navigation parameter is available in the LoadState method 
        /// in addition to page state preserved during an earlier session.

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            navigationHelper.OnNavigatedTo(e);
        }

        protected override void OnNavigatedFrom(NavigationEventArgs e)
        {
            navigationHelper.OnNavigatedFrom(e);
        }

        #endregion

        private void LogoutButton_Click(object sender, RoutedEventArgs e)
        {
            var settings = new SettingsHelper();
            settings.DeleteAll();
            
            while(Frame.CanGoBack)
            {
                Frame.GoBack();
            }
        }

        private void UpdateProfileButton_Click(object sender, RoutedEventArgs e)
        {
            Frame.Navigate(typeof(UpdateAccountPage));
        }

        private void AddNewEventButton_Click(object sender, RoutedEventArgs e)
        {
            Frame.Navigate(typeof(AddNewEvent));
        }
    }
}
