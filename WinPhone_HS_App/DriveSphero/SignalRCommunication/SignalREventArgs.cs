using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SRHS2backend;

namespace DriveSample
{
    public class SignalREventArgs : EventArgs
    {
        // Custom args.
        public Game CustomGameObject { get; set; } //This is the listener from the server in MessagingHub
        public ServerMessage CustomServerMessage { get; set; }
        public InGameMessage InGameActionMessageEvent { get; set; }
        public User UserUpdate { get; set; }
        public List<Game> CustomGameList { get; set; }
        public List<User> CustomAvailableOpponents { get; set; }
        public int TeamAScore { get; set; }
        public int TeamBScore { get; set; }
        public string ChatTest { get; set; }
        public string ChatMessageFromServer { get; set; }
    }
}
