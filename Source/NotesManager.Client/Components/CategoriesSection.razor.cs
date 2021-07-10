using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using MudBlazor;
using NotesManager.Client.Data.Models;
using NotesManager.Client.Events.Categories;
using NotesManager.Client.Services;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace NotesManager.Client.Components
{
    public partial class CategoriesSection : ComponentBase
    {
        #region Protected Members

        protected string _searchText = string.Empty;
        protected bool _addCategoryError = false;
        protected string _addCategoryErrorText = string.Empty;
        protected string _newCategoryName = string.Empty;
        protected bool _addingCategory = false;

        protected ICollection<Category> _categoriesBackingField = new List<Category>();

        [Inject]
        protected ICategoryService CategoryService { get; set; }

        [Inject]
        protected ISnackbar SnackBar{ get; set; }

        #endregion

        #region Public Properties

        [Parameter]
        public ICollection<Category> Categories { get; set; }

        [Parameter]
        public Category SelectedCategory { get; set; }

        #endregion

        #region OnInitialized

        protected override void OnInitialized()
        {
            _categoriesBackingField = Categories;

            base.OnInitialized();
        }

        #endregion

        #region Handlers

        protected void HandleKeyPressedOnSearch(KeyboardEventArgs args)
        {
            if (args.Key == "Enter") FilterCategories(_searchText);
        }

        protected void HandleLostFocus() => FilterCategories(_searchText);

        protected async Task HandleKeyPressedOnAddCategory(KeyboardEventArgs args)
        {
            if (args.Key == "Enter") await AddCategory();
        }

        #endregion

        #region Protected Methods

        protected async Task AddCategory()
        {
            ResetAddCategoryError();

            _addingCategory = true;

            if (string.IsNullOrEmpty(_newCategoryName))
            {
                _addCategoryError = true;
                _addCategoryErrorText = "Name can't be empty";
                _addingCategory = false;
                StateHasChanged();
                return;
            }

            var result = await CategoryService.AddCategoryAsync(new Category
            {
                Name = _newCategoryName.Trim()
            });

            
            if (!result.Success)
            {
                _addCategoryError = true;
                _addCategoryErrorText = result.Message;
                _addingCategory = false;
                StateHasChanged();
                return;
            }

            SnackBar.Add(result.Message, Severity.Success);

            if (OnCategoryAdded.HasDelegate)
                await OnCategoryAdded.InvokeAsync(new CategoryAddedEventArgs(result.Data));

            _addingCategory = false;
            _newCategoryName = string.Empty;
        }

        #endregion

        #region Help Methods

        protected void FilterCategories(string newText)
        {
            if (string.IsNullOrEmpty(newText))
            {
                _categoriesBackingField = Categories;
            }
            else
            {
                _categoriesBackingField = Categories.Where(c => c.Name.Contains(newText)).ToList();
            }

            StateHasChanged();
        }

        protected void ResetAddCategoryError()
        {
            _addCategoryError = false;
            _addCategoryErrorText = string.Empty;
        }

        #endregion
    }
}
