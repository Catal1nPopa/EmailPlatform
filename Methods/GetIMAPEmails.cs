using MailKit.Net.Imap;
using MailKit;
using MimeKit;
using EmailApp.Entities;
using System.Text;

namespace EmailApp.Methods
{
    public class GetIMAPEmails
    {
        public static async Task<List<(string, (string, byte[])[])>> GetEmailsIMAP(Credentials credentials)
        {
            string host = "imap.gmail.com";
            int port = 993;
            bool useSsl = true;

            List<(string, (string, byte[])[])> emailsList = new List<(string, (string, byte[])[])>();

            using (ImapClient imap = new ImapClient())
            {
                imap.Connect(host, port, useSsl); 
                imap.Authenticate(credentials.Email, credentials.Password); 

                imap.Inbox.Open(FolderAccess.ReadOnly); 
                var messages = imap.Inbox.Fetch(0, -1, MessageSummaryItems.Full | MessageSummaryItems.UniqueId)
                                       .OrderByDescending(m => m.Date)
                                       .ToList();

                foreach (var message in messages)
                {
                    string emailContent = $"Subject: {message.Envelope.Subject}\n";
                    emailContent += $"From: {message.Envelope.From}\n";
                    emailContent += $"Date: {message.Date}\n";

                    var fullMessage = imap.Inbox.GetMessage(message.UniqueId);

                    emailContent += $"Body: {fullMessage.TextBody ?? ""}\n";

                    List<(string, byte[])> attachments = new List<(string, byte[])>();

                    foreach (var attachment in fullMessage.Attachments)
                    {
                        if (attachment is MimePart mimePart)
                        {
                            using (var memoryStream = new MemoryStream())
                            {
                                mimePart.Content.DecodeTo(memoryStream);
                                attachments.Add((mimePart.FileName ?? "Unknown", memoryStream.ToArray()));
                            }
                        }
                    }

                    emailsList.Add((emailContent, attachments.ToArray()));
                }
            }

            return emailsList;
        }
        ////
        ///
        public static async Task<List<(string, (string, byte[])[])>> GetEmailsIMAP2(Credentials credentials)
        {
            string host = "imap.gmail.com";
            int port = 993;
            bool useSsl = true;

            List<(string, (string, byte[])[])> emailsList = new List<(string, (string, byte[])[])>();

            using (ImapClient imap = new ImapClient())
            {
                imap.Connect(host, port, useSsl);
                imap.Authenticate(credentials.Email, credentials.Password);

                imap.Inbox.Open(FolderAccess.ReadOnly);
                var messages = imap.Inbox.Fetch(0, -1, MessageSummaryItems.Full | MessageSummaryItems.UniqueId)
                                       .OrderByDescending(m => m.Date)
                                       .ToList();

                foreach (var message in messages)
                {
                    string emailContent = $"Subject: {message.Envelope.Subject}\n";
                    emailContent += $"From: {message.Envelope.From}\n";
                    emailContent += $"Date: {message.Date}\n";

                    var fullMessage = imap.Inbox.GetMessage(message.UniqueId);

                    emailContent += $"Body: {fullMessage.TextBody ?? ""}\n";

                    List<(string, byte[])> attachments = new List<(string, byte[])>();

                    foreach (var attachment in fullMessage.Attachments)
                    {
                        if (attachment is MimePart mimePart)
                        {
                            using (var memoryStream = new MemoryStream())
                            {
                                mimePart.Content.DecodeTo(memoryStream);
                                byte[] attachmentData = memoryStream.ToArray();

                                if (IsVideoAttachment(mimePart))
                                {
                                    string fileName = mimePart.FileName ?? "Unknown";
                                    string filePath = SaveVideoAttachment(fileName, attachmentData);

                                    attachments.Add((fileName, Encoding.UTF8.GetBytes(filePath)));
                                }
                                else
                                {
                                    attachments.Add((mimePart.FileName ?? "Unknown", attachmentData));
                                }
                            }
                        }
                    }

                    emailsList.Add((emailContent, attachments.ToArray()));
                }
            }

            return emailsList;
        }

     
        private static bool IsVideoAttachment(MimePart attachment)
        {
           
            string fileName = attachment.FileName ?? "";
            string extension = Path.GetExtension(fileName).ToLower();

            return extension == ".mp4" || extension == ".mov" || extension == ".avi" || extension == ".mkv";
        }

 
        private static string SaveVideoAttachment(string fileName, byte[] data)
        {
            string directoryPath = "VideoAttachments"; 
            string filePath = Path.Combine(directoryPath, fileName);

        
            if (!Directory.Exists(directoryPath))
            {
                Directory.CreateDirectory(directoryPath);
            }

            File.WriteAllBytes(filePath, data);

            return filePath;
        }


    }
}
