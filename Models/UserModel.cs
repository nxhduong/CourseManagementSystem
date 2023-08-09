using System.ComponentModel.DataAnnotations;

namespace CourseManagementSystem.Models
{
    public class UserModel
    {
        [Required]
        public string ID { get; set; } = default!;
        [Required]
        [DataType(DataType.Password)]
        public string Password { get; set; } = default!;
        [Required]
        public bool IsStaff { get; set; }
        public string Class { get; set; } = "";
        public string FullName { get; set; } = "";
    }
}
