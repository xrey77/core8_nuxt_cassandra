using System;

namespace core8_nuxt_cassandra.Models.dto
{
    public class UserModel {
        public Guid Id { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }
        public string Username { get; set; }
        public string Password { get; set; }
        public string Mobile { get; set; }
        public string Roles { get; set; }
        public string Profilepic { get; set; }
        public string Secretkey { get; set; }
        public int Isactivated { get; set; }        
        public int Isblocked { get; set; }
        public string Qrcodeurl { get; set; }
        public int Mailtoken { get; set; }
        public DateTime UpdatedAt { get; set; } = default;
    }    
}