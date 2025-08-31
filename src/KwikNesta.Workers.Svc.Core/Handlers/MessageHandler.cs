using CSharpTypes.Extensions.Enumeration;
using DiagnosKit.Core.Logging.Contracts;
using EFCore.CrudKit.Library.Data.Interfaces;
using KwikNesta.Contracts.Enums;
using KwikNesta.Contracts.Models;
using KwikNesta.Workers.Svc.Application.Extensions;
using KwikNesta.Workers.Svc.Application.Interfaces;
using KwikNesta.Workers.Svc.Core.Handlers.Interfaces;

namespace KwikNesta.Workers.Svc.Core.Handlers
{
    public class MessageHandler : IMessageHandler
    {
        private readonly ILoggerManager _logger;
        private readonly IEmailNotificationService _notificationService;
        private readonly IEFCoreMongoCrudKit _mongoCrudKit;

        public MessageHandler(ILoggerManager logger,
                              IEmailNotificationService notificationService,
                              IEFCoreMongoCrudKit mongoCrudKit)
        {
            _logger = logger;
            _notificationService = notificationService;
            _mongoCrudKit = mongoCrudKit;
        }

        public async Task HandleAsync(NotificationMessage message)
        {
            if (message != null)
            {
                switch (message.Type)
                {
                    case EmailType.AccountActivation:
                        await _notificationService.SendAccountActivationEmail(message);
                        break;
                    case EmailType.PasswordReset:
                        await _notificationService.SendPasswordResetEmail(message);
                        break;
                    case EmailType.PasswordResetNotification:
                        await _notificationService.SendPasswordResetNotificationEmail(message);
                        break;
                    case EmailType.AccountDeactivation:
                        await _notificationService.SendAccountDeactivationEmail(message);
                        break;
                    case EmailType.AccountReactivation:
                        await _notificationService.SendAccountReactivationEmail(message);
                        break;
                    case EmailType.AccountSuspension:
                        await _notificationService.SendAccountSuspensionEmail(message);
                        break;
                    case EmailType.AccountReactivationNotification:
                        await _notificationService.SendAccountReactivationNotificationEmail(message);
                        break;
                    case EmailType.AdminAccountReactivation:
                        await _notificationService.SendAdminRectivationNotificationEmail(message);
                        break;
                }
            }
            else
            {
                _logger.LogWarn($"Message content came null");
            }
        }

        public async Task HandleAsync(AuditLog message)
        {
            try
            {
                if (message != null)
                {
                    var isValid = message.ValidatePayload();
                    if (!isValid)
                    {
                        _logger.LogWarn("Invalid audit payload");
                        return;
                    }

                    await _mongoCrudKit.InsertAsync(message);
                    _logger.LogInfo("Audit trail successfully added. Action Performed: {Action}", message.Action.GetDescription());
                    return;
                }
                else
                {
                    _logger.LogWarn($"Message content came null");
                    return;
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred: {Message}", ex.Message);
            }
        }
    }
}
