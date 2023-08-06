using CourseManagementSystem.Models;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Server.ProtectedBrowserStorage;

namespace CourseManagementSystem.Shared
{
    public partial class NavMenu
    {
        [Inject]
        private ProtectedSessionStorage ProtectedSessionStore { get; set; }
        private UserModel? _user = null;
        private bool _collapseNavMenu = true;
        private string? _navMenuCssClass => _collapseNavMenu ? "collapse" : null;

        private void ToggleNavMenu()
        {
            _collapseNavMenu = !_collapseNavMenu;
        }

        protected override async Task OnAfterRenderAsync(bool firstRender)
        {
            var sessionResult = await ProtectedSessionStore.GetAsync<UserModel>("cms_access_token");
            _user = sessionResult.Success ? sessionResult.Value : null;
            if (_user is not null) StateHasChanged();
        }
    }
}
