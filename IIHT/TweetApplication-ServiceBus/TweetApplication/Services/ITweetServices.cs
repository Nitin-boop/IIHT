using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TweetApplication.Models;

namespace TweetApplication.Services
{
    public interface ITweetServices
    {
        List<Tweet> GetTweets();
        List<Tweet> GetTweetsByUser(string username);
        bool AddTweetToDB(Tweet tweet);
        bool AddTweet(Tweet tweet);
        void UpdateTweet(Tweet tweet);
        void DeleteTweet( Guid id);
        void LikeTweet(Tweet tweet);
        void ReplyToTweet(Tweet tweet);
    }
}
