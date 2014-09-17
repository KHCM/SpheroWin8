using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SRHS2backend;
using WinPhone_HS_App; 

namespace WinPhone_HS_App
{
    // Custom delegate.
    public delegate void SignalRServerHandler(object sender, SignalREventArgs e);

    public interface ISignalRHub
    {
        // This is the interface to SignalRMessagingHub
        // Custom event to act when something happens on SignalR Server.
        event SignalRServerHandler SignalRServerNotification;

        // Operations on SignalR server.
        void UserLogin(User user);
        void UserUpdate(User user, ServerMessage s);
        void CreateGame(User client, Game game);
        void JoinGame(User user, Game game);
        void StartGame(Game game);
        void InGameMessageCall(Game game, InGameMessage im);
        void Test(string x, User u);
        //void JoinGameLobby(User client, Game gameID);
        //void LeaveLobby(User client, Game gameID);

        //void Chat(string tabletChatMessage, string user, string gameID);
        //void LoadMainUpdate(User user);

        //void GameAction(InGameMessage gameMessage);
        //void GameInit(Game gameSpecs, InGameMessage message);

    }
}