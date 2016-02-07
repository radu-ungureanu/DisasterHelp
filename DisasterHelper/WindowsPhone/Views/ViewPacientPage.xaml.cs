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
    public partial class ViewPacientPage : PhoneApplicationPage
    {
        private string _pacientId;
        private Pacient _pacient;

        public ViewPacientPage()
        {
            InitializeComponent();
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            SystemTray.IsVisible = true;
            _pacientId = NavigationContext.QueryString["PacientId"];
        }
    }
}