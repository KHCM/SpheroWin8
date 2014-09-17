using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Microsoft.AspNet.SignalR;
using Microsoft.AspNet.SignalR.Hubs;
using System.Diagnostics;
using System.Timers;
using CommonUtilities;

namespace SRHS2backend
{
   [HubName("gameHub")]
    public class GameHub : Hub
    {
        private static List<User> OnlineUsers = new List<User>();
        private static List<string> UsersConextID = new List<string>();
        private static List<Game> AvailableGames = new List<Game>(); 
        private static List<Game> AllGamesList = new List<Game>();
        private static List<Game> InPlayGames = new List<Game>();

        public static User tu1 = new User("tu1ID", "tu1");
        public static User tu2 = new User("tu2ID", "tu2");
        public static User tu3 = new User("tu3ID", "tu3");
        public static User tu4 = new User("tu4ID", "tu4");

        private Game testGame1 = new Game("testG1", testClientUser, tu1, 0, 0, 5, 1, tu1.UserName);
        private Game testGame2 = new Game("testG2", testClientUser, tu2, 0,0, 5, 2, tu2.UserName);
        private Game testGame3 = new Game("testG3", testClientUser, tu3, 0,0, 10, 1, tu3.UserName);
        private Game testGame4 = new Game("testG4", testClientUser, tu1, 0, 0,10, 2, tu1.UserName);
        private Game testGame5 = new Game("testG5", testClientUser, tu4, 0,0, 10, 3, tu4.UserName);

        private static User testClientUser = new User("userID", "test Client User");


        private static User testUser = new User("opponentID", "opponent");
        private static User testUser2 = new User("op2ID", "user1");
        private static Game testGame = new Game("test", testUser, testUser2, 0,0, 3, 1, testUser2.UserName);

        //public DispatcherTimer timer = new DispatcherTimer();
        //public Timer _serverTimer = new Timer(3000);
        public bool playingGames = false; 

        public void Login(User user)
        {
            if (user == null)
            {
                //TODO: YOU WILL NEED Context.ConnectionId
                //throw new ArgumentException("User handed to Login() is null");
            }
            AllGamesList.Add(testGame);
            AvailableGames.Add(testGame);
            AvailableGames.Add(testGame1);
            AvailableGames.Add(testGame2);
            AvailableGames.Add(testGame3);
            AvailableGames.Add(testGame4);
            user.ContextId = Context.ConnectionId;
            tu1.ContextId = Context.ConnectionId;
            tu2.ContextId = Context.ConnectionId;
            tu3.ContextId = Context.ConnectionId;
            tu4.ContextId = Context.ConnectionId;
            // Add client & broadcast new chat user message to all clients connected to this Hub.
            OnlineUsers.Add(user);
            OnlineUsers.Add(tu1);
            OnlineUsers.Add(tu2);
            OnlineUsers.Add(tu3);
            OnlineUsers.Add(tu4);
            UsersConextID.Add(Context.ConnectionId);
            //Clients.All.addChatMessage(newPlayer.UserName + " has joined the Chatroom!");
            //Clients.All.addNewMessageToPage("Loing", "New user = " + user.UserName);

            ServerMessage message = new ServerMessage();
            message.Command = 0;
            message.Action = "login";
            message.Message = "You are Logged in.";
            //Clients.All.addNewUpdate(user, message);

            //OnlineUsers.Add(testUser);
            //UsersConextID.Add(testUser.UserId);
            //Clients.User(user.UserName).mainUpdate("user", "  hi");
            //Clients.All.mainUpdate("ALL", "  SENt");
            //Clients.User(user.UserName).mainUpdate(user, usersGames, message);
            // Organizing clients in groups ..
            //Groups.Add(Context.ConnectionId, "OnlineUsers");
            //Clients.Group("OnlineUsers").addNewUpdate(user, message);
            //Clients.Group("OnlineUsers").addNewMessageToPage("LOGIN", "From Group addNewMessagePage");
            //Clients.All.addNewUpdate(user, message);
            //Test("Server", user);
            UpdateUser(user, message);
        }
        /*******
         * 
         * EDIT THIS
         * 
        public void LeaveChatRoom(User userToRemove)
        {
            // Clean-up.
            ConnectedClientsList.Remove(userToRemove);
            Clients.All.addChatMessage(userToRemove.UserName + " has left the Chatroom!");

            Groups.Remove(Context.ConnectionId, "ChatRoom A");
        }

        public void PushChatMessageToClients(string message)
        {
            // Push to all connected clients.
            Clients.All.addChatMessage(message);

            // Communicate to a Group.
            // Clients.Group("ChatRoom A").addChatMessage(message);

            // Invoke a method on the calling client only.
            // Clients.Caller.addChatMessage(message);

            // Similar to above, the more verbose way.
            // Clients.Client(Context.ConnectionId).addChatMessage(message);            
        }
         */
        public void Send(string name, string message)
        {
            // Call the addNewMessageToPage method to update clients.
            User temp = new User();
            temp.UserName = "Temp";
            ServerMessage sm = new ServerMessage();
            sm.Command = 0; 
            sm.Action = "Login";
            sm.Message = "TEST ON WEB CLICK";
            Clients.All.addNewMessageToPage(name, message);
            Clients.All.addNewUpdate(temp, sm);
            //Clients.User("user1").addNewMessageToPage(name, message);
            Clients.Group("OnlineUsers").addNewMessageToPage(name, message);
        }
        public void Test(string name, User u)
        {
            // Call the addNewMessageToPage method to update clients.
            User temp = new User();
            temp.UserName = "Temp";
            ServerMessage sm = new ServerMessage();
            sm.Command = 0;
            sm.Action = "Click";
            sm.Message = "Test Method - " + u.UserName;
            //Clients.All.addNewMessageToPage(name);
            Clients.All.addNewUpdate(temp, sm);
            //Clients.User("user1").addNewMessageToPage(name, message);
            Clients.Group("OnlineUsers").addNewMessageToPage("Groups is working");
        }
        public void UpdateUser(User u, ServerMessage s)
        {
            //TODO: CHECK if u == null or s == null

            int i = -1;

            i = OnlineUsers.FindIndex(x => x.UserName.Equals(u.UserName));
            if (i >-1)
            {
                if (u.ContextId == null)
                {
                    //make u = FindIndex
                    u = OnlineUsers[i];
                }
                else
                {
                    OnlineUsers.RemoveAt(i);
                    OnlineUsers.Insert(i, u);
                }

            }
            else
            {
                OnlineUsers.Add(u);
            }
            List<Game> usersGames = new List<Game>();
            List<User> oppList = new List<User>();
            foreach (User opp in OnlineUsers)
            {
                if (opp.UserName != u.UserName)
                    oppList.Add(opp);
            }

            //Check to see if there are any available games, //... check to see game history...
            foreach (Game g in AvailableGames)
            {
                if (g.DronePlayer.UserName == u.UserName || g.SpheroPlayer.UserName == u.UserName)
                {
                    usersGames.Add(g);
                }
            }
            ServerMessage sm = new ServerMessage();

            if(s.Action == Common.ServerCommand.login.ToString()){
                sm.Command = 0;
                sm.Action = Common.ServerCommand.login.ToString();
                sm.Message = "User Login successful.";
            }
            if (s.Action == "update")
            {
                sm.Command = 1;
                sm.Action = "update";
                sm.Message = "Updateing Games and Sats";
            }
            if(s.Action == "end"){
                sm.Command = 1;
                sm.Action = "update";
                sm.Message = "User Returning to Main Hub ";
            }

            // Call the addNewMessageToPage method to update clients.
            
           /* if (u.UserId != null)
            {

                Clients.User(u.UserId).update(u, usersGames, sm);
            }
            else{
                Clients.All.update(u, usersGames, sm);
            }*/
            Clients.All.update(u, usersGames, oppList,  sm);

            //Clients.User("user1").addNewMessageToPage(name, message);
            //Clients.Group("OnlineUsers").update(u, usersGames, sm);
        }

 /*
  * We want to create a game
  * Then send a game created event - this triggers Client side (Do you want to Join this game)
  * So the created events needs game message and message and sent to all Users!
  * 
  */
        public void CreateGame(User user, Game g)
        {
            // Call the addNewMessageToPage method to update clients.
            AllGamesList.Add(g);
            AvailableGames.Add(g);
            //int i = AllGamesList.FindIndex(x => x.Equals(g));
           // g.Id = i.ToString(); 
            //AvailableGames.Add(g);


            ServerMessage sm = new ServerMessage();
            sm.Command = 2;
            sm.Action = "created";
            sm.Message = "game created";
            //Clients.User(g.SpheroPlayer.UserName).gameCreated(g, sm); //The User() takes contextID - so make UID = contextID

            //string context;

            //Clients.User("user1").addNewMessageToPage(name, message);
            //if (user.UserId != null) {
            //    context = user.UserId;
            //    Clients.User(context).gameCreated(g, sm);

            //}
            //else
            //{
            //    Clients.All.gameCreated(g, sm);

            //}
            Clients.All.gameCreated(g, sm);
        }

        public void JoinGame(User user, Game game)
        {
            ServerMessage jm = new ServerMessage();
            jm.Command = 3;
            int a = -1;
            a = AvailableGames.FindIndex(fg => fg.GameId.Equals(game.GameId) && fg.DateCreated.Equals(game.DateCreated) && (fg.DronePlayer.UserId.Equals(user.UserId) || fg.SpheroPlayer.UserId.Equals(user.UserId)));
            bool triggerStart = false;
            if (a > 0 )
            {
                game.GameStatus++;
                if (game.GameStatus <= 2)
                {
                    jm.Action = "join";
                    jm.Message = user.UserName + " has entered the Lobby";
                    AvailableGames.RemoveAt(a);
                    AvailableGames.Insert(a, game);
                    Groups.Add(Context.ConnectionId, game.GameId);
                    Clients.Group(game.GameId).lobbyMessage(user, game, jm);
                }
                else
                {
                    if (game.GameStatus==3)
                    {
                        jm.Action = "ready";
                        jm.Message = user.UserName + " is Ready to Start the game. ";
                        AvailableGames.RemoveAt(a);
                        AvailableGames.Insert(a, game);
                        //Groups.Add(Context.ConnectionId, game.GameId);
                        Clients.Group(game.GameId).lobbyMessage(user, game, jm);
                    }
                    if (game.GameStatus == 4)
                    {
                        InGameMessage igm = new InGameMessage();
                        igm.Action = "start";
                        
                        game.GameState = 0;

                        /*
                        if (InPlayGames.Count == 0) //No Games Are Running
                        {
                            //Start Timer ...for duration of the game
                            //on every tick check all the games end time === startime + timeInterval of maxtime*60*1000
                            //if curtime < endtime Do nothing... if curtime> endtime then send end event! for that game with igm and keep going...
                            startServerTimer(game);
                        }*/
                        AvailableGames.RemoveAt(a);

                        InPlayGames.Add(game);

                        //Groups.Add(Context.ConnectionId, game.GameId);
                        Clients.Group(game.GameId).inGameMessage(game, igm);
                    }

                }
               
            }
            else
            {
                if (AvailableGames.ElementAt(a).GameId != game.GameId)
                {
                    jm.Message = "ERROR - TRIED TO JOIN A GAME THAT DOESN'T EXIST";
                    //game not in list
                    Debug.WriteLine("ERROR - TRIED TO JOIN A GAME THAT DOESN'T EXIST");
                }
                else
                {
                    jm.Message = "ERROR - Tried to Join a game you weren't invited to";
                    //game not in list
                    Debug.WriteLine("ERROR - WRONG USER, trying to access game");
                }
            }

        }

       /*
        private async void startServerTimer(Game g)
        {
            await Dispatcher.RunAsync(DispatcherProiority.High, () =>
           {

               g.StartTime = DateTime.Now; 
               _serverTimer.AutoReset = true;
               _serverTimer.Elapsed += new ElapsedEventHandler(OnTimedEvent);
               _serverTimer.Start();
           });
        }*/

        private void OnTimedEvent(object sender, ElapsedEventArgs e)
        {
            Debug.WriteLine("TIMER WORKING!");
        }


        public void InGameMessageCall(Game game, InGameMessage im)
        {
            //send start message
            //Check to see if timer is up
            //Check to see what states match //Correct State
            //check to see what is the command //Valid Command
            Game g = new Game();


            g = InPlayGames.Find(fg => fg.GameId.Equals(game.GameId) && fg.DateCreated.Equals(game.DateCreated));// && (fg.DronePlayer.UserId.Equals(user.UserId) || fg.SpheroPlayer.UserId.Equals(user.UserId)));
            int place = InPlayGames.FindIndex(fg => fg.GameId.Equals(game.GameId) && fg.DateCreated.Equals(game.DateCreated));
            if (game.GameState != g.GameState)
            {
                //User is in the wrong state send inGameMessage with the correct state and Game object
            }
            else
            {
                switch (game.GameState)
                {
                    case 0:
                        //Actions - update GameObject - start Time, targeted, end
                        if (im.Action == "target")
                        {
                            im.Action = "targeted";
                            //if I'm a sphero display Warning
                            game.GameState = 1;
                            InPlayGames.RemoveAt(place);
                            InPlayGames.Insert(place, game);
                            Clients.Group(game.GameId).inGameMessage(game, im);
                           
                        }
                        break;
                    case 1:
                        if (im.Action == "lost")
                        {
                            im.Action = "lost";
                            //if I'm a sphero display Warning
                            game.GameState = 0;
                            InPlayGames.RemoveAt(place);
                            InPlayGames.Insert(place, game);
                            Clients.Group(game.GameId).inGameMessage(game, im);

                        }
                        if (im.Action == "fire")
                        {
                            im.Hits++; 
                            if (im.Hits == game.MaxHits)
                            {
                                im.Action = "end";
                                //_serverTimer.Stop();
                                game.Winner = game.DronePlayer; 
                                game.GameState = 2;

                                InPlayGames.RemoveAt(place);
                                AllGamesList.Add(game);
                                //AvailableGames.Insert(place, game);
                                Clients.Group(game.GameId).inGameMessage(game, im);
                                //find game replace..
                                //send end message im.Action end, 
                                RemovePlayer(game.DronePlayer, game.GameId);
                                RemovePlayer(game.SpheroPlayer, game.GameId);
                                
                                
                            }
                            else
                            {
                                im.Action = "hit";                               
                                //if I'm a sphero display Warning
                                game.GameState = 0;
                                InPlayGames.RemoveAt(place);
                                InPlayGames.Insert(place, game);
                                Clients.Group(game.GameId).inGameMessage(game, im);
                            }

                        }

                        break; 
                    case 2:
                        break; 


                }
                //targeted - change state of Game Object (insert it into GameList)
                //
            }
            //Debug.WriteLine("Server is in Start Method");
            //Clients.Group(game.GameId).inGameMessage(game, im);
        }

        private void RemovePlayer(User user, string gameID)
        {
            if (user != null && !String.IsNullOrEmpty(user.ContextId) && !String.IsNullOrEmpty(gameID))
            {
                Groups.Remove(user.ContextId, gameID);
            }
            else
            {
                throw new ArgumentException("Invalid param sent to removePlayer()");
            }
        }
    }
}