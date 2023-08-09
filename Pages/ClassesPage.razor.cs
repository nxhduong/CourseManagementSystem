using Microsoft.AspNetCore.Components;
using CourseManagementSystem.Models;
using System.Text;
using Microsoft.Data.Sqlite;
using Microsoft.AspNetCore.Components.Server.ProtectedBrowserStorage;
using Microsoft.AspNetCore.Components.Forms;

namespace CourseManagementSystem.Pages
{
    public partial class ClassesPage: ComponentBase
    {
        [Parameter]
        public string? Class { get; set; }
        [Inject]
        public ProtectedSessionStorage ProtectedSessionStore { get; set; } = default!;
        [Inject]
        private NavigationManager? Navigation { get; set; }
        private UserModel? _user = null;
        private bool _invalidInput = false;
        private bool _importStudents = false;
        private bool _reRender = true;
        private readonly List<string> _classesList = new();
        private readonly List<string[]> _studentsList = new();
        private readonly ClassChangeModel _class = new();
        private const string _defaultHashPass = "d00f5d5217896fb7fd601412cb890830";

        protected override async Task OnAfterRenderAsync(bool firstRender)
        {
            // Authentication
            var sessionResult = await ProtectedSessionStore.GetAsync<UserModel>("cms_access_token");
            _user = sessionResult.Success ? sessionResult.Value : null;

            if (_user?.IsStaff == true)
            {
                StateHasChanged();

                if (_reRender)
                {
                    _reRender = false;

                    using var connection = new SqliteConnection("Data Source=Data/CMS_DATABASE.db;Mode=ReadOnly");
                    connection.Open();

                    // Display classes upon successful authentication
                    var command = connection.CreateCommand();
                    command.CommandText = "SELECT * FROM Classes";

                    using (var reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            _classesList.Add(reader.GetString(0));
                        }
                    }

                    // Show students when a class is selected
                    if (Class != null)
                    {
                        command = connection.CreateCommand();
                        command.CommandText = "SELECT Class, StudentName, ID, Male, DOB, SocialID FROM Students";

                        using var reader = command.ExecuteReader();
                        while (reader.Read())
                        {
                            var studentInfo = new string[6];
                            for (var i = 0; i < 6; i++)
                            {
                                studentInfo[i] = reader.GetString(i);
                            }
                            _studentsList.Add(studentInfo);
                        }

                        _class.OldClass = Class;
                    }
                }
            }
            else
            {
                Navigation?.NavigateTo("/Login");
            }
        }

        private void HandleValidClassSubmit() 
        {
            var classFound = _classesList.Contains(_class.NewClass ?? "");

            using var connection = new SqliteConnection("Data Source=Data/CMS_DATABASE.db;Mode=ReadWrite");
            connection.Open();
            var command = connection.CreateCommand();

            // Create class
            if (String.IsNullOrWhiteSpace(_class.OldClass) && !String.IsNullOrWhiteSpace(_class.NewClass) && !classFound)
            {
                command.CommandText = "INSERT INTO Classes VALUES ($class)";
                command.Parameters.AddWithValue("$class", _class.NewClass);
            } 
            // Delete class
            else if (!String.IsNullOrWhiteSpace(_class.OldClass) && String.IsNullOrWhiteSpace(_class.NewClass) && classFound)
            {
                command.CommandText = "DELETE FROM Classes WHERE Class = $class";
                command.Parameters.AddWithValue("$class", _class.OldClass);
            }
            // Rename class
            else if (!String.IsNullOrWhiteSpace(_class.OldClass) && !String.IsNullOrWhiteSpace(_class.NewClass) && classFound)
            {
                command.CommandText = "UPDATE Classes SET Classes = $newclass WHERE Classes = $oldclass";
                command.Parameters.AddWithValue("$oldclass", _class.OldClass);
                command.Parameters.AddWithValue("$newclass", _class.NewClass);
            }
            else
            {
                _invalidInput = true;
            }

            command.ExecuteNonQuery();
            _reRender = true;
            StateHasChanged();
        }

        private async Task ReadStudentsCSVFile(InputFileChangeEventArgs eventArgs) 
        {
            // Add rows from valid CSV file to list and database
            if (eventArgs.File.Size <= 2000000 && eventArgs.File.Name.Substring(eventArgs.File.Name.Length - 3, 3).ToLower() == "csv") 
            {
                using var connection = new SqliteConnection("Data Source=Data/CMS_DATABASE.db;Mode=ReadWrite");
                connection.Open();

                var memStream = new MemoryStream();
                await eventArgs.File.OpenReadStream().CopyToAsync(memStream);
                var rows = Encoding.UTF8.GetString(memStream.ToArray()).Split("\n");


                for (var i = 1; i < rows.Length; i++)
                {
                    var data = rows[i].Split(",");
                    if (rows.Length != 6) {
                        Console.WriteLine("invalid input");
                        return;
                    }
                    if (!_studentsList.Where(student => student[2].ToLower() == student[2].ToLower()).Any())
                    {
                        _studentsList.Add(new string[] { Class!, data[0], data[1], data[2], data[3], data[4] });

                        var command = connection.CreateCommand();
                        command.CommandText = $"INSERT INTO Students VALUES ($class, $studentname, $id, $male, $dob, $socialid, '{_defaultHashPass}')";
                        command.Parameters.AddWithValue("$class", Class);
                        command.Parameters.AddWithValue("$studentname", data[0]);
                        command.Parameters.AddWithValue("$id", data[1]);
                        command.Parameters.AddWithValue("$male", data[2]);
                        command.Parameters.AddWithValue("$dob", data[3]);
                        command.Parameters.AddWithValue("$socialid", data[4]);
                        command.ExecuteNonQuery();
                    } 
                    else
                    {
                        //TODO
                    }
                }

                _reRender = true;
                StateHasChanged();
            }
        }
    }
}