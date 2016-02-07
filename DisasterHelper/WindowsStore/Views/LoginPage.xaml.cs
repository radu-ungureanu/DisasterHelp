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
using Windows.Networking.Connectivity;
using Windows.UI.Popups;
using Common;
using Common.Utils;
using WindowsStore.Utils;
using Common.Tables.User;
using System.Threading.Tasks;

// The Basic Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=234237

namespace WindowsStore.Views
{
    /// <summary>
    /// A basic page that provides characteristics common to most applications.
    /// </summary>
    public sealed partial class LoginPage : Page
    {

        private NavigationHelper navigationHelper;

        public NavigationHelper NavigationHelper
        {
            get { return this.navigationHelper; }
        }


        public LoginPage()
        {
            this.InitializeComponent();
            this.navigationHelper = new NavigationHelper(this);
            this.navigationHelper.LoadState += navigationHelper_LoadState;
            this.navigationHelper.SaveState += navigationHelper_SaveState;
        }
        private async void navigationHelper_LoadState(object sender, LoadStateEventArgs e)
        {
            var stack = Frame.GetNavigationState();

            if (!IsNetworkAvailable())
            {
                await new MessageDialog("Please check your internet connection!").ShowAsync();
            }

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
            //TODO: show progress bar
        }

        private static bool IsNetworkAvailable()
        {
            var connections = NetworkInformation.GetInternetConnectionProfile();
            if (connections == null)
            {
                return false;
            }
            return connections.GetNetworkConnectivityLevel() == NetworkConnectivityLevel.InternetAccess;
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

        private async void CreateNewAccountButton_Click(object sender, RoutedEventArgs e)
        {
            if (!IsNetworkAvailable())
            {
                await new MessageDialog("Please check your internet connection!").ShowAsync();
                return;
            }
            Frame.Navigate(typeof(CreateNewAccountPage));
        }

        private async void LoginButton_Click(object sender, RoutedEventArgs e)
        {
            DisableErrors();
            if (!IsNetworkAvailable())
            {
                await new MessageDialog("Please check your internet connection!").ShowAsync();
                return;
            }
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
            bool error = false;
            User user = null;

            try
            {
                user = (await Service.GetTable<User>()
                    .Where(t => t.Username == username
                        && t.Password == password).ToListAsync()).FirstOrDefault();
            }
            catch (Exception ex)
            {
                error = true;
            }

            if (error)
            {
                await new MessageDialog("Something went wrong!").ShowAsync();
                return;
            }

            if (user == null)
            {
                await new MessageDialog("The username, password combination does not match!").ShowAsync();
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

            switch (user.Type)
            {
                case UserType.Volunteer:
                    Frame.Navigate(typeof(MainPage));
                    break;
                case UserType.Medic:
                    Frame.Navigate(typeof(MainPage));
                    break;
                case UserType.Manager:
                    Frame.Navigate(typeof(MainPage));
                    break;
                default:
                    break;
            }
        }
    }
}
