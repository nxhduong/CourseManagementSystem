using CourseManagementSystem.Models;
using CourseManagementSystem.Utilities;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Server.ProtectedBrowserStorage;
using Microsoft.Data.Sqlite;
using System.Text;
using System.Text.RegularExpressions;

namespace CourseManagementSystem.Pages
{
    public partial class HomePage
    {
        [Inject]
        private ProtectedSessionStorage ProtectedSessionStore { get; set; } = default!;
        [Inject]
        private NavigationManager? Navigation { get; set; }
        private UserModel? _user;
        private readonly PasswordChangeModel _passwords = new();
        private bool _invalidNewPassword = false;

        protected override async Task OnAfterRenderAsync(bool firstRender)
        {
            // Authentication
            var sessionResult = await ProtectedSessionStore.GetAsync<UserModel>("cms_access_token");
            _user = sessionResult.Success ? sessionResult.Value : null;
            if (_user is not null) StateHasChanged();
        }

        public async Task HandleValidSubmit()
        {
            // Change password
            if (_passwords.NewPassword?.Length > 6 && Regex.IsMatch(_passwords.NewPassword, @"(\w*\d\w+)|(\w+\d\w*)"))
            {
                _invalidNewPassword = false;

                using var connection = new SqliteConnection("Data Source=Data/CMS_DATABASE.db;Mode=ReadWrite");

                connection.Open();

                var command = connection.CreateCommand();
                var newHashPass = Encoding.ASCII.GetBytes(_passwords.OldPassword ?? "").ComputeMD5().ByteArrayToString().ToLower();
                command.CommandText = _user?.IsStaff ?? false ?
                    "UPDATE Staff SET HashPass = $newhashpass WHERE HashPass = $oldhashpass" :
                    "UPDATE Students SET HashPass = $newhashpass WHERE HashPass = $oldhashpass";
                command.Parameters.AddWithValue("$oldhashpass", _passwords.NewPassword);
                command.Parameters.AddWithValue("$newhashpass", newHashPass);

                if (command.ExecuteNonQuery() == 1)
                {
                    await ProtectedSessionStore.DeleteAsync("cms_access_token");
                    Navigation?.NavigateTo("/Login", true);
                }
                else
                {
                    Console.Error.WriteLine("Error updating password");
                }
            }
            else
            {
                _invalidNewPassword = true;
            }
        }
    }
}
