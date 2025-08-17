using System;
using Cassandra.Mapping.Attributes; // For mapping attributes

namespace core8_nuxt_cassandra.Entities
{
    [Table("users", Keyspace = "core8")] // Maps to a Cassandra table
    public class User {

        [Column("id")]
        // [PartitionKey(0)] // Marks as part of the partition key (if composite, specify order)
        public Guid Id { get; set; }

        [Column("lastname")]
        public string LastName {get; set;}

        [Column("firstname")]
        public string FirstName {get; set;}

        [Column("username")]
        public string UserName {get; set;}

        [Column("password")]
        public string Password {get; set;}

        [Column("email")]
        public string Email { get; set; }

        [Column("mobile")]
        public string Mobile { get; set; }

        [Column("roles")]
        public string Roles { get; set; }

        [Column("isactivated")]
        public int Isactivated {get; set;}

        [Column("isblocked")]
        public int Isblocked {get; set;}

        [Column("mailtoken")]
        public int Mailtoken {get; set;}

        [Column("qrcodeurl")]
        public string Qrcodeurl {get; set;}

        [Column("profilepic")]
        public string Profilepic {get; set;}

        [Column("secretkey")]
        public string Secretkey  {get; set;}

        [Column("createdAt")]
        public DateTime CreatedAt  {get; set;}

        [Column("updatedAt")]
        public DateTime UpdatedAt  {get; set;}

    }
}