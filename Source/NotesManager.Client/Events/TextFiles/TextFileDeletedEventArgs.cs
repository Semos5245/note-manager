using System;

namespace NotesManager.Client.Events.TextFiles
{
    public class TextFileDeletedEventArgs : EventArgs
    {
        public string DeletedTextFileId { get; }

        public TextFileDeletedEventArgs() { }
        public TextFileDeletedEventArgs(string deletedTextFileId) => DeletedTextFileId = deletedTextFileId;
    }
}
