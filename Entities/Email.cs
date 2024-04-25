using System.Web.Mvc;

namespace EmailApp.Entities
{
    [Serializable]
    public class Email
    {
        public Email()
        {
            Attachments = new List<Attachments>();
        }
        public int MessageNumber { get; set; }
        [AllowHtml]
        public string From { get; set; }
        [AllowHtml]
        public string Subject { get; set; }
        [AllowHtml]
        public string Body { get; set; }
        public DateTime DateSent { get; set; }
        [AllowHtml]
        public List<Attachments> Attachments { get; set; }
    }
    [Serializable]
    public class Attachments
    {
        public string FileName { get; set; }
        public string ContentType { get; set; }
        public byte[] Content { get; set; }
    }
}
