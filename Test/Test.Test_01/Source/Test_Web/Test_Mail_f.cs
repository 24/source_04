using System;
using System.Net.Mail;
using pb;
using pb.Compiler;
using pb.Data.Xml;
using pb.Web;

namespace Test.Test_Web
{
    public static class Test_Mail_f
    {
        private static XmlConfig _localConfig = null;
        private static MailSender _mailSender = null;

        //public static void Test_Mail_01()
        //{
        //    System.Net.Mail.MailMessage message = new System.Net.Mail.MailMessage();
        //    message.To.Add("labeuzbeuz@gmail.com");
        //    message.Subject = "test";
        //    message.From = new System.Net.Mail.MailAddress("pierre.beuzart@gmail.com");
        //    message.Body = "test";
        //    System.Net.Mail.SmtpClient smtp = new System.Net.Mail.SmtpClient("yoursmtphost");
        //    smtp.Send(message);
        //}

        //public static void Test_Mail_02()
        //{
        //    // obsolete
        //    System.Web.Mail.MailMessage message = new System.Web.Mail.MailMessage();
        //    message.From = "from e-mail";
        //    message.To = "to e-mail";
        //    message.Subject = "Message Subject";
        //    message.Body = "Message Body";
        //    System.Web.Mail.SmtpMail.SmtpServer = "SMTP Server Address";
        //    System.Web.Mail.SmtpMail.Send(message);

        //    System.Web.Mail.SmtpMail.SmtpServer = "SMTP Host Address";
        //    System.Web.Mail.SmtpMail.Send("from", "To", "Subject", "MessageText");
        //}

        public static void Test_Mail_01()
        {
            //var fromAddress = new MailAddress("pierre.beuzart@gmail.com", "Pierre Beuzart");
            //var toAddress = new MailAddress("labeuzbeuz@gmail.com", "To Name");
            //var fromAddress = new MailAddress(localConfig.Get("Gmail/Mail"));
            //var toAddress = new MailAddress("pierre.beuzart@gmail.com");
            //string subject = "test";
            //string body = "test";
            //using (var message = new MailMessage(fromAddress, toAddress)
            //{
            //    Subject = subject,
            //    Body = body
            //})
            //{
            //    GetSmtpClient().Send(message);
            //}

            XmlConfig localConfig = GetLocalConfig();
            MailMessage message = new MailMessage(new MailAddress(localConfig.Get("DownloadAutomateManager/Gmail/Mail")), new MailAddress("pierre.beuzart@gmail.com"))
            {
                Subject = "test",
                Body = "test"
            };
            GetSmtpClient().Send(message);
        }

        public static void Test_Mail_02(string recipients, string subject, string body, string from = null)
        {
            GetMailSender().Send(recipients, subject, body, from);
        }

        public static SmtpClient GetSmtpClient()
        {
            XmlConfig localConfig = GetLocalConfig();
            string mail = localConfig.Get("DownloadAutomateManager/Gmail/Mail");
            string password = localConfig.Get("DownloadAutomateManager/Gmail/Password");
            string smtpHost = localConfig.Get("DownloadAutomateManager/Gmail/SmtpHost");
            int smtpPort = localConfig.Get("DownloadAutomateManager/Gmail/SmtpPort").zTryParseAs<int>(587);
            return new SmtpClient
            {
                Host = smtpHost,
                Port = smtpPort,
                EnableSsl = true,
                DeliveryMethod = SmtpDeliveryMethod.Network,
                UseDefaultCredentials = false,
                Credentials = new System.Net.NetworkCredential(new MailAddress(mail).Address, password)
            };
        }

        public static MailSender GetMailSender()
        {
            if (_mailSender == null)
            {
                XmlConfig localConfig = GetLocalConfig();
                _mailSender = new MailSender(localConfig.Get("DownloadAutomateManager/Gmail/Mail"), localConfig.Get("DownloadAutomateManager/Gmail/Password"),
                    localConfig.Get("DownloadAutomateManager/Gmail/SmtpHost"), localConfig.Get("DownloadAutomateManager/Gmail/SmtpPort").zTryParseAs<int>(587));
            }
            return _mailSender;
        }

        public static XmlConfig GetLocalConfig()
        {
            if (_localConfig == null)
                _localConfig = new XmlConfig(XmlConfig.CurrentConfig.Get("LocalConfig"));
            return _localConfig;
        }
    }
}
