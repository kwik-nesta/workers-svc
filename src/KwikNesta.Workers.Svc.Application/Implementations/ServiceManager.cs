using DiagnosKit.Core.Logging.Contracts;
using DRY.MailJetClient.Library;
using EFCore.CrudKit.Library.Data.Interfaces;
using KwikNesta.Workers.Svc.Application.Interfaces;
using Microsoft.Extensions.Hosting;

namespace KwikNesta.Workers.Svc.Application.Implementations
{
    public class ServiceManager : IServiceManager
    {
        private readonly Lazy<IEmailNotificationService> _emailNotificationService;
        private readonly Lazy<ILocationServiceClient> _locationServiceClient;

        public ServiceManager(ILoggerManager logger,
                              IMailjetClientService mailjet,
                              IHostEnvironment host,
                              IEFCoreMongoCrudKit mongoCrudKit,
                              IHttpClientFactory httpClient)
        {
            _emailNotificationService = new Lazy<IEmailNotificationService>(() =>
                new EmailNotificationService(mailjet, host, logger));
            _locationServiceClient = new Lazy<ILocationServiceClient>(() =>
                new LocationServiceClient(httpClient, mongoCrudKit, logger));
        }

        public IEmailNotificationService Notification => _emailNotificationService.Value;
        public ILocationServiceClient Location => _locationServiceClient.Value;
    }
}