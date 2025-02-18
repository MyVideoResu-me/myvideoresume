using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using MimeKit;
using MailKit.Net.Smtp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyVideoResume.Application;

public class ZohoEmailService : IEmailService
{

    private readonly IConfiguration _configuration;
    private readonly ILogger<ZohoEmailService> _logger;

    public ZohoEmailService(IConfiguration configuration, ILogger<ZohoEmailService> logger)
    {
        this._configuration = configuration;
        this._logger = logger;
    }

    public async Task SendEmailAsync(string to, string subject, string body)
    {
        try
        {
            var message = new MimeMessage();
            message.From.Add(new MailboxAddress("MyVideoResu.ME", _configuration.GetValue<string>("Smtp:Email")));
            message.To.Add(new MailboxAddress("Recipient", to));
            message.Subject = subject;
            message.Body = new TextPart("html")
            {
                Text = body
            };
            var client = new SmtpClient();
            try
            {
                client.SslProtocols = System.Security.Authentication.SslProtocols.Tls12;
                client.Connect(_configuration.GetValue<string>("Smtp:Host"), _configuration.GetValue<int>("Smtp:Port"), false);
                client.Authenticate("emailapikey", _configuration.GetValue<string>("Smtp:Key"));
                client.Send(message);
                client.Disconnect(true);
            }
            catch (Exception e)
            {
                Console.Write(e.Message);
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex.Message, ex);
        }
    }

}
