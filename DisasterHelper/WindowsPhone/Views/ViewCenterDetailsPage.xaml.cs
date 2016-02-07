using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Shell;
using Common.Tables.Disaster;
using Windows.Devices.Geolocation;
using Common;
using System.Device.Location;
using Microsoft.Phone.Controls.Maps;
using WindowsPhone.Utils;
using Common.Tables.User;
using System.Threading.Tasks;

namespace WindowsPhone.Views
{
    public partial class ViewCenterDetailsPage : PhoneApplicationPage
    {
        private string _centerId;
        private string _centerName;
        private Center _center;

        public ViewCenterDetailsPage()
        {
            InitializeComponent();
        }

        protected override async void OnNavigatedTo(NavigationEventArgs e)
        {
            SystemTray.IsVisible = true;
            _centerId = NavigationContext.QueryString["CenterId"];
            _centerName = NavigationContext.QueryString["CenterName"];

            pivot.Title = _centerName;

            try
            {
                txtNoPacients.Visibility = Visibility.Collapsed;
                txtNoNeeds.Visibility = Visibility.Collapsed;

                _center = (await Service.GetTable<Center>()
                    .Where(t => t.Id == _centerId).ToListAsync()).FirstOrDefault();

                if (e.NavigationMode == NavigationMode.New)
                {
                    var isAttending = (await Service.GetTable<DisasterAttend>()
                    .Where(t => t.DisasterId == _center.DisasterId && t.ParticipantId == App.User.Id)
                    .ToListAsync()).FirstOrDefault();

                    if (App.User.Type == UserType.Medic && isAttending != null)
                    {
                        var button = new ApplicationBarIconButton
                        {
                            IconUri = new Uri("/Assets/AppBar/add.png", UriKind.RelativeOrAbsolute),
                            IsEnabled = true,
                            Text = "add patient"
                        };
                        button.Click += AddPatient_Click;
                        ApplicationBar.Buttons.Add(button);
                    }
                }

                var pin = new Pushpin()
                {
                    Location = new GeoCoordinate(_center.Latitude, _center.Longitude),
                    Content = _centerName,
                };
                map.Children.Add(pin);
                map.Center = new GeoCoordinate(_center.Latitude, _center.Longitude);

                var patients = await Service.GetTable<Pacient>().Where(t => t.CenterId == _centerId).ToListAsync();
                if (patients.Count == 0)
                    txtNoPacients.Visibility = Visibility.Visible;
                else lstPacients.ItemsSource = patients;

                var needs = await Service.GetTable<Needs>().Where(t => t.CenterId == _centerId).ToListAsync();
                if (needs.Count == 0)
                    txtNoNeeds.Visibility = Visibility.Visible;
                else 
                    lstNeeds.ItemsSource = needs;

            }
            catch (Exception ex)
            {
                MessageBox.Show("Something went wrong!");
            }
        }

        private async Task AddPatientButtonToAppBar()
        {

        }

        private void Pacient_Tap(object sender, System.Windows.Input.GestureEventArgs e)
        {
            var pacient = (sender as StackPanel).DataContext as Pacient;
            var path = string.Format("{0}?PacientId={1}", PageConstants.ViewPacientPage, pacient.Id);
            var uri = new Uri(path, UriKind.RelativeOrAbsolute);
            NavigationService.Navigate(uri);
        }

        private void AddPatient_Click(object sender, EventArgs e)
        {
            var path = string.Format("{0}?CenterId={1}", PageConstants.AddNewPatient, _centerId);
            var uri = new Uri(path, UriKind.RelativeOrAbsolute);
            NavigationService.Navigate(uri);
        }

        private void AddNeed_Click(object sender, EventArgs e)
        {
            var path = string.Format("{0}?CenterId={1}", PageConstants.AddNewNeed, _centerId);
            var uri = new Uri(path, UriKind.RelativeOrAbsolute);
            NavigationService.Navigate(uri);
        }
    }
}