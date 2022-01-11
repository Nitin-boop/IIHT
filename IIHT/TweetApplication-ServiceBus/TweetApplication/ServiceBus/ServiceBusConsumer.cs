using Azure.Messaging.ServiceBus;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TweetApplication.Models;
using TweetApplication.Services;

namespace TweetApplication.ServiceBus
{
    public class ServiceBusConsumer : IServiceBusConsumer
    {
        private readonly ServiceBusClient _client;
        private ServiceBusProcessor _processor;
        private ITweetServices _tweetServices;
        private readonly string QUEUE_NAME = "tweet";
        private readonly IConfiguration _config;
        public ServiceBusConsumer(ITweetServices tweetServices, IConfiguration config)
        {
            _config = config;
            var connString = _config.GetConnectionString("AzureListener");
            _client = new ServiceBusClient(connString);
            _tweetServices = tweetServices;
        }
        public async Task Register()
        {
            ServiceBusProcessorOptions _serviceBusProcessorOptions = new ServiceBusProcessorOptions
            {
                MaxConcurrentCalls = 1,
                AutoCompleteMessages = false
            };
            _processor = _client.CreateProcessor(QUEUE_NAME, _serviceBusProcessorOptions);
            _processor.ProcessMessageAsync += ProcessMessages;
            _processor.ProcessErrorAsync += ProcessError;
            await _processor.StartProcessingAsync().ConfigureAwait(false);
        }
        private Task ProcessMessages(ProcessMessageEventArgs args)
        {
            var tweet = args.Message.Body.ToObjectFromJson<Tweet>();
            Console.WriteLine("Consuming Message");
            _tweetServices.AddTweetToDB(tweet);
            return Task.CompletedTask;
        }
        private Task ProcessError(ProcessErrorEventArgs args)
        {
            return Task.CompletedTask;
        }
        public void CloseQueue()
        {
            _client.DisposeAsync().ConfigureAwait(false);
        }

        public void Dispose()
        {
            if (_client != null)
            {
                _processor.DisposeAsync().ConfigureAwait(false);
            }
            if (_processor != null)
            {
                _client.DisposeAsync().ConfigureAwait(false);
            }
        }

        
    }
}
