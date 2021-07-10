using Microsoft.AspNetCore.Components;
using NotesManager.Client.Events.TextFiles;
using NotesManager.Client.Services;
using System.Collections.Generic;

namespace NotesManager.Client.Components
{
    public partial class SearchFilesByContentSection : ComponentBase
    {
        #region Protected Properties

        [Parameter]
        public List<TextFileByContent> FilteredFiles { get; set; }

        [Parameter]
        public TextFileByContent SelectedTextFile { get; set; }

        [Parameter]
        public string SearchText { get; set; }

        #endregion

        #region Event Callbacks

        [Parameter]
        public EventCallback<TextFileByContentSelectedEventArgs> TextFileByContentSelected { get; set; }

        #endregion
    }
}
