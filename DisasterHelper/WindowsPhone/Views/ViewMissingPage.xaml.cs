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

namespace WindowsPhone.Views
{
    public partial class ViewMissingPage : PhoneApplicationPage
    {
        private string _missingId;
        private Missing _missing;

        public ViewMissingPage()
        {
            InitializeComponent();
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            SystemTray.IsVisible = true;
            _missingId = NavigationContext.QueryString["MissingId"];
        }
    }
}