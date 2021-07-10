using Microsoft.AspNetCore.Components;
using MudBlazor;
using NotesManager.Client.Data.Models;
using NotesManager.Client.Events.Categories;
using NotesManager.Client.Events.TextFiles;
using NotesManager.Client.Services;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace NotesManager.Client.Pages
{
    public partial class MainPage : ComponentBase
    {
        #region Protected Members

        protected List<Category> _categories;
        protected List<TextFile> _currentTextFiles;
        protected List<TextFileByContent> _filteredTextFiles;
        protected Category _selectedCategory;
        protected TextFile _selectedTextFile;
        protected TextFileByContent _selectedTextFileByContent;
        protected bool _inGlobalSearchMode = false;
        protected string _globalSearchText = string.Empty;

        #endregion

        #region Protected Properties

        [Inject]
        protected ICategoryService CategoryService { get; set; }

        [Inject]
        protected ITextFileService TextFileService { get; set; }

        [Inject]
        protected IDialogService DialogService { get; set; }

        [Inject]
        protected ISnackbar SnackBar { get; set; }

        #endregion

        #region OnInitializedAsync

        protected override async Task OnInitializedAsync()
        {
            _categories = (await CategoryService.GetAllCategoriesAsync()).Data.ToList();
            _currentTextFiles = new List<TextFile>();
            await base.OnInitializedAsync();
        }

        #endregion

        #region Handlers

        protected async Task HandleEnterPressedOnTopSearchBox(string searchText)
        {
            _globalSearchText = searchText;
            await SearchFilesByContent(searchText);
        }
        
        protected async Task HandleSearchTextChanged(string searchText)
        {
            _globalSearchText = searchText;
            await SearchFilesByContent(searchText);
        }

        protected async Task HandleCategorySelected(CategorySelectedEventArgs args)
        {
            if (_selectedCategory != args.SelectedCategory)
            {
                _selectedCategory = args.SelectedCategory;

                var result = await CategoryService.GetRelatedFilesAsync(_selectedCategory.Id);

                if (!result.Success)
                {
                    SnackBar.Add(result.Message, Severity.Error);
                    return;
                }

                _currentTextFiles = result.Data.ToList();
                _selectedTextFile = null;
            }
        }

        protected void HandleCategoryEdited(CategoryEditedEventArgs args)
        {
            
        }

        protected void HandleCategoryDeleted(CategoryDeletedEventArgs args)
        {
            _categories.RemoveAll(c => c.Id == args.DeletedCategoryId);

            if (_selectedCategory?.Id == args.DeletedCategoryId)
            {
                _currentTextFiles = new List<TextFile>();
                _selectedTextFile = null;
            }
        }

        protected void HandleCategoryAdded(CategoryAddedEventArgs args)
        {
            _categories.Add(args.AddedCategory);
        }

        protected void HandleTextFileSelected(TextFileSelectedEventArgs args)
        {
            if (_selectedTextFile != args.SelectedTextFile)
                _selectedTextFile = args.SelectedTextFile;
        }

        protected void HandleTextFileAdded(TextFileAddedEventArgs args)
        {
            if (args.AddedTextFile.CategoryId == _selectedCategory?.Id)
                _currentTextFiles.Add(args.AddedTextFile);
        }

        protected void HandleTextFileEdited(TextFileEditedEventArgs args)
        {

        }

        protected void HandleTextFileDeleted(TextFileDeletedEventArgs args)
        {
            if ( _selectedTextFile?.Id == args.DeletedTextFileId)
            {
                _selectedTextFile = null;
            }

            _currentTextFiles.RemoveAll(t => t.Id == args.DeletedTextFileId);
        }

        protected void HandleTextFileByContentSelected(TextFileByContentSelectedEventArgs args)
        {
            if (_selectedTextFileByContent != args.TextFileByContent)
            {
                _selectedTextFileByContent = args.TextFileByContent;
                _selectedTextFile = args.TextFileByContent.TextFile;
            }
        }

        #endregion

        #region Helper Methods

        protected async Task SearchFilesByContent(string searchText)
        {
            if (string.IsNullOrEmpty(searchText))
            {
                await ResetGlobalSearchMode();
                return;
            }

            var result = await TextFileService.SearchTextFilesByContentAsync(searchText);

            _filteredTextFiles = result.Data.ToList();

            _inGlobalSearchMode = true;

            _selectedTextFile = null;
        }

        protected Task ResetGlobalSearchMode()
        {
            _filteredTextFiles = null;
            _selectedTextFile = null;
            _inGlobalSearchMode = false;

            return Task.CompletedTask;
        }

        #endregion
    }
}
