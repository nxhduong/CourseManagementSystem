using CourseManagementSystem.Models;
using Microsoft.AspNetCore.Components.Server.ProtectedBrowserStorage;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Data.Sqlite;
using System.Linq;
using System.Globalization;
using System.Text.RegularExpressions;

namespace CourseManagementSystem.Pages
{
    public partial class SemestersPage
    {
        [Inject]
        private ProtectedSessionStorage ProtectedSessionStore { get; set; }
        [Inject]
        private NavigationManager? Navigation { get; set; }
        private UserModel? _user = null;
        private SemesterModel Semester { get; set; } = new();
        private SemesterModel SelectedSemester { get; set; } = new();
        private List<string[]> _semestersList = new();
        private bool _isModifying = false;
        private bool _invalidInput = false;
        private bool _newSemester = false;

        protected override async Task OnAfterRenderAsync(bool firstRender)
        {
            var sessionResult = await ProtectedSessionStore.GetAsync<UserModel>("cms_access_token");
            _user = sessionResult.Success ? sessionResult.Value : null;

            if (_user is not null)
            {
                StateHasChanged();

                if (firstRender)
                {
                    using (var connection = new SqliteConnection("Data Source=Data/CMS_DATABASE.db;Mode=ReadOnly"))
                    {
                        connection.Open();

                        var command = connection.CreateCommand();
                        command.CommandText = "SELECT * FROM Semesters";

                        using var reader = command.ExecuteReader();
                        while (reader.Read())
                        {
                            _semestersList.Add(new string[] { reader.GetString(0), reader.GetString(1), reader.GetString(2), reader.GetString(3) });
                        }
                    }

                    if (_semestersList is not null)
                    {
                        _semestersList = (from row in _semestersList orderby row[0], row[1] select row).ToList();
                        StateHasChanged();
                    }
                }
            } 
            else
            {
                Navigation?.NavigateTo("/Login");
            }
        }

        private void AddSemester()
        {
            _newSemester = true;
            _isModifying = true;
        }

        private void ModifyRow(int row)
        {
            SelectedSemester.Year = _semestersList[row][0];
            SelectedSemester.SemesterNumber = _semestersList[row][1];
            Semester.Year = _semestersList[row][0];
            Semester.SemesterNumber = _semestersList[row][1];
            Semester.StartDate = DateTime.ParseExact(_semestersList[row][2], "dd/MM/yyyy", CultureInfo.InvariantCulture);
            Semester.EndDate = DateTime.ParseExact(_semestersList[row][3], "dd/MM/yyyy", CultureInfo.InvariantCulture);
            _newSemester = false;
            _isModifying = true;
        }

        private void HandleValidSubmit()
        {
            if (String.IsNullOrWhiteSpace(Semester.Year) && String.IsNullOrWhiteSpace(Semester.SemesterNumber) && !_newSemester)
            {
                using var connection = new SqliteConnection("Data Source=Data/CMS_DATABASE.db;Mode=ReadWrite");
                connection.Open();

                var command = connection.CreateCommand();
                command.CommandText = "DELETE FROM Semesters WHERE Year = $year AND Semester = $semester";
                command.Parameters.AddWithValue("$year", SelectedSemester.Year);
                command.Parameters.AddWithValue("$semester", SelectedSemester.SemesterNumber);

                if (command.ExecuteNonQuery() == 1)
                {
                    Navigation?.NavigateTo("/Semesters", true);
                }
                else
                {
                    Console.Error.WriteLine("Error updating semester");
                }
            }
            else if (Regex.IsMatch(Semester.Year.Trim(), @"\d{4}-\d{4}") && Regex.IsMatch(Semester.SemesterNumber.Trim(), @"^\d{1}$"))
            {
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
                    command.CommandText = "INSERT INTO Semesters VALUES ($year, $semester, $startdate, $enddate)";
                    command.Parameters.AddWithValue("$startdate", Semester.StartDate.ToString("dd/MM/yyyy"));
                    command.Parameters.AddWithValue("$enddate", Semester.EndDate.ToString("dd/MM/yyyy"));
                    command.Parameters.AddWithValue("$year", Semester.Year.Trim());
                    command.Parameters.AddWithValue("$semester", Semester.SemesterNumber.Trim());
                }

                if (command.ExecuteNonQuery() == 1)
                {
                    Navigation?.NavigateTo("/Semesters", true);
                }
                else
                {
                    Console.Error.WriteLine("Error updating semester");
                }
            }
            else
            {
                _invalidInput = true;
            }
        }
    }
}