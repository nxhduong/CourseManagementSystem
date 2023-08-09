using System.ComponentModel.DataAnnotations;

namespace CourseManagementSystem.Models
{
    public class ClassChangeModel
    {
        [Required]
        public string OldClass { get; set; } = "";
        public string? NewClass { get; set; }
    }
}
