using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using System.Threading.Tasks;

namespace NotesManager.Client.Components
{
    public partial class TopSearchBox : ComponentBase
    {
        protected string _searchText;

        [Parameter]
        public EventCallback<string> OnEnterPressed { get; set; }

        [Parameter]
        public EventCallback<string> OnSearchTextChanged { get; set; }

        protected async Task HandleKeyPressed(KeyboardEventArgs args)
        {
            if (args.Key == "Enter" && OnEnterPressed.HasDelegate)
                await OnEnterPressed.InvokeAsync(_searchText);
        }

        protected async Task HandleLostFocus()
        {
            if (OnSearchTextChanged.HasDelegate)
                await OnSearchTextChanged.InvokeAsync(_searchText);
        }
    }
}
