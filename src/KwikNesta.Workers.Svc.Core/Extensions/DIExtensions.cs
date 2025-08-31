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
            services.AddHostedService<EmailNotificationWorker>()
                .AddHostedService<AuditLoggerWorker>()
                .AddScoped<IMessageHandler, MessageHandler>()
                .AddScoped<IEmailNotificationService, EmailNotificationService>()
                .ConfigureMailJet(configuration)
                .AddCrossQueueHubRabbitMqBus(configuration)
                .AddDiagnosKitObservability(serviceName: serviceName, serviceVersion: "1.0.0");

            return services;
        }
    }
}
