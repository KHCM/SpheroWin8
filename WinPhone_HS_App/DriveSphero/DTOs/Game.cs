using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;


namespace DriveSample
{

    public class Game
    {
        public Game()
        {

        }
        public Game(string gid, User testSpheroP1, User testDroneP2, int state, int status, int time, int hit, string opn, string crtn)
        {
            this.SpheroPlayer = testSpheroP1;
            this.DronePlayer = testDroneP2;
            this.GameStatus = status;
            this.OpponentName = opn;
            this.CreatorName = crtn; 
            this.GameState = state;
            this.MaxTime = time;
            this.MaxHits = hit;
            this.GameId = gid;
        }

        public string Id { get; set; }

        //Guid.NewGuid() Use this to get unique id in server
        [JsonProperty(PropertyName = "gameid")]
        public string GameId { get; set; }

        public string OpponentName { get; set; }
        public string CreatorName { get; set; }

        public string DronePlayerName { get; set; }
        public string SpheroPlayerName { get; set; }

        [JsonProperty(PropertyName = "datecreated")]
        public DateTime DateCreated { get; set; }
        public DateTime StartTime { get; set; }

        [JsonProperty(PropertyName = "gamestatus")]
        public int GameStatus { get; set; }
        public int GameState { get; set; }

        [JsonProperty(PropertyName = "spheroplayer")]
        public User SpheroPlayer { get; set; }

        [JsonProperty(PropertyName = "droneplayer")]
        public User DronePlayer { get; set; }

        [JsonProperty(PropertyName = "maxtime")]
        public int MaxTime { get; set; }

        [JsonProperty(PropertyName = "maxhits")]
        public int MaxHits { get; set; }

        [JsonProperty(PropertyName = "winner")]
        public User Winner { get; set; }

    }


}
