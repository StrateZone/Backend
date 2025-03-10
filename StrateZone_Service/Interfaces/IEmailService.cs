using StrateZone_Service.CustomModels.RequestModels;
using System.Net.Mail;

namespace StrateZone_Service.Interfaces
{
    public interface IEmailService
    {
        Task<MailMessage> SendEmailAsync(EmailRequest request);
    }
}