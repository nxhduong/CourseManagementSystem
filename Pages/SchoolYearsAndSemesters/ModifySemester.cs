using Microsoft.Data.Sqlite;

namespace CourseManagementSystem.Pages.SchoolYearsAndSemesters
{
    public partial class SchoolYearsAndSemestersPage
    {
        private void ModifySemester()
        {
            if (_user?.IsStaff != true)
            {
                Navigation?.NavigateTo("/login");
                return;
            }

            // Delete semester when both fields are empty
            if (string.IsNullOrWhiteSpace(Semester.Year) && string.IsNullOrWhiteSpace(Semester.SemesterNumber) && !_newSemester)
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
            else if (s_yearRangeRegex().IsMatch(Semester.Year.Trim()) && s_semesterRegex().IsMatch(Semester.SemesterNumber.Trim()))
            {
                try
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
