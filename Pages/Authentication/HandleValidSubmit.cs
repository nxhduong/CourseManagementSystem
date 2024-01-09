using CourseManagementSystem.Utilities;
using Microsoft.Data.Sqlite;
using System.Text;

namespace CourseManagementSystem.Pages.Authentication
{
    public partial class AuthenticationPage
    {
        public async Task HandleValidSubmit()
        {
            var hashInput = Encoding.ASCII.GetBytes(_user.Password).ComputeMD5().FromByteArrayToString().ToLower();

            using var connection = new SqliteConnection("Data Source=Data/CMS_DATABASE.db;Mode=ReadOnly");
            connection.Open();

            var command = connection.CreateCommand();
            var username = "";
            var hashPass = "";

            // Find staff
            command.CommandText = "SELECT * FROM Staff WHERE ID = $username";
            command.Parameters.AddWithValue("$username", _user.ID);

            using (var reader = command.ExecuteReader())
            {
                reader.Read();
                if (reader.GetString(0) is not null)
                {
                    username = reader.GetString(0);
                    hashPass = reader.GetString(1);

                    if (hashInput == hashPass)
                    {
                        _user.IsStaff = true;
                        _invalidCredentials = false;

                        await ProtectedSessionStore.SetAsync("cms_access_token", _user);
                        Navigation?.NavigateTo("/", true);
                        return;
                    }
                }
            }

            // Find student
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
                    hashPass = reader.GetString(6);

                    if (hashInput == hashPass)
                    {
                        _user.IsStaff = false;
                        _user.Class = reader.GetString(0);
                        _user.FullName = reader.GetString(1);
                        _invalidCredentials = false;

                        await ProtectedSessionStore.SetAsync("cms_access_token", _user);
                        Navigation?.NavigateTo("/", true);
                        return;
                    }
                }
            }

            _invalidCredentials = true;
            StateHasChanged();
        }
    }
}
