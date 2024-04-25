using Microsoft.AspNetCore.Mvc;
using System.Net.Mail;
using System.Net;

namespace EmailApp.Methods
{
    public static class SendEmail
    {
        public static async Task<string> GetEmails(string fromEmail, string password, string toEmail, string subject, string body, string attachmentPath)
        {
            //string fromEmail = "misterco2002@gmail.com";
            //string password = "vtht tuyh rktv drbr";
            //string toEmail = "catalin.p2002@gmail.com";

            MailMessage mailMessage = new MailMessage();
            mailMessage.From = new MailAddress(fromEmail);
            mailMessage.Subject = subject;
            mailMessage.To.Add(new MailAddress(toEmail));
            mailMessage.Body = $"<html><body> {body} </body></html>";
            mailMessage.IsBodyHtml = true;

            string filePath = attachmentPath;
            Attachment attachment = new Attachment(filePath);
            mailMessage.Attachments.Add(attachment);

            using (var smtpClient = new SmtpClient("smtp.gmail.com"))
            {
                smtpClient.Port = 587;
                smtpClient.Credentials = new NetworkCredential(fromEmail, password);
                smtpClient.EnableSsl = true;

                try
                {
                    await smtpClient.SendMailAsync(mailMessage);
                    return ("Email sent successfully.");
                }
                catch (Exception ex)
                {
                    // Log or handle the exception appropriately
                    return (ex.Message);
                }
            }
        }
    }
}
