using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MailKit.Net.Smtp;
using MailKit.Security;
using MimeKit;
using Microsoft.Extensions.Options;
using DAL.Models;
using Microsoft.Extensions.Logging;
using DAL.Interface;

namespace DAL.Services
{
    public class MailService : IMailService
    {
        private readonly SmtpSettings _smtpSettings;
        private readonly ILogger<MailService> _logger;

        public MailService(IOptions<SmtpSettings> smtpSettings, ILogger<MailService> logger)
        {
            _smtpSettings = smtpSettings.Value;
            _logger = logger;
        }

        public async Task SendEmailAsync(MailParams mailParams)
        {
            try
            {
                if (mailParams == null)
                {
                    throw new ArgumentNullException(nameof(mailParams), "MailParams cannot be null.");
                }

                var emailMessage = new MimeMessage();
                emailMessage.Sender = MailboxAddress.Parse(_smtpSettings.UserName);
                emailMessage.From.Add(new MailboxAddress(_smtpSettings.DisplayName, _smtpSettings.UserName));

                var mailRequest = ProcessPlaceHolders(mailParams);
                if (!string.IsNullOrEmpty(mailParams.ToEmail))
                {
                    foreach (var email in mailParams.ToEmail.Split(';'))
                    {
                        emailMessage.To.Add(MailboxAddress.Parse(email.Trim()));
                    }
                }

                emailMessage.Subject = mailRequest.Subject;
                var builder = new BodyBuilder { HtmlBody = mailRequest.Body };
                emailMessage.Body = builder.ToMessageBody();

                using var smtp = new SmtpClient();
                await smtp.ConnectAsync(_smtpSettings.Host, int.Parse(_smtpSettings.Port), SecureSocketOptions.StartTls);
                await smtp.AuthenticateAsync(_smtpSettings.UserName, _smtpSettings.Password);
                await smtp.SendAsync(emailMessage);
                await smtp.DisconnectAsync(true);

                _logger.LogInformation("Email sent successfully to {Recipients}", mailParams.ToEmail);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error sending email: {ex.Message}");
                throw;
            }
        }

        private MailRequest ProcessPlaceHolders(MailParams mailParams)
        {
            string subject = mailParams.Subject ?? "";
            string body = mailParams.Body ?? "";

            //if (mailParams.PlaceHolderKeyValuePairs != null)
            //{
            //    foreach (var keyValuePair in mailParams.PlaceHolderKeyValuePairs)
            //    {
            //        body = body.Replace(keyValuePair.Key, keyValuePair.Value);
            //        subject = subject.Replace(keyValuePair.Key, keyValuePair.Value);
            //    }
            //}

            return new MailRequest { Subject = subject, Body = body, ToEmail = mailParams.ToEmail };
        }
    }

}
