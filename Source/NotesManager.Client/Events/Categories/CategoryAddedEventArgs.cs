using NotesManager.Client.Data.Models;
using System;

namespace NotesManager.Client.Events.Categories
{
    public class CategoryAddedEventArgs : EventArgs
    {
        public Category AddedCategory { get; }

        public CategoryAddedEventArgs() { }
        public CategoryAddedEventArgs(Category addedCategory)
        {
            AddedCategory = addedCategory;
        }
    }
}
