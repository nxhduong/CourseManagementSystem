using Microsoft.AspNetCore.Components;
using Microsoft.Data.Sqlite;

namespace CourseManagementSystem.Pages.ClassesAndStudents
{
    public partial class ClassesAndStudentsPage : ComponentBase
    {
        private void ModifyStudent()
        {
            try
            {
                using var connection = new SqliteConnection("Data Source=Data/CMS_DATABASE.db;Mode=ReadWrite");
                connection.Open();

                var command = connection.CreateCommand();

                if (!_deleteStudent)
                {
                    command.CommandText = $@"UPDATE Students 
                                        SET Class = $class, StudentName = $studentname, Male = $male, 
                                        DOB = $dob, SocialID = $socialid, ID = $id
                                        WHERE ID = $oldid";
                    command.Parameters.AddWithValue("$class", _selectedStudent!.Class);
                    command.Parameters.AddWithValue("$studentname", _selectedStudent.FullName);
                    command.Parameters.AddWithValue("$id", _selectedStudent.Id);
                    command.Parameters.AddWithValue("$oldid", _selectedStudentOldId);
                    command.Parameters.AddWithValue("$male", _selectedStudent.Male);
                    command.Parameters.AddWithValue("$dob", _selectedStudent.DOB);
                    command.Parameters.AddWithValue("$socialid", _selectedStudent.SocialId);
                }
                else
                {
                    command.CommandText = "DELETE FROM Students WHERE ID = $id";
                    command.Parameters.AddWithValue("$id", _selectedStudentOldId);
                }

                command.ExecuteNonQuery();
            }
            catch (SqliteException err)
            {
                Console.Error.WriteLine(err);
                _errorMessage = "SQL Exception";
            }

            StateHasChanged();
        }
    }
}
