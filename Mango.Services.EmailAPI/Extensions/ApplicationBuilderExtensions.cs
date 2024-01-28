using Mango.Services.EmailAPI.Messaging;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Reflection.Metadata;
using System.Runtime.CompilerServices;
using System.Text;

namespace Mango.Services.EmailAPI.Extensions
{
    public static class ApplicationBuilderExtensions
    {
        private static IAzureServiceBusConsumer azureServiceBusConsumer { get; set; }
        public static IApplicationBuilder UseAzureServiceBusConsumer(this IApplicationBuilder app)
        {
            azureServiceBusConsumer = app.ApplicationServices.GetService<IAzureServiceBusConsumer>();
            var hostApplicationLife = app.ApplicationServices.GetService<IHostApplicationLifetime>();
            hostApplicationLife.ApplicationStarted.Register(OnStart);
            hostApplicationLife.ApplicationStopped.Register(OnStop);

            return app;
        }

        private static void OnStart()
        {
            azureServiceBusConsumer.Start();
        }

        private static void OnStop()
        {
            azureServiceBusConsumer.Stop();
        }
    }
}
