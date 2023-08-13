using Microsoft.AspNetCore.Components;

namespace CourseManagementSystem.Pages
{
    public partial class CoursesPage
    {
        [Parameter]
        public string? Course { get; set; }
    }
}
