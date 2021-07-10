using System;

namespace NotesManager.Client.Events.Categories
{
    public class CategoryDeletedEventArgs : EventArgs
    {
        public string DeletedCategoryId { get; set; }

        public CategoryDeletedEventArgs() { }

        public CategoryDeletedEventArgs(string deletedCategoryId)
        {
            DeletedCategoryId = deletedCategoryId;
        }
    }
}
