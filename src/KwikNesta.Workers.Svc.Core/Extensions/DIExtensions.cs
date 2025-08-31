using DiagnosKit.Core.Extensions;
using DRY.MailJetClient.Library.Extensions;
using KwikNesta.Workers.Svc.Core.Handlers.Interfaces;
using KwikNesta.Workers.Svc.Core.Handlers;
using KwikNesta.Workers.Svc.Core.Workers;
using KwikNesta.Workers.Svc.Application.Interfaces;
using KwikNesta.Workers.Svc.Application.Implementations;
using CrossQueue.Hub.Shared.Extensions;
using EFCore.CrudKit.Library.Extensions;
using EFCore.CrudKit.Library.Models.Enums;
using Polly;

namespace KwikNesta.Workers.Svc.Core.Extensions
{
    public static class DIExtensions
    {
        public static IServiceCollection RegisterServices(this IServiceCollection services,
                                                         IConfiguration configuration,
                                                         string serviceName)
        {
            services.ConfigureMongoEFCoreDataForge(configuration, idSerializationMode: IdSerializationMode.Guid);
            services.AddLoggerManager();
            services.AddHttpClient("LocationService", client =>
            {
                client.BaseAddress = new Uri(configuration["ExternalServices:LocationMarker"]!);
                client.Timeout = TimeSpan.FromMinutes(2);
            }).AddTransientHttpErrorPolicy(policy => policy.WaitAndRetryAsync(3, attempt => TimeSpan.FromSeconds(10)));

            services.RegisterWorkers()
                .AddScoped<IMessageHandler, MessageHandler>()
                .AddScoped<IServiceManager, ServiceManager>()
                .ConfigureMailJet(configuration)
                .AddCrossQueueHubRabbitMqBus(configuration)
                .AddDiagnosKitObservability(serviceName: serviceName, serviceVersion: "1.0.0");

            return services;
        }

        private static IServiceCollection RegisterWorkers(this IServiceCollection services)
        {
            services.AddHostedService<EmailNotificationWorker>()
               .AddHostedService<AuditLoggerWorker>()
               .AddHostedService<LocationLoaderWorker>();

            return services;
        }
    }
}
