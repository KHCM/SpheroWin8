using WinPhone_HS_App.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Shell;
using Microsoft.WindowsAzure.MobileServices;

namespace WinPhone_HS_App
{
    public partial class SignUpPage : PhoneApplicationPage
    {
        private string _username;
        private string _usernamecon;
        private bool un2;
        private bool un1;

        private MobileServiceUser user;
        private IMobileServiceTable<Users> usersTable = App.MobileService.GetTable<Users>(); //TodoItem == TableName

        
        public SignUpPage()
        {
            InitializeComponent();
        }

        private void userName_TextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            _username = userName_TextBox.Text;
            un1 = true;
            canEnable();
        }

        private void userNameCon_TextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            _usernamecon = userNameCon_TextBox.Text;
            if (_usernamecon != _username)
            {
                un2 = false;
                //alert message
            }
            else
            {
                un2 = true;
                canEnable();
            }
        }

        private void Confrim_Button_Clicked(object sender, RoutedEventArgs e)
        {
            var userItem = new Users { UserName = _username, UserId = user.UserId };
            InsertUserSingleItem(userItem);
        }

        private async void InsertUserSingleItem(Users userItem)
        {
            MobileServiceInvalidOperationException exception = null;

            try
            {
                await usersTable.InsertAsync(userItem);
                NavigationService.Navigate(new Uri("/AppLandingPage.xaml", UriKind.RelativeOrAbsolute));
            }
            catch (MobileServiceInvalidOperationException e)
            {
                exception = e;
            }
        }

        private void canEnable()
        {
            if (_username != "" && un1 && _username != "" && un2)
            {
                Confirm_Button.IsEnabled = true;
                Confirm_Button_Cop.IsEnabled = false;
            }
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {

        }

        private void OrientationChanged(object sender, OrientationChangedEventArgs e)
        {
            Thickness marginT = LayoutRoot.Margin;
            // Switch the placement of the buttons based on an orientation change.
            if ((e.Orientation & PageOrientation.Portrait) == (PageOrientation.Portrait))
            {
                marginT = new Thickness(0);
            }
            // If not in portrait, move buttonList content to visible row and column.
            else
            {
                marginT = new Thickness(-70);
            }
        } 



    }
}