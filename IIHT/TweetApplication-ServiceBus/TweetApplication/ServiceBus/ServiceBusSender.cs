using Azure.Messaging.ServiceBus;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;
using TweetApplication.Models;

namespace TweetApplication.ServiceBus
{
    public class ServiceBusSender
    {
        private readonly ServiceBusClient _client;
        private readonly Azure.Messaging.ServiceBus.ServiceBusSender _clientSender;
        private readonly string QUEUE_NAME = "tweet";
        private readonly IConfiguration _config;

        public ServiceBusSender(IConfiguration config)
        {
            _config = config;
            string connString = _config.GetConnectionString("AzureSender");
            _client = new ServiceBusClient(connString);
            _clientSender = _client.CreateSender(QUEUE_NAME);
        }

        public void SendMessage(Tweet tweet)
        {
            string _tweet = JsonSerializer.Serialize(tweet);
            Console.WriteLine("Sending Message:" + tweet);
            ServiceBusMessage message = new ServiceBusMessage(_tweet);
            _clientSender.SendMessageAsync(message).ConfigureAwait(false);
        }
    }
}
