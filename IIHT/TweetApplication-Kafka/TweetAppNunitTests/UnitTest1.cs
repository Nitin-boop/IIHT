using Microsoft.Extensions.Logging;
using MongoDB.Bson;
using MongoDB.Driver;
using Moq;
using NUnit.Framework;
using System;
using TweetApplication.Models;
using TweetApplication.Repository;
using TweetApplication.Services;
using TweetAppNunitTests.TestRepo;

namespace TweetAppNunitTests
{
    [TestFixture]
    public class UserServicesTests
    {
        private IDbClient _client = new TestDbClient();
        private Mock<ILogger<UserServices>> _logger = new Mock<ILogger<UserServices>>();
        private UserServices _userService;
        private IMongoCollection<User> _users;
        public UserServicesTests()
        {
            _userService = new UserServices(_client, _logger.Object);
            _users = _client.GetUsersCollection();
        }
        private User GetUser(User user) => _users.Find(u => u.Username == user.Username).FirstOrDefault();
        [Test]
        public void AddUserTest()
        {
            User _user = new User { UserId = new Guid(), Username = "admin", Password = "admin", Dob = DateTime.Now };
            bool? add = _userService.AddUser(_user);
            Assert.IsTrue(add);
            RemoveUser(_user);
        }
        [Test]
        public void GetUsersTest()
        {
            User _user = new User { UserId = new Guid(), Username = "admin", Password = "admin", Dob = DateTime.Now };
            _userService.AddUser(_user);
            var users = _userService.GetUsers();
            Assert.IsTrue(users.Count > 0);
            RemoveUser(_user);
        }
        [Test]
        public void LoginTest_when_password_is_correct()
        {
            User _user = new User { UserId = new Guid(), Username = "admin", Password = "admin", Dob = DateTime.Now };
            _userService.AddUser(_user);
            bool login = _userService.Login(_user);
            Assert.IsTrue(login);
            RemoveUser(_user);
        }
        [Test]
        public void LoginTest_when_password_is_wrong()
        {
            User _user = new User { UserId = new Guid(), Username = "admin", Password = "admin", Dob = DateTime.Now };
            _userService.AddUser(_user);
            _user.Password = "123";
            bool login = _userService.Login(_user);
            Assert.IsFalse(login);
            RemoveUser(_user);
        }
        [Test]
        public void ForgotPasswordTest_when_dob_matches()
        {
            User _user = new User { UserId = new Guid(), Username = "admin", Password = "admin", Dob = DateTime.Now };
            _userService.AddUser(_user);
            string password = "abc";
            _user.Password = password;
            _userService.Forgot(_user);
            var user = GetUser(_user);
            Assert.AreEqual(user.Password, password);
            RemoveUser(_user);
        }
        [Test]
        public void ForgotPasswordTest_when_dob_wrong()
        {
            User _user = new User { UserId = new Guid(), Username = "admin", Password = "admin", Dob = DateTime.Now };
            _userService.AddUser(_user);
            string password = "abc";
            _user.Dob = DateTime.Now.AddDays(2);
            _user.Password = password;
            _userService.Forgot(_user);
            var user = GetUser(_user);
            Assert.AreEqual(user.Password, "admin");
            RemoveUser(_user);
        }
        public void RemoveUser(User user)
        {
            var filter = Builders<User>.Filter.Eq(x => x.UserId, user.UserId);
            _users.DeleteOne(filter);
        }
        
    }
}