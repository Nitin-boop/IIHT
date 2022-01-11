using MongoDB.Driver;
using TweetApplication.Models;

namespace TweetApplication.Repository
{
    public class DbClient : IDbClient
    {

        private readonly IMongoCollection<User> _users;
        private readonly IMongoCollection<Tweet> _tweets;
        public DbClient()
        {
            var client = new MongoClient("mongodb+srv://admin:admin@cluster0.wnme0.mongodb.net/TweetApp?retryWrites=true&w=majority");
            var database = client.GetDatabase("TweetApp");
            _users = database.GetCollection<User>("Users");
            _tweets = database.GetCollection<Tweet>("Tweets");
        }
        public IMongoCollection<User> GetUsersCollection() => _users;
        public IMongoCollection<Tweet> GetTweetsCollection() => _tweets;
    }
}
