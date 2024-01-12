using CourseManagementSystem.Models;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Server.ProtectedBrowserStorage;

namespace CourseManagementSystem.Pages.ClassesAndStudents
{
    public partial class ClassesAndStudentsPage : ComponentBase
    {
        [Parameter]
        public string? Class { get; set; }
        [Inject]
        public ProtectedSessionStorage SessionStorage { get; set; } = default!;
        [Inject]
        private NavigationManager? Navigation { get; set; }
        private PersonModel? _user = null;
        private PersonModel _selectedStudent = new();
        private string _selectedStudentOldId = default!;
        private string _errorMessage = "";
        private bool _deleteStudent = false;
        private bool _resetPassword = false;
        private readonly List<string> _classesList = [];
        private readonly List<string[]> _studentsList = [];
        private IEnumerable<string>? _duplicates;
        private readonly ClassChangeModel _class = new();
        private const string _defaultHashPass = "d00f5d5217896fb7fd601412cb890830";
    }
}