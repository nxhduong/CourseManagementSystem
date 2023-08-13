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

        public async Task HandleValidSubmit()
        {
            // Change password
            try
            {
                using var connection = new SqliteConnection("Data Source=Data/CMS_DATABASE.db;Mode=ReadWrite");

                connection.Open();

                var command = connection.CreateCommand();
                var newHashPass = Encoding.ASCII.GetBytes(_passwords.OldPassword ?? "").ComputeMD5().ByteArrayToString().ToLower();
                command.CommandText = _user?.IsStaff ?? false ?
                    "UPDATE Staff SET HashPass = $newhashpass WHERE HashPass = $oldhashpass" :
                    "UPDATE Students SET HashPass = $newhashpass WHERE HashPass = $oldhashpass";
                command.Parameters.AddWithValue("$oldhashpass", _passwords.NewPassword);
                command.Parameters.AddWithValue("$newhashpass", newHashPass);

                command.ExecuteNonQuery();
                await ProtectedSessionStore.DeleteAsync("cms_access_token");
                Navigation?.NavigateTo("/login", true);
            }
            catch (SqliteException err)
            {
                Console.Error.WriteLine(err);
                _errorMessage = "SQL Exception";
            }
        }
    }
}
