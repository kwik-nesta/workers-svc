using KwikNesta.Contracts.Models;

namespace KwikNesta.Workers.Svc.Application.Interfaces
{
    public interface IEmailNotificationService
    {
        Task SendAccountActivationEmail(NotificationMessage notification);
        Task SendAccountDeactivationEmail(NotificationMessage notification);
        Task SendAccountReactivationEmail(NotificationMessage notification);
        Task SendAccountReactivationNotificationEmail(NotificationMessage notification);
        Task SendAccountSuspensionEmail(NotificationMessage notification);
        Task SendAdminRectivationNotificationEmail(NotificationMessage notification);
        Task SendPasswordResetEmail(NotificationMessage notification);
        Task SendPasswordResetNotificationEmail(NotificationMessage notification);
    }
}
