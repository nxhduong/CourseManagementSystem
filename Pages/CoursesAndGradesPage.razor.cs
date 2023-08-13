using Microsoft.AspNetCore.Components;

namespace CourseManagementSystem.Pages
{
    public partial class CoursesAndGradesPage
    {
        [Parameter]
        public string? Course { get; set; }
    }
}
