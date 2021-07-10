using ElectronNET.API;
using ElectronNET.API.Entities;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.JSInterop;
using MudBlazor;
using NotesManager.Client.Data.Models;
using NotesManager.Client.Events.TextFiles;
using NotesManager.Client.Extensions;
using NotesManager.Client.Services;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NotesManager.Client.Components
{
    public partial class TextFileTableRow : ComponentBase
    {
        #region Protected Members

        protected bool _inEditMode = false;
        protected string _newName = string.Empty;
        protected bool _error = false;
        protected string _errorText = string.Empty;
        protected bool _editingTextFile = false;
        protected bool _deletingTextFile = false;

        #endregion

        #region Properties

        [Inject]
        protected ITextFileService TextFileService { get; set; }

        [Inject]
        protected ISnackbar SnackBar { get; set; }

        [Inject]
        public IDialogService _dialogService { get; set; }

        [Parameter]
        public TextFile TextFile { get; set; }

        [Parameter]
        public bool IsSelected { get; set; }

        [Inject]
        public IJSRuntime JsRuntime { get; set; }

        #endregion

        #region Event Callbacks

        [Parameter]
        public EventCallback<TextFileSelectedEventArgs> OnTextFileSelected { get; set; }

        [Parameter]
        public EventCallback<TextFileEditedEventArgs> OnTextFileEdited { get; set; }

        [Parameter]
        public EventCallback<TextFileDeletedEventArgs> OnTextFileDeleted { get; set; }

        #endregion

        #region OnParametersSet

        protected override void OnParametersSet()
        {
            base.OnParametersSet();

            _newName = TextFile?.FileName;
        }

        #endregion

        #region Handlers

        protected async Task HandleTextFileSelected()
        {
            if (OnTextFileSelected.HasDelegate)
                await OnTextFileSelected.InvokeAsync(new TextFileSelectedEventArgs(TextFile));
        }

        protected async Task HandleDeleteButtonClicked()
        {
            ResetError();

            _deletingTextFile = true;

            var title = "Delete file";
            var message = "File can't be recovered after confirmation!!";

            var confirmationCallback = EventCallback.Factory.Create(this, HandleDeleteConfirmed);
            var cancelCallback = EventCallback.Factory.Create(this, () => _deletingTextFile = false);

            // Show delete confirmation
            var parameters = new DialogParameters
            {
                { "Message", message },
                { "OnConfirmClicked", confirmationCallback },
                { "OnCancelClicked", cancelCallback }
            };

            var options = new DialogOptions
            {
                DisableBackdropClick = true
            };

            await InvokeAsync(() =>
            {
                _dialogService.Show<CustomDialog>(title, parameters, options);
            });
        }

        protected async Task HandleDeleteConfirmed()
        {
            ResetError();

            // Do all the deleting stuff here
            var result = await TextFileService.DeleteTextFileAsync(TextFile.Id);

            if (!result.Success)
            {
                // Show error message
                SnackBar.Add(result.Message, Severity.Error);
                _deletingTextFile = false;
                return;
            }

            SnackBar.Add(result.Message, Severity.Success);

            _deletingTextFile = false;

            // If everything is ok then just send to the parent that the textFile has been deleted
            if (OnTextFileDeleted.HasDelegate)
                await OnTextFileDeleted.InvokeAsync(new TextFileDeletedEventArgs(TextFile.Id));
        }

        protected async Task HandleKeyPressed(KeyboardEventArgs args)
        {
            if (args.Key == "Enter") await EditTextFile();
        }

        protected void HandleLostFocus()
        {
            _inEditMode = false;
        }

        protected async Task HandleExportButtonClicked()
        {
            if (HybridSupport.IsElectronActive)
            {
                var options = new SaveDialogOptions
                {
                    Title = "Choose path and filename to save your file",
                    Filters = new FileFilter[]
                    {
                        new FileFilter{ Name = "Text files", Extensions = new[] { "txt" } }
                    }
                };

                var mainWindow = Electron.WindowManager.BrowserWindows.First();

                var filePath = await Electron.Dialog.ShowSaveDialogAsync(mainWindow, options);

                if (string.IsNullOrEmpty(filePath)) return;

                File.WriteAllText(filePath, TextFile.Content);

                SnackBar.Add("File exported", Severity.Success);

                return;
            }

            using (var memoryStream = new MemoryStream(Encoding.UTF8.GetBytes(TextFile.Content)))
            {
                await JsRuntime.SaveAs(TextFile.FileName + ".txt", memoryStream.ToArray());
            }
        }

        #endregion

        #region Protected Methods

        protected async Task EditTextFile()
        {
            ResetError();

            _editingTextFile = true;

            // Do all the editing stuff here
            var result = await TextFileService.EditTextFileNameAsync(TextFile.Id, _newName.Trim());

            if (!result.Success)
            {
                _errorText = result.Message;
                _error = true;
                _editingTextFile = false;

                StateHasChanged();

                return;
            }

            TextFile = result.Data;

            SnackBar.Add(result.Message, Severity.Success);

            // If everything is ok then just notify the parent that the textFile has been edited
            if (OnTextFileEdited.HasDelegate)
                await OnTextFileEdited.InvokeAsync(new TextFileEditedEventArgs(TextFile));

            _inEditMode = false;
            _newName = TextFile.FileName;

            _editingTextFile = false;
        }

        #endregion

        #region Helper Methods

        protected void GoToEditMode()
        {
            _inEditMode = true;
            StateHasChanged();
        }

        protected void ResetError()
        {
            _error = false;
            _errorText = string.Empty;
        }

        #endregion
    }
}
