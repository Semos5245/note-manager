using Microsoft.AspNetCore.Components;
using NotesManager.Client.Events.TextFiles;
using NotesManager.Client.Services;
using System.Threading.Tasks;

namespace NotesManager.Client.Components
{
    public partial class FileByContentRow : ComponentBase
    {
        #region Protected Properties

        [Parameter]
        public TextFileByContent FilteredTextFile { get; set; }

        [Parameter]
        public bool IsSelected { get; set; }

        [Parameter]
        public string SearchText { get; set; }

        #endregion

        #region Event Callbacks

        [Parameter]
        public EventCallback<TextFileByContentSelectedEventArgs> OnTextFileByContentSelected { get; set; }

        #endregion

        #region Handlers

        protected async Task HandleTextFileSelected()
        {
            if (OnTextFileByContentSelected.HasDelegate)
                await OnTextFileByContentSelected.InvokeAsync(new TextFileByContentSelectedEventArgs(FilteredTextFile));
        }

        #endregion
    }
}
