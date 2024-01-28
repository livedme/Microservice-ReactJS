using Azure.Messaging.ServiceBus;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace Mango.MessageBus
{
    public class MessageBug : IMessageBug
    {
        private readonly string connectionString="put your azur message bus connectionstring";
        public async Task PublishMessage(object message, string topic_queue_name)
        {
            await using var client=new ServiceBusClient(connectionString);
            ServiceBusSender sender=client.CreateSender(topic_queue_name);
            var jsonMessage=JsonConvert.SerializeObject(message);
            ServiceBusMessage finalMessage = new(Encoding.UTF8.GetBytes(jsonMessage))
            {
                CorrelationId = Guid.NewGuid().ToString()
            };
            await sender.SendMessageAsync(finalMessage);
            await client.DisposeAsync();            
        }
    }
}
