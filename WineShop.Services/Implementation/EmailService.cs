using MailKit.Security;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using MimeKit;
using MimeKit.Text;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mail;
using System.Text;
using System.Threading.Tasks;
using WineShop.Domain.DomainModels;
using WineShop.Domain.EMail;
using WineShop.Services.Interface;

namespace WineShop.Services.Implementation
{
    public class EmailService : IEmailService
    {
        private readonly MailSettings _mailSettings;
        private readonly ILogger<EmailService> _logger;


        public EmailService(IOptions<MailSettings> mailSettings, ILogger<EmailService> logger)
        {
            _mailSettings = mailSettings.Value;
            _logger = logger;
        }

        public async Task SendEmailAsync(EmailMessage allMails)
        {
         

            var emailMessage = new MimeMessage
                {
                    // Sender = new MailboxAddress("EShop Application", "integrated_systems_finki@outlook.com"),
                    Sender = new MailboxAddress(_mailSettings.SendersName, _mailSettings.SmtpUserName),
                    Subject = allMails.Subject
                };

                //emailMessage.From.Add(new MailboxAddress("EShop Application", "integrated_systems_finki@outlook.com"));
                emailMessage.From.Add(new MailboxAddress(_mailSettings.EmailDisplayName, _mailSettings.SmtpUserName));

                emailMessage.Body = new TextPart(MimeKit.Text.TextFormat.Plain) { Text = allMails.Content };
                emailMessage.To.Add(new MailboxAddress(allMails.MailTo, allMails.MailTo));

                try
                {
                using (var smtp = new MailKit.Net.Smtp.SmtpClient())
                {
                    var socketOptions = SecureSocketOptions.Auto;

                    await smtp.ConnectAsync(_mailSettings.SmtpServer, 587, socketOptions);

                    if (!string.IsNullOrEmpty(_mailSettings.SmtpUserName))
                    {
                        await smtp.AuthenticateAsync(_mailSettings.SmtpUserName, _mailSettings.SmtpPassword);
                    }
                    await smtp.SendAsync(emailMessage);


                    await smtp.DisconnectAsync(true);

                }

            }
            catch (SmtpException ex)
                {
                throw ex;
            }
            }
        }
    }
