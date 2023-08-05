using CourseManagementSystem.Models;
using Microsoft.AspNetCore.Components.Server.ProtectedBrowserStorage;

namespace CourseManagementSystem.Pages
{
    public partial class HomePage
    {
        private ProtectedSessionStorage ProtectedSessionStore { get; set; }
        private UserModel? User;

        protected override async Task OnInitializedAsync()
        {
            var sessionResult = await ProtectedSessionStore.GetAsync<UserModel>("cms_access_token");
            User = sessionResult.Success ? sessionResult.Value : null;
            if (User is not null)
            {
                //
            }
        }
    }
}
