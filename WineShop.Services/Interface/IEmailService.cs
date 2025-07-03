using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WineShop.Domain.DomainModels;

namespace WineShop.Services.Interface
{
    public interface IEmailService
    {
        Task SendEmailAsync(EmailMessage allMails);


    }
}
