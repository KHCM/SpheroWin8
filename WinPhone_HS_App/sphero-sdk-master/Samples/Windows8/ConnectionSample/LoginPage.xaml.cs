using ConnectionSample.Common;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Core;
using Windows.UI.Xaml;
using Windows.UI.Popups;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;
using SRHS2backend;
using Microsoft.WindowsAzure.MobileServices;

// The Basic Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=234237

namespace ConnectionSample
{
    /// <summary>
    /// A basic page that provides characteristics common to most applications.
    /// </summary>
    public sealed partial class LoginPage : Page
    {

        private NavigationHelper navigationHelper;
        private ObservableDictionary defaultViewModel = new ObservableDictionary();
 //       private IMobileServiceTable<User> usersTable = App.MobileService.GetTable<User>(); //TodoItem == TableName
        private string username;

        private List<Game> games = new List<Game>();
        private bool called = false;

        private bool _ismember;
        private string message;

        private MobileServiceUser mobileServiceUser; 
        /// <summary>
        /// This can be changed to a strongly typed view model.
        /// </summary>
        public ObservableDictionary DefaultViewModel
        {
            get { return this.defaultViewModel; }
        }

        /// <summary>
        /// NavigationHelper is used on each page to aid in navigation and 
        /// process lifetime management
        /// </summary>
        public NavigationHelper NavigationHelper
        {
            get { return this.navigationHelper; }
        }


        public LoginPage()
        {
            this.InitializeComponent();
            this.navigationHelper = new NavigationHelper(this);
            this.navigationHelper.LoadState += navigationHelper_LoadState;
            this.navigationHelper.SaveState += navigationHelper_SaveState;
        }

        /// <summary>
        /// Populates the page with content passed during navigation. Any saved state is also
        /// provided when recreating a page from a prior session.
        /// </summary>
        /// <param name="sender">
        /// The source of the event; typically <see cref="NavigationHelper"/>
        /// </param>
        /// <param name="e">Event data that provides both the navigation parameter passed to
        /// <see cref="Frame.Navigate(Type, Object)"/> when this page was initially requested and
        /// a dictionary of state preserved by this page during an earlier
        /// session. The state will be null the first time a page is visited.</param>
        private void navigationHelper_LoadState(object sender, LoadStateEventArgs e)
        {
            App.Current.SignalRHub.SignalRServerNotification += new SignalRServerHandler(SignalRHub_SignalRServerNotification);

        }

        /// <summary>
        /// Preserves state associated with this page in case the application is suspended or the
        /// page is discarded from the navigation cache.  Values must conform to the serialization
        /// requirements of <see cref="SuspensionManager.SessionState"/>.
        /// </summary>
        /// <param name="sender">The source of the event; typically <see cref="NavigationHelper"/></param>
        /// <param name="e">Event data that provides an empty dictionary to be populated with
        /// serializable state.</param>
        private void navigationHelper_SaveState(object sender, SaveStateEventArgs e)
        {
        }

        #region NavigationHelper registration

        /// The methods provided in this section are simply used to allow
        /// NavigationHelper to respond to the page's navigation methods.
        /// 
        /// Page specific logic should be placed in event handlers for the  
        /// <see cref="GridCS.Common.NavigationHelper.LoadState"/>
        /// and <see cref="GridCS.Common.NavigationHelper.SaveState"/>.
        /// The navigation parameter is available in the LoadState method 
        /// in addition to page state preserved during an earlier session.

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            navigationHelper.OnNavigatedTo(e);
        }

        protected override void OnNavigatedFrom(NavigationEventArgs e)
        {
            navigationHelper.OnNavigatedFrom(e);
        }

        #endregion


        private void CallLogin()
        {
            if (called)
            {
                ServerMessage s = new ServerMessage(0, "update", "null");
                try
                {
                    App.Current.SignalRHub.UserUpdate(App.Current.AppUser, s);
                }
                catch(Exception ex)
                {
                    
                    ServerDown(ex);
                }
                
            }
            else
            {
                called = true;
                App.Current.SignalRHub.UserLogin(App.Current.AppUser);
            }
        }

        public async void ServerDown(Exception exc)
        {
            // Ready and waiting for other user
            var messageDialog = new MessageDialog("The server seems to be down please try again later. Encountered this error: " + exc.Message);
            // Add commands and set their command ids
            messageDialog.Commands.Add(new UICommand("Okay", (command) =>
            {
                //ServerMessage s1m = new ServerMessage(0, "", "null");
                //App.Current.SignalRHub.UserUpdate(App.Current.AppUser, s1m);
            },
            0));


            // Set the command that will be invoked by default
            messageDialog.DefaultCommandIndex = 0;

            // Show the message dialog and get the event that was invoked via the async operator
            var commandChosen = await messageDialog.ShowAsync();

        }

        protected async void SignalRHub_SignalRServerNotification(object sender, SignalREventArgs e)
        {
            await Dispatcher.RunAsync(CoreDispatcherPriority.High, () =>
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
                            
                                if(App.Current.AppUser.UserName == e.UserUpdate.UserName){
                                    App.Current.AppUser = e.UserUpdate;
                                    //Upload Games
                                    App.Current.AllGames = e.CustomGameList;
                                    App.Current.OppUsers = e.CustomAvailableOpponents;

                                //App.Current.SignalRHub.SignalRServerNotification -= new SignalRServerHandler(SignalRHub_SignalRServerNotification);

                                Frame.Navigate(typeof(HubPage));
                            }

                            break;
                        case 1:
                            if (e.CustomServerMessage.Action == "update")
                            {
                                if (App.Current.AppUser.UserName == e.UserUpdate.UserName)
                                {
                                    App.Current.AppUser = e.UserUpdate;
                                    App.Current.AllGames = e.CustomGameList;
                                    App.Current.OppUsers = e.CustomAvailableOpponents;

                                    //Upload Games

                                    Frame.Navigate(typeof(HubPage));
                                }

                            }
                            break;
                        default:
                            break;
                    }
                }
            });
        }

        private async void Loginbutton_Click(object sender, RoutedEventArgs e)
        {
            if (usernameTextBox.Text != "")
            {
                App.Current.JustLoggedIn = true;
                App.Current.UserName = usernameTextBox.Text;
                App.Current.AppUser = new User(App.Current.UserName + "ID", App.Current.UserName);
                App.Current.AllGames = games;

                //await Authenticate();
                //isAMember();
                CallLogin();
                //App.Current.SignalRHub.UserLogin(App.Current.AppUser);
                //App.Current.SignalRHub.SignalRServerNotification -= new SignalRServerHandler(SignalRHub_SignalRServerNotification);
                //Frame.Navigate(typeof(HubPage));
            }
            else
            {

            }

        }

/*        private async System.Threading.Tasks.Task Authenticate()
        {
            while (mobileServiceUser == null)
            {
                try
                {
                    mobileServiceUser = await App.MobileService.LoginAsync(MobileServiceAuthenticationProvider.Twitter, null);
                    isAMember();
                }
                catch (InvalidOperationException)
                {
                    message = "You must log in. Login Required";
                    //MessageBox.Show(message);
                }
            }
        }

        private async void isAMember()
        {
            MobileServiceInvalidOperationException exception = null;
            try
            {
                // This code refreshes the entries in the list view by querying the TodoItems table.
                // The query excludes completed TodoItems
                var match = await usersTable.Where(users => users.UserId == mobileServiceUser.UserId).ToEnumerableAsync();
                var matchItem = match.Single();
                if (matchItem != null)
                    _ismember = true;
                else
                    _ismember = false;

                if (_ismember)
                {
                    // message = "Success! You are now logged in";
                    // MessageBox.Show(message);
                    App.Current.AppUser.UserId = mobileServiceUser.UserId;
                    App.Current.JustLoggedIn = true;
                    CallLogin();
                    //NavigationService.Navigate(new Uri("/HubPage1.xaml", UriKind.RelativeOrAbsolute));
                    //ListItems.ItemsSource = items; // XAML code
                }
                else
                {
                    message = "It's your first time logging in. SignUp!";
                    //MessageBox.Show(message);
                    App.Current.AppUser.UserId = mobileServiceUser.UserId;
                    App.Current.JustLoggedIn = true;
                    CallLogin();
                    //NavigationService.Navigate(new Uri(string.Format("/SignUpPage.xaml?parameter={0}", user), UriKind.RelativeOrAbsolute));

                    //Frame.Navigate(typeof(SignUpPage), user); //pass user item to the next page
                }

            }
            catch (MobileServiceInvalidOperationException e)
            {
                _ismember = false;
                exception = e;
            }

        }*/
    }
}
