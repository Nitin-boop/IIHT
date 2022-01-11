using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TweetApplication.Models;
using TweetApplication.Repository;

namespace TweetAppNunitTests.TestRepo
{
    class TestDbClient : IDbClient
    {
        private readonly IMongoCollection<User> _users;
        private readonly IMongoCollection<Tweet> _tweets;
        public TestDbClient()
        {
            var client = new MongoClient();
            var database = client.GetDatabase("TweetAppTestDb");
            _users = database.GetCollection<User>("Users");
            _tweets = database.GetCollection<Tweet>("Tweets");
        }
        public IMongoCollection<User> GetUsersCollection() => _users;
        public IMongoCollection<Tweet> GetTweetsCollection() => _tweets;
    }
}
