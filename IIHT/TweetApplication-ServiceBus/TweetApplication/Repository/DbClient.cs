using Microsoft.Extensions.Configuration;
using MongoDB.Driver;
using System.Security.Authentication;
using TweetApplication.Models;

namespace TweetApplication.Repository
{
    public class DbClient : IDbClient
    {
        private readonly IMongoCollection<User> _users;
        private readonly IMongoCollection<Tweet> _tweets;
        public DbClient(IConfiguration config)
        {
            var connectionString = config.GetConnectionString("CosmosDB");
            MongoClientSettings settings = MongoClientSettings.FromUrl(
              new MongoUrl(connectionString)
            );
            settings.SslSettings =
              new SslSettings() { EnabledSslProtocols = SslProtocols.Tls12 };
            var client = new MongoClient(settings);
            var database = client.GetDatabase("TweetApp");
            _users = database.GetCollection<User>("Users");
            _tweets = database.GetCollection<Tweet>("Tweets");
        }
        public IMongoCollection<User> GetUsersCollection() => _users;
        public IMongoCollection<Tweet> GetTweetsCollection() => _tweets;
    }
}
