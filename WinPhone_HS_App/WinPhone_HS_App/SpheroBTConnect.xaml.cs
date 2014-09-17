using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Shell;
using Sphero.Communication;

namespace WinPhone_HS_App
{
    public partial class SpheroBTConnect : PhoneApplicationPage
    {
        public SpheroBTConnect()
        {
            InitializeComponent();

            DiscoverSpheros();
        }

        private async void DiscoverSpheros()
        {
            try
            {
                // Discover paired Spheros
                List<SpheroInformation> spheroInformations = new List<SpheroInformation>(await SpheroConnectionProvider.DiscoverSpheros());

                if (spheroInformations != null && spheroInformations.Count > 0)
                {
                    // Populate list with Discovered Spheros
                    FoundSpheros.ItemsSource = spheroInformations;
                }
                else
                {
                    // No sphero Paired
                    MessageBox.Show("No sphero Paired");
                }
            }
            catch (BluetoothDeactivatedException)
            {
                MessageBox.Show("Bluetooth deactivated");

            }
        }

        private async void Connect_Button_Click(object sender, RoutedEventArgs e)
        {
            if (FoundSpheros.SelectedItem != null)
            {
                SpheroInformation ballInformation = (SpheroInformation)FoundSpheros.SelectedItem;
//                SpheroConnection connection = await SpheroConnectionProvider.CreateConnection(ballInformation);
                SpheroConnection connection = await SpheroConnectionProvider.CreateConnection(ballInformation);

                if (connection == null)
                {
                    MessageBox.Show("Connection Failed, Try Again");
                }
                else
                {
                    App.CurrentConnection = connection;
                    NavigationService.Navigate(new Uri("/HSControlPage.xaml", UriKind.RelativeOrAbsolute));
                }
            }
        }


        private void Find_Button_Click(object sender, RoutedEventArgs e)
        {
            DiscoverSpheros();
        }
       
    }
}