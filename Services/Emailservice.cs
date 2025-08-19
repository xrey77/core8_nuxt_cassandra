using System;
// using System.net.Mail;
using System.Threading.Tasks;
using MailKit.Net.Smtp;
using MailKit;
using MailKit.Security;
using MimeKit;
using Microsoft.Extensions.Configuration; // For configuration

namespace core8_nuxt_cassandra.Services
{

    public interface IEmailService {
        Task<bool> sendMail(string to, string subject, string msgBody);
        void sendMailToken(string to, string subject, string msgBody);
       /* void SendEmail(string from, string from_name, string to, string cc, string bcc, string subject, string body, bool isHtml);*/
    }


public class EmailService : IEmailService
{

        IConfiguration _configuration = new ConfigurationBuilder()
    .AddJsonFile("appsettings.json")
    .AddEnvironmentVariables()
    .Build();

    public EmailService(IConfiguration configuration)
    {
        _configuration = configuration;
    }

        public async Task<bool> sendMail(string to, string subject, string msgBody) {
                    MimeMessage message = new MimeMessage();
                    message.From.Add(MailboxAddress.Parse(_configuration["EmailSettings:SenderEmail"]));
                    message.To.Add(MailboxAddress.Parse(to));                    
                    message.Subject = subject;

                    message.Body = new TextPart(MimeKit.Text.TextFormat.Html) 
                    { 
                        Text = msgBody
                    };

                    // BodyBuilder bodyBuilder = new BodyBuilder();
                    // bodyBuilder.HtmlBody = msgBody;
                    
                    using var smtp = new SmtpClient();
                    try
                    {                        //  await smtp.ConnectAsync("EmailSettings:SmtpServer", 465, SecureSocketOptions.SslOnConnect);
                         await smtp.ConnectAsync("EmailSettings:SmtpServer", 587, SecureSocketOptions.StartTls);
                         await smtp.AuthenticateAsync(_configuration["EmailSettings:SenderEmail"], _configuration["EmailSettings:SenderPassword"]);
                         await smtp.SendAsync(message);
                         return true;
                    }
                    finally
                    {
                         await smtp.DisconnectAsync(true);
                    }
        }

        public void sendMailToken(string to, string subject, string msgBody) {
                    MimeMessage message = new MimeMessage();
                    message.From.Add(MailboxAddress.Parse(_configuration["EmailSettings:SenderEmail"]));
                    message.To.Add(MailboxAddress.Parse(to));                    
                    message.Subject = subject;

                    message.Body = new TextPart(MimeKit.Text.TextFormat.Html) 
                    { 
                        Text = msgBody
                    };

                    // BodyBuilder bodyBuilder = new BodyBuilder();
                    // bodyBuilder.HtmlBody = msgBody;
                    
                    using var smtp = new SmtpClient();
                    try
                    {
                         smtp.ConnectAsync(_configuration["EmailSettings:SmtpServer"], 587, SecureSocketOptions.StartTls);
                         smtp.AuthenticateAsync(_configuration["EmailSettings:SenderEmail"], _configuration["EmailSettings:SenderPassword"]);
                         smtp.SendAsync(message);
                    }
                    finally
                    {
                         smtp.DisconnectAsync(true);
                    }
        }               
/*
        public void SendEmail(string from, string from_name, string to, string cc, string bcc, string subject, string body, bool isHtml)
        {
            var mailClient = new SmtpClient(_configuration["EmailSettings:smtpserver"]);
            mailClient.Credentials = new NetworkCredential(_configuration["EmailSettings:fromEmail"], _configuration["EmailSettings:emailPassword"]);
            mailClient.Port = Config.SmptSettings.Port;

            MailMessage message = new MailMessage();
            if (!string.IsNullOrEmpty(from_name))
            {
                message.From = new MailAddress(from, from_name);
            }
            else
            {
                message.From = new MailAddress(Formatter.UnFormatSqlInput(from));
            }

            message.To.Add(new MailAddress(to));

            if (!string.IsNullOrEmpty(bcc, cc))
            {
                message.CC.Add(cc);
                message.Bcc.Add(bcc);
            }

            message.Subject = subject;
            message.Body = body;
            message.IsBodyHtml = isHtml;

            mailClient.EnableSsl = true;
            mailClient.Send(message); 
        }
*/

 }
}