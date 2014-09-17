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
using WinPhone_HS_App.Model;
using Windows.UI.Core;
using SRHS2backend;
using System.Diagnostics;

namespace WinPhone_HS_App
{
    public partial class LoginPage : PhoneApplicationPage
    {
        private MobileServiceCollection<Users, Users> usersList; //GamesObject replace TodoItem
        private IMobileServiceTable<Users> usersTable = App.MobileService.GetTable<Users>(); //TodoItem == TableName

        private MobileServiceUser user;
        private bool _ismember = false;
        private string message;
        private bool called = false;


        public LoginPage()
        {
            InitializeComponent();
            //StartSignalRListener();
            App.Current.AppUser = new User();
            App.Current.AllGames = new List<Game>();
        }

        protected override void OnNavigatedTo(System.Windows.Navigation.NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);
            App.Current.SignalRHub.SignalRServerNotification += new SignalRServerHandler(SignalRHub_SignalRServerNotification);


        }


        private async void Twitter_Button_Click(object sender, RoutedEventArgs e)
        {
            //TEST CODE
            /*
            try
            {
                NavigationService.Navigate(new Uri("/HubPage1.xaml", UriKind.RelativeOrAbsolute));

            }
            catch (Exception ex)
            {
                Debug.WriteLine("NAV TO HS CONTROL BROKE: " + ex.Message);
            }
            */
            await Authenticate();
            isAMember();
        }

        private void StartSignalRListener()
        {
            App.Current.SignalRHub.SignalRServerNotification += new SignalRServerHandler(SignalRHub_SignalRServerNotification);

        }


        private async System.Threading.Tasks.Task Authenticate()
        {
            while (user == null)
            {
                try
                {
                    user = await App.MobileService
                        .LoginAsync(MobileServiceAuthenticationProvider.Twitter);
                   
                }
                catch (InvalidOperationException)
                {
                    message = "You must log in. Login Required";
                    MessageBox.Show(message);
                }
            }
            
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            NavigationService.Navigate(new Uri("/MainPage.xaml", UriKind.RelativeOrAbsolute));

        }

        private async void isAMember()
        {
            MobileServiceInvalidOperationException exception = null;
            try
            {
                // This code refreshes the entries in the list view by querying the TodoItems table.
                // The query excludes completed TodoItems
                var match = await usersTable.Where(users => users.UserId == user.UserId).ToEnumerableAsync();
                if (match.Count() < 1)
                {
                    _ismember = false; 
                }
                else
                {
                    var matchItem = match.Single();
                    if (matchItem != null)
                        _ismember = true;
                    else
                        _ismember = false;

                }
                if (_ismember)
                {
                    // message = "Success! You are now logged in";
                    // MessageBox.Show(message);
                    CreateUserCallLogin();
                    //NavigationService.Navigate(new Uri("/HubPage1.xaml", UriKind.RelativeOrAbsolute));
                    //ListItems.ItemsSource = items; // XAML code
                }
                else
                {
                    //message = "It's your first time logging in. SignUp!";
                    //MessageBox.Show(message);
                    //NavigationService.Navigate(new Uri(string.Format("/SignUpPage.xaml?parameter={0}", user), UriKind.RelativeOrAbsolute));
                    CreateUserCallLogin();
                    //Frame.Navigate(typeof(SignUpPage), user); //pass user item to the next page
                }

            }
            catch (MobileServiceInvalidOperationException e)
            {
                _ismember = false;
                exception = e;
            }

        }

        private void CreateUserCallLogin()
        {
            if (user.UserId != null)
            {
                App.Current.AppUser.UserId = user.UserId;
                App.Current.AppUser.UserName = userNameTextBox.Text;
                App.Current.JustLoggedIn = true;
                CallLogin();
            }

        }

        private void CallLogin()
        {
            if (called)
            {
                ServerMessage s = new ServerMessage(0, "update", "null");
                try
                {
                    App.Current.SignalRHub.UserUpdate(App.Current.AppUser, s);
                }
                catch (Exception ex)
                {

                    ServerDown(ex);
                }

            }
            else
            {
                called = true;

                App.Current.SignalRHub.UserLogin(App.Current.AppUser);
 //               App.Current.SignalRHub.SignalRServerNotification -= new SignalRServerHandler(SignalRHub_SignalRServerNotification);

//                NavigationService.Navigate(new Uri("/HubPage1.xaml", UriKind.RelativeOrAbsolute));

            }
        }

        private void ServerDown(Exception ex)
        {

        }


        protected void SignalRHub_SignalRServerNotification(object sender, SignalREventArgs e)
        {
            Deployment.Current.Dispatcher.BeginInvoke( () => 
            //await CoreWindow.GetForCurrentThread().Dispatcher.RunAsync ( CoreDispatcherPriority.High, () => 
            {
                //Debug.WriteLine("SEND, - SIMPLE CHAT WORKS- "+e.ChatMessageFromServer);
                if (e.CustomServerMessage != null)
                {
                    switch (e.CustomServerMessage.Command)
                    {
                        //Login
                        case 0:
                            //Update Game List
                            if (e.CustomServerMessage.Action == "login")
                            {
                                UpdateWithServerData(e);
                                //LoggedInMessageBox();
                                NavigationService.Navigate(new Uri("/HubPage1.xaml", UriKind.Relative));

                            }

                            break;
                        case 1:
                            if (e.CustomServerMessage.Action == "update")
                            {
                                UpdateWithServerData(e);
                                //LoggedInMessageBox();
                                NavigationService.Navigate(new Uri("/HubPage1.xaml", UriKind.Relative));


                            }
                            break;
                        default:
                            break;
                    }
                }
            });
        }

        private void LoggedInMessageBox()
        {
           // Deployment.Current.Dispatcher.BeginInvoke(() => {
                MessageBoxResult result = MessageBox.Show("You are now logged in",
                 "Log In", MessageBoxButton.OK);

                if (result == MessageBoxResult.OK)
                {
                    NavigationService.Navigate(new Uri("/HubPage1.xaml", UriKind.Relative));

                }
            
           // });
        }

        private void UpdateWithServerData(SignalREventArgs e)
        {
            App.Current.AppUser = e.UserUpdate;
            //Upload Games
            App.Current.AllGames = e.CustomGameList;
            App.Current.OppUsers = e.CustomAvailableOpponents;
            App.Current.SignalRHub.SignalRServerNotification -= new SignalRServerHandler(SignalRHub_SignalRServerNotification);


        }

        private void testButton_Clicked(object sender, RoutedEventArgs e)
        {
            LoggedInMessageBox();
        }

    }
}