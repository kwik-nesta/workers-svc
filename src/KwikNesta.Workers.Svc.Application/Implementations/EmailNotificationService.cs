using CSharpTypes.Extensions.String;
using DiagnosKit.Core.Logging.Contracts;
using DRY.MailJetClient.Library;
using KwikNesta.Contracts.Models;
using KwikNesta.Workers.Svc.Application.Extensions;
using KwikNesta.Workers.Svc.Application.Interfaces;
using Microsoft.Extensions.Hosting;

namespace KwikNesta.Workers.Svc.Application.Implementations
{
    public class EmailNotificationService : IEmailNotificationService
    {
        private readonly IMailjetClientService _mailJet;
        private readonly IHostEnvironment _env;
        private readonly ILoggerManager _logger;
        private readonly string _templateRoot;

        public EmailNotificationService(IMailjetClientService mailJet,
                                        IHostEnvironment env,
                                        ILoggerManager logger)
        {
            _mailJet = mailJet;
            _env = env;
            _logger = logger;
            _templateRoot = Path.Combine(env.ContentRootPath, "wwwroot", "templates");
        }

        public async Task SendAccountActivationEmail(NotificationMessage notification)
        {
            var valid = notification.ValidatePayload();
            if (!valid)
            {
                _logger.LogWarn("Invalid notification payload");
                return;
            }

            var template = notification.Type.LoadTemplate(_templateRoot);
            if (template.IsNullOrEmpty())
            {
                _logger.LogWarn("The template returned an empty string");
                return;
            }

            var body = template.Replace("{{FirstName}}", notification.ReceipientName)
                .Replace("{{OTP}}", notification.Otp?.Value)
                .Replace("{{validity}}", notification.Otp?.Span.ToString())
                .Replace("{{Year}}", DateTime.UtcNow.Year.ToString());

            var isSent = await _mailJet.SendAsync(notification.EmailAddress, body, notification.Subject);
            if (isSent)
            {
                _logger.LogInfo("Account activation notification email successfully sent to {EmailAddress}", notification.EmailAddress);
                return;
            }
            else
            {
                _logger.LogWarn("Account activation notification email failed for {EmailAddress}", notification.EmailAddress);
                return;
            }
        }

        public async Task SendPasswordResetEmail(NotificationMessage notification)
        {
            var valid = notification.ValidatePayload();
            if (!valid)
            {
                _logger.LogWarn("Invalid notification payload");
                return;
            }

            var template = notification.Type.LoadTemplate(_templateRoot);
            if (template.IsNullOrEmpty())
            {
                _logger.LogWarn("The template returned an empty string");
                return;
            }

            var body = template.Replace("{{FirstName}}", notification.ReceipientName)
                .Replace("{{OTP}}", notification.Otp?.Value)
                .Replace("{{validity}}", notification.Otp?.Span.ToString())
                .Replace("{{Year}}", DateTime.UtcNow.Year.ToString());

            var isSent = await _mailJet.SendAsync(notification.EmailAddress, body, notification.Subject);
            if (isSent)
            {
                _logger.LogInfo("Password reset email successfully sent to {EmailAddress}", notification.EmailAddress);
                return;
            }
            else
            {
                _logger.LogWarn("Password reset email failed for {EmailAddress}", notification.EmailAddress);
                return;
            }
        }

        public async Task SendPasswordResetNotificationEmail(NotificationMessage notification)
        {
            var valid = notification.ValidatePayload();
            if (!valid)
            {
                _logger.LogWarn("Invalid notification payload");
                return;
            }

            var template = notification.Type.LoadTemplate(_templateRoot);
            if (template.IsNullOrEmpty())
            {
                _logger.LogWarn("The template returned an empty string");
                return;
            }

            var body = template.Replace("{{FirstName}}", notification.ReceipientName)
                .Replace("{{Year}}", DateTime.UtcNow.Year.ToString());

            var isSent = await _mailJet.SendAsync(notification.EmailAddress, body, notification.Subject);
            if (isSent)
            {
                _logger.LogInfo("Password reset notification email successfully sent to {EmailAddress}", notification.EmailAddress);
                return;
            }
            else
            {
                _logger.LogWarn("Password reset notification email failed for {EmailAddress}", notification.EmailAddress);
                return;
            }
        }

        public async Task SendAccountDeactivationEmail(NotificationMessage notification)
        {
            var valid = notification.ValidatePayload();
            if (!valid)
            {
                _logger.LogWarn("Invalid notification payload");
                return;
            }

            var template = notification.Type.LoadTemplate(_templateRoot);
            if (template.IsNullOrEmpty())
            {
                _logger.LogWarn("The template returned an empty string");
                return;
            }

            var now = DateTime.UtcNow;
            var body = template.Replace("{{FirstName}}", notification.ReceipientName)
                .Replace("{{DeactivationDate}}", now.ToString("d"))
                .Replace("{{Year}}", now.Year.ToString());

            var isSent = await _mailJet.SendAsync(notification.EmailAddress, body, notification.Subject);
            if (isSent)
            {
                _logger.LogInfo("Account deactivation notification email successfully sent to {EmailAddress}", notification.EmailAddress);
                return;
            }
            else
            {
                _logger.LogWarn("Account deactivation notification email failed for {EmailAddress}", notification.EmailAddress);
                return;
            }
        }

        public async Task SendAccountReactivationEmail(NotificationMessage notification)
        {
            var valid = notification.ValidatePayload();
            if (!valid)
            {
                _logger.LogWarn("Invalid notification payload");
                return;
            }

            var template = notification.Type.LoadTemplate(_templateRoot);
            if (template.IsNullOrEmpty())
            {
                _logger.LogWarn("The template returned an empty string");
                return;
            }

            var now = DateTime.UtcNow;
            var body = template.Replace("{{FirstName}}", notification.ReceipientName)
                .Replace("{{OTP}}", notification.Otp?.Value)
                .Replace("{{validity}}", notification.Otp?.Span.ToString())
                .Replace("{{Year}}", now.Year.ToString());

            var isSent = await _mailJet.SendAsync(notification.EmailAddress, body, notification.Subject);
            if (isSent)
            {
                _logger.LogInfo("Account reactivation OTP email successfully sent to {EmailAddress}", notification.EmailAddress);
                return;
            }
            else
            {
                _logger.LogWarn("Account reactivation OTP email failed for {EmailAddress}", notification.EmailAddress);
                return;
            }
        }

        public async Task SendAccountReactivationNotificationEmail(NotificationMessage notification)
        {
            var valid = notification.ValidatePayload();
            if (!valid)
            {
                _logger.LogWarn("Invalid notification payload");
                return;
            }

            var template = notification.Type.LoadTemplate(_templateRoot);
            if (template.IsNullOrEmpty())
            {
                _logger.LogWarn("The template returned an empty string");
                return;
            }

            var body = template.Replace("{{FirstName}}", notification.ReceipientName)
                .Replace("{{Year}}", DateTime.UtcNow.Year.ToString());

            var isSent = await _mailJet.SendAsync(notification.EmailAddress, body, notification.Subject);
            if (isSent)
            {
                _logger.LogInfo("Account suspension notification email successfully sent to {EmailAddress}", notification.EmailAddress);
                return;
            }
            else
            {
                _logger.LogWarn("Account suspension notification email failed for {EmailAddress}", notification.EmailAddress);
                return;
            }
        }

        public async Task SendAccountSuspensionEmail(NotificationMessage notification)
        {
            var valid = notification.ValidatePayload();
            if (!valid)
            {
                _logger.LogWarn("Invalid notification payload");
                return;
            }

            var template = notification.Type.LoadTemplate(_templateRoot);
            if (template.IsNullOrEmpty())
            {
                _logger.LogWarn("The template returned an empty string");
                return;
            }

            var body = template.Replace("{{FirstName}}", notification.ReceipientName)
                .Replace("{{Reason}}", notification.Reason)
                .Replace("{{Year}}", DateTime.UtcNow.Year.ToString());

            var isSent = await _mailJet.SendAsync(notification.EmailAddress, body, notification.Subject);
            if (isSent)
            {
                _logger.LogInfo("Account suspension notification email successfully sent to {EmailAddress}", notification.EmailAddress);
                return;
            }
            else
            {
                _logger.LogWarn("Account suspension notification email failed for {EmailAddress}", notification.EmailAddress);
                return;
            }
        }

        public async Task SendAdminRectivationNotificationEmail(NotificationMessage notification)
        {
            var valid = notification.ValidatePayload();
            if (!valid)
            {
                _logger.LogWarn("Invalid notification payload");
                return;
            }

            var template = notification.Type.LoadTemplate(_templateRoot);
            if (template.IsNullOrEmpty())
            {
                _logger.LogWarn("The template returned an empty string");
                return;
            }

            var body = template.Replace("{{FirstName}}", notification.ReceipientName)
                .Replace("{{Year}}", DateTime.UtcNow.Year.ToString());

            var isSent = await _mailJet.SendAsync(notification.EmailAddress, body, notification.Subject);
            if (isSent)
            {
                _logger.LogInfo("Account suspension lift notification email successfully sent to {EmailAddress}", notification.EmailAddress);
                return;
            }
            else
            {
                _logger.LogWarn("Account suspension lift notification email failed for {EmailAddress}", notification.EmailAddress);
                return;
            }
        }
    }
}