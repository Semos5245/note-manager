using ElectronNET.API;
using ElectronNET.API.Entities;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.JSInterop;
using MudBlazor;
using NotesManager.Client.Data.Models;
using NotesManager.Client.Extensions;
using NotesManager.Client.Services;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NotesManager.Client.Components
{
    public partial class TextEditorSection : ComponentBase
    {
        #region Protected Members

        protected string _lastSavedContent = string.Empty;

        #endregion

        #region Properties

        [Parameter]
        public TextFile SelectedTextFile { get; set; }

        [Inject]
        public ISnackbar SnackBar { get; set; }

        [Inject]
        public ITextFileService TextFileService { get; set; }

        [Inject]
        public IJSRuntime JsRuntime { get; set; }
        #endregion

        #region OnParametersSet

        protected override async Task OnParametersSetAsync()
        {
            await base.OnParametersSetAsync();

            _lastSavedContent = (await TextFileService.GetTextFileAsync(SelectedTextFile.Id))
                .Data.Content;
        }

        #endregion

        #region Handlers

        protected async Task HandleKeyPressed(KeyboardEventArgs args)
        {
            if (args.Key == "S" && args.CtrlKey)
                await HandleSavePressed();
        }

        protected async Task HandleSavePressed()
        {
            var result = await TextFileService.SaveTextFileContentAsync(SelectedTextFile.Id, SelectedTextFile.Content);

            if (!result.Success)
            {
                SnackBar.Add(result.Message, Severity.Error);
                return;
            }

            SelectedTextFile.Content = result.Data.Content;
            SelectedTextFile.ModificationDateUtc = result.Data.ModificationDateUtc;
            _lastSavedContent = SelectedTextFile.Content;
        }

        protected async Task HandleExportClicked()
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

                File.WriteAllText(filePath, SelectedTextFile.Content);

                SnackBar.Add("File exported", Severity.Success);

                return;
            }

            using (var memoryStream = new MemoryStream(Encoding.UTF8.GetBytes(SelectedTextFile.Content)))
            {
                await JsRuntime.SaveAs(SelectedTextFile.FileName + ".txt", memoryStream.ToArray());
            }
        }

        protected void HandleTextChanged(string newText)
        {
            SelectedTextFile.Content = newText;
            StateHasChanged();
        }

        #endregion

        #region HelpMethods

        protected bool DidContentChange() => _lastSavedContent != SelectedTextFile.Content;

        #endregion
    }
}
