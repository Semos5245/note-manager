using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using MudBlazor;
using NotesManager.Client.Data.Models;
using NotesManager.Client.Events.TextFiles;
using NotesManager.Client.Services;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace NotesManager.Client.Components
{
    public partial class TextFilesSection : ComponentBase
    {
        #region Protected Members

        protected string _searchText = string.Empty;
        protected bool _addTextFileError = false;
        protected string _addTextFileErrorText = string.Empty;
        protected string _newTextFileName = string.Empty;
        protected bool _addingTextFile = false;

        protected ICollection<TextFile> _textFilesBackingField = new List<TextFile>();

        [Inject]
        protected ITextFileService TextFileService { get; set; }

        [Inject]
        protected ISnackbar SnackBar { get; set; }

        #endregion

        #region Public Properties

        [Parameter]
        public ICollection<TextFile> TextFiles { get; set; }

        [Parameter]
        public TextFile SelectedTextFile { get; set; }

        [Parameter]
        public Category SelectedCategory { get; set; }

        #endregion

        #region OnInitialized

        protected override void OnInitialized()
        {
            _textFilesBackingField = TextFiles;

            base.OnInitialized();
        }

        #endregion

        #region Handlers

        protected void HandleKeyPressedOnSearch(KeyboardEventArgs args)
        {
            if (args.Key == "Enter") FilterTextFiles(_searchText);
        }

        protected void HandleLostFocus() => FilterTextFiles(_searchText);

        protected async Task HandleKeyPressedOnAddTextFile(KeyboardEventArgs args)
        {
            if (args.Key == "Enter") await AddTextFile();
        }

        #endregion

        #region Protected Methods

        protected async Task AddTextFile()
        {
            ResetAddTextFileError();

            _addingTextFile = true;

            if (SelectedCategory is null)
            {
                _addTextFileError = true;
                _addTextFileErrorText = "A category should be selected";
                _addingTextFile = false;
                StateHasChanged();
                return;
            }

            if (string.IsNullOrEmpty(_newTextFileName))
            {
                _addTextFileError = true;
                _addTextFileErrorText = "Name can't be empty";
                _addingTextFile = false;
                StateHasChanged();
                return;
            }

            var result = await TextFileService.AddTextFileAsync(new TextFile
            {
                FileName = _newTextFileName.Trim(),
                CategoryId = SelectedCategory.Id
            });

            if (!result.Success)
            {
                _addTextFileError = true;
                _addTextFileErrorText = result.Message;
                _addingTextFile = false;
                StateHasChanged();
                return;
            }

            SnackBar.Add(result.Message, Severity.Success);

            if (OnTextFileAdded.HasDelegate)
                await OnTextFileAdded.InvokeAsync(new TextFileAddedEventArgs(result.Data));

            _addingTextFile = false;
            _newTextFileName = string.Empty;
        }

        #endregion

        #region Help Methods

        protected void FilterTextFiles(string newText)
        {
            if (string.IsNullOrEmpty(newText))
            {
                _textFilesBackingField = TextFiles;
            }
            else
            {
                _textFilesBackingField = TextFiles.Where(c => c.FileName.Contains(newText)).ToList();
            }

            StateHasChanged();
        }

        protected void ResetAddTextFileError()
        {
            _addTextFileError = false;
            _addTextFileErrorText = string.Empty;
        }

        #endregion
    }
}
