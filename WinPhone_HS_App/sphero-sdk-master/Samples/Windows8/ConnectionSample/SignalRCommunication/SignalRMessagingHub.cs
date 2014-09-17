using Microsoft.AspNet.SignalR.Client;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SRHS2backend;

namespace ConnectionSample
{
    public class SignalRMessagingHub : ISignalRHub
    {
        #region "Members"

        
        IHubProxy SignalRGameHub;


        // Use the specific port# for local server or actual URI if SignalR backend is hosted.     

        /*HubConnection mapConnection = new HubConnection("http://kaharri.azurewebsites.net/");
        HubConnection chatConnection = new HubConnection("http://kaharri.azurewebsites.net/");
        HubConnection gameConnection = new HubConnection("http://kaharri.azurewebsites.net/");
        HubConnection objConnection = new HubConnection("http://kaharri.azurewebsites.net/");
         */
        //HubConnection gameConnection = new HubConnection("http://localhost:62127/");

        HubConnection gameConnection = new HubConnection("http://handssignalr.azurewebsites.net/");
        public event SignalRServerHandler SignalRServerNotification = (s, e) => { };

        #endregion

        #region "Constructor"

        public SignalRMessagingHub()
        {
            // Reference to SignalR Server Hub & Proxy.           
            SignalRGameHub = gameConnection.CreateHubProxy("gameHub");
            //SignalRMapHub = mapConnection.CreateHubProxy("MapHub");
            // SignalRChatHub = chatConnection.CreateHubProxy("ChatHub");
            //SignalRGameScoreHub = gameConnection.CreateHubProxy("GameScoreHub");
            //SignalRObjSyncHub = objConnection.CreateHubProxy("ObjectSyncHub");
        }

        #endregion

        #region "Implementation"

        public async virtual void UserLogin(User tabletChatClient)
        {
            // Fire up SignalR Connection & join chatroom.  
            try
            {
                await gameConnection.Start();

                if (gameConnection.State == Microsoft.AspNet.SignalR.Client.ConnectionState.Connected)
                {
                    await SignalRGameHub.Invoke("Login", tabletChatClient);
                }
            }
            catch (Exception ex)
            {

                // Do some error handling. Could not connect to Sever Error.
                Debug.WriteLine("Error: "+ ex.Message);
            }

            // On 

            // Listen to chat events on SignalR Server & wire them up appropriately.
            SignalRGameHub.On<User, ServerMessage>("addNewUpdate", (user, message) =>
            {
                SignalREventArgs userArgs = new SignalREventArgs();
                userArgs.UserUpdate = user;
                userArgs.CustomServerMessage = message;

                // Raise custom event & let it bubble up.
                SignalRServerNotification(this, userArgs);
            });
            SignalRGameHub.On<string, string>("addNewMessageToPage", (message, word) =>
            {
                SignalREventArgs chatArgs = new SignalREventArgs();
                chatArgs.ChatMessageFromServer = message;
                chatArgs.ChatMessageFromServer = word;
                // Raise custom event & let it bubble up.
                SignalRServerNotification(this, chatArgs);
            });
            SignalRGameHub.On<User, List<Game>, List<User>, ServerMessage>("update", (user, agl, ul, sm) =>
            {
                SignalREventArgs uArgs = new SignalREventArgs();
                uArgs.UserUpdate = user;
                uArgs.CustomGameList = agl;
                uArgs.CustomAvailableOpponents = ul;
                uArgs.CustomServerMessage = sm;
                // Raise custom event & let it bubble up.
                SignalRServerNotification(this, uArgs);
            });
            SignalRGameHub.On<Game, ServerMessage>("gameCreated", (g, sm) =>
            {
                SignalREventArgs gArgs = new SignalREventArgs();
                gArgs.CustomGameObject = g;
                gArgs.CustomServerMessage = sm;
                // Raise custom event & let it bubble up.
                SignalRServerNotification(this, gArgs);
            });
            SignalRGameHub.On<User, Game, ServerMessage>("lobbyMessage", (u, g, sm) =>
            {
                SignalREventArgs gArgs = new SignalREventArgs();
                gArgs.UserUpdate = u;
                gArgs.CustomGameObject = g;
                gArgs.CustomServerMessage = sm;
                // Raise custom event & let it bubble up.
                SignalRServerNotification(this, gArgs);
            });
            SignalRGameHub.On<Game, InGameMessage>("inGameMessage", (g, im) =>
            {
                SignalREventArgs gArgs = new SignalREventArgs();
                gArgs.CustomGameObject = g;
                gArgs.InGameActionMessageEvent = im;
                // Raise custom event & let it bubble up.
                SignalRServerNotification(this, gArgs);
            });
        }

        public async virtual void UserUpdate(User tabletChatMessage, ServerMessage s)
        {
            // Post message to Server Chatroom.
            await SignalRGameHub.Invoke("UpdateUser", tabletChatMessage, s);
        }

        public async virtual void CreateGame(User tabletChatClient, Game game)
        {
            // Leave the Server's Chatroom.
            await SignalRGameHub.Invoke("CreateGame", tabletChatClient, game);
        }

        public async virtual void JoinGame(User x, Game g)
        {
            // Post message to Server Chatroom.
            await SignalRGameHub.Invoke("JoinGame", x, g);
        }

        public async virtual void StartGame(Game game)
        {
            // Post message to Server Chatroom.
            await SignalRGameHub.Invoke("StartGame", game);
        }

        public async virtual void InGameMessageCall(Game game, InGameMessage im)
        {
            // Post message to Server Chatroom.
            await SignalRGameHub.Invoke("InGameMessageCall", game, im);
        }

        public async virtual void Test(string x, User y)
        {
            // Post message to Server Chatroom.
            await SignalRGameHub.Invoke("Test", x, App.Current.AppUser);
        }





        #endregion

        #region "Methods"

        public virtual void OnSignalRServerNotificationReceived(SignalREventArgs e)
        {
            if (SignalRServerNotification != null)
            {
                SignalRServerNotification(this, e);
            }
        }

        #endregion
    }
}
