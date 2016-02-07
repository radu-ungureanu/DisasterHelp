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

namespace WindowsPhone.Views
{
    public partial class ViewUserProfile : PhoneApplicationPage
    {
        

        private string _userId;
        private User _user;

        public ViewUserProfile()
        {
            InitializeComponent();
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            SystemTray.IsVisible = true;
            _userId = NavigationContext.QueryString["UserId"];
        }
    }
}