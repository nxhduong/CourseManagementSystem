using CourseManagementSystem.Models;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Server.ProtectedBrowserStorage;
using Microsoft.Data.Sqlite;
using System.Globalization;
using System.Text.RegularExpressions;

namespace CourseManagementSystem.Pages
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
        private readonly List<SemesterModel> _semestersList = new();
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

        private void AddSemester()
        {
            // Show modifying semester form
            _newSemester = true;
        }

        private void ModifyRow(int row)
        {
            // Save selected row data for modifying
            SelectedSemester.Year = _semestersList[row].Year;
            SelectedSemester.SemesterNumber = _semestersList[row].SemesterNumber;
            Semester.Year = _semestersList[row].Year;
            Semester.SemesterNumber = _semestersList[row].SemesterNumber;
            Semester.StartDate = _semestersList[row].StartDate;
            Semester.EndDate = _semestersList[row].EndDate;
            _newSemester = false;
        }

        private void HandleValidSubmit()
        {
            if (_user?.IsStaff != true)
            {
                Navigation?.NavigateTo("/login");
                return;
            }

            // Delete semester when both fields are empty
            if (String.IsNullOrWhiteSpace(Semester.Year) && String.IsNullOrWhiteSpace(Semester.SemesterNumber) && !_newSemester)
            {
                try
                {
                    using var connection = new SqliteConnection("Data Source=Data/CMS_DATABASE.db;Mode=ReadWrite");
                    connection.Open();

                    var command = connection.CreateCommand();
                    command.CommandText = "DELETE FROM Semesters WHERE Year = $year AND Semester = $semester";
                    command.Parameters.AddWithValue("$year", SelectedSemester.Year);
                    command.Parameters.AddWithValue("$semester", SelectedSemester.SemesterNumber);

                    command.ExecuteNonQuery();
                    Navigation?.NavigateTo("/semesters", true);
                }
                catch (SqliteException err)
                {
                    _errorMessage = "SQL Exception";
                    Console.Error.WriteLine(err);
                }
            }
            // Modify semester
            else if (Regex.IsMatch(Semester.Year.Trim(), @"\d{4}-\d{4}") && Regex.IsMatch(Semester.SemesterNumber.Trim(), @"^\d{1}$"))
            {
                try { 
                    using var connection = new SqliteConnection("Data Source=Data/CMS_DATABASE.db;Mode=ReadWrite");
                    connection.Open();
                    var command = connection.CreateCommand();

                    if (!_newSemester)
                    {
                        command.CommandText = @"UPDATE Semesters 
                                                SET Year = $newyear, Semester = $newsemester, StartDate = $newstartdate, EndDate = $newenddate 
                                                WHERE Year = $year AND Semester = $semester";
                        command.Parameters.AddWithValue("$newyear", Semester.Year.Trim());
                        command.Parameters.AddWithValue("$newsemester", Semester.SemesterNumber.Trim());
                        command.Parameters.AddWithValue("$newstartdate", Semester.StartDate.ToString("dd/MM/yyyy"));
                        command.Parameters.AddWithValue("$newenddate", Semester.EndDate.ToString("dd/MM/yyyy"));
                        command.Parameters.AddWithValue("$year", SelectedSemester.Year.Trim());
                        command.Parameters.AddWithValue("$semester", SelectedSemester.SemesterNumber.Trim());
                    } 
                    else
                    {
                        // Check for duplicates
                        var duplicates = from semester in _semestersList
                                         where semester.Year == Semester.Year.Trim() && semester.SemesterNumber == Semester.SemesterNumber.Trim()
                                         select semester;
                        if (duplicates.Any())
                        {
                            _errorMessage = "Semester already exists!";
                            return;
                        }

                        command.CommandText = "INSERT INTO Semesters VALUES ($year, $semester, $startdate, $enddate)";
                        command.Parameters.AddWithValue("$startdate", Semester.StartDate.ToString("dd/MM/yyyy"));
                        command.Parameters.AddWithValue("$enddate", Semester.EndDate.ToString("dd/MM/yyyy"));
                        command.Parameters.AddWithValue("$year", Semester.Year.Trim());
                        command.Parameters.AddWithValue("$semester", Semester.SemesterNumber.Trim());
                    }

                    command.ExecuteNonQuery();
                    Navigation?.NavigateTo("/semesters", true);
                }
                catch (SqliteException err)
                {
                    _errorMessage = "SQL Exception";
                    Console.Error.WriteLine(err);
                }
            }
            else
            {
                _errorMessage = "Invalid input";
            }
        }
    }
}