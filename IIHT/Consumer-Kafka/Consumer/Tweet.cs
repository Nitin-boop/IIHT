using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Consumer
{
    class Tweet
    {
        
        public string Username { get; set; }
        public string TweetMessage { get; set; }
        public DateTime TweetTime { get; set; }
        public int TweetNoOfLikes { get; set; }
        public List<ArrayList> TweetReplies { get; set; }
    }
}
