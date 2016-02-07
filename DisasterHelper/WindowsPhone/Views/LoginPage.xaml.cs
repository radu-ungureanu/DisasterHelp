using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Shell;
using WindowsPhone.Utils;
using Microsoft.Phone.Net.NetworkInformation;
using Common.Utils;
using Common;
using Common.Tables.User;
using System.Threading.Tasks;

namespace WindowsPhone.Views
{
    public partial class LoginPage : PhoneApplicationPage
    {
        public LoginPage()
        {
            InitializeComponent();
        }

        protected override async void OnNavigatedTo(NavigationEventArgs e)
        {
            SystemTray.IsVisible = true;
            if (!IsInternetConnection())
                return;

            var settings = new SettingsHelper();
            if (!settings.GetRememberMeFromSettings()) return;

            var username = settings.GetUsernameFromSettings();
            var password = settings.GetPasswordFromSettings();

            txtUsername.Text = username;
            txtPassword.Password = password;
            chkRemember.IsChecked = true;

            await Login(username, password, false);
        }

        private void ActivateLoading()
        {
            //TODO: show progress bar in UI for authentication
        }

        private static bool IsInternetConnection()
        {
            if (!DeviceNetworkInformation.IsNetworkAvailable)
            {
                MessageBox.Show("Please check your internet connection!");
                return false;
            }
            return true;
        }

        private void CreateAccountButton_Click(object sender, RoutedEventArgs e)
        {
            if (!IsInternetConnection())
                return;

            var uri = new Uri(PageConstants.CreateNewProfilePage, UriKind.RelativeOrAbsolute);
            NavigationService.Navigate(uri);
        }

        private async void LoginButton_Click(object sender, RoutedEventArgs e)
        {
            DisableErrors();
            if (!IsInternetConnection()) return;
            if (!CheckInput()) return;

            var username = txtUsername.Text;
            var password = txtPassword.Password;

            await Login(username, password, chkRemember.IsChecked.Value);
        }

        private void DisableErrors()
        {
            errorUsername.Visibility = Visibility.Collapsed;
            errorPassword.Visibility = Visibility.Collapsed;
        }

        private bool CheckInput()
        {
            var everythingAllRight = true;

            if (txtUsername.Text.IsNullOrEmptyOrWhiteSpace())
            {
                errorUsername.Visibility = Visibility.Visible;
                everythingAllRight = false;
            }
            if (txtPassword.Password.IsNullOrEmptyOrWhiteSpace())
            {
                errorPassword.Visibility = Visibility.Visible;
                everythingAllRight = false;
            }

            return everythingAllRight;
        }

        private async Task Login(string username, string password, bool rememberMe)
        {
            ActivateLoading();

            var error = false;
            User user = null;
            try
            {
                user = (await Service.GetTable<User>()
                    .Where(t => t.Username == username && t.Password == password).ToListAsync()).FirstOrDefault();
            }
            catch (Exception ex)
            {
                error = true;
            }

            if (error)
            {
                MessageBox.Show("Something went wrong!");
                return;
            }
            if (user == null)
            {
                MessageBox.Show("The username, password combination does not match!");
                return;
            }

            App.User = user;

            if (rememberMe)
            {
                var settings = new SettingsHelper();
                settings.SetUsernameToSettings(username);
                settings.SetPasswordToSettings(password);
                settings.SetRememberMeToSettings(true);
            }

            Uri uri = null;
            switch (user.Type)
            {
                case UserType.Volunteer:
                    uri = new Uri(PageConstants.MainPage, UriKind.RelativeOrAbsolute);
                    break;
                case UserType.Medic:
                    uri = new Uri(PageConstants.MainPage, UriKind.RelativeOrAbsolute);
                    break;
                case UserType.Manager:
                    uri = new Uri(PageConstants.MainPage, UriKind.RelativeOrAbsolute);
                    break;
            }
            NavigationService.Navigate(uri);
        }
    }
}