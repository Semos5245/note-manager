namespace NotesManager.Client.Data.Models
{
    public class TextFileAdditionRequest
    {
        public string FileName { get; set; }
        public string Content { get; set; }
        public string CategoryId { get; set; }
        public RequestAction Action { get; set; }

        public TextFileAdditionRequest() { }
        public TextFileAdditionRequest(string filename, string categoryId, RequestAction action = RequestAction.Add)
        {
            FileName = filename;
            CategoryId = categoryId;
            Action = action;
        }
    }
}
