﻿using CourseManagementSystem.Models;
using Microsoft.AspNetCore.Components;

namespace CourseManagementSystem.Pages.ClassesAndStudents
{
    public partial class ClassesAndStudentsPage : ComponentBase
    {
        public void ModifyStudent(string[] studentInfo)
        {
            _selectedStudent = new PersonModel
            {
                Class = studentInfo[0],
                FullName = studentInfo[1],
                ID = studentInfo[2],
                Male = studentInfo[3],
                DOB = studentInfo[4],
                SocialID = studentInfo[5]
            };
            _selectedStudentOldID = studentInfo[2];
        }
    }
}
