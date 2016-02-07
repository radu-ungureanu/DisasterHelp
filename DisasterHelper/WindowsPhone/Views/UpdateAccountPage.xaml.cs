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
using Common.Extensions;
using Common;
using Common.Utils;
using WindowsPhone.Utils;

namespace WindowsPhone.Views
{
    public partial class UpdateAccountPage : PhoneApplicationPage
    {
        public UpdateAccountPage()
        {
            InitializeComponent();
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            SystemTray.IsVisible = true;
            txtUsername.Text = App.User.Username;
            txtPassword.Password = App.User.Password;
            txtRepeatPassword.Password = App.User.Password;
            txtName.Text = App.User.Fullname;
            txtEmail.Text = App.User.Email;
            txtAddress.Text = App.User.Address;
            txtPhone.Text = App.User.PhoneNumber;
            cmbUserType.ItemsSource = UserTypeExtensions.GetUserTypes();
            cmbUserType.SelectedIndex = App.User.Type.AsInt();
        }

        private async void SaveButton_Click(object sender, EventArgs e)
        {
            DisableErrors();
            if (!CheckInput()) return;

            if (!txtPassword.Password.Equals(txtRepeatPassword.Password))
            {
                MessageBox.Show("The password doesn't match!");
                return;
            }

            var error = false;
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
                MessageBox.Show("Something went wrong!");
                return;
            }

            if (NavigationService.CanGoBack)
                NavigationService.GoBack();
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

        private void CancelButton_Click(object sender, EventArgs e)
        {
            NavigationService.GoBack();
        }
    }
}