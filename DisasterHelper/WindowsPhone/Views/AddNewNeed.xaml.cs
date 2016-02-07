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
using Common;
using Common.Utils;

namespace WindowsPhone.Views
{
    public partial class AddNewNeed : PhoneApplicationPage
    {
        private string _centerId;

        public AddNewNeed()
        {
            InitializeComponent();
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            SystemTray.IsVisible = true;
            _centerId = NavigationContext.QueryString["CenterId"];
        }

        private async void SubmitButton_Click(object sender, RoutedEventArgs e)
        {
            DisableErrors();
            if (!CheckInput()) return;

            var need = new Needs
            {
                CenterId = _centerId,
                Description = txtDescription.Text
            };
            var error = false;
            try
            {
                await Service.InsertItemAsync(need);
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

            NavigationService.GoBack();
        }

        private void DisableErrors()
        {
            errorDescription.Visibility = Visibility.Collapsed;
        }

        private bool CheckInput()
        {
            var everythingAllRight = true;

            if (txtDescription.Text.IsNullOrEmptyOrWhiteSpace())
            {
                errorDescription.Visibility = Visibility.Visible;
                everythingAllRight = false;
            }

            return everythingAllRight;
        }
    }
}