using System;

namespace NotesManager.Client.Events.Categories
{
    public class CategoryEditedEventArgs : EventArgs
    {
        public string EditedCategoryId { get; set; }
        public string OldName { get; set; }
        public string NewName { get; set; }

        public CategoryEditedEventArgs() { }
        public CategoryEditedEventArgs(string editeCategoryId, string oldName, string newName)
        {
            EditedCategoryId = editeCategoryId;
            OldName = OldName;
            NewName = newName;
        }
    }
}
