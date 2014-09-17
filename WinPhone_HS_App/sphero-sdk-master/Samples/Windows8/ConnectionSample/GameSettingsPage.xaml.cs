using ConnectionSample.Common;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Popups;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;
using SRHS2backend;

// The Basic Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=234237

namespace ConnectionSample
{
    /// <summary>
    /// A basic page that provides characteristics common to most applications.
    /// </summary>
    public sealed partial class GameSettingsPage : Page
    {

        private NavigationHelper navigationHelper;
        private ObservableDictionary defaultViewModel = new ObservableDictionary();

        private int _maxTime = 0;
        private int _maxHits = 1;
        private User opponent = new User();
        private int _playerType = 1;
        public List<User> opponents = new List<User>();

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


        public GameSettingsPage()
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
            App.Current.JustLoggedIn = false;

            // App.Current.SignalRHub.SignalRServerNotification += new SignalRServerHandler(SignalRHub_SignalRServerNotification);


            Debug.WriteLine("USERS LIST");
            foreach (User uu in App.Current.OppUsers)
            {
                Debug.WriteLine("USERS " + uu.UserName);
            }
            //opponents = App.Current.OppUsers;

            if (App.Current.OppUsers.Count > 0)
            {
                opponentListView.ItemsSource = App.Current.OppUsers;
            }
        }

        protected override void OnNavigatedFrom(NavigationEventArgs e)
        {
            navigationHelper.OnNavigatedFrom(e);
        }

        #endregion

        private void rbt3_Checked(object sender, RoutedEventArgs e)
        {
            _maxTime = 3;// *60;
        }

        private void rbt5_Checked(object sender, RoutedEventArgs e)
        {
            _maxTime = 5;// *60;
        }

        private void sbSelected_Button_Click(object sender, RoutedEventArgs e)
        {
            _playerType = 1;
        }


        private void CreateGame_Button_Click(object sender, RoutedEventArgs e)
        {
            string s;

            if (_maxTime < 0 || _maxHits < 0 || App.Current.OppUserTest == null)
            {
                if (_maxHits < 0)
                {
                    s = "Error: You need to choose how many lives Sphero Player will have.";
                    GameError(s);
                }
                else
                {
                    if (opponent.UserName == null || App.Current.OppUserTest == null)
                    {
                        s = "Error: You need to select an opponent";
                        GameError(s);
                    }
                    else
                    {
                        if (_maxTime <0)
                        {
                            s = "Error: You need to choose how long the game will last.";
                            GameError(s);
                        }
                    }
                }
            }
            else
            {
                Game ugame = new Game();
                ugame.SpheroPlayer = App.Current.AppUser;
                ugame.DronePlayer = App.Current.OppUserTest;
                ugame.OpponentName = App.Current.OppUserTest.UserName;
                ugame.CreatorName = App.Current.AppUser.UserName;
                ugame.MaxTime = _maxTime;
                ugame.MaxHits = _maxHits;
                ugame.GameStatus = 0;
                ugame.DronePlayerName = App.Current.OppUserTest.UserName;
                ugame.SpheroPlayerName = App.Current.AppUser.UserName;
                ugame.GameId = App.Current.AppUser.UserId + "game";
                ugame.DateCreated = DateTime.Now;
                App.Current.SignalRHub.CreateGame(App.Current.AppUser, ugame);
                App.Current.CurrentGame = ugame;
                Frame.Navigate(typeof(HubPage));
            }


            //Frame.Navigate(typeof(LobbyPage));
        }

        private async void GameError(string s)
        {
            var messageDialog = new MessageDialog(s);
            // Add commands and set their command ids
            messageDialog.Commands.Add(new UICommand("Ok", (command) =>
            {

            },
            0));

            // Set the command that will be invoked by default
            messageDialog.DefaultCommandIndex = 0;

            // Show the message dialog and get the event that was invoked via the async operator
            var commandChosen = await messageDialog.ShowAsync();
        }


        private void rbh1_clicked(object sender, RoutedEventArgs e)
        {
            _maxHits = 1;
        }

        private void rbh2_clicked(object sender, RoutedEventArgs e)
        {
            _maxHits = 2;
        }

        private void rbh3_clicked(object sender, RoutedEventArgs e)
        {
            _maxHits = 3;
        }

        private void itemclicked(object sender, ItemClickEventArgs e)
        {
            opponent = e.ClickedItem as User;
            App.Current.OppUserTest = e.ClickedItem as User;
        }



    }
}