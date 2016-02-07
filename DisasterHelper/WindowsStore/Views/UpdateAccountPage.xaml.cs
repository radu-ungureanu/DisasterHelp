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
using Common.Tables.User;
using Common.Extensions;
using Common.Utils;
using Windows.UI.Popups;
using Common;
using WindowsStore.Utils;

// The Basic Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=234237

namespace WindowsStore.Views
{
    /// <summary>
    /// A basic page that provides characteristics common to most applications.
    /// </summary>
    public sealed partial class UpdateAccountPage : Page
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
        public UpdateAccountPage()
        {
            this.InitializeComponent();
            this.navigationHelper = new NavigationHelper(this);
            this.navigationHelper.LoadState += navigationHelper_LoadState;
            this.navigationHelper.SaveState += navigationHelper_SaveState;
        }
        private void navigationHelper_LoadState(object sender, LoadStateEventArgs e)
        {
            cmbUserType.ItemsSource = UserTypeExtensions.GetUserTypes();
            cmbUserType.SelectedIndex = App.User.Type.AsInt();
            txtUsername.Text = App.User.Username;
            txtPassword.Password = App.User.Password;
            txtRepeatPassword.Password = App.User.Password;
            txtName.Text = App.User.Fullname;
            txtEmail.Text = App.User.Email;
            txtPhone.Text = App.User.PhoneNumber;
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

        private async void UpdateButton_Click(object sender, RoutedEventArgs e)
        {
            bool error = false;

            DisableErrors();
            if (!CheckInput()) return;

            if (!txtPassword.Password.Equals(txtRepeatPassword.Password))
            {
                await new MessageDialog("Passwords do not match!").ShowAsync();
                return;
            }
            try
            {
                var existingUser = (await Service.GetTable<User>()
                    .Where(t => t.Id == App.User.Id).ToListAsync()).FirstOrDefault();
                existingUser.Password = txtPassword.Password;
                existingUser.Fullname = txtName.Text;
                existingUser.Email = txtEmail.Text;
                existingUser.Address = txtAddress.Text;
                existingUser.PhoneNumber = txtPhone.Text;
                existingUser.Type = (cmbUserType.SelectedItem as NameType).Type;
                await Service.UpdateItemAsync(existingUser);
                App.User = existingUser;

                var settings = new SettingsHelper();
                if (settings.GetRememberMeFromSettings())
                {
                    settings.SetPasswordToSettings(existingUser.Password);
                }
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
            Frame.GoBack();
        }

        private void DisableErrors()
        {
            errorPassword.Visibility = Visibility.Collapsed;
            errorRepeatPassword.Visibility = Visibility.Collapsed;
            errorName.Visibility = Visibility.Collapsed;
        }

        private bool CheckInput()
        {
            var everythingAllRight = true;

            if (txtPassword.Password.IsNullOrEmptyOrWhiteSpace())
            {
                errorPassword.Visibility = Visibility.Visible;
                everythingAllRight = false;
            }
            if (txtRepeatPassword.Password.IsNullOrEmptyOrWhiteSpace())
            {
                errorRepeatPassword.Visibility = Visibility.Visible;
                everythingAllRight = false;
            }
            if (txtName.Text.IsNullOrEmptyOrWhiteSpace())
            {
                errorName.Visibility = Visibility.Visible;
                everythingAllRight = false;
            }

            return everythingAllRight;
        }
    }
}
