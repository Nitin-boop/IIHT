using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Linq;
using TweetApplication.Models;
using TweetApplication.Repository;

namespace TweetApplication.Services
{
    public class UserServices : IUserServices
    {
        private readonly IMongoCollection<User> _users;
        private ILogger<UserServices> _logger;
        public UserServices(IDbClient dbClient, ILogger<UserServices> logger)
        {
            _users = dbClient.GetUsersCollection();
            _logger = logger;
        }
        private User GetUser(User user) => _users.Find(u => u.Username == user.Username).FirstOrDefault();
        public bool? AddUser(User user)
        {
            try
            {
                User _user = GetUser(user);
                if (_user == null && user.Username != null && user.Password != null && user.Dob != null)
                {
                    _users.InsertOne(user);
                    return true;
                }
                else if (_user != null) { return null; }
                else { return false; }
            }
            catch(Exception ex)
            {
                _logger.LogError(ex.ToString());
                return false;
            }
        }
        public bool Login(User user)
        {
            try
            {
                User _user = GetUser(user);
                if (_user != null && _user.Password == user.Password) { return true; }
                else { return false; }
            }
            catch(Exception ex)
            {
                _logger.LogError(ex.ToString());
                return false;
            }
        }
        public void Forgot(User user)
        {
            try
            {
                var filter = Builders<User>.Filter.Eq("Username", user.Username) & Builders<User>.Filter.Eq("Dob", user.Dob);
                var update = Builders<User>.Update.Set("Password", user.Password);
                _users.UpdateOne(filter, update);
            }
            catch (Exception ex) { _logger.LogError(ex.ToString()); } 
        }
        public List<string> GetUsers() => _users.Find(user => true).Project(u => u.Username ).ToList();
    }
}
