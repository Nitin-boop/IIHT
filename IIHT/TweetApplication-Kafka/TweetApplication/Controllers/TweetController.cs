using Confluent.Kafka;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using TweetApplication.Models;
using TweetApplication.Services;

namespace TweetApplication.Controllers
{
    [Route("api/v1.0/tweets")]
    [ApiController]
    public class TweetController : Controller
    {
        private ITweetServices _tweetServices;
        private ILogger<TweetController> _logger;
        public TweetController(ITweetServices tweetServices, ILogger<TweetController> logger)
        {
            _tweetServices = tweetServices;
            _logger = logger;
        }
        
        [HttpGet]
        [Route("all")]
        public IActionResult GetAllTweets()
        {
            try
            {
                List<Tweet> tweets = _tweetServices.GetTweets();
                return Ok(tweets);
            }
            catch (Exception ex) 
            { 
                _logger.LogError(ex.ToString()); 
                return StatusCode(500); 
            }
        }

        [HttpGet]
        [Route("{username}")]
        public IActionResult GetAllTweetsOfUser(string username)
        {
            try
            {
                List<Tweet> tweets = _tweetServices.GetTweetsByUser(username);
                return Ok(tweets);
            }
            catch (Exception ex) { _logger.LogError(ex.ToString()); return StatusCode(500); }
        }

        [HttpPost]
        [Route("{username}/add")]
        public IActionResult PostTweet([FromBody] Tweet tweet)
        {
            try
            {
                var config = new ProducerConfig()
                {
                    BootstrapServers = "localhost:9093"
                };

                const string topic = "tweets";
                using (var producer = new ProducerBuilder<string, string>(config).Build())
                {
                    producer.Produce(topic, new Message<string, string> { Key = tweet.Username, Value = tweet.TweetMessage });
                    producer.Flush(TimeSpan.FromSeconds(10));
                }
                return Ok();
            }
            catch (Exception ex) { _logger.LogError(ex.ToString()); return StatusCode(500); }
        }

        [HttpPut("{username}/update/{id}")]
        public IActionResult UpdateTweet([FromBody] Tweet tweet)
        {
            try
            {
                _tweetServices.UpdateTweet(tweet);
                return Ok();
            }
            catch (Exception ex) { _logger.LogError(ex.ToString()); return StatusCode(500); }
        }

        [HttpDelete("{username}/delete/{id}")]
        public IActionResult DeleteTweet( Guid id)
        {
            try
            {
                _tweetServices.DeleteTweet(id);
                return Ok();
            }
            catch (Exception ex) { _logger.LogError(ex.ToString()); return StatusCode(500); }
        }

        [HttpPut("{username}/like/{id}")]
        public IActionResult LikeTweet([FromBody]Tweet tweet)
        {
            try
            {
                _tweetServices.LikeTweet(tweet);
                return Ok();
            }
            catch (Exception ex) { _logger.LogError(ex.ToString()); return StatusCode(500); }
        }

        [HttpPost]
        [Route("{username}/reply/{id}")]
        public IActionResult ReplyToTweet([FromBody]Tweet tweet)
        {
            try
            {
                _tweetServices.ReplyToTweet(tweet);
                return Ok();
            }
            catch (Exception ex) { _logger.LogError(ex.ToString()); return StatusCode(500); }
        }

        [HttpPost]
        [Route("addTweet")]
        public IActionResult AddTweet([FromBody] Tweet tweet)
        {
            try
            {
                _tweetServices.AddTweet(tweet);
                return Ok();
            }
            catch(Exception ex) 
            {
                _logger.LogError(ex.ToString());
                return StatusCode(500); 
            }
        }
    }
}
