using CourseManagementSystem.Models;
using Microsoft.AspNetCore.Components;

namespace CourseManagementSystem.Pages.ClassesAndStudents
{
    public partial class ClassesAndStudentsPage : ComponentBase
    {
        public void SelectStudentForModification(string[] studentInfo)
        {
            _selectedStudent = new PersonModel
            {
                Class = studentInfo[0],
                FullName = studentInfo[1],
                Id = studentInfo[2],
                Male = studentInfo[3],
                DOB = studentInfo[4],
                SocialId = studentInfo[5]
            };
            _selectedStudentOldId = studentInfo[2];
        }
    }
}
