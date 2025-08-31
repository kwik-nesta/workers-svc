using KwikNesta.Contracts.Models;

namespace KwikNesta.Workers.Svc.Core.Handlers.Interfaces
{
    public interface IMessageHandler
    {
        Task HandleAsync(NotificationMessage message);
        Task HandleAsync(AuditLog message);
        Task HandleAsync(DataLoadRequest message);
    }
}
