using CourseManagementSystem.Utilities;
using Microsoft.Data.Sqlite;
using System.Text;

namespace CourseManagementSystem.Pages.Home
{
    public partial class HomePage
    {
        public async Task PerformPasswordChange()
        {
            // Change password
            try
            {
                using var connection = new SqliteConnection("Data Source=Data/CMS_DATABASE.db;Mode=ReadWrite");
                connection.Open();

                var command = connection.CreateCommand();
                var newHashPass = Crypto.ConvertByteArrayToString(Encoding.ASCII.GetBytes(_passwords.OldPassword ?? "").ComputeMD5()).ToLower();

                command.CommandText = _user?.IsStaff ?? false ?
                    "UPDATE Staff SET HashPass = $newhashpass WHERE HashPass = $oldhashpass" :
                    "UPDATE Students SET HashPass = $newhashpass WHERE HashPass = $oldhashpass";
                command.Parameters.AddWithValue("$oldhashpass", _passwords.NewPassword);
                command.Parameters.AddWithValue("$newhashpass", newHashPass);

                command.ExecuteNonQuery();
                await SessionStorage.DeleteAsync("cms_access_token");
                Navigation?.NavigateTo("/login", true);
            }
            catch (SqliteException err)
            {
                Console.Error.WriteLine(err);
                _errorMessage = "SQL Exception. Please contact administrator";
            }
        }
    }
}