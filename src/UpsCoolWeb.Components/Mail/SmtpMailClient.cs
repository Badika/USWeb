using Microsoft.Extensions.Configuration;
using System;
using System.Net;
using System.Net.Mail;
using System.Text;
using System.Threading.Tasks;

namespace UpsCoolWeb.Components.Mail
{
    public class SmtpMailClient : IMailClient
    {
        private IConfiguration Config { get; }

        public SmtpMailClient(IConfiguration config)
        {
            Config = config.GetSection("Mail");
        }

        public async Task SendAsync(String email, String subject, String body)
        {
            using (SmtpClient client = new SmtpClient(Config["Host"], Int32.Parse(Config["Port"])))
            {
                client.Credentials = new NetworkCredential(Config["Sender"], Config["Password"]);
                client.EnableSsl = Boolean.Parse(Config["EnableSsl"]);

                MailMessage mail = new MailMessage(Config["Sender"], email, subject, body);
                mail.SubjectEncoding = Encoding.UTF8;
                mail.BodyEncoding = Encoding.UTF8;
                mail.IsBodyHtml = true;

                await client.SendMailAsync(mail);
            }
        }
    }
}
