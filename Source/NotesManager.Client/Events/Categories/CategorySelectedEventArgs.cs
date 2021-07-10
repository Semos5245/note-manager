using NotesManager.Client.Data.Models;
using System;

namespace NotesManager.Client.Events.Categories
{
    public class CategorySelectedEventArgs : EventArgs
    {
        public Category SelectedCategory { get; }

        public CategorySelectedEventArgs() { }
        public CategorySelectedEventArgs(Category selectedCategory)
        {
            SelectedCategory = selectedCategory;
        }
    }
}
