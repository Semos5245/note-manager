using NotesManager.Client.Data.Models;
using System;

namespace NotesManager.Client.Events.TextFiles
{
    public class TextFileAddedEventArgs : EventArgs
    {
        public TextFile AddedTextFile { get; }

        public TextFileAddedEventArgs() { }
        public TextFileAddedEventArgs(TextFile addedTextFile) => AddedTextFile = addedTextFile;
    }
}
