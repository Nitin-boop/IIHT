using MongoDB.Bson.Serialization.Attributes;
using System;
using System.Collections;
using System.Collections.Generic;

namespace TweetApplication.Models
{
    public class Tweet
    {
        [BsonId]
        public Guid TweetId { get; set; }
        public string Username { get; set; }
        public string TweetMessage { get; set; }
        public DateTime TweetTime { get; set; }
        public int TweetNoOfLikes { get; set; }
        public List<ArrayList> TweetReplies { get; set; }
    }
}
