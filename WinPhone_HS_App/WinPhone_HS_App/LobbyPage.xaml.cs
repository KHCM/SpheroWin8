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
using System.Diagnostics;
using Windows.UI.Core;

namespace WinPhone_HS_App
{
    public partial class LobbyPage : PhoneApplicationPage
    {
        private bool connected = false;
        private bool willPlay = false;
        string md;
        public LobbyPage()
        {
            InitializeComponent();
            App.Current.SignalRHub.SignalRServerNotification += new SignalRServerHandler(SignalRHub_SignalRServerNotification);
            Debug.WriteLine("LobbyPage: Page initialized");
        }

        protected override void OnNavigatedTo(System.Windows.Navigation.NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);

            #region DEMO
            /***
 * DEMO CODE ONLY 
 * 
 */
            chatDialog.Text = "Chrstine has entered lobby";
            chatDialog.Text += "\r\n" + "Christine is Ready";
            chatDialog.Text += "\r\n"+"Kat has entered lobby";
            /* End of DEMO */

            #endregion 
            /* REal Code
 * 
            App.Current.SignalRHub.JoinGame(App.Current.AppUser, App.Current.CurrentGame);
 * 
 */

            //App.Current.SignalRHub.SignalRServerNotification += new SignalRServerHandler(SignalRHub_SignalRServerNotification);
            Debug.WriteLine("LobbyPage: init Listener");
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
                    SpherosDiscovered.ItemsSource = spheroInformations;
                }
                else
                {
                    // No sphero Paired
                    MessageBox.Show("No sphero Paired");
                }
            }

            catch (BluetoothDeactivatedException)
            {
                // Bluetooth deactivated
                MessageBox.Show("Bluetooth deactivated");
            }
        }

        private void FindSpheros_Button_Click(object sender, RoutedEventArgs e)
        {
            DiscoverSpheros();
        }

        private async void Connect_Button_Click(object sender, RoutedEventArgs e)
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
                    SpheroActingAFool(ex);
                }
                App.CurrentConnection = connection;
                chatDialog.Text += "\r\n" + connection.BluetoothName + " is connected.";
                connected = true;

            }

        }

        private void SpheroActingAFool(Exception except)
        {
            MessageBox.Show(except.Message);

        }

        public void SignalRHub_SignalRServerNotification(object sender, SignalREventArgs e)
        {
            //await CoreWindow.GetForCurrentThread().Dispatcher.RunAsync ( CoreDispatcherPriority.High, () => 
            //{
                if (e.CustomServerMessage != null)
                {
                    //App.Current.CurrentGame = e.CustomGameObject;

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
                            Debug.WriteLine("LobbyPage: Action " + e.CustomServerMessage.Action + " " +e.CustomServerMessage.Message);
                            if (e.CustomServerMessage.Action == "join")
                            {
                                //This means you are the first , or second to join
                                // Message: User has joined the Game
                                // Message: User has left Game Lobby
                                Debug.WriteLine("LobbyPage: JOIN "+e.CustomServerMessage.Message);
                                
                                //CoreWindow.GetForCurrentThread().Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () =>
                                Deployment.Current.Dispatcher.BeginInvoke(() =>
                                {
                                    chatDialog.Text += "\r\n" + e.CustomServerMessage.Message;
                                });

                            }
                            if (e.CustomServerMessage.Action == "ready")
                            {
                                //if you get here then you were not the one to hit ready
                                Debug.WriteLine("LobbyPage: REady other opp is ready " + e.CustomServerMessage.Message);

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
                            Debug.WriteLine("LobbyPage: Case4");
                            ChangeToGamePage();
                            break;
                        //start command
                        default:
                            break;
                    }

                }
                // Add to local ChatRoom.
                //chatDialog.Text += "\r\n" + e.ChatMessageFromServer;
            //});
        }

        private void ChangeToGamePage()
        {
            InGameMessage im = new InGameMessage();
            im.GameState = 0;
            im.Action = "start";

            App.Current.SignalRHub.InGameMessageCall(App.Current.CurrentGame, im);

            App.Current.SignalRHub.SignalRServerNotification -= new SignalRServerHandler(SignalRHub_SignalRServerNotification);
            Debug.WriteLine("LobbyPage: remove listener");
            NavigationService.Navigate(new Uri("/HSControlPage.xaml", UriKind.Relative));
            Debug.WriteLine("START GAME!");
        }




        private void ReadyButton_Click(object sender, RoutedEventArgs e)
        {
            //Testing
            MessageBoxResult result = MessageBox.Show("Are you sure you're OK to start?",
 "Ready?", MessageBoxButton.OKCancel);

            if (result == MessageBoxResult.OK)
            {
                JoinGamePlay();

                //Frame.Navigate(typeof(GamePlayPage));
            }
            if (result == MessageBoxResult.Cancel)
            {

            }

            /*
            if (connected)
            {
                MessageBoxResult result = MessageBox.Show("Are you sure you're OK to start?",
                 "Ready?", MessageBoxButton.OKCancel);

                if (result == MessageBoxResult.OK)
                {
                    App.Current.SignalRHub.JoinGame(App.Current.AppUser, App.Current.CurrentGame);

                    App.Current.SignalRHub.SignalRServerNotification -= new SignalRServerHandler(SignalRHub_SignalRServerNotification);
                    NavigationService.Navigate(new Uri("/HSControlPage.xaml", UriKind.Relative));

                    //Frame.Navigate(typeof(GamePlayPage));
                }
                if (result == MessageBoxResult.Cancel)
                {

                }

            }
            else
            {
                MessageBox.Show("You are not connected to a Sphero, yet.");

            }*/
        }

        private void JoinGamePlay()
        {
            #region DEMO
            /*Real Code
             * 
             * App.Current.SignalRHub.JoinGame(App.Current.AppUser, App.Current.CurrentGame);
             * 
            /* End Real Code */

            #endregion 

            App.Current.SignalRHub.SignalRServerNotification -= new SignalRServerHandler(SignalRHub_SignalRServerNotification);
            Debug.WriteLine("LobbyPage: Removed Listener in JoinGamePlay()");
            NavigationService.Navigate(new Uri("/HSControlPage.xaml", UriKind.Relative));

        }
    }
}