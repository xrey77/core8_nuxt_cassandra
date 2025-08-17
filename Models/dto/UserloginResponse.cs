using System;

namespace core8_nuxt_cassandra.Models.dto
{
    public class UserloginResponse {
        public string Id { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string UserName { get; set; }
        public string Roles { get; set; }
        public string Profilepic { get; set; }
        public int Isactivated { get; set; }        
        public int Isblocked { get; set; }
    }    
}