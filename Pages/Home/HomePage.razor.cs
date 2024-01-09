using CourseManagementSystem.Models;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Server.ProtectedBrowserStorage;

namespace CourseManagementSystem.Pages.Home
{
    public partial class HomePage
    {
        [Inject]
        private ProtectedSessionStorage ProtectedSessionStore { get; set; } = default!;
        [Inject]
        private NavigationManager? Navigation { get; set; }
        private PersonModel? _user;
        private readonly PasswordChangeModel _passwords = new();
        private string _errorMessage = "";

        protected override async Task OnAfterRenderAsync(bool firstRender)
        {
            // Authentication
            var sessionResult = await ProtectedSessionStore.GetAsync<PersonModel>("cms_access_token");
            _user = sessionResult.Success ? sessionResult.Value : null;
            if (_user is not null) StateHasChanged();
        }
    }
}