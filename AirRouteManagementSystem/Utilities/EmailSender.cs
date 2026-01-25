using Microsoft.AspNetCore.Identity.UI.Services;
using System.Net;
using System.Net.Mail;

namespace AirRouteManagementSystem.Utilities
{
    public class EmailSender : IEmailSender
    {
        private readonly IConfiguration _configuration;

        public EmailSender(IConfiguration configuration)
        {
            _configuration = configuration;
        }
        public async Task SendEmailAsync(string email, string subject, string htmlMessage)
        {

            var smtp = new SmtpClient
            {
                Host = _configuration["EmailSettings:Host"]!,
                Port = int.Parse(_configuration["EmailSettings:Port"]!),
                EnableSsl = true,
                UseDefaultCredentials = false,
                Credentials = new NetworkCredential(_configuration["EmailSettings:Email"],
                _configuration["EmailSettings:Password"])
            };

            var message = new MailMessage
            {
                From = new MailAddress(_configuration["EmailSettings:Email"]!, "Air Route System"),
                Body = htmlMessage,
                Subject = subject,
                IsBodyHtml = true
            };

            message.To.Add(email);

            await smtp.SendMailAsync(message);
            
        }
    }
}
