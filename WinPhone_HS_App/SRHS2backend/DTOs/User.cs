using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace SRHS2backend
{
    public class User
    {
        public User()
        {

        }
        public User(string UID, string uName)
        {
            this.UserId = UID;
            this.UserName = uName;
        }
        //ID | RANK | User Name | Password | Total Points | Games Won as Sphero | Games Won as  Drone | Games Lost as Sphero | Games Lost as Drone | 

        public string ContextId { get; set; }

        [JsonProperty(PropertyName = "userid")]
        public string UserId { get; set; }

        [JsonProperty(PropertyName = "username")]
        public string UserName { get; set; }

        [JsonProperty(PropertyName = "score")]
        public int Score { get; set; }

        [JsonProperty(PropertyName = "datejoined")]
        public DateTime DateJoined { get; set; }

        [JsonProperty(PropertyName = "gwas")]
        public int GWAS { get; set; }

        [JsonProperty(PropertyName = "gwad")]
        public int GWAD { get; set; }

        [JsonProperty(PropertyName = "glas")]
        public int GLAS { get; set; }

        [JsonProperty(PropertyName = "glad")]
        public int GLAD { get; set; }

    }
}
