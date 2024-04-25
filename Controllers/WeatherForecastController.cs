using EmailApp.Methods;
using MailKit;
using MailKit.Net.Imap;
using MailKit.Net.Pop3;
using Microsoft.AspNetCore.Mvc;
using MimeKit;
using System.Net;
using System.Net.Mail;
using System.Net.Mime;

namespace EmailApp.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class WeatherForecastController : ControllerBase
    {


        [HttpPost(Name = "GetWeatherForecast")]
        public void Get()
        {
            string fromEmail = "misterco2002@gmail.com";
            string password = "vtht tuyh rktv drbr";
            string toEmail = "catalin.p2002@gmail.com";
            string subject = "new subject";
            string body = "My body";
            string attachmentPath = "D:\\Projects\\EmailApp\\Assets\\img.jpg";

            _ = SendEmail.GetEmails(fromEmail, password, toEmail, subject, body, attachmentPath);
        }



        [HttpGet("GetEmails")]
        public ActionResult<IEnumerable<string>> GetEmails()
        {
            List<string> emailsList = new List<string>();

            string host = "pop.gmail.com";
            int port = 995;
            bool useSsl = true;
            string username = "catalin.p2002@gmail.com";
            string password = "kmpa alvc brdy pqyq";

            using (Pop3Client pop3 = new Pop3Client())
            {
                pop3.Connect(host, port, useSsl); // Connect to server and login
                pop3.Authenticate(username, password);

                int messageCount = pop3.Count;
                List<MimeMessage> messages = new List<MimeMessage>();

                for (int i = 0; i < messageCount; i++)
                {
                    var message = pop3.GetMessage(i);
                    messages.Add(message);
                }

                // Sort messages by Date received in descending order
                messages = messages.OrderByDescending(m => m.Date).ToList();

                foreach (var message in messages)
                {
                    string emailContent = $"Subject: {message.Subject}\n";
                    emailContent += $"From: {message.From}\n";
                    emailContent += $"Date: {message.Date}\n";
                    emailContent += $"Body: {message.TextBody ?? ""}\n";

                    emailsList.Add(emailContent);
                }
            }

            return emailsList;
        }

        [HttpGet("GetEmail22s")]
        public ActionResult<IEnumerable<string>> GetEmailsIMAP()
        {
            List<string> emailsList = new List<string>();

            string host = "imap.gmail.com";
            int port = 993;
            bool useSsl = true;
            string username = "catalin.p2002@gmail.com";
            string password = "kmpa alvc brdy pqyq";

            using (ImapClient imap = new ImapClient())
            {
                imap.Connect(host, port, useSsl); // Connect to server
                imap.Authenticate(username, password); // Login

                imap.Inbox.Open(MailKit.FolderAccess.ReadOnly); // Open Inbox folder

                // Get list of messages sorted by Date received in descending order
                var messages = imap.Inbox.Fetch(0, -1, MessageSummaryItems.Full | MessageSummaryItems.UniqueId)
                                       .OrderByDescending(m => m.Date)
                                       .ToList();

                foreach (var message in messages)
                {
                    string emailContent = $"Subject: {message.Envelope.Subject}\n";
                    emailContent += $"From: {message.Envelope.From}\n";
                    emailContent += $"Date: {message.Date}\n";

                    // Fetch the full message body
                    var fullMessage = imap.Inbox.GetMessage(message.UniqueId);
                    emailContent += $"Body: {fullMessage.TextBody ?? ""}\n";

                    emailsList.Add(emailContent);
                }

            }

            return emailsList;
        } 
    }
}