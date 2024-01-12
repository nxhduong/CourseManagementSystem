using CourseManagementSystem.Models;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Server.ProtectedBrowserStorage;

namespace CourseManagementSystem.Pages.Home
{
    public partial class HomePage : ComponentBase
    {
        [Inject]
        private ProtectedSessionStorage SessionStorage { get; set; } = default!;
        [Inject]
        private NavigationManager? Navigation { get; set; }
        private PersonModel? _user;
        private readonly PasswordChangeModel _passwords = new();
        private string _errorMessage = "";
    }
}