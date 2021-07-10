using System.Collections.Generic;

namespace NotesManager.Client.Data.Models
{
    public class Category : BaseEntity
    {
        public Category()
        {
            TextFiles = new HashSet<TextFile>();
        }

        public string Name { get; set; }
        public virtual ICollection<TextFile> TextFiles { get; set; }
    }
}
