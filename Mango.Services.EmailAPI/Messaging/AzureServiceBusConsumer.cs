using Azure.Messaging.ServiceBus;
using Mango.Services.EmailAPI.Models.Dto;
using Mango.Services.EmailAPI.Service.IService;
using Microsoft.EntityFrameworkCore.Storage.Json;
using Newtonsoft.Json;
using System.Text;

namespace Mango.Services.EmailAPI.Messaging
{
    public class AzureServiceBusConsumer: IAzureServiceBusConsumer
    {
        private readonly string serviceBusConnectionString;
        private readonly string  emailCartQueue;
        private readonly IConfiguration configuration;
        private ServiceBusProcessor emailCartProcessor;
        private readonly IEmailService emailService;

        public AzureServiceBusConsumer(IConfiguration _configuration, IEmailService _emailService)
        {
            configuration = _configuration;
            emailService = _emailService;

            serviceBusConnectionString = configuration.GetValue<string>("ServiceBusConnectionString");
            emailCartQueue = configuration.GetValue<string>("TopicAndQueueNames:EmailShoppingCartQueue");

            var client = new ServiceBusClient(serviceBusConnectionString);
            emailCartProcessor = client.CreateProcessor(emailCartQueue);

            //emailCartProcessor.ProcessMessageAsync()
        }

        public async Task Start()
        {
            emailCartProcessor.ProcessMessageAsync += OnEmailCartRequestReceived;
            emailCartProcessor.ProcessErrorAsync += ErrorHandler;
            await emailCartProcessor.StartProcessingAsync();
        }

        public async Task Stop()
        {
            await emailCartProcessor.StopProcessingAsync();
            await emailCartProcessor.DisposeAsync();

         //   throw new NotImplementedException();
            // return Task.CompletedTask;
        }
        private async Task OnEmailCartRequestReceived(ProcessMessageEventArgs args)
        {
            var message= args.Message;
            var body = Encoding.UTF8.GetString(message.Body);
            CartDto objMessage=JsonConvert.DeserializeObject<CartDto>(body);

            try
            {
                //try to log email in database 

                await emailService.EmailCartAndLog(objMessage);

                await args.CompleteMessageAsync(args.Message);

            }catch (Exception ex)
            {
                throw new NotImplementedException();
            }

            //throw new NotImplementedException();
        }
        private Task ErrorHandler(ProcessErrorEventArgs args)
        {
            Console.WriteLine(args.Exception.Message);
            return Task.CompletedTask;
        }




    }
}
