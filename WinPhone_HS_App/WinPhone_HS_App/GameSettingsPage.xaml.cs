using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Shell;
using System.Diagnostics;

namespace WinPhone_HS_App
{
    public partial class GameSettingsPage : PhoneApplicationPage
    {
        
        private int _maxTime = -1;
        private int _maxHits = -1;
        public List<User> opponents = new List<User>();
        public bool iscreatingagame = true; 
        public bool everythingselected; 
        public GameSettingsPage()
        {
            InitializeComponent();
            StartSignalRListener();
            opponentsListBox.Items.Clear();
            opponentsListBox.ItemsSource = App.Current.OppUsers;
            //opponentsList.ItemsSource = App.Current.OppUsers;
           
            //opponentsListBox.ItemsSource = App.Current.OppUsers;
        }

        private void StartSignalRListener()
        {
            App.Current.SignalRHub.SignalRServerNotification += new SignalRServerHandler(SignalRHub_SignalRServerNotification);
        }

        private void CreateGame_Button_Click(object sender, RoutedEventArgs e)
        {
            Game ugame = new Game();
            ugame.DateCreated = DateTime.Now;
            ugame.GameId = App.Current.AppUser.UserName;
            string s;

            if (_maxTime <= 0 || _maxHits <= 0 || App.Current.OppUserTest == null)
            {
                if (_maxHits <= 0)
                {
                    s = "Error: You need to choose how many lives Sphero Player will have.";
                    GameError(s);
                }
                else
                {
                    if (App.Current.OppUserTest.UserName == null)
                    {
                        s = "Error: You need to select an opponent";
                        GameError(s);
                    }
                    else
                    {
                        if (_maxTime <= 0)
                        {
                            s = "Error: You need to choose how long the game will last.";
                            GameError(s);
                        }
                    }
                }
            }
            else
            {
                ugame.MaxTime = _maxTime;
                ugame.MaxHits = _maxHits;
                ugame.GameStatus = 0;
                ugame.GameId = App.Current.AppUser.UserId + "game";
                App.Current.SignalRHub.SignalRServerNotification -= new SignalRServerHandler(SignalRHub_SignalRServerNotification);
                Debug.WriteLine("GameSettings: romve listener");
                App.Current.SignalRHub.CreateGame(App.Current.AppUser, ugame);
                App.Current.CurrentGame = ugame;
                //Frame.Navigate(typeof(HubPage));
            }


            //Frame.Navigate(typeof(LobbyPage));
        }

        private void GameError(string s)
        {
           
            MessageBox.Show("Error:", s, MessageBoxButton.OK);
        }


        private void rbmaxhits1_Checked(object sender, RoutedEventArgs e)
        {
            _maxHits = 1; 
        }

        private void maxHits2_Checked(object sender, RoutedEventArgs e)
        {
            _maxHits = 2;
        }

        private void rbmaxhits3_Checked(object sender, RoutedEventArgs e)
        {
            _maxHits = 3; 
        }

        private void mt3_checked(object sender, RoutedEventArgs e)
        {
            _maxTime = 3; 
        }

        private void mt5_Checked(object sender, RoutedEventArgs e)
        {
            _maxTime = 5;
        }

        private void mt10_Checked(object sender, RoutedEventArgs e)
        {
            _maxTime = 10; 
        }

        //private void opponentsList_SelectionChanged(object sender, SelectionChangedEventArgs e)
        //{
            
        //}

        private void oppListBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            App.Current.OppUserTest = (User) e.AddedItems[0];
        }

 /*       private void CreateGameButton_Clicked(object sender, RoutedEventArgs e)
        {
            //send call to create
            //wait till server resonds
            //flag do you wnat to join now?
            //if no update user on this page then transfer to HUB
            //if yes go to Lobby send lobby shit

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
                ugame.DateCreated = DateTime.Now;
                ugame.OpponentName = App.Current.OppUserTest.UserName;
                //your good
                ugame.MaxTime = _maxTime;
                ugame.MaxHits = _maxHits;
                ugame.GameStatus = 0;
                ugame.GameId = App.Current.AppUser.UserId + "game";

                App.Current.CurrentGame = ugame;
                //Frame.Navigate(typeof(HubPage));
            }

        }*/

        protected void SignalRHub_SignalRServerNotification(object sender, SignalREventArgs e)
        {
            Deployment.Current.Dispatcher.BeginInvoke(() =>
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
                            if (e.UserUpdate.UserName != App.Current.AppUser.UserName)
                            {
                                e.CustomAvailableOpponents.Add(e.UserUpdate);
                            }
                            else
                            {
                                UpdateWithServerData(e);

                            }
                        }

                        break;
                    case 1:
                        if (e.CustomServerMessage.Action == "update")
                        {
                            if (e.UserUpdate.UserName == App.Current.AppUser.UserName)
                            {
                                App.Current.AppUser = e.UserUpdate;
                                App.Current.AllGames = e.CustomGameList;
                                App.Current.OppUsers = e.CustomAvailableOpponents;
                                if (!iscreatingagame)
                                {
                                    Debug.WriteLine("Game Settings: Update " + e.CustomServerMessage.Message);

                                    NavigationService.Navigate(new Uri("/HubPage1.xaml", UriKind.RelativeOrAbsolute));

                                }
                            }
                            

                        }
                        break;
                    case 2:
                        if (e.CustomServerMessage.Action == "created")
                        {
                            if (App.Current.AppUser.UserName == e.CustomGameObject.CreatorName)
                            {
                                Debug.WriteLine("SettingsPage created game listener called.");
                                //Add game to users game list
                                App.Current.AllGames.Add(e.CustomGameObject);
                                App.Current.CurrentGame = e.CustomGameObject;
                                Debug.WriteLine("Created A Game " + e.CustomServerMessage.Message);
                                //Frame.Navigate(typeof(LobbyPage));
                                //Update User information
                                //Ask if they want to join
                                ServerMessage sm = new ServerMessage(0, "update", "created a game");
                                App.Current.SignalRHub.UserUpdate(App.Current.AppUser, sm);
                                Debug.WriteLine("SettingsPage: Starting OpenJoinMessageDIalog");

                                //OpenJoinMessageDialog();
                                Debug.WriteLine("SettingsPage: COmpleted Dialog");
                            }

                        }
                        break;
                    default:
                        break;
                }
            }
          });
        }

        private void OpenJoinMessageDialog()
        {
            MessageBoxResult result = MessageBox.Show("Do you want to Join Game now?",
            "Game Successfully Created", MessageBoxButton.OKCancel);

            if (result == MessageBoxResult.OK)
            {
                //Enter Lobby
                App.Current.SignalRHub.JoinGame(App.Current.AppUser, App.Current.CurrentGame);
                ChangeListener();

                NavigationService.Navigate(new Uri("/LobbyPage.xaml", UriKind.Relative));
            }
            if (result == MessageBoxResult.Cancel)
            {
                //Go back
                ChangeListener();
                ServerMessage sm = new ServerMessage(0, "update", "created a game"); 
                App.Current.SignalRHub.UserUpdate(App.Current.AppUser, sm);
                Debug.WriteLine("SettingsPage: Hit Cancel");
                NavigationService.Navigate(new Uri("/HubPage1.xaml", UriKind.Relative));

                
            }
            Debug.WriteLine("GameSettingsPage: Done with OpenJoinMessageDialog");

        }

        private void ChangeListener()
        {
            //App.Current.SignalRHub.SignalRServerNotification -= new SignalRServerHandler(SignalRHub_SignalRServerNotification);
            //Debug.WriteLine("GameSettings: remove listener in ChangeListener()");
        }

        private void UpdateWithServerData(SignalREventArgs e)
        {
            App.Current.AppUser = e.UserUpdate;
            //Upload Games
            App.Current.AllGames = e.CustomGameList;
            App.Current.OppUsers = e.CustomAvailableOpponents;
            //App.Current.SignalRHub.SignalRServerNotification -= new SignalRServerHandler(SignalRHub_SignalRServerNotification);
            Debug.WriteLine("GameSettings: remove listener in UpdateWithServerData");
            //NavigationService.Navigate(new Uri("/HubPage1.xaml", UriKind.RelativeOrAbsolute));
        }

        private void CreateGame_Button_Clicked(object sender, RoutedEventArgs e)
        {
            if (isEverythingSelected())
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

                //App.Current.CurrentGame = ugame;
                iscreatingagame = false;
                App.Current.SignalRHub.CreateGame(App.Current.AppUser, ugame);
                //goToGamePage();
                //OpenJoinMessageDialog();
            }
           // NavigationService.Navigate(new Uri("/HubPage1.xaml", UriKind.RelativeOrAbsolute));

        }

        protected override void OnNavigatingFrom(NavigatingCancelEventArgs e)
        {
            App.Current.SignalRHub.SignalRServerNotification -= new SignalRServerHandler(SignalRHub_SignalRServerNotification);
            Debug.WriteLine("GameSettings:Remove listener");
        }

        private void goToGamePage()
        {
            NavigationService.Navigate(new Uri("/SpheroBTConnect.xaml", UriKind.RelativeOrAbsolute));

        }

        private bool isEverythingSelected()
        {
            string s = "";
            if (_maxHits < 0)
            {
                s = s + "Error: You need to choose how many lives Sphero Player will have.";
                GameError(s);
                return false;
            }
            if ( App.Current.OppUserTest == null)
            {
                s = s + "Error: You need to select an opponent";
                GameError(s);
                return false;
            }
            if (_maxTime < 0)
            {
                s = "Error: You need to choose how long the game will last.";
                GameError(s);
                return false;
            }
            if (s.Equals(""))
            {
                return true;
            }
            else
            {
                return false;
            }

        }
        

    }
}