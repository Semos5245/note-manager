using NotesManager.Client.Data.Models;

namespace NotesManager.Client.Services
{
    public class ServiceResponse
    {
        public bool Success { get; }
        public string Message { get; }

        public ServiceResponse(bool success = true, string message = "") => (Success, Message) = (success, message);
    }

    public class ServiceResponse<T> : ServiceResponse
    {
        public T Data { get; }

        public ServiceResponse(bool success = true, string message = "", T data = default) 
            : base(success, message) => Data = data;
    }

    public class TextFileByContent
    {
        public TextFile TextFile { get; set; }
        public string DisplayText { get; }
        public int HighlightStartIndex { get; }
        public int HighlightEndIndex { get; }
        public int StartIndex { get; }
        public int EndIndex { get; }

        public TextFileByContent(TextFile textFile, string displayText,
            int startIndex, int endIndex, int highlightStartIndex, int highlightEndIndex)
            => (TextFile, DisplayText, StartIndex, EndIndex, HighlightStartIndex, HighlightEndIndex)
            = (textFile, displayText, startIndex, endIndex, highlightStartIndex, highlightEndIndex);
    }
}
