using System.ComponentModel.DataAnnotations;

namespace CourseManagementSystem.Models
{
    public class UserModel
    {
        [Required]
        public string Username { get; set; }
        [Required]
        public string Password { get; set; }
        [Required]
        public bool IsStaff { get; set; }
    }
}
