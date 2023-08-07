using CourseManagementSystem.Models;
using Microsoft.Data.Sqlite;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Server.ProtectedBrowserStorage;
using CourseManagementSystem.Data;
using System.Text;

namespace CourseManagementSystem.Pages
{
    public partial class LoginPage
    {
        [Inject]
        private NavigationManager? Navigation { get; set; }
        [Inject]
        private ProtectedSessionStorage? ProtectedSessionStore { get; set; }
        private readonly UserModel _user = new();
        private bool wrongCredentials = false;

        public async Task HandleValidSubmit()
        {
            var hashInput = Crypto.ByteArrayToString(Crypto.MD5Encryptor.ComputeHash(Encoding.ASCII.GetBytes(_user.Password))).ToUpper();

            using (var connection = new SqliteConnection("Data Source=Data/CMS_DATABASE.db;Mode=ReadOnly"))
            {
                connection.Open();

                var command = connection.CreateCommand();
                var username = "";
                var hashPass = "";

                command.CommandText = "SELECT * FROM Staff WHERE ID = $username";
                command.Parameters.AddWithValue("$username", _user.ID);

                using (var reader = command.ExecuteReader())
                {
                    reader.Read();
                    if (reader.GetString(0) != null)
                    {
                        username = reader.GetString(0);
                        hashPass = reader.GetString(1).ToUpper();

                        if (hashInput == hashPass)
                        {
                            _user.IsStaff = true;
                            wrongCredentials = false;

                            await ProtectedSessionStore.SetAsync("cms_access_token", _user);
                            Navigation?.NavigateTo("/", true);
                            return;
                        }
                    }
                }

                username = "";
                hashPass = "";
                command = connection.CreateCommand();
                command.CommandText = "SELECT ID, HashPass FROM Students WHERE ID = $username";
                command.Parameters.AddWithValue("$username", _user.ID);

                using (var reader = command.ExecuteReader())
                {
                    if (reader.Read())
                    {
                        username = reader.GetString(2);
                        hashPass = reader.GetString(6).ToUpper();

                        if (hashInput == hashPass)
                        {
                            _user.IsStaff = false;
                            _user.Class = reader.GetString(0);
                            _user.FullName = reader.GetString(1);
                            wrongCredentials = false;

                            await ProtectedSessionStore.SetAsync("cms_access_token", _user);
                            Navigation?.NavigateTo("/", true);
                            return;
                        }
                    }
                }
            }
            wrongCredentials = true;
            StateHasChanged();
        }
    }
}