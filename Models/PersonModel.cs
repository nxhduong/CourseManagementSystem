using System.ComponentModel.DataAnnotations;

namespace CourseManagementSystem.Models
{
    public class PersonModel
    {
        [Required]
        public string Id { get; set; } = default!;
        [Required]
        [DataType(DataType.Password)]
        [RegularExpression(@"(\w*\d\w+)|(\w+\d\w*)")]
        [StringLength(25, MinimumLength = 6, ErrorMessage = "Password must be 6-25 characters long")]
        public string Password { get; set; } = default!;
        [Required]
        public bool IsStaff { get; set; }
        public string? Class;
        public string? FullName;
        public string? Male;
        [RegularExpression(@"\d{2}\/\d{2}\/\d{4}")]
        public string? DOB;
        public string? SocialId;
    }
}
