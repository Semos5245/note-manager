using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using MudBlazor;
using NotesManager.Client.Data.Models;
using NotesManager.Client.Events.Categories;
using NotesManager.Client.Services;
using System;
using System.Threading.Tasks;

namespace NotesManager.Client.Components
{
    public partial class CategoryTableRow : ComponentBase
    {
        #region Protected Members

        protected bool _inEditMode = false;
        protected string _newName = string.Empty;
        protected bool _error = false;
        protected string _errorText = string.Empty;
        protected bool _editingCategory = false;
        protected bool _deletingCategory = false;

        #endregion

        #region Properties

        [Inject]
        protected ICategoryService CategoryService { get; set; }

        [Inject]
        protected ISnackbar SnackBar { get; set; }

        [Inject]
        public IDialogService _dialogService { get; set; }

        [Parameter]
        public Category Category { get; set; }
        
        [Parameter]
        public bool IsSelected { get; set; }

        #endregion

        #region OnParametersSet

        protected override void OnParametersSet()
        {
            base.OnParametersSet();

            _newName = Category?.Name;
        }

        #endregion

        #region Handlers

        protected async Task HandleCategorySelected()
        {
            if (OnCategorySelected.HasDelegate)
                await OnCategorySelected.InvokeAsync(new CategorySelectedEventArgs(Category));
        }

        protected async Task HandleDeleteButtonClicked()
        {
            ResetError();

            _deletingCategory = true;

            var result = await CategoryService.GetNumberOfRelatedFilesAsync(Category.Id);

            if (!result.Success) return;

            var relatedFiles = result.Data;

            var title = "Delete category";
            var message = relatedFiles > 0 ?
                $"Category has {relatedFiles} related file{(relatedFiles > 1 ? "s" : "")}{Environment.NewLine}" +
                $"Deleting the category will delete all related files" : $"Category has no related files";

            var confirmationCallback = EventCallback.Factory.Create(this, HandleDeleteConfirmed);
            var cancelCallback = EventCallback.Factory.Create(this, () => _deletingCategory = false);

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
            var result = await CategoryService.DeleteCategoryAsync(Category.Id, true);

            if (!result.Success)
            {
                // Show error message
                SnackBar.Add(result.Message, Severity.Error);
                _deletingCategory = false;
                return;
            }

            SnackBar.Add(result.Message, Severity.Success);

            _deletingCategory = false;

            // If everything is ok then just send to the parent that the category has been deleted
            if (OnCategoryDeleted.HasDelegate)
                await OnCategoryDeleted.InvokeAsync(new CategoryDeletedEventArgs(Category.Id));

        }

        protected async Task HandleKeyPressed(KeyboardEventArgs args)
        {
            if (args.Key == "Enter") await EditCategory();
        }

        protected void HandleLostFocus()
        {
            _inEditMode = false;
        }

        #endregion

        #region Protected Methods

        protected async Task EditCategory()
        {
            ResetError();

            _editingCategory = true;

            // Do all the editing stuff here
            var result = await CategoryService.EditCategoryAsync(new Category
            {
                Id = Category.Id,
                Name = _newName.Trim()
            });

            if (!result.Success)
            {
                _errorText = result.Message;
                _error = true;
                _editingCategory = false;

                StateHasChanged();

                return;
            }

            Category = result.Data;

            SnackBar.Add(result.Message, Severity.Success);

            // If everything is ok then just notify the parent that the category has been edited
            if (OnCategoryEdited.HasDelegate)
                await OnCategoryEdited.InvokeAsync(new CategoryEditedEventArgs(Category.Id, Category.Name, _newName));

            _inEditMode = false;
            _newName = Category.Name;

            _editingCategory = false;
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
