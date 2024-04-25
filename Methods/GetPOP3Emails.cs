using MailKit.Net.Imap;
using MailKit;
using Microsoft.AspNetCore.Mvc;
using EmailApp.Entities;
using MailKit.Net.Pop3;
using MimeKit;

namespace EmailApp.Methods
{
    public class GetPOP3Emails
    {
        public static async Task<List<(string, List<MimeEntity>)>> GetEmailsIMAP(Credentials credentials)
        {
            List<(string, List<MimeEntity>)> emailsList = new List<(string, List<MimeEntity>)>();

            string host = "pop.gmail.com";
            int port = 995;
            bool useSsl = true;

            using (Pop3Client pop3 = new Pop3Client())
            {
                pop3.Connect(host, port, useSsl);
                pop3.Authenticate(credentials.Email, credentials.Password);

                int messageCount = pop3.Count;
                List<MimeMessage> messages = new List<MimeMessage>();

                for (int i = 0; i < messageCount; i++)
                {
                    var message = pop3.GetMessage(i);
                    messages.Add(message);
                }

                messages = messages.OrderByDescending(m => m.Date).ToList();

                foreach (var message in messages)
                {
                    string emailContent = $"Subject: {message.Subject}\n";
                    emailContent += $"From: {message.From}\n";
                    emailContent += $"Date: {message.Date}\n";
                    emailContent += $"Body: {message.TextBody ?? ""}\n";

                    List<MimeEntity> attachments = new List<MimeEntity>();

                    foreach (var attachment in message.Attachments)
                    {
                        attachments.Add(attachment);
                    }

                    emailsList.Add((emailContent, attachments));
                }
                Console.WriteLine(emailsList);
                // pop3.Disconnect();
            }

            return emailsList;
        }

    }
}
