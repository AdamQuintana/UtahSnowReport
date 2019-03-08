using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Mail;

namespace UtahSnowReport
{
    public interface IEmailService
    {
        bool SendAll(string subject, string content, List<string> recipients, Stream imageStream);
    }

    public class EmailService : IEmailService
    {
        private readonly EmailCredentials _credentials;
        private readonly SmtpClient _client;

        public EmailService(IOptions<EmailCredentials> credentials)
        {
            _credentials = credentials.Value;
            _client = new SmtpClient
            {
                Host = "smtp.gmail.com",
                Port = 587,
                EnableSsl = true,
                UseDefaultCredentials = false,
                Credentials = new NetworkCredential(_credentials.Email, _credentials.Password)
            };
        }

        private MailMessage BuildMessage(string subject, string content, Stream imageStream)
        {
            var msg = new MailMessage()
            {
                From = new MailAddress(_credentials.Email),
                IsBodyHtml = true,
                Subject = subject,
                Body = content,
            };
            msg.Attachments.Add(new Attachment(imageStream, "image/png"));
            return msg;
        }

        public bool SendAll(string subject, string content, List<string> recipients, Stream imageStream)
        {
            if (recipients == null || recipients.Count == 0)
                throw new ArgumentException("recipients");

            MailMessage msg = BuildMessage(subject, content, imageStream);
            foreach (string recipient in recipients)
            {
                var toEmail = new MailAddress(recipient);
                msg.To.Add(toEmail);
                try
                {
                    _client.Send(msg);
                }
                catch (Exception)
                {
                    return false;
                }
                msg.To.Remove(toEmail);
            }
            return true;
        }
    }
}
