using System.Net.Mail;
using System.Net;
using Trivesta.Model;

namespace App.Services
{
    public class EmailService
    {
        public static int SendMail(Email email, string sender, string password)
        {
            MailMessage m = new MailMessage();
            SmtpClient sc = new SmtpClient();
            m.From = new MailAddress(sender, email.DisplayName);

            foreach (var recipient in email.Recipients)
            {
                m.To.Add(recipient);
            }

            m.Subject = email.Subject;
            m.Body = email.Message;
            m.IsBodyHtml = true;

            sc.Host = "mail.trendycampus.com";
            string str1 = "gmail.com";
            string str2 = sender.ToLower();

            if (str2.Contains(str1))
            {
                try
                {
                    sc.Port = 465;
                    sc.Credentials = new NetworkCredential(sender, password);
                    sc.EnableSsl = true;
                    sc.Send(m);
                    return 1;
                }
                catch (Exception)
                {
                    //Response.Write("<BR><BR>* Please double check the From Address and Password to confirm that both of them are correct. <br>");
                    //Response.Write("<BR><BR>If you are using gmail smtp to send email for the first time, please refer to this KB to setup your gmail account: http://www.smarterasp.net/support/kb/a1546/send-email-from-gmail-with-smtp-authentication-but-got-5_5_1-authentication-required-error.aspx?KBSearchID=137388");
                    //Response.End();
                    return 0;
                }
            }
            else
            {
                try
                {
                    sc.Port = 25;
                    sc.Credentials = new NetworkCredential(sender, password);
                    sc.EnableSsl = false;
                    sc.Send(m);
                    return 1;
                    //Response.Write("Email Send successfully");
                }
                catch (Exception)
                {
                    //Response.Write("<BR><BR>* Please double check the From Address and Password to confirm that both of them are correct. <br>");
                    //Response.End();
                    return 0;
                }
            }
        }

    }

}