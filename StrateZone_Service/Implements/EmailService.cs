using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mail;
using System.Net;
using StrateZone_Service.Interfaces;
using StrateZone_Service.CustomModels.RequestModels;

namespace StrateZone_Service.Implements
{
    public class EmailService : IEmailService
    {
        private readonly IConfiguration _configuration;

        public EmailService(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public async Task<MailMessage> SendEmailAsync(EmailRequest request)
        {
            var smtpClient = new SmtpClient(_configuration["EmailSettings:SmtpServer"])
            {
                Port = int.Parse(_configuration["EmailSettings:Port"]),
                Credentials = new NetworkCredential(_configuration["EmailSettings:Username"], _configuration["EmailSettings:Password"]),
                EnableSsl = true,
            };

            var mailMessage = new MailMessage
            {
                From = new MailAddress(
                    _configuration["EmailSettings:FromEmail"], 
                    "StrateZone"
                ),
                Subject = request.Subject,
                Body = request.Content,
                IsBodyHtml = true,
            };

            mailMessage.To.Add(request.ToEmail);

            await smtpClient.SendMailAsync(mailMessage);

            return mailMessage;
        }
    }
}
