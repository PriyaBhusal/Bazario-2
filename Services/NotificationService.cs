using System;
using System.Configuration;
using System.Net;
using System.Net.Mail;
using OnlineRetailStore.Mvc.Models;

namespace OnlineRetailStore.Mvc.Services
{
    public static class NotificationService
    {
        /// <summary>Records an in-app notification. Always succeeds independently of email delivery.</summary>
        public static void Notify(AppDbContext db, int userId, string message)
        {
            db.Notifications.Add(new Notification { UserId = userId, Message = message, IsRead = false, CreatedAt = DateTime.Now });
            db.SaveChanges();
        }

        /// <summary>
        /// Best-effort order email. Silently does nothing if SMTP isn't configured (SmtpHost blank),
        /// and never throws - a failed send must not break checkout or order status updates.
        /// </summary>
        public static void SendEmail(string toEmail, string subject, string body)
        {
            var host = ConfigurationManager.AppSettings["SmtpHost"];
            if (string.IsNullOrWhiteSpace(host) || string.IsNullOrWhiteSpace(toEmail)) return;

            try
            {
                var port = int.TryParse(ConfigurationManager.AppSettings["SmtpPort"], out var p) ? p : 587;
                var user = ConfigurationManager.AppSettings["SmtpUser"];
                var pass = ConfigurationManager.AppSettings["SmtpPass"];
                var from = ConfigurationManager.AppSettings["SmtpFrom"];
                var enableSsl = !bool.TryParse(ConfigurationManager.AppSettings["SmtpEnableSsl"], out var ssl) || ssl;

                using (var client = new SmtpClient(host, port))
                {
                    client.EnableSsl = enableSsl;
                    if (!string.IsNullOrEmpty(user))
                    {
                        client.Credentials = new NetworkCredential(user, pass);
                    }

                    using (var msg = new MailMessage(from, toEmail, subject, body))
                    {
                        client.Send(msg);
                    }
                }
            }
            catch
            {
                // Email is a nice-to-have on top of the in-app notification; never let a
                // misconfigured or unreachable SMTP server break the calling operation.
            }
        }

        /// <summary>Notifies both in-app and by email in one call.</summary>
        public static void NotifyAndEmail(AppDbContext db, int userId, string toEmail, string message, string emailSubject, string emailBody)
        {
            Notify(db, userId, message);
            SendEmail(toEmail, emailSubject, emailBody);
        }
    }
}
