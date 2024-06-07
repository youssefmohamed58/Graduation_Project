using System.Net;
using System.Net.Mail;
using Microsoft.AspNetCore.Identity;
using Project.Core.Entities;

namespace Project.Apis.Extensions
{
    public static class EmailSettings
    {
        public static void SendEmail(Email email)
        {
            var client = new SmtpClient("smtp.gmail.com", 587);
            client.EnableSsl = true;
            client.Credentials = new NetworkCredential("moy728262@gmail.com", "ymsvfhlxxonzollt");
            client.Send("ym098844@gmail.com", email.To, email.Subject, email.Body.ToString());
        }
    }
}
