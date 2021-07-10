using System;

namespace NotesManager.Client.Data.Models
{
    public class TextFile : BaseEntity
    {
        public TextFile()
        {
            CreationDateUtc = DateTime.UtcNow;
            ModificationDateUtc = DateTime.UtcNow;
        }

        public string FileName { get; set; }
        public string Content { get; set; }
        public string CategoryId { get; set; }
        public DateTime CreationDateUtc { get; set; }
        public DateTime ModificationDateUtc { get; set; }

        public virtual Category Category { get; set; }
    }
}
