using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace DriveSample
{
    public class User
    {
        public User() : this("defaultID","defaultName")
        {
         
        }
        public User(string UID, string uName)
        {
            if( UID == null || String.IsNullOrEmpty(UID))
            {
                throw new ArgumentException("User must have a UID");
            }

            if( uName == null || String.IsNullOrEmpty(uName))
            {
                throw new ArgumentException("A user requires a user name");
            }

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
