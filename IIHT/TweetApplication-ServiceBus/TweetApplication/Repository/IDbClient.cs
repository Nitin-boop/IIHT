using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TweetApplication.Models;

namespace TweetApplication.Repository
{
    public interface IDbClient
    {
        IMongoCollection<User> GetUsersCollection();
        IMongoCollection<Tweet> GetTweetsCollection();
    }
}
