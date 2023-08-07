using System.ComponentModel.DataAnnotations;

namespace CourseManagementSystem.Models
{
    public class UserModel
    {
        [Required]
        public string ID { get; set; }
        [Required]
        [DataType(DataType.Password)]
        public string Password { get; set; }
        [Required]
        public bool IsStaff { get; set; }
        public string Class { get; set; } = "";
        public string FullName { get; set; } = "";
    }
}
