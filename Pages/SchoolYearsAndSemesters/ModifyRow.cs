namespace CourseManagementSystem.Pages.SchoolYearsAndSemesters
{
    public partial class SchoolYearsAndSemestersPage
    {
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
    }
}
