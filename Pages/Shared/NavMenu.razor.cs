using CourseManagementSystem.Models;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Server.ProtectedBrowserStorage;

namespace CourseManagementSystem.Pages.Shared
{
    public partial class NavMenu
    {
        [Inject]
        private ProtectedSessionStorage ProtectedSessionStore { get; set; } = default!;
        private PersonModel? _user = null;
        private bool _collapseNavMenu = true;
        private string? NavMenuCssClass => _collapseNavMenu ? "collapse" : null;

        private void ToggleNavMenu()
        {
            _collapseNavMenu = !_collapseNavMenu;
        }

        protected override async Task OnAfterRenderAsync(bool firstRender)
        {
            var sessionResult = await ProtectedSessionStore.GetAsync<PersonModel>("cms_access_token");
            _user = sessionResult.Success ? sessionResult.Value : null;
            if (_user is not null) StateHasChanged();
        }
    }
}
