using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;
using MudBlazor;
using NotesManager.Client.Data.Models;
using NotesManager.Client.Services;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;

namespace NotesManager.Client.Components
{
    public partial class UploadFileItem : ComponentBase
    {
        #region Protected Members

        protected bool _error = false;
        protected string _errorText = string.Empty;
        protected bool _categoryError = false;
        protected string _categoryErrorText = string.Empty;
        protected bool _canChooseAddFile = true;

        #endregion

        #region Protected Properties

        [Parameter] public List<Category> Categories { get; set; }

        [Inject]
        public ISnackbar SnackBar { get; set; }

        [Inject]
        public ITextFileService TextFileService { get; set; }

        [Parameter]
        public IBrowserFile UploadedFile { get; set; }

        public TextFileAdditionRequest AdditionRequest { get; set; }

        #endregion

        #region OnParametersSetAsync

        protected override async Task OnParametersSetAsync()
        {
            await base.OnParametersSetAsync();

            AdditionRequest = new TextFileAdditionRequest
            {
                FileName = Path.GetFileNameWithoutExtension(UploadedFile.Name)
            };

            using var stream = new StreamReader(UploadedFile.OpenReadStream());
            AdditionRequest.Content = await stream.ReadToEndAsync();
        }

        #endregion

        #region Handlers

        protected async Task HandleFileNameChanged(string newName)
        {
            AdditionRequest.FileName = newName;

            await ChangeRequestActionsIfRequired();
        }

        protected async Task HandleCatgoryValueChanged(string newText)
        {
            AdditionRequest.CategoryId = newText;

            await ChangeRequestActionsIfRequired();
        }

        #endregion

        #region Help Methods

        public bool IsReadyToBeImported()
        {
            return !string.IsNullOrEmpty(AdditionRequest.CategoryId) && !_error && !_categoryError;
        }

        protected async Task ChangeRequestActionsIfRequired()
        {
            if (!string.IsNullOrEmpty(AdditionRequest.CategoryId))
            {
                var existsResult = await TextFileService.ExistsWithinCategoryAsync(AdditionRequest.FileName, AdditionRequest.CategoryId);

                if (!existsResult.Success)
                {
                    SnackBar.Add(existsResult.Message, Severity.Error);
                    return;
                }

                switch (existsResult.Data, _canChooseAddFile)
                {
                    case (true, true):
                        _canChooseAddFile = false;
                        AdditionRequest.Action = RequestAction.Append;
                        break;

                    case (false, false):
                        _canChooseAddFile = true;
                        AdditionRequest.Action = RequestAction.Add;
                        break;

                    default:
                        break;
                }
            }
        }

        #endregion
    }
}

