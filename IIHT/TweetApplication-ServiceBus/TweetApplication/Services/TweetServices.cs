using System;
using System.Collections.Generic;
using MongoDB.Driver;
using System.Linq;
using TweetApplication.Models;
using TweetApplication.Repository;
using System.Collections;
using Microsoft.Extensions.Logging;
using TweetApplication.ServiceBus;
using Microsoft.Extensions.Configuration;

namespace TweetApplication.Services
{
    public class TweetServices : ITweetServices
    {
        private readonly IMongoCollection<Tweet> _tweets;
        private readonly IMongoCollection<User> _users;
        private ILogger<TweetServices> _logger;
        private IConfiguration _config;
        public TweetServices(IDbClient dbClient, ILogger<TweetServices> logger, IConfiguration config)
        {
            _config = config;
            _users = dbClient.GetUsersCollection();
            _tweets = dbClient.GetTweetsCollection();
            _logger = logger;
        }
        private User GetUser(string username) => _users.Find(u => u.Username == username).FirstOrDefault();
        public List<Tweet> GetTweets()
        {
            try
            {
                List<Tweet> tweets = _tweets.Find(t => true).ToList();
                return Enumerable.Reverse(tweets).ToList();
            }
            catch(Exception ex)
            {
                _logger.LogError(ex.ToString());
                return null;
            }
        }
        public List<Tweet> GetTweetsByUser(string username)
        {
            try
            {
                List<Tweet> tweets = _tweets.Find(t => t.Username == username).ToList();
                return Enumerable.Reverse(tweets).ToList();
            }
            catch(Exception ex)
            {
                _logger.LogError(ex.ToString());
                return null;
            }
        }
        public bool AddTweetToDB(Tweet tweet)
        {
            try
            {
                User _user = GetUser(tweet.Username);
                if (_user != null)
                {
                    _tweets.InsertOne(new Tweet
                    {
                        Username = _user.Username,
                        TweetMessage = tweet.TweetMessage,
                        TweetNoOfLikes = 0,
                        TweetTime = tweet.TweetTime,
                        TweetReplies = new List<ArrayList> { }
                    });
                    return true;
                }
                return false;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.ToString());
                return false;
            }
        }
        public bool AddTweet(Tweet tweet)
        {
            try
            {
                var sender = new ServiceBusSender(_config);
                sender.SendMessage(new Tweet
                {
                    Username = tweet.Username,
                    TweetMessage = tweet.TweetMessage,
                    TweetNoOfLikes = 0,
                    TweetTime = DateTime.Now,
                    TweetReplies = new List<ArrayList> { }
                });
                return true;
            }
            catch(Exception ex) 
            {
                _logger.LogError(ex.ToString());
                return false; 
            }
        }
        public void UpdateTweet(Tweet tweet)
        {
            try
            {
                var filter = Builders<Tweet>.Filter.Eq("TweetId", tweet.TweetId);
                var update = Builders<Tweet>.Update.Set("TweetMessage", tweet.TweetMessage);
                _tweets.UpdateOne(filter, update);
            }
            catch(Exception ex) { _logger.LogError(ex.ToString()); }
        }
        public void DeleteTweet(Guid id)
        {
            try
            {
                var filter = Builders<Tweet>.Filter.Eq("TweetId", id);
                _tweets.DeleteOne(filter);
            }
            catch (Exception ex) { _logger.LogError(ex.ToString()); }
        }
        public void LikeTweet(Tweet tweet)
        {
            try
            {
                int i = 1;
                var filter = Builders<Tweet>.Filter.Eq("TweetId", tweet.TweetId);
                Tweet _tweet = _tweets.Find(filter).FirstOrDefault();
                if (tweet.TweetNoOfLikes == -1)
                {
                    i = -1;
                }
                var update = Builders<Tweet>.Update.Set("TweetNoOfLikes", _tweet.TweetNoOfLikes + i);
                _tweets.UpdateOne(filter, update);
            }
            catch (Exception ex) { _logger.LogError(ex.ToString()); }
        }
        public void ReplyToTweet(Tweet tweet)
        {
            try
            {
                var filter = Builders<Tweet>.Filter.Eq("TweetId", tweet.TweetId);
                Tweet _tweet = _tweets.Find(filter).FirstOrDefault();
                _tweet.TweetReplies.Add(new ArrayList {tweet.Username, tweet.TweetMessage, tweet.TweetTime });
                var update = Builders<Tweet>.Update.Set("TweetReplies", _tweet.TweetReplies);
                _tweets.UpdateOne(filter, update);
            }
            catch (Exception ex) { _logger.LogError(ex.ToString()); }
        }
    }
}
