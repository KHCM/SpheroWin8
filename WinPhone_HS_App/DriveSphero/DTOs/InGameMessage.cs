using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DriveSample
{
    public class InGameMessage
    {
        public InGameMessage()
        {

        }
        public InGameMessage(string jsonMessage)
        {

            // Parse string
            // Fill in properties here
            // this.ID = whaever came from json
            // this.GameID = whatever else came from json

            InGameMessage gameObjectMessage = JsonConvert.DeserializeObject<InGameMessage>(jsonMessage);
            this.CurrentTime = (gameObjectMessage.CurrentTime);
            this.UserID = (gameObjectMessage.UserID);
            this.GameID = (gameObjectMessage.GameID);
            this.Action = (gameObjectMessage.Action);
            this.GameState = (gameObjectMessage.GameState);
            this.OpponentID = (gameObjectMessage.OpponentID);
            this.MaxHits = (gameObjectMessage.MaxHits);
            this.MaxTime = (gameObjectMessage.MaxTime);
            this.Hits = (gameObjectMessage.Hits);

        }

        //currentTime, userID, gameID, action, data);
        public DateTime CurrentTime { get; set; }
        public string UserID { get; set; }
        public string GameID { get; set; }
        public string Action { get; set; }
        //Data
        public int GameState { get; set; }
        public string OpponentID { get; set; }
        public int MaxHits { get; set; }
        public string MaxTime { get; set; }
        public int Hits { get; set; }
    }

}
