using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;
using MudBlazor;
using NotesManager.Client.Components;
using NotesManager.Client.Data.Models;
using NotesManager.Client.Extensions;
using NotesManager.Client.Services;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace NotesManager.Client.Pages
{
    public partial class ImportFiles : ComponentBase
    {
        #region Protected Members

        protected List<Category> _categories;
        protected IReadOnlyList<IBrowserFile> _uploadedFiles;
        protected string _errorText = string.Empty;
        protected UploadFileItem[] _uploadFileItems;
        protected bool _processing = false;

        #endregion

        #region Protected Properties

        protected bool AlreadyUploadedFiles => _uploadedFiles?.Any() ?? false;

        [Inject]
        protected ITextFileService TextFileService { get; set; }
        
        [Inject]
        protected ICategoryService CategoryService { get; set; }

        [Inject]
        protected ISnackbar SnackBar { get; set; }

        #endregion

        #region OnParametersSetAsync

        protected override async Task OnParametersSetAsync()
        {
            await base.OnParametersSetAsync();
            _categories = (await CategoryService.GetAllCategoriesAsync()).Data.ToList();
        }

        #endregion

        #region Handlers

        private void HandleInputFileChanged(InputFileChangeEventArgs e)
        {
            _uploadedFiles = e.GetMultipleFiles();
            _uploadFileItems = new UploadFileItem[_uploadedFiles.Count];
        }

        protected async Task HandleImportClicked()
        {
            _processing = true;

            _errorText = string.Empty;

            var allReadyToImport = _uploadFileItems?.All(i => i.IsReadyToBeImported()) ?? false;

            if (!allReadyToImport)
            {
                _errorText = "Imported files have some errors";
                _processing = false;
                return;
            }

            // Check if filenames are all different
            var hasDuplicates = _uploadFileItems.GroupBy(item => item.AdditionRequest.CategoryId, item => item.AdditionRequest.FileName)
                .ToDictionary(pair => pair.Key, pair => pair.ToList()).Any(pair => pair.Value.HasDuplicates());

            if (hasDuplicates)
            {
                _errorText = "There are duplicates in the file names";
                _processing = false;
                return;
            }

            var uploadFileItems = _uploadFileItems.Select(i => i.AdditionRequest).ToList();

            var result = await TextFileService.ImportTextFilesAsync(uploadFileItems);

            if (!result.Success)
            {
                SnackBar.Add(result.Message, Severity.Error);
                _processing = false;
                return;
            }

            await Task.Delay(2000);

            SnackBar.Add(result.Message, result.Data > 0 ? Severity.Warning : Severity.Success);

            _processing = false;

            Reset();
        }

        #endregion

        #region Helper Methods

        protected void Reset()
        {
            _uploadedFiles = null;
            _uploadFileItems = null;
        }

        #endregion
    }
}
