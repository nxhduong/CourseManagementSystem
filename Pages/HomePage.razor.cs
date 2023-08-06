using CourseManagementSystem.Models;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Server.ProtectedBrowserStorage;
using Microsoft.Data.Sqlite;

namespace CourseManagementSystem.Pages
{
    public partial class HomePage
    {
        [Inject]
        private ProtectedSessionStorage ProtectedSessionStore { get; set; }
        private UserModel? _user;
        private PasswordChangeModel? _passwords = new();

        protected override async Task OnAfterRenderAsync(bool firstRender)
        {
            var sessionResult = await ProtectedSessionStore.GetAsync<UserModel>("cms_access_token");
            _user = sessionResult.Success ? sessionResult.Value : null;
            if (_user is not null) StateHasChanged();
        }

        public async Task HandleValidSubmit()
        {
            /* TODO
            using (var connection = new SqliteConnection("Data Source=Data/CMS_DATABASE.db;Mode=ReadWrite"))
            {
                connection.Open();

                var command = connection.CreateCommand();
                var username = "";
                var hashPass = "";

                command.CommandText = "SELECT * FROM Staff WHERE Username = $username";
                command.Parameters.AddWithValue("$username", _user.Username);

                using (var reader = command.ExecuteReader())
                {
                    reader.Read();
                    if (reader.GetString(0) != null) { }
                }
            }*/
        }
    }
}
