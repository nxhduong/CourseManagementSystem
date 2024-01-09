using CourseManagementSystem.Models;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Server.ProtectedBrowserStorage;
using Microsoft.Data.Sqlite;
using System.Globalization;

namespace CourseManagementSystem.Pages.SchoolYearsAndSemesters
{
    public partial class SchoolYearsAndSemestersPage
    {
        [Inject]
        private ProtectedSessionStorage ProtectedSessionStore { get; set; } = default!;
        [Inject]
        private NavigationManager? Navigation { get; set; }
        private PersonModel? _user = null;
        private readonly SemesterModel Semester = new();
        private readonly SemesterModel SelectedSemester = new();
        private readonly List<SemesterModel> _semestersList = [];
        private string _errorMessage = "";
        private bool _newSemester = false;

        protected override async Task OnAfterRenderAsync(bool firstRender)
        {
            // Authentication
            var sessionResult = await ProtectedSessionStore.GetAsync<PersonModel>("cms_access_token");
            _user = sessionResult.Success ? sessionResult.Value : null;

            if (_user?.IsStaff == true)
            {
                StateHasChanged();

                if (firstRender)
                {
                    using (var connection = new SqliteConnection("Data Source=Data/CMS_DATABASE.db;Mode=ReadOnly"))
                    {
                        connection.Open();

                        // Display semesters upon successful authentication
                        var command = connection.CreateCommand();
                        command.CommandText = "SELECT * FROM Semesters ORDER BY Year DESC, Semester ASC";

                        using var reader = command.ExecuteReader();
                        while (reader.Read())
                        {
                            if (!_semestersList.Where(semester => semester.Year == reader.GetString(0) && semester.SemesterNumber == reader.GetString(1)).Any())
                            {
                                _semestersList.Add(new SemesterModel()
                                {
                                    Year = reader.GetString(0),
                                    SemesterNumber = reader.GetString(1),
                                    StartDate = DateTime.ParseExact(reader.GetString(2), "dd/MM/yyyy", CultureInfo.InvariantCulture),
                                    EndDate = DateTime.ParseExact(reader.GetString(3), "dd/MM/yyyy", CultureInfo.InvariantCulture)
                                });
                            }
                        }
                    }

                    StateHasChanged();
                }
            } 
            else
            {
                Navigation?.NavigateTo("/login");
            }
        }
    }
}