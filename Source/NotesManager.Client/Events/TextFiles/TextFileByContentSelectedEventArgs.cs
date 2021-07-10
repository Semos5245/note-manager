using NotesManager.Client.Services;
using System;

namespace NotesManager.Client.Events.TextFiles
{
    public class TextFileByContentSelectedEventArgs : EventArgs
    {
        public TextFileByContent TextFileByContent { get; }

        public TextFileByContentSelectedEventArgs() { }
        public TextFileByContentSelectedEventArgs(TextFileByContent textFileByContent) => TextFileByContent = textFileByContent;
    }
}
