using Confluent.Kafka;
using MongoDB.Bson;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Consumer
{

    class Program
    {
        static void Main()
        {
            var config = new ConsumerConfig()
            {
                BootstrapServers = "localhost:9093",
                GroupId = "kafka-dotnet",
                EnableAutoCommit = false
            };

            const string topic = "tweets";

            CancellationTokenSource cts = new CancellationTokenSource();
            Console.CancelKeyPress += (_, e) => {
                e.Cancel = true;
                cts.Cancel();
            };
            
            using (var consumer = new ConsumerBuilder<string, string>(config).Build())
            {
                consumer.Assign(new TopicPartitionOffset(topic, new Partition(0), Offset.End));
                try
                {
                    while (true)
                    {
                        var cr = consumer.Consume(cts.Token);
                        Console.WriteLine($"Consumed event from topic {topic} with key {cr.Message.Key} and value {cr.Message.Value}");
                        Task.Run(()=>MainAsync(cr.Message.Key, cr.Message.Value));
                    }
                }
                catch (OperationCanceledException)
                {
                    Console.WriteLine("Error Occured");
                }
                finally
                {
                    Console.WriteLine("Closing");
                }
            }
        }

        static async Task MainAsync(string username, string message)
        {
            using (HttpClient client = new HttpClient())
            {
                HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Post, "https://localhost:44333/api/v1.0/tweets/addTweet");
                request.Content = new StringContent("{\"Username\":\"" + username + "\",\"TweetMessage\":\"" + message + "\"}", Encoding.UTF8, "application/json");
                await client.SendAsync(request);
            }
        }
    }
}
