using CourseManagementSystem.Models;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Server.ProtectedBrowserStorage;
using Microsoft.AspNetCore.Components.Forms;
using Microsoft.Data.Sqlite;
using System.Text;

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
        private PersonModel? _user = null;
        private PersonModel? _selectedStudent = null;
        private string _selectedStudentOldID = default!;
        private string _errorMessage = "";
        private bool _importStudents = false;
        private bool _deleteStudent = false;
        private bool _resetPassword = false;
        private bool _renderClassesStudents = true;
        private readonly List<string> _classesList = new();
        private readonly List<string[]> _studentsList = new();
        private IEnumerable<string>? _duplicates;
        private readonly ClassChangeModel _class = new();
        private const string _defaultHashPass = "d00f5d5217896fb7fd601412cb890830";

        protected override async Task OnAfterRenderAsync(bool firstRender)
        {
            // Authentication
            var sessionResult = await ProtectedSessionStore.GetAsync<PersonModel>("cms_access_token");
            _user = sessionResult.Success ? sessionResult.Value : null;

            if (_user?.IsStaff == true)
            {
                StateHasChanged();

                if (_renderClassesStudents)
                {
                    using var connection = new SqliteConnection("Data Source=Data/CMS_DATABASE.db;Mode=ReadOnly");
                    connection.Open();

                    // Display classes upon successful authentication
                    var command = connection.CreateCommand();
                    command.CommandText = "SELECT * FROM Classes";

                    using (var reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            if (!_classesList.Contains(reader.GetString(0)))
                            {
                                _classesList.Add(reader.GetString(0));
                            }
                        }
                    }

                    StateHasChanged();
                    _renderClassesStudents = false;
                }
            }
            else
            {
                Navigation?.NavigateTo("/login");
            }
        }

        protected override void OnParametersSet()
        {
            // Show students when a class is selected
            using var connection = new SqliteConnection("Data Source=Data/CMS_DATABASE.db;Mode=ReadOnly");
            connection.Open();
            var command = connection.CreateCommand();

            if (Class is not null && _classesList.Contains(Class))
            {
                command = connection.CreateCommand();
                command.CommandText = "SELECT Class, StudentName, ID, Male, DOB, SocialID FROM Students ORDER BY ID";

                using var reader = command.ExecuteReader();
                while (reader.Read())
                {
                    if (!_studentsList.Where(student => student[2] == reader.GetString(2)).Any())
                    {
                        var studentInfo = new string[6];
                        for (var i = 0; i < 6; i++)
                        {
                            studentInfo[i] = reader.GetString(i);
                        }
                        _studentsList.Add(studentInfo);
                    }
                }

                _class.OldClass = Class;
            }
            else if (_classesList.Any())
            {
                Navigation?.NavigateTo("/classes");
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
                _errorMessage = "Invalid input";
            }

            command.ExecuteNonQuery();
            _renderClassesStudents = true;
            StateHasChanged();
        }

        private void HandleValidStudentSubmit()
        {
            try { 
                using var connection = new SqliteConnection("Data Source=Data/CMS_DATABASE.db;Mode=ReadWrite");
                connection.Open();

                var command = connection.CreateCommand();

                if (!_deleteStudent)
                {
                    command.CommandText = $@"UPDATE Students 
                                        SET Class = $class, StudentName = $studentname, Male = $male, 
                                        DOB = $dob, SocialID = $socialid, ID = $id
                                        WHERE ID = $oldid";
                    command.Parameters.AddWithValue("$class", _selectedStudent!.Class);
                    command.Parameters.AddWithValue("$studentname", _selectedStudent.FullName);
                    command.Parameters.AddWithValue("$id", _selectedStudent.ID);
                    command.Parameters.AddWithValue("$oldid", _selectedStudentOldID);
                    command.Parameters.AddWithValue("$male", _selectedStudent.Male);
                    command.Parameters.AddWithValue("$dob", _selectedStudent.DOB);
                    command.Parameters.AddWithValue("$socialid", _selectedStudent.SocialID);
                }
                else
                {
                    command.CommandText = "DELETE FROM Students WHERE ID = $id";
                    command.Parameters.AddWithValue("$id", _selectedStudentOldID);
                }

                command.ExecuteNonQuery();
            }
            catch (SqliteException err)
            {
                Console.Error.WriteLine(err);
                _errorMessage = "SQL Exception";
            }

            StateHasChanged();
        }

        private async Task ImportStudentsFromCSVFile(InputFileChangeEventArgs eventArgs) 
        {
            // Add rows from valid CSV file to list and database
            if (eventArgs.File.Size <= 2000000 && eventArgs.File.Name.Substring(eventArgs.File.Name.Length - 3, 3).ToLower() == "csv") 
            {
                using var connection = new SqliteConnection("Data Source=Data/CMS_DATABASE.db;Mode=ReadWrite");
                connection.Open();

                var memoryStream = new MemoryStream();
                await eventArgs.File.OpenReadStream().CopyToAsync(memoryStream);
                var rows = Encoding.UTF8.GetString(memoryStream.ToArray()).Split("\n");


                for (var i = 1; i < rows.Length; i++)
                {
                    var data = rows[i].Split(",");
                    _duplicates = from student in _studentsList
                                  where student[2].ToLower() == student[2].ToLower()
                                  select student[2];

                    if (data.Length != 5) {
                        _errorMessage = "Number of columns must be exactly 5";
                        return;
                    }
                    if (!_duplicates.Any())
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
                }

                _renderClassesStudents = true;
                StateHasChanged();
            }
        }

        public void ModifyStudent(string[] studentInfo)
        {
            _selectedStudent = new PersonModel
            {
                Class = studentInfo[0],
                FullName = studentInfo[1],
                ID = studentInfo[2],
                Male = studentInfo[3],
                DOB = studentInfo[4],
                SocialID = studentInfo[5]
            };
            _selectedStudentOldID = studentInfo[2];
        }
    }
}