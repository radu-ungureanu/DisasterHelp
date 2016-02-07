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
using Windows.Devices.Geolocation;
using Common.Utils;
using Common.Tables.Disaster;
using Common;
using Windows.UI.Popups;

// The Basic Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=234237

namespace WindowsStore.Views
{
    public sealed partial class AddNewEvent: Page
    {

        private NavigationHelper navigationHelper;
        private ObservableDictionary defaultViewModel = new ObservableDictionary();

        public ObservableDictionary DefaultViewModel
        {
            get { return this.defaultViewModel; }
        }

        public NavigationHelper NavigationHelper
        {
            get { return this.navigationHelper; }
        }

        Geolocator geolocator;

        public AddNewEvent()
        {
            this.InitializeComponent();
            geolocator = new Geolocator();
            geolocator.StatusChanged += geolocator_StatusChanged;
            this.navigationHelper = new NavigationHelper(this);
            this.navigationHelper.LoadState += navigationHelper_LoadState;
            this.navigationHelper.SaveState += navigationHelper_SaveState;            
        }

        private async void geolocator_StatusChanged(Geolocator sender, StatusChangedEventArgs args)
        {
            if (args.Status == PositionStatus.Ready)
            {
                geolocator.DesiredAccuracy = PositionAccuracy.High;
                geolocator.DesiredAccuracyInMeters = 100;
                var location = await geolocator.GetGeopositionAsync();
                map.Center = new Bing.Maps.Location(location.Coordinate.Point.Position.Latitude, 
                                                    location.Coordinate.Point.Position.Longitude);
            }
        }

        private void navigationHelper_LoadState(object sender, LoadStateEventArgs e)
        {
            
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

        private async void AddNewEventButton_Click(object sender, RoutedEventArgs e)
        {
            DisableErrors();
            if (!CheckInput()) return;

            var disaster = new Disaster
            {
                Active = DisasterStatusType.Active,
                Country = txtCountry.Text,
                Description = txtDescription.Text,
                InitiatorId = App.User.Id,
                InitiatorName = App.User.Fullname,
                Latitude = map.Center.Latitude,
                Longitude = map.Center.Longitude,
                RatingMinus = 0,
                RatingPlus = 0
            };

            var error = false;
            try
            {
                await Service.InsertItemAsync(disaster);
            }
            catch (Exception ex)
            {
                error = true;
            }
            if (error)
            {
                await new MessageDialog("Something went wrong!").ShowAsync();
            }
            Frame.GoBack();
        }

        private void DisableErrors()
        {
            errDescription.Visibility = Visibility.Collapsed;
            errCountry.Visibility = Visibility.Collapsed;
        }

        private bool CheckInput()
        {
            var everythingAllRight = true;
            if (txtDescription.Text.IsNullOrEmptyOrWhiteSpace())
            {
                errDescription.Visibility = Visibility.Visible;
                everythingAllRight = false;
            }
            if (txtCountry.Text.IsNullOrEmptyOrWhiteSpace())
            {
                errCountry.Visibility = Visibility.Visible;
                everythingAllRight = false;
            }

            return everythingAllRight;
        }
    }
}
