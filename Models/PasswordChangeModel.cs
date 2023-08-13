using System.ComponentModel.DataAnnotations;

namespace CourseManagementSystem.Models
{
    public class PasswordChangeModel
    {
        [Required]
        [DataType(DataType.Password)]
        [RegularExpression(@"(\w*\d\w+)|(\w+\d\w*)")]
        [StringLength(25, MinimumLength = 6, ErrorMessage = "Password must be 6-25 characters long")]
        public string? OldPassword { get; set; }
        [Required]
        [DataType(DataType.Password)]
        [RegularExpression(@"(\w*\d\w+)|(\w+\d\w*)")]
        [StringLength(25, MinimumLength = 6, ErrorMessage = "Password must be 6-25 characters long")]
        public string? NewPassword { get; set; }
    }
}
