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
                                    // Salvați conținutul videoclipului pe server sau într-o locație specifică
                                    string fileName = mimePart.FileName ?? "Unknown";
                                    string filePath = SaveVideoAttachment(fileName, attachmentData);

                                    // Trimiteți URL-ul videoclipului către client
                                    attachments.Add((fileName, Encoding.UTF8.GetBytes(filePath)));
                                }
                                else
                                {
                                    // Adăugați atașamentul în mod normal
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

        // Funcție pentru a verifica dacă un atașament este un videoclip
        private static bool IsVideoAttachment(MimePart attachment)
        {
            // Aici puteți adăuga condițiile necesare pentru a identifica un videoclip
            // De exemplu, puteți verifica tipul MIME al atașamentului sau extensia fișierului
            // În acest exemplu, vom verifica extensia fișierului pentru simplitate
            string fileName = attachment.FileName ?? "";
            string extension = Path.GetExtension(fileName).ToLower();

            return extension == ".mp4" || extension == ".mov" || extension == ".avi" || extension == ".mkv";
        }

        // Funcție pentru a salva conținutul videoclipului și returnarea URL-ului acestuia
        private static string SaveVideoAttachment(string fileName, byte[] data)
        {
            string directoryPath = "VideoAttachments"; // Directorul în care vor fi stocate videoclipurile
            string filePath = Path.Combine(directoryPath, fileName);

            // Asigurați-vă că directorul există
            if (!Directory.Exists(directoryPath))
            {
                Directory.CreateDirectory(directoryPath);
            }

            // Salvați conținutul videoclipului în fișier
            File.WriteAllBytes(filePath, data);

            // Returnați URL-ul videoclipului
            return filePath;
        }


    }
}
