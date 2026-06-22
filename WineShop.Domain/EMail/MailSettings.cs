using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WineShop.Domain.EMail
{
    public class MailSettings
    {
        public string SmtpServer { get; set; }
        public int SmtpServerPort { get; set; }
        public string EmailDisplayName { get; set; }
        public string SendersName { get; set; }
        public string SmtpUserName { get; set; }
        public string SmtpPassword { get; set; }
        public bool EnableSsl { get; set; }

        public MailSettings() { }


        public MailSettings(string smtpServer, int smtpServerPort, string emailDisplayName, string sendersName, string smtpUserName, string smtpPassword, bool enableSsl)
        {
            this.SmtpServer = smtpServer;
            this.SmtpServerPort = smtpServerPort;
            EmailDisplayName = emailDisplayName;
            this.SendersName = sendersName;
            this.SmtpUserName = smtpUserName;
            this.SmtpPassword = smtpPassword;
            this.EnableSsl = enableSsl;
        }
    }
}
