using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Shell;
using Common.Tables.User;
using Common.Utils;
using Common;
using WindowsPhone.Utils;

namespace WindowsPhone.Views
{
    public partial class CreateNewAccountPage : PhoneApplicationPage
    {
        public CreateNewAccountPage()
        {
            InitializeComponent();
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            SystemTray.IsVisible = true;
            cmbUserType.ItemsSource = UserTypeExtensions.GetUserTypes();
        }

        private async void SubmitButton_Click(object sender, RoutedEventArgs e)
        {
            DisableErrors();
            if (!CheckInput()) return;

            if (!txtPassword.Password.Equals(txtRepeatPassword.Password))
            {
                MessageBox.Show("The password doesn't match!");
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
                ChannelUri = "",
                ChanneltType = ChannelType.WP8
            };
            var error = false;
            try
            {
                var existingUsers = await Service.GetTable<User>()
                    .Where(t => t.Username == txtUsername.Text).ToListAsync();
                if (existingUsers.Count > 0)
                {
                    MessageBox.Show("Username already exists!");
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
                MessageBox.Show("Something went wrong!");
                return;
            }

            App.User = user;
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