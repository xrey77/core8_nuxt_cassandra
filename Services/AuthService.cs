using System;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using Cassandra;
using core8_nuxt_cassandra.Entities;
using core8_nuxt_cassandra.Helpers;
using core8_nuxt_cassandra.Models;
using core8_nuxt_cassandra.Models.dto;

namespace core8_nuxt_cassandra.Services
{    
    public interface IAuthService {
        Task<User> SignupUser(User userdata, string passwd);
        Task<User> SigninUser(string usrname, string pwd);
    }

    public class AuthService : IAuthService
    {
        private readonly Cassandra.ISession session;                

        IConfiguration config = new ConfigurationBuilder()
        .AddJsonFile("appsettings.json")
        .AddEnvironmentVariables()
        .Build();

        public AuthService(IConfiguration config)
        {
            try {
                var cluster = Cluster.Builder()
                    .AddContactPoint(config["CassandraSettings:ContactPoints"])
                    .WithPort(int.Parse(config["CassandraSettings:port"]))
                    .WithCredentials(config["CassandraSettings:Username"], config["CassandraSettings:Password"])
                    .Build();

                session = cluster.Connect(config["CassandraSettings:Keyspace"]);
                Console.WriteLine($"Connected to Cassandra...");            
            } catch(Exception ex) {
                Console.WriteLine($"Failed to connect to Cassandra: {ex.Message}");            
            }
        }

        public async Task<User> SignupUser(User userdata, string passwd)
        {
            var sql1 = new SimpleStatement("SELECT * FROM core8.users WHERE email = ? ALLOW FILTERING", userdata.Email);
            var rowSet1 = await session.ExecuteAsync(sql1);
            foreach(var row in rowSet1) {
                if (row is not null) {
                    throw new AppException("Email Address is already taken...");
                }
            }

            var sql2 = new SimpleStatement("SELECT * FROM core8.users WHERE username = ? ALLOW FILTERING", userdata.UserName);
            var rowSet2 = await session.ExecuteAsync(sql2);
            foreach(var row in rowSet2) {
                if (row is not null) {
                    throw new AppException("Username is already taken...");
                }
            }

            var tokenHandler = new JwtSecurityTokenHandler();
            var xkey = config["Jwt:Key"];
            var key = Encoding.ASCII.GetBytes(xkey);

            // CREATE SECRET KEY FOR USER TOKEN===============
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new Claim[]
                {
                    new Claim(ClaimTypes.Name, userdata.Email)
                }),
                // Expires = DateTime.UtcNow.AddDays(7),
                Expires = DateTime.UtcNow.AddHours(4),
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
            };
            var secret = tokenHandler.CreateToken(tokenDescriptor);
            var secretkey = tokenHandler.WriteToken(secret);
            userdata.Isactivated = 0;
            userdata.Isblocked = 0;
            userdata.Secretkey = secretkey.ToUpper();             
            userdata.Password = BCrypt.Net.BCrypt.HashPassword(passwd);
            userdata.Profilepic = "https://localhost:7100/users/pix.png";
            userdata.Roles="USER";
            DateTime now = DateTime.Now;
            userdata.Id = Guid.NewGuid(); 
            string sqlReg = "INSERT INTO core8.users(id, firstname, lastname, email, mobile, username, password, profilepic, roles, createdAt, isactivated, isblocked, secretkey) Values(?,?,?,?,?,?,?,?,?,?,?,?,?)";
            var prep = await session.PrepareAsync(sqlReg);
            var boundStm = prep.Bind(userdata.Id, userdata.FirstName, userdata.LastName, userdata.Email, userdata.Mobile, userdata.UserName, userdata.Password, userdata.Profilepic, userdata.Roles, now, userdata.Isactivated, userdata.Isblocked, userdata.Secretkey);
            await session.ExecuteAsync(boundStm);            
            return userdata;
        }

        public async Task<User> SigninUser(string usrname, string pwd)
        {
            try {
                var userdata = new User();
                var sqlLogin = new SimpleStatement("SELECT * FROM core8.users WHERE username = ? ALLOW FILTERING", usrname);
                var rowSet = await session.ExecuteAsync(sqlLogin);
                var xrow = rowSet.FirstOrDefault();
                if (xrow is null) {
                    throw new AppException("Username not found.");
                }

                var cql = new SimpleStatement("SELECT * FROM core8.users WHERE username = ? ALLOW FILTERING", usrname);
                var rowSet2 = await session.ExecuteAsync(cql);

                foreach(var row in rowSet2) {
                    if (row.GetValue<int>("isactivated") == 0) {
                        throw new AppException("Please activate your account, check your email client inbox and click or tap the Activate button.");
                    }

                    if (!BCrypt.Net.BCrypt.Verify(pwd, row.GetValue<string>("password"))) {
                        throw new AppException("Incorrect Password...");
                    }

                    userdata.Id = row.GetValue<Guid>("id");
                    userdata.FirstName = row.GetValue<string>("firstname");
                    userdata.LastName = row.GetValue<string>("lastname");
                    userdata.UserName = row.GetValue<string>("username");
                    userdata.Profilepic = row.GetValue<string>("profilepic");
                    userdata.Roles = row.GetValue<string>("roles");
                    userdata.Isactivated = row.GetValue<int>("isactivated");
                    userdata.Isblocked = row.GetValue<int>("isblocked");       
                }
                return userdata;
            } catch(AppException ex) {
                    throw new AppException(ex.Message);                   
            }
        }
    }
}