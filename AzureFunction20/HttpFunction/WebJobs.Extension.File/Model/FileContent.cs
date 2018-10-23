using System;

namespace WebJobs.Extension.File
{
    public class FileContent
    {
        public string FileName { get; set; }
        public string Content { get; set; }
        public Guid MessageID { get; }
        public FileContent() => MessageID = Guid.NewGuid();
    }
}
