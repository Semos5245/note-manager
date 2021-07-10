using NotesManager.Client.Data.Models;
using System;

namespace NotesManager.Client.Events.TextFiles
{
    public class TextFileEditedEventArgs : EventArgs
    {
        public TextFile EditedTextFile { get; }

        public TextFileEditedEventArgs() { }
        public TextFileEditedEventArgs(TextFile editedTextFile) => EditedTextFile = editedTextFile;
    }
}
