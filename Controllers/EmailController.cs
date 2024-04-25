using EmailApp.Entities;
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
    public class EmailController : ControllerBase
    {


        [HttpPost(Name = "GetWeatherForecast")]
        public void Get()
        {
            Credentials credentials = new Credentials();
            credentials.Email = "misterco2002@gmail.com";
            credentials.Password = "vtht tuyh rktv drbr";
            string toEmail = "catalin.p2002@gmail.com";
            string subject = "new subject";
            string body = "My body";
            string attachmentPath = "D:\\Projects\\EmailApp\\Assets\\img.jpg";

            _ = SendEmail.GetEmails(credentials, toEmail, subject, body, attachmentPath);
        }



        [HttpGet("GetEmailsPOP3")]
        public async Task<IActionResult> GetEmailsPop3()
        {
            Credentials credentials = new Credentials();
            credentials.Email = "catalin.p2002@gmail.com";
            credentials.Password = "kmpa alvc brdy pqyq";

            var emailsWithAttachments = await GetPOP3Emails.GetEmailsIMAP(credentials);

            List<object> response = new List<object>();

            foreach (var email in emailsWithAttachments)
            {
                var emailContent = email.Item1;
                var attachments = email.Item2;

                List<string> attachmentNames = new List<string>();

                foreach (var attachment in attachments)
                {
                    attachmentNames.Add(attachment.ContentDisposition?.FileName ?? "Unknown");
                }

                response.Add(new { EmailContent = emailContent, Attachments = attachmentNames });
            }

            return Ok(response);
        }


        [HttpGet("GetEmailsIMAP")]
        public async Task<IActionResult> GetEmailsIMAP()
        {
            string host = "imap.gmail.com";
            int port = 993;
            bool useSsl = true;
            Credentials credentials = new Credentials();
            credentials.Email = "catalin.p2002@gmail.com";
            credentials.Password = "kmpa alvc brdy pqyq";

            var emailsWithAttachments = await GetIMAPEmails.GetEmailsIMAP2(credentials);

            List<object> response = new List<object>();

            foreach (var email in emailsWithAttachments)
            {
                var emailContent = email.Item1;
                var attachments = email.Item2;

                List<object> attachmentList = new List<object>();

                foreach (var attachment in attachments)
                {
                    attachmentList.Add(new { FileName = attachment.Item1, Content = Convert.ToBase64String(attachment.Item2) });
                }

                response.Add(new { EmailContent = emailContent, Attachments = attachmentList });
            }

            return Ok(response);
        }

    }
}