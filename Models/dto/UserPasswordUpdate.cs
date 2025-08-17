using System;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Http;

namespace core8_nuxt_cassandra.Models.dto
{
  public class UserPasswordUpdate
    {        
        public string Password { get; set; }
    }

    
}