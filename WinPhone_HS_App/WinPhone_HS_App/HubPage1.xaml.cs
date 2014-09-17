using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Shell;
using Windows.Foundation.Collections;
using System.Diagnostics;

namespace WinPhone_HS_App
{
    public partial class HubPage1 : PhoneApplicationPage
    {

        private List<Game> gameList = new List<Game>();
        private bool willJoin = false;
        private bool alreadyCreated = false;
        private Game selectedGame;

        private SampleGameDataGroup _spheroGames = new SampleGameDataGroup("SpheroGames", "Sphero Games", "");

        /// <summary>
        /// This can be changed to a strongly typed view model.
        /// </summary>
        /// 
        //public ObservableDictionary DefaultViewModel
        //{
        //    get { return this.defaultViewModel; }
        //}
        
        public ObservableCollection<Game> AvailableGames = new ObservableCollection<Game>();
        
        public HubPage1()
        {
            Debug.WriteLine("Initializeing HubPage");
            InitializeComponent();
            //Debug.WriteLine("HubPage: Starting += SignalR");
            //StartSignalRListener();
            Debug.WriteLine("HubPage: fILList");
            FillLists();
        }


        private void FillLists()
        {
            try
            {
                /* DEMO Code only */
                if (App.Current.AllGames.Count < 1)
                {
                    App.Current.OppUserTest = new User("droneDemoPlayerID", "Christine");
                    Game demo = new Game("demoGameId", App.Current.AppUser, App.Current.OppUserTest, 0, 2, 3, 1, "Chrsitine", "Kat"); //int state, int status, int time, int hit, string opn, string crtn)
                    App.Current.AllGames.Add(demo);
                }
                /* End of DEMO code */
                SpheroGameListBox.ItemsSource = App.Current.AllGames;

            }
            catch (Exception ex)
            {
                Debug.WriteLine("ERROR: " + ex.Message);
            }
            //_spheroGames = await SampleGameDataSource.GetGameGroupAsync("SpheroGroupID");
            //this.DefaultViewModel["AvailableSpheroGames"] = _spheroGames;
            //this.AvailableGames = new ObservableCollection<Game>();
            //this.AvailableGames.Add()
        }


        private void StartSignalRListener()
        {
            if (!alreadyCreated)
            {
                App.Current.SignalRHub.SignalRServerNotification += new SignalRServerHandler(SignalRHub_SignalRServerNotification);
                alreadyCreated = true;
            }

        }

        protected override void OnNavigatedTo(System.Windows.Navigation.NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);
            FillLists();
            App.Current.SignalRHub.SignalRServerNotification += new SignalRServerHandler(SignalRHub_SignalRServerNotification);
            Debug.WriteLine("HubPage: Created listener");
        }

        protected override void OnNavigatingFrom(NavigatingCancelEventArgs e)
        {
            //App.Current.SignalRHub.SignalRServerNotification -= new SignalRServerHandler(SignalRHub_SignalRServerNotification);
            Debug.WriteLine("HubPage: Remove Listener");
            base.OnNavigatingFrom(e);
        }


        protected void SignalRHub_SignalRServerNotification(object sender, SignalREventArgs e)
        {
            /* await Dispatcher.RunAsync(CoreDispatcherPriority.High, () =>
              */
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
                                App.Current.OppUsers.Add(e.UserUpdate);
                            }
                            else
                            {
                                UpdateWithServerData(e);

                            }

                            // Frame.Navigate(typeof(HubPage));
                        }

                        break;
                    case 1:
                        if (e.CustomServerMessage.Action == "update")
                        {
                            if (App.Current.AppUser.UserName == e.UserUpdate.UserName)
                            {

                                UpdateWithServerData(e);

                            }
                            

                        }
                        break;
                   case 2:
                        if (e.CustomServerMessage.Action == "created")
                        {

                            if (e.CustomGameObject.OpponentName == App.Current.AppUser.UserName)
                            {
                                Debug.WriteLine("HubPage: CREATED "+e.CustomServerMessage.Message);

                                App.Current.AllGames.Add(e.CustomGameObject);
                                SpheroGameListBox.Items.Clear();
                                SpheroGameListBox.ItemsSource = App.Current.AllGames; 
                                //Someone else created the game notify user to call on navigated to
                            }
                            else
                            {
                                //OpenMessageDialog(e.CustomGameObject);
                            }
                        }
                        break;
                    case 3:
                        Debug.WriteLine("Hub Page: Join happend on this page instead of Lobby");
                        break;
                    default:
                        break;
                }
            }
            // });
        }

        private void UpdateWithServerData(SignalREventArgs e)
        {
            App.Current.AppUser = e.UserUpdate;
            //Upload Games
            App.Current.AllGames = e.CustomGameList;
            App.Current.OppUsers = e.CustomAvailableOpponents;
           // App.Current.SignalRHub.SignalRServerNotification -= new SignalRServerHandler(SignalRHub_SignalRServerNotification);

        }

        private void CreateButton_Clicked(object sender, RoutedEventArgs e)
        {
            //App.Current.SignalRHub.SignalRServerNotification -= new SignalRServerHandler(SignalRHub_SignalRServerNotification);
            NavigationService.Navigate(new Uri("/GameSettingsPage.xaml", UriKind.RelativeOrAbsolute));
            //NavigationService.Navigate(new Uri("/SpheroBTConnect.xaml", UriKind.RelativeOrAbsolute));

        }

        private void Join_Button_Clicked(object sender, RoutedEventArgs e)
        {
            if (selectedGame == null)
            {
                //needs to select 
            }
            else
            {
                App.Current.CurrentGame = selectedGame;
                //App.Current.SignalRHub.JoinGame(App.Current.AppUser, App.Current.CurrentGame);
               
                NavigationService.Navigate(new Uri("/LobbyPage.xaml", UriKind.RelativeOrAbsolute));
                
                
                
                //App.Current.SignalRHub.Test("from CLIENT! test click on main page", App.Current.AppUser);
                //OpenMessageDialog();
            }
        }

        private void gamelist_changed(object sender, SelectionChangedEventArgs e)
        {
            selectedGame = e.AddedItems[0] as Game; 
        }




    }
}