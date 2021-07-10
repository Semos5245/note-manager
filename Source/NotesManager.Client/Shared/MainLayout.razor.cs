using Microsoft.AspNetCore.Components;
using MudBlazor;
using NotesManager.Client.Constants;
using System.Threading.Tasks;

namespace NotesManager.Client.Shared
{
    public partial class MainLayout : LayoutComponentBase
    {
        [Inject]
        protected NavigationManager NavigationManager { get; set; }

        protected MudTheme _defaultTheme = new MudTheme() { Palette = new Palette { Black = "#272c34" } };
        protected MudTheme _currentTheme;
        protected MudTheme _darkModeTheme = new MudTheme
        {
            Palette = new Palette()
            {
                Black = "#27272f",
                Background = "#32333d",
                BackgroundGrey = "#27272f",
                Surface = "#373740",
                DrawerBackground = "#27272f",
                DrawerText = "rgba(255,255,255, 0.50)",
                AppbarBackground = "#27272f",
                AppbarText = "rgba(255,255,255, 0.70)",
                TextPrimary = "rgba(255,255,255, 0.70)",
                TextSecondary = "rgba(255,255,255, 0.50)",
                ActionDefault = "#adadb1",
                ActionDisabled = "rgba(255,255,255, 0.26)",
                ActionDisabledBackground = "rgba(255,255,255, 0.12)",
                DrawerIcon = "rgba(255,255,255, 0.50)"
            }
        };

        protected override void OnInitialized()
        {
            base.OnInitialized();

            _currentTheme = _defaultTheme;
        }

        void ToggleThemeMode() => _currentTheme = _currentTheme == _defaultTheme ? _darkModeTheme : _defaultTheme;

        protected async Task Logout() => await Task.CompletedTask;
    }
}
