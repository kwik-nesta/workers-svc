namespace KwikNesta.Workers.Svc.Application.Interfaces
{
    public interface IServiceManager
    {
        IEmailNotificationService Notification { get; }
        ILocationServiceClient Location { get; }
    }
}
