using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WinPhone_HS_App
{
    public class ServerMessage
    {
        public ServerMessage()
        {

        }
        public ServerMessage(int command, string action, string message)
        {
            this.Command = command;
            this.Action = action;
            this.Message = message;
        }
        public int Command { get; set; }
        public string Message { get; set; }
        public string Action { get; set; }
    }
}
