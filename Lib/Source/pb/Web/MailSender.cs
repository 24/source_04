using System;
using System.Net.Mail;

namespace pb.Web
{
    public class MailSender
    {
        private static bool __trace = false;
        private SmtpClient _smtpClient;
        private MailAddress _mail;

        public MailSender(string mail, string password, string smtpHost, int smtpPort = 587)
        {
            _mail = new MailAddress(mail);
            _smtpClient = new SmtpClient
            {
                Host = smtpHost,
                Port = smtpPort,
                EnableSsl = true,
                DeliveryMethod = SmtpDeliveryMethod.Network,
                UseDefaultCredentials = false,
                Credentials = new System.Net.NetworkCredential(_mail.Address, password)
            };
        }

        public static bool Trace
        {
            get { return __trace; }
            set { __trace = value; }
        }

        public void Send(MailMessage message)
        {
            if (message.From == null)
                message.From = _mail;
            if (__trace)
                System.Diagnostics.Trace.TraceInformation("Send mail to {0} subject \"{1}\"", message.To, message.Subject);
            _smtpClient.Send(message);
        }

        public void Send(string recipients, string subject, string body, string from = null)
        {
            if (from == null)
                from = _mail.Address;
            if (__trace)
                System.Diagnostics.Trace.TraceInformation("Send mail to {0} subject \"{1}\"", recipients, subject);
            _smtpClient.Send(from, recipients, subject, body);
        }
    }
}
