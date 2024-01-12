using CourseManagementSystem.Models;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Server.ProtectedBrowserStorage;
using System.Text.RegularExpressions;

namespace CourseManagementSystem.Pages.SchoolYearsAndSemesters
{
    public partial class SchoolYearsAndSemestersPage : ComponentBase
    {
        [Inject]
        private ProtectedSessionStorage ProtectedSessionStore { get; set; } = default!;
        [Inject]
        private NavigationManager? Navigation { get; set; }
        [GeneratedRegex(@"\d{4}-\d{4}")]
        private static partial Regex s_yearRangeRegex();
        [GeneratedRegex(@"^\d{1}$")]
        private static partial Regex s_semesterRegex();
        private PersonModel? _user = null;
        private readonly SemesterModel Semester = new();
        private readonly SemesterModel SelectedSemester = new();
        private readonly List<SemesterModel> _semestersList = [];
        private string _errorMessage = "";
        private bool _newSemester = false;
    }
}