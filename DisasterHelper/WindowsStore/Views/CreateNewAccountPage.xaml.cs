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
using Common.Utils;
using Windows.UI.Popups;
using Common;

// The Basic Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=234237

namespace WindowsStore.Views
{
    /// <summary>
    /// A basic page that provides characteristics common to most applications.
    /// </summary>
    public sealed partial class CreateNewAccountPage : Page
    {

        private NavigationHelper navigationHelper;

        public NavigationHelper NavigationHelper
        {
            get { return this.navigationHelper; }
        }


        public CreateNewAccountPage()
        {
            this.InitializeComponent();
            this.navigationHelper = new NavigationHelper(this);
            this.navigationHelper.LoadState += navigationHelper_LoadState;
            this.navigationHelper.SaveState += navigationHelper_SaveState;
        }
        private void navigationHelper_LoadState(object sender, LoadStateEventArgs e)
        {
            cmbUserType.ItemsSource = UserTypeExtensions.GetUserTypes();
            cmbUserType.SelectedIndex = 0;
        }
        private void navigationHelper_SaveState(object sender, SaveStateEventArgs e)
        {
        }

        #region NavigationHelper registration
        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            navigationHelper.OnNavigatedTo(e);
        }

        protected override void OnNavigatedFrom(NavigationEventArgs e)
        {
            navigationHelper.OnNavigatedFrom(e);
        }

        #endregion

        private async void SubmitButton_Click(object sender, RoutedEventArgs e)
        {
            bool error = false;

            DisableErrors();
            if (!CheckInput()) return;

            if (!txtPassword.Password.Equals(txtRepeatPassword.Password))
            {
                await new MessageDialog("The password doesn't match!").ShowAsync();
                return;
            }

            var user = new User
            {
                Username = txtUsername.Text,
                Password = txtPassword.Password,
                Fullname = txtName.Text,
                Email = txtEmail.Text,
                Address = txtAddress.Text,
                PhoneNumber = txtPhone.Text,
                Type = (cmbUserType.SelectedItem as NameType).Type,
                ChanneltType = ChannelType.W8,
                ChannelUri = ""
            };
            try
            {
                var existingUser = await Service.GetTable<User>()
                    .Where(t => t.Username == txtUsername.Text).ToListAsync();
                if (existingUser.Count > 0)
                {
                    await new MessageDialog("Username already exists!").ShowAsync();
                    return;
                }
                await Service.InsertItemAsync(user);
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
            
            App.User = user;
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

        private void DisableErrors()
        {
            errorUsername.Visibility = Visibility.Collapsed;
            errorPassword.Visibility = Visibility.Collapsed;
            errorRepeatPassword.Visibility = Visibility.Collapsed;
            errorName.Visibility = Visibility.Collapsed;
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
