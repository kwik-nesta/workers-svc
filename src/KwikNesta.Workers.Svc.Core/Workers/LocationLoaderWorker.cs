using CrossQueue.Hub.Services.Interfaces;
using CSharpTypes.Extensions.Enumeration;
using DiagnosKit.Core.Logging.Contracts;
using KwikNesta.Contracts.Enums;
using KwikNesta.Contracts.Models;
using KwikNesta.Workers.Svc.Core.Handlers.Interfaces;

namespace KwikNesta.Workers.Svc.Core.Workers
{
    internal class LocationLoaderWorker : BackgroundService
    {
        private readonly IRabbitMQPubSub _pubSub;
        private readonly ILoggerManager _logger;
        private readonly IMessageHandler _handler;

        public LocationLoaderWorker(IServiceScopeFactory scopeFactory,
            IRabbitMQPubSub pubSub, ILoggerManager logger)
        {
            using var scope = scopeFactory.CreateScope();
            var handler = scope.ServiceProvider.GetRequiredService<IMessageHandler>();
            _pubSub = pubSub;
            _logger = logger;
            _handler = handler;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            _logger.LogInfo("Location Dataloader Worker started....");

            _pubSub.Subscribe<DataLoadRequest>(MQs.DataLoad.GetDescription(), async msg =>
            {
                _logger.LogInfo("Received data-load request.\nType: {Type}. \nRequest By: {RequesterEmail}.\nDate: {Date}", msg.Type.GetDescription(), msg.RequesterEmail, msg.Date);
                await _handler.HandleAsync(msg);
            }, routingKey: MQRoutingKey.DataLoad.GetDescription());

            await Task.CompletedTask;
        }
    }
}
