using NotesManager.Client.Data.Models;
using System;

namespace NotesManager.Client.Events.TextFiles
{
    public class TextFileSelectedEventArgs : EventArgs
    {
        public TextFile SelectedTextFile { get; }

        public TextFileSelectedEventArgs() { }
        public TextFileSelectedEventArgs(TextFile selectedTextFile) => SelectedTextFile = selectedTextFile;
    }
}
