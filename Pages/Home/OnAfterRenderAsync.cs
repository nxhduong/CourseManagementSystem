using CourseManagementSystem.Models;
using Microsoft.AspNetCore.Components;

namespace CourseManagementSystem.Pages.Home
{
    public partial class HomePage : ComponentBase
    {
        protected override async Task OnAfterRenderAsync(bool firstRender)
        {
            // Authentication
            var sessionResult = await SessionStorage.GetAsync<PersonModel>("cms_access_token");
            
            if (sessionResult.Success)
            {
                _user = sessionResult.Value;
                StateHasChanged();
            }
        }
    }
}
