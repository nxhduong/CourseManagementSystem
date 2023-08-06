using Microsoft.EntityFrameworkCore;
using CourseManagementSystem.Models;

namespace CourseManagementSystem.Data
{
    public class SchoolContext: DbContext
    {
        public DbSet<SemesterModel> Semesters { get; set; }
    }
}
