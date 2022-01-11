using MongoDB.Bson.Serialization.Attributes;
using System;
using System.Collections.Generic;

namespace TweetApplication.Models
{
    public class User
    {
        [BsonId]
        public Guid UserId { get; set; }
        public string Username { get; set; }
        public string Password { get; set; }
        public DateTime? Dob { get; set; }
    }
}
