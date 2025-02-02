using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace MyVideoResume.Application;

public class EmailService
{
    private readonly IConfiguration _configuration;
    private readonly ILogger<EmailService> _logger;

    public EmailService(IConfiguration configuration, ILogger<EmailService> logger)
    {
        this._configuration = configuration;
        this._logger = logger;
    }

    public async Task SendEmailAsync(string to, string subject, string body)
    {
        try
        {
            var mailMessage = new System.Net.Mail.MailMessage();
            mailMessage.From = new System.Net.Mail.MailAddress(_configuration.GetValue<string>("Smtp:Email"));
            mailMessage.Body = body;
            mailMessage.Subject = subject;
            mailMessage.BodyEncoding = System.Text.Encoding.UTF8;
            mailMessage.SubjectEncoding = System.Text.Encoding.UTF8;
            mailMessage.IsBodyHtml = true;
            mailMessage.To.Add(to);

            var client = new System.Net.Mail.SmtpClient(_configuration.GetValue<string>("Smtp:Host"))
            {
                UseDefaultCredentials = false,
                EnableSsl = _configuration.GetValue<bool>("Smtp:Ssl"),
                Port = _configuration.GetValue<int>("Smtp:Port"),
                Credentials = new System.Net.NetworkCredential(_configuration.GetValue<string>("Smtp:User"), _configuration.GetValue<string>("Smtp:Password"))
            };

            await client.SendMailAsync(mailMessage);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex.Message, ex);
        }
    }
}
