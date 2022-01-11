using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace TweetApplication.ServiceBus
{
    public interface IServiceBusConsumer
    {
        Task Register();
        void CloseQueue();
        void Dispose();
    }
}
