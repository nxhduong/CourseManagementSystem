using CourseManagementSystem.Models;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Server.ProtectedBrowserStorage;

namespace CourseManagementSystem.Pages.Authentication
{
    public partial class AuthenticationPage
    {
        [Inject]
        private NavigationManager? Navigation { get; set; }
        [Inject]
        private ProtectedSessionStorage SessionStorage { get; set; } = default!;
        private readonly PersonModel _user = new();
        private bool _invalidCredentials = false;
    }
}