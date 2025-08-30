using CrossQueue.Hub.Services.Interfaces;
using CSharpTypes.Extensions.Enumeration;
using DiagnosKit.Core.Logging.Contracts;
using KwikNesta.Contracts.Enums;
using KwikNesta.Contracts.Models;
using KwikNesta.Workers.Svc.Core.Handlers.Interfaces;

namespace KwikNesta.Workers.Svc.Core.Workers
{
    public class EmailNotificationWorker : BackgroundService
    {
        private readonly ILoggerManager _logger;
        private readonly IRabbitMQPubSub _pubSub;
        private readonly IMessageHandler _handler;

        public EmailNotificationWorker(IServiceScopeFactory scopeFactory,
            ILoggerManager logger, IRabbitMQPubSub pubSub)
        {
            using var scope = scopeFactory.CreateScope();
            var handler = scope.ServiceProvider.GetRequiredService<IMessageHandler>();
            _logger = logger;
            _pubSub = pubSub;
            _handler = handler;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            _logger.LogInfo("Email Notification Worker started....");

            _pubSub.Subscribe<NotificationMessage>(MQs.Notification.GetDescription(), async msg =>
            {
                _logger.LogInfo("Received email notification for {EmailAddress}", msg.EmailAddress);
                await _handler.HandleAsync(msg);
            }, routingKey: MQRoutingKey.AccountEmail.GetDescription());

            await Task.CompletedTask;
        }
    }
}