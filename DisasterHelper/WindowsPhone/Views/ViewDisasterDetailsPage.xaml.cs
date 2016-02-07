using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Shell;
using Common;
using Common.Tables.Disaster;
using Microsoft.Phone.Controls.Maps;
using System.Device.Location;
using Windows.Devices.Geolocation;
using WindowsPhone.Utils;
using Common.Tables.User;
using System.Threading.Tasks;

namespace WindowsPhone.Views
{
    public partial class ViewDisasterDetailsPage : PhoneApplicationPage
    {
        Geolocator geolocator;
        private string _disasterId;
        private string _disasterName;
        private Disaster _disaster;

        public ViewDisasterDetailsPage()
        {
            InitializeComponent();
        }

        protected override async void OnNavigatedTo(NavigationEventArgs e)
        {
            SystemTray.IsVisible = true;
            _disasterId = NavigationContext.QueryString["DisasterId"];
            _disasterName = NavigationContext.QueryString["DisasterName"];

            pivot.Title = _disasterName;

            try
            {
                _disaster = (await Service.GetTable<Disaster>()
                    .Where(t => t.Id == _disasterId).ToListAsync()).FirstOrDefault();

                if (e.NavigationMode == NavigationMode.New)
                {
                    if (_disaster.InitiatorId == App.User.Id)
                        AddMarkAsInactiveButtonToAppBar();
                    else
                        await AddAttendButtonTopAppBar();
                }

                var items = await Service.GetTable<Center>()
                    .Where(t => t.DisasterId == _disasterId)
                    .ToListAsync();

                if (items.Count == 0)
                {
                    txtNoCenters.Visibility = Visibility.Visible;
                    txtNoInvolved.Visibility = Visibility.Visible;
                    txtNoMissing.Visibility = Visibility.Visible;
                    txtNoNeeds.Visibility = Visibility.Visible;
                    txtNoPacients.Visibility = Visibility.Visible;
                }
                else
                {
                    txtNoCenters.Visibility = Visibility.Collapsed;

                    lstCenters.ItemsSource = items;
                    foreach (var item in items)
                    {
                        var pin = new Pushpin()
                        {
                            Location = new GeoCoordinate(item.Latitude, item.Longitude),
                            Content = item.Name,
                            Tag = string.Format("{0}?CenterId={1}&CenterName={2}", PageConstants.ViewCenterDetailsPage, item.Id, item.Name)
                        };
                        pin.Tap += pin_Tap;
                        map.Children.Add(pin);
                    }
                }

                txtNoInvolved.Visibility = Visibility.Collapsed;
                txtNoMissing.Visibility = Visibility.Collapsed;
                txtNoNeeds.Visibility = Visibility.Collapsed;
                txtNoPacients.Visibility = Visibility.Collapsed;

                var missing = await Service.GetTable<Missing>().Where(t => t.DisasterId == _disasterId).ToListAsync();
                if (missing.Count == 0)
                    txtNoMissing.Visibility = Visibility.Visible;
                else lstMissing.ItemsSource = missing;

                var centers = items.Where(t => t.DisasterId == _disasterId).Select(t => t.Id);
                var needs = await Service.GetTable<Needs>().Where(t => centers.Contains(t.CenterId)).ToListAsync();
                if (needs.Count == 0)
                    txtNoNeeds.Visibility = Visibility.Visible;
                else
                    lstNeeds.ItemsSource = needs;

                var patients = await Service.GetTable<Pacient>().Where(t => centers.Contains(t.CenterId)).ToListAsync();
                if (patients.Count == 0)
                    txtNoPacients.Visibility = Visibility.Visible;
                else
                    lstPacients.ItemsSource = patients;

                var involved = await Service.GetTable<DisasterAttend>().Where(t => t.DisasterId == _disasterId).Select(t => t.ParticipantId).ToListAsync();
                var people = await Service.GetTable<User>().Where(t => involved.Contains(t.Id)).ToListAsync();
                if (people.Count == 0)
                    txtNoInvolved.Visibility = Visibility.Visible;
                else
                    lstInvolved.ItemsSource = people;

                staDetails.DataContext = _disaster;
                pivot.Title = _disaster.Country;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Something went wrong!");
            }

            geolocator = new Geolocator();
            geolocator.DesiredAccuracy = PositionAccuracy.High;
            geolocator.DesiredAccuracyInMeters = 100;
            var location = await geolocator.GetGeopositionAsync();
            map.Center = new GeoCoordinate(location.Coordinate.Latitude, location.Coordinate.Longitude);
        }

        private async Task AddAttendButtonTopAppBar()
        {
            var isParticipant = (await Service.GetTable<DisasterAttend>().Where(t => t.DisasterId == _disasterId && t.ParticipantId == App.User.Id).ToListAsync()).FirstOrDefault();
            if (isParticipant != null)
            {
                var button = new ApplicationBarIconButton
                {
                    IconUri = new Uri("/Assets/AppBar/favs.png", UriKind.RelativeOrAbsolute),
                    IsEnabled = true,
                    Text = "attend"
                };
                button.Click += async (a, b) =>
                    {
                        await Service.DeleteItemAsync(isParticipant);
                        ApplicationBar.Buttons.Remove(button);
                    };
                ApplicationBar.Buttons.Add(button);
            }
            else
            {
                var button = new ApplicationBarIconButton
                {
                    IconUri = new Uri("/Assets/AppBar/favs.png", UriKind.RelativeOrAbsolute),
                    IsEnabled = true,
                    Text = "attend"
                };
                button.Click += AttendButton_Click;
                ApplicationBar.Buttons.Add(button);
            }
        }

        async void AttendButton_Click(object sender, EventArgs e)
        {
            var disasterAttend = new DisasterAttend
            {
                ParticipantId = App.User.Id,
                DisasterId = _disasterId
            };
            await Service.InsertItemAsync(disasterAttend);
            ApplicationBar.Buttons.Remove(sender as ApplicationBarIconButton);
        }

        private void AddMarkAsInactiveButtonToAppBar()
        {
            if (_disaster.Active == DisasterStatusType.Active)
            {
                var button = new ApplicationBarIconButton
                {
                    IconUri = new Uri("/Assets/AppBar/close.png", UriKind.RelativeOrAbsolute),
                    IsEnabled = true,
                    Text = "mark inactive"
                };
                button.Click += MarkInactiveButton_Click;
                ApplicationBar.Buttons.Add(button);
            }
            else
            {
                var button = new ApplicationBarIconButton
                {
                    IconUri = new Uri("/Assets/AppBar/check.png", UriKind.RelativeOrAbsolute),
                    IsEnabled = true,
                    Text = "mark active"
                };
                button.Click += MarkActiveButton_Click;
                ApplicationBar.Buttons.Add(button);
            }
        }

        private async void MarkActiveButton_Click(object sender, EventArgs e)
        {
            _disaster.Active = DisasterStatusType.Active;
            await Service.UpdateItemAsync(_disaster);
            OnNavigatedTo(null);
        }

        async void MarkInactiveButton_Click(object sender, EventArgs e)
        {
            _disaster.Active = DisasterStatusType.Inactive;
            await Service.UpdateItemAsync(_disaster);
            OnNavigatedTo(null);
        }

        void pin_Tap(object sender, System.Windows.Input.GestureEventArgs e)
        {
            var center = sender as Pushpin;
            var uri = new Uri((string)center.Tag, UriKind.RelativeOrAbsolute);
            NavigationService.Navigate(uri);
        }

        private void Center_Tap(object sender, System.Windows.Input.GestureEventArgs e)
        {
            var center = (sender as StackPanel).DataContext as Center;
            var path = string.Format("{0}?CenterId={1}&CenterName={2}",
                PageConstants.ViewCenterDetailsPage,
                center.Id,
                center.Name);
            var uri = new Uri(path, UriKind.RelativeOrAbsolute);
            NavigationService.Navigate(uri);
        }

        private async void UpButton_Click(object sender, RoutedEventArgs e)
        {
            _disaster.RatingPlus++;
            staDetails.DataContext = null;
            staDetails.DataContext = _disaster;
            await Service.UpdateItemAsync(_disaster);
        }

        private async void DownButton_Click(object sender, RoutedEventArgs e)
        {
            _disaster.RatingMinus++;
            staDetails.DataContext = null;
            staDetails.DataContext = _disaster;
            await Service.UpdateItemAsync(_disaster);
        }

        private void AddCenterButton_Click(object sender, EventArgs e)
        {
            var path = string.Format("{0}?DisasterId={1}", PageConstants.AddNewCenterPage, _disasterId);
            var uri = new Uri(path, UriKind.RelativeOrAbsolute);
            NavigationService.Navigate(uri);
        }

        private void MissingPeople_Tap(object sender, System.Windows.Input.GestureEventArgs e)
        {
            var missing = (sender as StackPanel).DataContext as Missing;
            var path = string.Format("{0}?MissingId={1}", PageConstants.ViewMissingPage, missing.Id);
            var uri = new Uri(path, UriKind.RelativeOrAbsolute);
            NavigationService.Navigate(uri);
        }

        private void Pacient_Tap(object sender, System.Windows.Input.GestureEventArgs e)
        {
            var pacient = (sender as StackPanel).DataContext as Pacient;
            var path = string.Format("{0}?PacientId={1}", PageConstants.ViewPacientPage, pacient.Id);
            var uri = new Uri(path, UriKind.RelativeOrAbsolute);
            NavigationService.Navigate(uri);
        }

        private async void Need_Tap(object sender, System.Windows.Input.GestureEventArgs e)
        {
            var need = (sender as StackPanel).DataContext as Needs;
            var center = (await Service.GetTable<Center>().Where(t => t.Id == need.CenterId).ToListAsync()).FirstOrDefault();
            var path = string.Format("{0}?CenterId={1}&CenterName={2}", PageConstants.ViewCenterDetailsPage, need.CenterId, center.Name);
            var uri = new Uri(path, UriKind.RelativeOrAbsolute);
            NavigationService.Navigate(uri);
        }

        private void PeopleInvolved_Tap(object sender, System.Windows.Input.GestureEventArgs e)
        {
            var user = (sender as StackPanel).DataContext as User;
            var path = string.Format("{0}?UserId={1}", PageConstants.ViewUserProfile, user.Id);
            var uri = new Uri(path, UriKind.RelativeOrAbsolute);
            NavigationService.Navigate(uri);
        }

        private void AddMissingPerson_Click(object sender, EventArgs e)
        {
            var path = string.Format("{0}?DisasterId={1}", PageConstants.AddNewMissing, _disasterId);
            var uri = new Uri(path, UriKind.RelativeOrAbsolute);
            NavigationService.Navigate(uri);
        }
    }
}