using CourseManagementSystem.Models;
using Microsoft.Data.Sqlite;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Server.ProtectedBrowserStorage;
using System.Security.Cryptography;
using System.Text;

namespace CourseManagementSystem.Pages
{
    public partial class LoginPage
    {
        [Inject]
        private NavigationManager? Navigation { get; set; }
        private ProtectedSessionStorage? ProtectedSessionStore { get; set; }
        private readonly MD5 MD5Encryptor = MD5.Create();
        private readonly UserModel User = new();
        private bool WrongCredentials = false;

        private static string ByteArrayToString(byte[] arrInput)
        {
            StringBuilder strOutput = new(arrInput.Length);
            for (var i = 0; i < arrInput.Length; i++)
            {
                strOutput.Append(arrInput[i].ToString("X2"));
            }
            return strOutput.ToString();
        }

        public void HandleValidSubmit()
        {
            var hashInput = ByteArrayToString(MD5Encryptor.ComputeHash(Encoding.ASCII.GetBytes(User.Password))).ToUpper();

            using (var connection = new SqliteConnection("Data Source=Data/CMSDatabase.db;Mode=ReadOnly"))
            {
                connection.Open();

                var command = connection.CreateCommand();
                var username = "";
                var hashPass = "";

                command.CommandText = "SELECT * FROM Staff WHERE Username = $username";
                command.Parameters.AddWithValue("$username", User.Username);

                using (var reader = command.ExecuteReader())
                {
                    reader.Read();
                    if (reader.GetString(0) != null)
                    {
                        username = reader.GetString(0);
                        hashPass = reader.GetString(1).ToUpper();

                        if (hashInput == hashPass)
                        {
                            var cookieProperties = new CookieOptions
                            {
                                Expires = DateTime.UtcNow.AddHours(1),
                                HttpOnly = true,
                                SameSite = SameSiteMode.Strict
                            };
                            User.IsStaff = true;
                            WrongCredentials = false;
                            ProtectedSessionStore?.SetAsync("cms_access_token", User);
                            Navigation?.NavigateTo("/", true);
                            return;
                        }
                    }
                }

                username = "";
                hashPass = "";
                command.CommandText = "SELECT StudentID, HashPass FROM Students WHERE StudentID = $username";
                command.Parameters.AddWithValue("$username", User.Username);

                using (var reader = command.ExecuteReader())
                {
                    reader.Read();
                    if (reader.GetString(0) != null)
                    {
                        username = reader.GetString(0);
                        hashPass = reader.GetString(1).ToUpper();

                        if (hashInput == hashPass)
                        {
                            User.IsStaff = false;
                            WrongCredentials = false;
                            ProtectedSessionStore?.SetAsync("cms_access_token", User);
                            Navigation?.NavigateTo("/", true);
                            return;
                        }
                    }
                }
            }
            WrongCredentials = true;
            StateHasChanged();
        }
    }
}