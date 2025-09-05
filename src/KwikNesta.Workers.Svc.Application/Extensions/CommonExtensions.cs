using CSharpTypes.Extensions.Guid;
using CSharpTypes.Extensions.Object;
using CSharpTypes.Extensions.String;
using KwikNesta.Contracts.Enums;
using KwikNesta.Contracts.Models;

namespace KwikNesta.Workers.Svc.Application.Extensions
{
    public static class CommonExtensions
    {
        public static string GetTemplateName(this EmailType type)
        {
            return type switch
            {
                EmailType.AccountActivation => "account-activation",
                EmailType.AccountDeactivation => "account-deactivation",
                EmailType.AccountReactivation => "account-reactivation",
                EmailType.AccountReactivationNotification => "account-reactivation-notification",
                EmailType.AccountSuspension => "account-suspension",
                EmailType.AdminAccountReactivation => "admin-account-reactivation",
                EmailType.PasswordReset => "password-reset",
                EmailType.PasswordResetNotification => "password-reset-notification",
                _ => throw new NotImplementedException()
            };
        }

        public static string LoadTemplate(this EmailType templateType, string templateRootDirectory)
        {
            var path = Path.Combine(templateRootDirectory, $"{GetTemplateName(templateType)}.html");
            if (File.Exists(path))
            {
                return File.ReadAllText(path);
            }

            return string.Empty;
        }

        public static bool ValidatePayload(this NotificationMessage notification)
        {
            if (notification == null)
            {
                return false;
            }

            if (notification.ReceipientName.IsNullOrEmpty() || notification.EmailAddress.IsNullOrEmpty())
            {
                return false;
            }

            if (notification.Type is EmailType.AccountActivation or EmailType.PasswordReset or EmailType.AccountReactivation)
            {
                if (notification.Otp.IsNull() || (notification.Otp != null && notification.Otp.Value.IsNullOrEmpty()))
                {
                    return false;
                }
            }

            if (notification.Type is EmailType.AccountSuspension && string.IsNullOrWhiteSpace(notification.Reason))
            {
                return false;
            }

            return true;
        }

        public static bool ValidatePayload(this AuditLog audit)
        {
            if (audit == null)
            {
                return false;
            }

            if (audit.DomainId.IsEmpty() || audit.PerformedBy.IsNullOrEmpty() || audit.PerformedOnProfileId.IsNullOrEmpty())
            {
                return false;
            }

            return true;
        }
    }
}
