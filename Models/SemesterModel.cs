namespace CourseManagementSystem.Models
{
    public class SemesterModel
    {
        public string Year { get; set; } = default!;
        public string SemesterNumber { get; set; } = default!;
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
    }
}
