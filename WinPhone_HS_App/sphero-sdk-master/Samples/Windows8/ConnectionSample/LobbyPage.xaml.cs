using Sphero.Communication;
using ConnectionSample.Common;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;
using Windows.UI.Popups;
using System.Diagnostics;
using Windows.UI.Core;
using SRHS2backend;

// The Basic Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=234237

namespace ConnectionSample
{
    /// <summary>
    /// A basic page that provides characteristics common to most applications.
    /// </summary>
    public sealed partial class LobbyPage : Page
    {

        private NavigationHelper navigationHelper;
        private ObservableDictionary defaultViewModel = new ObservableDictionary();

        private bool connected = false; 
        private bool willPlay = false;
        string md;

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


        public LobbyPage()
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
            App.Current.SignalRHub.SignalRServerNotification += new SignalRServerHandler(SignalRHub_SignalRServerNotification);
            Debug.WriteLine("Got to Lobby Page");
        }

        protected override void OnNavigatedFrom(NavigationEventArgs e)
        {
            navigationHelper.OnNavigatedFrom(e);
        }

        #endregion

        private async void DiscoverSpheros()
        {
            try
            {
                // Discover paired Spheros
                List<SpheroInformation> spheroInformations = new List<SpheroInformation>(await SpheroConnectionProvider.DiscoverSpheros());

                if (spheroInformations != null && spheroInformations.Count > 0)
                {
                    // Populate list with Discovered Spheros
                    SpherosDiscovered.ItemsSource = spheroInformations;
                }
                else
                {
                    // No sphero Paired
                    MessageDialog dialogNSP = new MessageDialog("No sphero Paired");
                    await dialogNSP.ShowAsync();
                }

            }
            catch (NoSpheroFoundException)
            {
                MessageDialog dialogNSF = new MessageDialog("No sphero Found");
                dialogNSF.ShowAsync();
            }
            catch (BluetoothDeactivatedException)
            {
                // Bluetooth deactivated
                MessageDialog dialogBD = new MessageDialog("Bluetooth deactivated");
                dialogBD.ShowAsync();
            }
        }

        private async void btnConnection_Click(object sender, RoutedEventArgs e)
        {
            if (SpherosDiscovered.SelectedItem != null)
            {

                SpheroConnection connection = null;
                try
                {
                    connection = await SpheroConnectionProvider.CreateConnection((SpheroInformation)SpherosDiscovered.SelectedItem);
                }
                catch (Exception ex)
                {
                    SpheroActingAFool();
                }
                App.CurrentConnection = connection;
                chatDialog.Text += "\r\n" + connection.BluetoothName+ " is connected.";
                connected = true;

                //Frame.Navigate(typeof(ConnectedPage), connection);
            }
        }

        private void SpheroActingAFool()
        {
            var messageDialog = new MessageDialog("Sphero Connection not working, try again.");
            // Add commands and set their command ids
            messageDialog.Commands.Add(new UICommand("Ok", (command) =>
            {

            },
            0));

            // Set the command that will be invoked by default
            messageDialog.DefaultCommandIndex = 0;

            // Show the message dialog and get the event that was invoked via the async operator
            messageDialog.ShowAsync();
        }


        private void btnRefresh_Click(object sender, RoutedEventArgs e)
        {
            DiscoverSpheros();
        }

        #region SignalR Interactions

        private async void JoinGameDialog()
        {
            var messageDialog = new MessageDialog("Do you want to Join the Game now?");
            // Add commands and set their command ids
            messageDialog.Commands.Add(new UICommand("Not Now", (command) =>
            {
                App.Current.CurrentGame = null;
                Frame.Navigate(typeof(HubPage));
            },
            0));
            messageDialog.Commands.Add(new UICommand("Join Game", (command) =>
            {
                App.Current.SignalRHub.JoinGame(App.Current.AppUser, App.Current.CurrentGame);
            },
            1));

            // Set the command that will be invoked by default
            messageDialog.DefaultCommandIndex = 1;

            // Show the message dialog and get the event that was invoked via the async operator
            var commandChosen = await messageDialog.ShowAsync();
        }

        public async void SignalRHub_SignalRServerNotification(object sender, SignalREventArgs e)
        {
            await Dispatcher.RunAsync(CoreDispatcherPriority.High, () =>
            {
                Debug.WriteLine("SEND, - SIMPLE CHAT WORKS- " + e.ChatMessageFromServer);
                if (e.CustomServerMessage != null)
                {
                    App.Current.CurrentGame = e.CustomGameObject;

                    switch (e.CustomServerMessage.Command)
                    {
                        //Login
                        case 0:
                            /*    
                            //Update Game List
                                if (e.CustomServerMessage.Action == "login")
                                {
                                    App.Current.AppUser = e.UserUpdate;
                                    //Upload Games
                                    App.Current.AllGames = e.CustomGameList;
                                    Debug.WriteLine(e.UserUpdate.UserName + " " + e.CustomServerMessage.Message);
                                }*/

                            break;
                        case 1:
                            Debug.WriteLine(e.CustomServerMessage.Action + "MESSAGE- " + e.CustomServerMessage.Message);
                            break;
                        case 2:
                            /*
                            if (e.CustomServerMessage.Action == "created")
                            {
                                //Add game to users game list
                                App.Current.AllGames.Add(e.CustomGameObject);
                                Debug.WriteLine("Created A Game " + e.ChatMessageFromServer);
                                Frame.Navigate(typeof(LobbyPage));
                                //Ask if they want to join
                                //OpenMessageDialog("join",e.CustomGameObject);
                            }*/
                            break;
                        case 3:
                            if (e.CustomServerMessage.Action == "join")
                            {
                                //This means you are the first , or second to join
                                // Message: User has joined the Game
                                // Message: User has left Game Lobby
                                chatDialog.Text += "\r\n" + e.CustomServerMessage.Message;

                            }
                            if (e.CustomServerMessage.Action == "ready")
                            {
                                //if you get here then you were not the one to hit ready
                                chatDialog.Text += "\r\n" + e.CustomServerMessage.Message;
                                /*
                                if (e.UserUpdate.UserName == App.Current.AppUser.UserName && e.CustomGameObject.GameStatus<3)
                                {
                                    chatDialog.Text += "\r\n" + e.CustomServerMessage.Message + " Waiting for opponent...";
                                    md = e.CustomServerMessage.Message + " Waiting for opponent...";
                                }
                                else
                                {
                                    chatDialog.Text += "\r\n" + e.CustomServerMessage.Message;
                                    md = e.CustomServerMessage.Message;
                                }
                                if (e.CustomGameObject.GameStatus > 2)
                                {
                                    StartingGame(App.Current.CurrentGame, e.UserUpdate);
                                }
                               */
                            }
                            /*                            if (e.CustomServerMessage.Action == "start")
                                                        {
                                                            chatDialog.Text += "\r\n" + e.CustomServerMessage.Message;

                                                            //chatDialog.Text += "\r\n" + "Game now starting...";
                                                            //if (e.UserUpdate.UserName == App.Current.AppUser.UserName)
                                                            //{
                                                                StartGame(App.Current.CurrentGame,e.UserUpdate);
                                                            //}
                                                        }
                             * */
                            break;
                        case 4:
                            ChangeToGamePage();
                            break;
                        //start command
                        default:
                            break;
                    }

                }
                // Add to local ChatRoom.
                //chatDialog.Text += "\r\n" + e.ChatMessageFromServer;
            });
        }

        private void ChangeToGamePage()
        {
            InGameMessage im = new InGameMessage();
            im.GameState = 0;
            im.Action = "start";

            App.Current.SignalRHub.InGameMessageCall(App.Current.CurrentGame, im);

            App.Current.SignalRHub.SignalRServerNotification -= new SignalRServerHandler(SignalRHub_SignalRServerNotification);

            Frame.Navigate(typeof(GamePlayPage));
            Debug.WriteLine("START GAME!");
        }

        private async void StartingGame(Game g, User u)
        {


            // Ready and waiting for other user
            if (g.GameStatus == 3 && u.UserName == App.Current.AppUser.UserName)
            {

            }
            var messageDialog = new MessageDialog("Do you want to Join the Game now?");
            // Add commands and set their command ids
            messageDialog.Commands.Add(new UICommand("Not Now", (command) =>
            {
                App.Current.CurrentGame = null;
            },
            0));
            messageDialog.Commands.Add(new UICommand("Join Game", (command) =>
            {
                App.Current.SignalRHub.JoinGame(App.Current.AppUser, App.Current.CurrentGame);
                App.Current.SignalRHub.SignalRServerNotification -= new SignalRServerHandler(SignalRHub_SignalRServerNotification);

                Frame.Navigate(typeof(LobbyPage));
            },
            1));

            // Set the command that will be invoked by default
            messageDialog.DefaultCommandIndex = 1;

            // Show the message dialog and get the event that was invoked via the async operator
            var commandChosen = await messageDialog.ShowAsync();
            /*
            //chatDialog.Text += "\r\n" + "Starting Game...";
            //Was the App User the last one to Join? If yes trigger Start Event
            if (u.UserName == App.Current.AppUser.UserName)
            {
                App.Current.SignalRHub.JoinGame(App.Current.AppUser, App.Current.CurrentGame);
                App.Current.SignalRHub.SignalRServerNotification -= new SignalRServerHandler(SignalRHub_SignalRServerNotification);

                Frame.Navigate(typeof(LobbyPage));
                //App.Current.SignalRHub.StartGame(App.Current.CurrentGame, u); //Lobby Message to start game
            }
             //            Add commands and set their command ids

             //Set the command that will be invoked by default
            //messageDialog.DefaultCommandIndex = 1;

             //Show the message dialog and get the event that was invoked via the async operator
            //await messageDialog.ShowAsync();

            */

        }

        #endregion

        private void TEST_Click(object sender, RoutedEventArgs e)
        {
            //This is to Test Join pt2

            App.Current.SignalRHub.JoinGame(App.Current.OppUserTest, App.Current.CurrentGame);
        }

        //OPP ready
        private void Ready_Button1_Click(object sender, RoutedEventArgs e)
        {
            App.Current.SignalRHub.JoinGame(App.Current.OppUserTest, App.Current.CurrentGame);
        }

        //AppUser Read
        private async void Ready_Button1_Click_1(object sender, RoutedEventArgs e)
        {
            ///Test Code
            ///
            var messageDialog = new MessageDialog("Are you sure, you're ready?");
            // Add commands and set their command ids
            messageDialog.Commands.Add(new UICommand("Ah, I'm not ready yet!", (command) =>
            {

            },
            0));
            messageDialog.Commands.Add(new UICommand("Yes I'm sure", (command) =>
            {
                App.Current.SignalRHub.JoinGame(App.Current.AppUser, App.Current.CurrentGame);

                App.Current.SignalRHub.SignalRServerNotification -= new SignalRServerHandler(SignalRHub_SignalRServerNotification);

                Frame.Navigate(typeof(GamePlayPage));
            },
            1));

            // Set the command that will be invoked by default
            messageDialog.DefaultCommandIndex = 1;

            // Show the message dialog and get the event that was invoked via the async operator
            var commandChosen = await messageDialog.ShowAsync();

            ///

            /*
            if (connected)
            {
                var messageDialog = new MessageDialog("Are you sure, you're ready?");
                // Add commands and set their command ids
                messageDialog.Commands.Add(new UICommand("Ah, I'm not ready yet!", (command) =>
                {

                },
                0));
                messageDialog.Commands.Add(new UICommand("Yes I'm sure", (command) =>
                {
                    //App.Current.SignalRHub.JoinGame(App.Current.AppUser, App.Current.CurrentGame);
                    
                    App.Current.SignalRHub.SignalRServerNotification -= new SignalRServerHandler(SignalRHub_SignalRServerNotification);

                    Frame.Navigate(typeof(GamePlayPage));
                },
                1));

                // Set the command that will be invoked by default
                messageDialog.DefaultCommandIndex = 1;

                // Show the message dialog and get the event that was invoked via the async operator
                var commandChosen = await messageDialog.ShowAsync();

            }
            else
            {
                var messageDialog = new MessageDialog("You are not connected to a Sphero, yet.");
                // Add commands and set their command ids
                messageDialog.Commands.Add(new UICommand("Ok", (command) =>
                {

                },
                0));
   

                // Set the command that will be invoked by default
                messageDialog.DefaultCommandIndex = 0;

                // Show the message dialog and get the event that was invoked via the async operator
                var commandChosen = await messageDialog.ShowAsync();
            }*/
        }
    }
}
