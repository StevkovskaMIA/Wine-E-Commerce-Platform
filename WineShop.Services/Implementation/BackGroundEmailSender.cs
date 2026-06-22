using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WineShop.Domain.DomainModels;
using WineShop.Repository.Interface;
using WineShop.Services.Interface;

namespace WineShop.Services.Implementation
{
    public class BackGroundEmailSender : IBackGroundEmailSender
    {
        private readonly IEmailService _emailService;
        private readonly IRepository<EmailMessage> _mailRepository;

        public BackGroundEmailSender(IEmailService emailService, IRepository<EmailMessage> mailRepository)

        {
            _emailService = emailService;
            _mailRepository = mailRepository;
        }

        public async Task DoWork()
        {
            var pendingEmails = _mailRepository.GetAll().Where(z => !z.status).ToList();

            foreach (var email in pendingEmails)
            {
                await _emailService.SendEmailAsync(email);
            }
        }

    }
}
