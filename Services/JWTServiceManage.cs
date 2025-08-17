using AutoMapper.Internal;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;
using System.Linq;
using System;
using Cassandra;
using core8_nuxt_cassandra.Helpers;
using core8_nuxt_cassandra.Entities;

namespace core8_nuxt_cassandra.Services
{
    public interface IJWTTokenServices
    {
        Task<JWTTokens> Authenticate(User users);
    }
    public class JWTServiceManage : IJWTTokenServices
    {
        private readonly IConfiguration _configuration;
        private readonly Cassandra.ISession session;                
        private readonly AppSettings _appSettings;
 
        public JWTServiceManage(
            IConfiguration config,            
            IOptions<AppSettings> appSettings)        
        {
            _appSettings = appSettings.Value;
            _configuration = config;
            var cluster = Cluster.Builder()
                .AddContactPoint(config["CassandraSettings:ContactPoints"])
                .WithPort(int.Parse(config["CassandraSettings:port"]))
                .WithCredentials(config["CassandraSettings:Username"], config["CassandraSettings:Password"])
                .Build();

            session = cluster.Connect(config["CassandraSettings:Keyspace"]);
        }

        public async Task<JWTTokens> Authenticate(User users)
        {
            string sqlAuth = $"SELECT * FROM core8.users WHERE username = ? AND password = ?";
            PreparedStatement prepStm = await session.PrepareAsync(sqlAuth);
            BoundStatement boundStm = prepStm.Bind(users.UserName, users.Password);
            var rowSet = await session.ExecuteAsync(boundStm);
            foreach(var row in rowSet) {
                if (row is null) {
                    throw new AppException("User not found");
                }
            }

            string sqlAuth2 = $"SELECT * FROM core8.users WHERE username = ?";
            var rowSet2 = await session.ExecuteAsync(new SimpleStatement(sqlAuth),users.UserName);
            foreach(var row in rowSet2) {
                if (row is null) {
                    throw new AppException("Username not found");
                }
            }
 
            var tokenhandler = new JwtSecurityTokenHandler();
            var tkey = Encoding.UTF8.GetBytes(_configuration["Jwt:Key"]);
            var ToeknDescp = new SecurityTokenDescriptor
            {
                Subject = new System.Security.Claims.ClaimsIdentity(new Claim[]
                {
                    new Claim(ClaimTypes.Name, users.UserName)
                }),
                Expires = DateTime.UtcNow.AddMinutes(5),
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(tkey), SecurityAlgorithms.HmacSha256Signature)
            };
            var toekn = tokenhandler.CreateToken(ToeknDescp);
            return new JWTTokens { Token = tokenhandler.WriteToken(toekn) };
        }
    }    
    
}