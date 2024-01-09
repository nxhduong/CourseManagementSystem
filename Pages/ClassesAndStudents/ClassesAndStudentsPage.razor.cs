using CourseManagementSystem.Models;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Server.ProtectedBrowserStorage;
using Microsoft.Data.Sqlite;

namespace CourseManagementSystem.Pages.ClassesAndStudents
{
    public partial class ClassesAndStudentsPage: ComponentBase
    {
        [Parameter]
        public string? Class { get; set; }
        [Inject]
        public ProtectedSessionStorage ProtectedSessionStore { get; set; } = default!;
        [Inject]
        private NavigationManager? Navigation { get; set; }
        private PersonModel? _user = null;
        private PersonModel _selectedStudent = new();
        private string _selectedStudentOldID = default!;
        private string _errorMessage = "";
        private bool _deleteStudent = false;
        private bool _resetPassword = false;
        private readonly List<string> _classesList = [];
        private readonly List<string[]> _studentsList = [];
        private IEnumerable<string>? _duplicates;
        private readonly ClassChangeModel _class = new();
        private const string _defaultHashPass = "d00f5d5217896fb7fd601412cb890830";

        protected override async Task OnParametersSetAsync()
        {
            // Authentication
            var sessionResult = await ProtectedSessionStore.GetAsync<PersonModel>("cms_access_token");
            _user = sessionResult.Success ? sessionResult.Value : null;

            if (_user?.IsStaff == true)
            {
                StateHasChanged();

                using var connection = new SqliteConnection("Data Source=Data/CMS_DATABASE.db;Mode=ReadOnly");
                connection.Open();
                var command = connection.CreateCommand();

                // Display classes upon successful authentication
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

                // Show students when a class is selected
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
                else if (_classesList.Count != 0)
                {
                    Navigation?.NavigateTo("/classes");
                }

                StateHasChanged();
            }
            else
            {
                Navigation?.NavigateTo("/login");
            }   
        }
    }
}