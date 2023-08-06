using CourseManagementSystem.Models;
using Microsoft.AspNetCore.Components.Server.ProtectedBrowserStorage;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace CourseManagementSystem.Pages
{
    public partial class ManageSemestersPage
    {
        [Inject]
        private ProtectedSessionStorage ProtectedSessionStore { get; set; }
        private UserModel? _user = null;

        protected override async Task OnAfterRenderAsync(bool firstRender)
        {
            var sessionResult = await ProtectedSessionStore.GetAsync<UserModel>("cms_access_token");
            _user = sessionResult.Success ? sessionResult.Value : null;
            if (_user is not null) StateHasChanged();
        }

        private EventCallback ModifyRow(int row)
        {
            EventCallback x = new();
            return x;
        }
    }
}
