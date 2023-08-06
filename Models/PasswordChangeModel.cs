using System.ComponentModel.DataAnnotations;

namespace CourseManagementSystem.Models
{
    public class PasswordChangeModel
    {
        [Required]
        [DataType(DataType.Password)]
        public string? OldPassword { get; set; }
        [Required]
        [DataType(DataType.Password)]
        public string? NewPassword { get; set; }
    }
}
