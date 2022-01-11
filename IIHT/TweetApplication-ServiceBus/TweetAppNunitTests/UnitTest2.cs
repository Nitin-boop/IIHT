using Microsoft.Extensions.Logging;
using MongoDB.Driver;
using Moq;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TweetApplication.Models;
using TweetApplication.Repository;
using TweetApplication.Services;
using TweetAppNunitTests.TestRepo;

namespace TweetAppNunitTests
{
    [TestFixture]
    public class TweetServicesTest
    {
        private IDbClient _client = new TestDbClient();
        private Mock<ILogger<TweetServices>> _logger = new Mock<ILogger<TweetServices>>();
        private TweetServices _tweetService;
        private IMongoCollection<Tweet> _tweets;
        private IMongoCollection<User> _users;
        public TweetServicesTest()
        {
            _tweetService = new TweetServices(_client, _logger.Object);
            _tweets = _client.GetTweetsCollection();
            _users = _client.GetUsersCollection();
        }
        private User GetUser(string username) => _users.Find(u => u.Username == username).FirstOrDefault();
        private Tweet GetTweet(Tweet tweet) => _tweets.Find(t => t.TweetId == tweet.TweetId).FirstOrDefault();
        [Test]
        public void PostTweetTest()
        {
            Tweet tweet = new Tweet { TweetMessage = "Test", Username = "test", TweetTime = DateTime.Now };
            bool add = _tweetService.AddTweet(tweet);
            Assert.IsTrue(add);
            RemoveTweet(tweet);
        }
        [Test]
        public void GetTweetsTest_before_adding_tweet()
        {
            var tweets = _tweetService.GetTweets();
            Assert.AreEqual(0, tweets.Count);
        }
        [Test]
        public void GetTweetsTest_after_adding_tweet()
        {
            Tweet tweet = new Tweet { TweetMessage = "Test", Username = "test", TweetTime = DateTime.Now };
            _tweetService.AddTweet(tweet);
            var tweets = _tweetService.GetTweets();
            Assert.IsTrue(tweets.Count > 0);
            RemoveTweet(tweet);
        }
        [Test]
        public void UpdateTweetTest_when_id_matches()
        {
            var time = DateTime.Now;
            Tweet tweet = new Tweet { TweetMessage = "Test", Username = "test", TweetTime = time };
            _tweetService.AddTweet(tweet);
            var filter = Builders<Tweet>.Filter.Eq("Username", tweet.Username);
            Tweet tweet1 = _tweets.Find(filter).First();
            tweet1.TweetMessage = "Working";
            _tweetService.UpdateTweet(tweet1);
            Tweet tweet2 = _tweets.Find(filter).First();
            Assert.AreEqual(tweet1.TweetId, tweet2.TweetId);
            RemoveTweet(tweet2);
        }
        [Test]
        public void UpdateTweetTest_when_id_not_match()
        {
            var time = DateTime.Now;
            Tweet tweet = new Tweet { TweetMessage = "Test", Username = "test", TweetTime = time };
            _tweetService.AddTweet(tweet);
            var filter = Builders<Tweet>.Filter.Eq("Username", tweet.Username);
            Tweet tweet1 = _tweets.Find(filter).First();
            tweet1.TweetId = new Guid();
            tweet1.TweetMessage = "Working";
            _tweetService.UpdateTweet(tweet1);
            Tweet tweet2 = _tweets.Find(filter).First();
            Assert.AreNotEqual(tweet1.TweetId, tweet2.TweetId);
            RemoveTweet(tweet2);
        }
        [Test]
        public void DeleteTweetTest()
        {
            Tweet tweet = new Tweet { TweetMessage = "Test", Username = "test", TweetTime = DateTime.Now};
            _tweetService.AddTweet(tweet);
            var filter = Builders<Tweet>.Filter.Eq("Username", tweet.Username);
            Tweet tweet1 = _tweets.Find(filter).FirstOrDefault();
            _tweetService.DeleteTweet(tweet1.TweetId);
            Tweet tweet2 = GetTweet(tweet1);
            Assert.AreEqual(tweet2, null);
        }
        [Test]
        public void LikeTweetTest()
        {
            Tweet tweet = new Tweet { TweetMessage = "Test", Username = "test", TweetTime = DateTime.Now };
            _tweetService.AddTweet(tweet);
            var filter = Builders<Tweet>.Filter.Eq("Username", tweet.Username);
            Tweet tweet1 = _tweets.Find(filter).FirstOrDefault();
            _tweetService.LikeTweet(tweet1);
            Tweet tweet2 = _tweets.Find(filter).FirstOrDefault();
            Assert.AreEqual(tweet1.TweetNoOfLikes + 1, tweet2.TweetNoOfLikes);
            RemoveTweet(tweet2);
        }
        [Test]
        public void ReplyToTweetTest()
        {
            Tweet tweet = new Tweet { TweetMessage = "Test", Username = "test", TweetTime = DateTime.Now };
            _tweetService.AddTweet(tweet);
            var filter = Builders<Tweet>.Filter.Eq("Username", tweet.Username);
            Tweet tweet1 = _tweets.Find(filter).FirstOrDefault();
            tweet1.TweetMessage = "Test1";
            _tweetService.ReplyToTweet(tweet1);
            Tweet tweet2 = _tweets.Find(filter).FirstOrDefault();
            Assert.AreEqual(tweet2.TweetReplies.Count, 1);
            RemoveTweet(tweet2);
        }
        public void RemoveTweet(Tweet tweet)
        {
            var filter = Builders<Tweet>.Filter.Eq("Username", tweet.Username);
            _tweets.DeleteOne(filter);
        }
    }
}
