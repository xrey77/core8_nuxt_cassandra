using System;
using System.Collections.Generic;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;
using Cassandra;
using core8_nuxt_cassandra.Entities;
using core8_nuxt_cassandra.Helpers;
using core8_nuxt_cassandra.Models.dto;

namespace core8_nuxt_cassandra.Services
{
    public interface IUserService {
        IEnumerable<User> GetAll();
        User GetById(Guid id);
        Task<bool> ProfileUpdate(User user);
        Task<bool> Delete(Guid id);
        Task<bool> ActivateMfa(Guid id, bool opt, string qrcode_url);
        Task<bool> UpdatePicture(Guid id, string file);
        Task<bool> UpdatePassword(User user);
        int EmailToken(int etoken);
        Task<int> SendEmailToken(string email);
        Task<bool> ActivateUser(Guid id);
        Task<string> FullName(string email);
    }

    public class UserService : IUserService
    {
        private readonly Cassandra.ISession session;   

        public UserService(IConfiguration config)            
        {
                var cluster = Cluster.Builder()
                .AddContactPoint(config["CassandraSettings:ContactPoints"])
                .WithPort(int.Parse(config["CassandraSettings:port"]))
                .WithCredentials(config["CassandraSettings:Username"], config["CassandraSettings:Password"])
                .Build();

                session = cluster.Connect(config["CassandraSettings:Keyspace"]);            
        }

        public async Task<bool> Delete(Guid id)
        {
                var preparedStatement = await session.PrepareAsync("SELECT * FROM core8.users WHERE id = ?");
                var statement1 = preparedStatement.Bind(id);
                var rowSet = await session.ExecuteAsync(statement1);
                var row = rowSet.FirstOrDefault();

                if (row is not null) {
                    var findRec = "DELETE FROM core8.users WHERE id = ? IF EXISTS";
                    var statement2 = new SimpleStatement(findRec, id);
                    await session.ExecuteAsync(statement2);
                    return true;
                } else {
                    return false;
                }
        }


        public IEnumerable<User> GetAll()
        {
            var records = new List<User>();
            var cql = $"SELECT * FROM core8.users";
            var rowSet = session.Execute(new SimpleStatement(cql));
            foreach (var row in rowSet)
            {
                records.Add(new User
                {
                    Id = row.GetValue<Guid>("id"),
                    FirstName = row.GetValue<string>("firstname"),
                    LastName = row.GetValue<string>("lastname"),
                    Email = row.GetValue<string>("email"),                    
                    Mobile = row.GetValue<string>("mobile"),
                    UserName = row.GetValue<string>("username"),
                });
            }
            return records;
        }

        public User GetById(Guid id)
        {
            var getRecs = new SimpleStatement("SELECT * FROM core8.users WHERE id = ?", id);
            var rowSet = session.Execute(getRecs);
            var user = new User();
            foreach(var row in rowSet)          
            {
                user.Id = row.GetValue<Guid>("id");
                user.FirstName = row.GetValue<string>("firstname");
                user.LastName = row.GetValue<string>("lastname");
                user.Email = row.GetValue<string>("email");              
                user.Mobile = row.GetValue<string>("mobile");
                user.UserName = row.GetValue<string>("username");
                user.Secretkey = row.GetValue<string>("secretkey");   
            }
            return user;
        }

        public async Task<bool> ProfileUpdate(User userParam)
        {
            var prep = await session.PrepareAsync("SELECT * FROM core8.users WHERE id = ?");
            var stm1 = prep.Bind(userParam.Id);
            var rowSet = await session.ExecuteAsync(stm1);
            var xrow = rowSet.FirstOrDefault();
            if (xrow is not null) {
                DateTime now = DateTime.Now;
                var updateInfo = session.Prepare("UPDATE core8.users SET firstname = ?, lastname = ?, mobile = ?, updatedat = ? WHERE id = ?");
                await session.ExecuteAsync(updateInfo.Bind(userParam.FirstName, userParam.LastName, userParam.Mobile, now, userParam.Id));
                return true;
            } else {
                return false;
            }
        }

        public async Task<bool> UpdatePassword(User userParam)
        {
            var preparedStatement = await session.PrepareAsync("SELECT * FROM core8.users WHERE id = ?");
            var statement1 = preparedStatement.Bind(userParam.Id);
            var rowSet = await session.ExecuteAsync(statement1);
            var row = rowSet.FirstOrDefault();
            if (row is not null) {

                DateTime now = DateTime.Now;
                var userPassword = BCrypt.Net.BCrypt.HashPassword(userParam.Password);
                var updateUser = session.Prepare("UPDATE core8.users SET password = ?, updatedat = ? WHERE id = ?");
                await session.ExecuteAsync(updateUser.Bind(userPassword, now, userParam.Id));
                return true;

            } else {
                return false;
            }
        }

        public async Task<bool> ActivateMfa(Guid id, bool opt, string qrcode_url)
        {
            var preparedStatement = await session.PrepareAsync("SELECT * FROM core8.users WHERE id = ?");
            var statement1 = preparedStatement.Bind(id);
            var rowSet = await session.ExecuteAsync(statement1);
            var row = rowSet.FirstOrDefault();
            if (row is not null) {                
                var userQrcodeurl = "";
                if (opt == true ) {
                    userQrcodeurl = qrcode_url;
                } else {
                    userQrcodeurl = null;
                }

                var updateCql = session.Prepare("UPDATE core8.users SET qrcodeurl = ? WHERE id = ?");
                await session.ExecuteAsync(updateCql.Bind(userQrcodeurl, id));
                return true;
            } else {
                return false;
            }
        }

        public async Task<bool> UpdatePicture(Guid id, string file)
        {
            var preparedStatement = await session.PrepareAsync("SELECT * FROM core8.users WHERE id = ?");
            var statement1 = preparedStatement.Bind(id);
            var rowSet = await session.ExecuteAsync(statement1);
            var row = rowSet.FirstOrDefault();
            if (row is not null) {                
                var updatePic = session.Prepare("UPDATE core8.users SET profilepic = ? WHERE id = ?");   
                await session.ExecuteAsync(updatePic.Bind(file, id));
                return true;
            } else {
                return false;
            }
        }

       public async Task<bool> ActivateUser(Guid id) 
       {
            var preparedStatement = await session.PrepareAsync("SELECT * FROM core8.users WHERE id = ?");
            var statement1 = preparedStatement.Bind(id);
            var rowSet = await session.ExecuteAsync(statement1);
            var row = rowSet.FirstOrDefault();
            if (row is not null) {                
                if (row.GetValue<string>("id") is null) {
                    throw new AppException("User not found");
                }                

                if (row.GetValue<int>("isblocked") == 1) {
                    throw new AppException("Account has been blocked.");
                }                

                if (row.GetValue<int>("isactivated") == 1) {
                    throw new AppException("Account is alread activated.");
                }                
                int isActivated = 1;
                var prepSql = session.Prepare("UPDATE core8.users SET isactivated = ? WHERE id = ?");
                await session.ExecuteAsync(prepSql.Bind(isActivated, id));
                return true;
            } else {
                return false;
            }
       }

        public async Task<int> SendEmailToken(string email)
        {
            var etoken = 0;
            var sqlMailtoken = new SimpleStatement("SELECT * FROM core8.users WHERE email = ? ALLOW FILTERING", email);
            var rowSet = await session.ExecuteAsync(sqlMailtoken);
            var xrow = rowSet.FirstOrDefault();
            if (xrow is not null) {
                etoken = EmailToken(1000);
                var prepSql = session.Prepare("UPDATE core8.users SET mailtoken = ? WHERE id = ?");
                await session.ExecuteAsync(prepSql.Bind(etoken, xrow.GetValue<Guid>("id")));
                return etoken;
            } else {
                return etoken;
            }
        }       

        public int EmailToken(int etoken)
        {
            int _min = etoken;
            int _max = 9999;
            Random _rdm = new Random();
            return _rdm.Next(_min, _max);
        }

        public async Task<string> FullName(string email) {
             var sqlMail = new SimpleStatement("SELECT * FROM core8.users WHERE email = ? ALLOW FILTERING", email);
            var rowSet = await session.ExecuteAsync(sqlMail);
            var xrow = rowSet.FirstOrDefault();
            if (xrow is not null) {
                return xrow.GetValue<string>("firstname") + " " + xrow.GetValue<string>("lastname");
            } else {
                return "";
            }        
        }

    }
}