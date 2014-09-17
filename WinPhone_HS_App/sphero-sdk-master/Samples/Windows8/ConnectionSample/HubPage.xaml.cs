using ConnectionSample.Common;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Core;
using Windows.UI.Popups;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;
using SRHS2backend;

// The Hub Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=321224

namespace ConnectionSample
{
    /// <summary>
    /// A page that displays a grouped collection of items.
    /// </summary>
    public sealed partial class HubPage : Page
    {
        private NavigationHelper navigationHelper;
        private ObservableDictionary defaultViewModel = new ObservableDictionary();

        private List<Game> gameList = new List<Game>();
        private bool willJoin = false;

        private Game selectedGame;

        private SampleGameDataGroup _spheroGames = new SampleGameDataGroup("SpheroGames","Sphero Games","");

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

        public HubPage()
        {
            this.InitializeComponent();
            App.Current.SignalRHub.SignalRServerNotification += new SignalRServerHandler(SignalRHub_SignalRServerNotification);

            this.navigationHelper = new NavigationHelper(this);
            this.navigationHelper.LoadState += navigationHelper_LoadState;
        }


        /// <summary>
        /// Populates the page with content passed during navigation.  Any saved state is also
        /// provided when recreating a page from a prior session.
        /// </summary>
        /// <param name="sender">
        /// The source of the event; typically <see cref="NavigationHelper"/>
        /// </param>
        /// <param name="e">Event data that provides both the navigation parameter passed to
        /// <see cref="Frame.Navigate(Type, Object)"/> when this page was initially requested and
        /// a dictionary of state preserved by this page during an earlier
        /// session.  The state will be null the first time a page is visited.</param>
        private async void navigationHelper_LoadState(object sender, LoadStateEventArgs e)
        {
            // TODO: Assign a collection of bindable groups to this.DefaultViewModel["Groups"]
//            _spheroGames = await SampleGameDataSource.GetGameGroupAsync("SpheroGroupID");
//            this.DefaultViewModel["AvailableSpheroGames"] = _spheroGames;
        }

        /// <summary>
        /// Invoked when a HubSection header is clicked.
        /// </summary>
        /// <param name="sender">The Hub that contains the HubSection whose header was clicked.</param>
        /// <param name="e">Event data that describes how the click was initiated.</param>
        void Hub_SectionHeaderClick(object sender, HubSectionHeaderClickEventArgs e)
        {
            HubSection section = e.Section;
            var group = section.DataContext;
            //this.Frame.Navigate(typeof(SectionPage), ((SampleDataGroup)group).UniqueId);
        }

        /// <summary>
        /// Invoked when an item within a section is clicked.
        /// </summary>
        /// <param name="sender">The GridView or ListView
        /// displaying the item clicked.</param>
        /// <param name="e">Event data that describes the item clicked.</param>
        void ItemView_ItemClick(object sender, ItemClickEventArgs e)
        {
            // Navigate to the appropriate destination page, configuring the new page
            // by passing required information as a navigation parameter
            //var itemId = ((SampleDataItem)e.ClickedItem).UniqueId;
            //this.Frame.Navigate(typeof(ItemPage), itemId);
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

        protected async override void OnNavigatedTo(NavigationEventArgs e)
        {
            navigationHelper.OnNavigatedTo(e);
            // TODO: Assign a collection of bindable groups to this.DefaultViewModel["Groups"]
            _spheroGames = await SampleGameDataSource.GetGameGroupAsync("SpheroGroupID");
            this.DefaultViewModel["AvailableSpheroGames"] = _spheroGames;


            
        }

        protected override void OnNavigatedFrom(NavigationEventArgs e)
        {
            navigationHelper.OnNavigatedFrom(e);

            //sListView.ItemsSource = App.Current.AllGames;
        }

        #endregion

        protected async void SignalRHub_SignalRServerNotification(object sender, SignalREventArgs e)
        {
            await Dispatcher.RunAsync(CoreDispatcherPriority.High, () =>
            {
                //Debug.WriteLine("SEND, - SIMPLE CHAT WORKS- "+e.ChatMessageFromServer);
                if (e.CustomServerMessage.Command >-1)
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
                                    App.Current.AppUser = e.UserUpdate;
                                    //Upload Games
                                    App.Current.AllGames = e.CustomGameList;
                                    App.Current.OppUsers = e.CustomAvailableOpponents;
                                }



                            }

                            break;
                        case 1:
                            if (e.CustomServerMessage.Action == "update")
                            {
                                if (App.Current.AppUser.UserName == e.UserUpdate.UserName)
                                {
                                    App.Current.AppUser = e.UserUpdate;
                                    //Upload Games
                                    // App.Current.AllGames = e.CustomGameList;
                                    App.Current.OppUsers = e.CustomAvailableOpponents;
                                    if (_spheroGames != null)
                                    {
                                        Debug.WriteLine(_spheroGames.GameItems.Count());
                                        _spheroGames.GameItems.Clear();

                                    }
                                    else
                                    {
                                        fillgamelist();
                                    }
                                    foreach (var g in e.CustomGameList)
                                    {
                                        if (_spheroGames.GameItems.Contains(g))
                                        {

                                        }
                                        else
                                        {
                                            _spheroGames.GameItems.Add(g);

                                        }
                                    }
                                }

                          
                            }
                            break;
                        case 2:
                            if (e.CustomServerMessage.Action == "created")
                            {
                                //Add game to users game list

                                Debug.WriteLine("Created A Game " + e.CustomServerMessage.Message);
                                //Frame.Navigate(typeof(LobbyPage));
                                //Ask if they want to join

                                if (e.CustomGameObject.OpponentName == App.Current.AppUser.UserName)
                                {
                                    App.Current.AllGames.Add(e.CustomGameObject);
                                    App.Current.CurrentGame = e.CustomGameObject;
                                    //Someone else created the game notify user to call on navigated to
                                    Debug.WriteLine("HubPage: CREATED < someone challenged you" + e.CustomServerMessage.Message);
                                    //updateGames();

                                }
                                else
                                {
                                    OpenMessageDialog(e.CustomGameObject);
                                }
                            }
                            break;
                        case 3:
                            if (e.CustomServerMessage.Action == "join")
                            {
                                //This means you are the first , or second to join
                                // Message: User has joined the Game
                                // Message: User has left Game Lobby
                                //ChangeListener();

                                //Frame.Navigate(typeof(LobbyPage));

                                //chatDialog.Text += "\r\n" + e.ChatMessageFromServer;
                                Debug.WriteLine("JOIN WENT TO HUBPAGE");
                            }
                            if (e.CustomServerMessage.Action == "ready")
                            {

                            }
                            break;
                        default:
                            //Notify the User that you're getting not defined Messages
                            Debug.WriteLine("Just Recieved unknown command"+e.CustomServerMessage.Action + " " + e.CustomServerMessage.Message);
                            break;
                    }

                }
                // Add to local ChatRoom.
                //chatDialog.Text += "\r\n" + e.ChatMessageFromServer;
            });
        }

        private async void updateGames()
        {
            _spheroGames = await SampleGameDataSource.GetGameGroupAsync("SpheroGroupID");
            this.DefaultViewModel["AvailableSpheroGames"] = _spheroGames;
        }

        private async void fillgamelist()
        {
            _spheroGames = await SampleGameDataSource.GetGameGroupAsync("SpheroGroupID");
            this.DefaultViewModel["AvailableSpheroGames"] = _spheroGames;
        }

        private async void LoadingGames()
        {

            // Ready and waiting for other user
            var messageDialog = new MessageDialog("Loading Games and Opponents");
            // Add commands and set their command ids
            messageDialog.Commands.Add(new UICommand("Okay", (command) =>
            {
                ServerMessage s1m = new ServerMessage(0, "", "null");
                App.Current.SignalRHub.UserUpdate(App.Current.AppUser, s1m);
            },
            0));


            // Set the command that will be invoked by default
            messageDialog.DefaultCommandIndex = 1;

            // Show the message dialog and get the event that was invoked via the async operator
            var commandChosen = await messageDialog.ShowAsync();

        }

        public bool j = true;
        private async void OpenMessageDialog(Game g)
        {
            try
            {
                var messageDialog = new MessageDialog("Do you want to Join the Game now?");
                // Add commands and set their command ids
                messageDialog.Commands.Add(new UICommand("Not Now", (command) =>
                {
                    //App.Current.CurrentGame = null;
                    //ServerMessage sm = new ServerMessage(0, "update", "created a game");

                    //App.Current.SignalRHub.UserUpdate(App.Current.AppUser,sm );
                    j = false;
                },
                0));
                messageDialog.Commands.Add(new UICommand("Join Game", (command) =>
                {
                    App.Current.CurrentGame = g;
                    //App.Current.SignalRHub.JoinGame(App.Current.AppUser, App.Current.CurrentGame);
                    //ChangeListener();

                    //Frame.Navigate(typeof(LobbyPage));
                    j = true;
                },
                1));

                // Set the command that will be invoked by default
                messageDialog.DefaultCommandIndex = 1;

                // Show the message dialog and get the event that was invoked via the async operator
                var commandChosen = await messageDialog.ShowAsync();

                if (j == true)
                {
                    App.Current.SignalRHub.JoinGame(App.Current.AppUser, App.Current.CurrentGame);
                    ChangeListener();

                    Frame.Navigate(typeof(LobbyPage));
                }
                else
                {
                    App.Current.CurrentGame = null;
                    ServerMessage sm = new ServerMessage(0, "update", "created a game");

                    App.Current.SignalRHub.UserUpdate(App.Current.AppUser, sm);
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine("Join Dialog Exception " + ex.Message);
            }
 
        }

        private void ChangeListener()
        {
            // Unwire.
            App.Current.SignalRHub.SignalRServerNotification -= new SignalRServerHandler(SignalRHub_SignalRServerNotification);
        }

        private void CommandInvokedHandler(IUICommand command)
        {
            if (command.Label == "Join")
            {
                //Go to Lobby of current Game
            }
        }

        private void createButton_Click(object sender, RoutedEventArgs e)
        {
            ChangeListener();
            Frame.Navigate(typeof(GameSettingsPage));
            //Frame.Navigate(typeof(LobbyPage));
        }

        private void spheroGridView_ItemClick(object sender, ItemClickEventArgs e)
        {
            //JoinGame
            selectedGame = (Game)e.ClickedItem;
            //Debug.WriteLine("IMPLEMENT");
        }

        private void joinButton_Clicked(object sender, RoutedEventArgs e)
        {
            if(selectedGame == null)
            {
                //needs to select 
            }
            else
            {
                App.Current.CurrentGame = selectedGame;
                App.Current.SignalRHub.JoinGame(App.Current.AppUser, App.Current.CurrentGame);
                ChangeListener();

                Frame.Navigate(typeof(LobbyPage));
                //App.Current.SignalRHub.Test("from CLIENT! test click on main page", App.Current.AppUser);
                //OpenMessageDialog();
            }

        }

    }
}
