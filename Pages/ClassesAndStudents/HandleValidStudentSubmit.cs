using Microsoft.AspNetCore.Components;
using Microsoft.Data.Sqlite;

namespace CourseManagementSystem.Pages.ClassesAndStudents
{
    public partial class ClassesAndStudentsPage : ComponentBase
    {
        private void HandleValidStudentSubmit()
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
                    command.Parameters.AddWithValue("$id", _selectedStudent.ID);
                    command.Parameters.AddWithValue("$oldid", _selectedStudentOldID);
                    command.Parameters.AddWithValue("$male", _selectedStudent.Male);
                    command.Parameters.AddWithValue("$dob", _selectedStudent.DOB);
                    command.Parameters.AddWithValue("$socialid", _selectedStudent.SocialID);
                }
                else
                {
                    command.CommandText = "DELETE FROM Students WHERE ID = $id";
                    command.Parameters.AddWithValue("$id", _selectedStudentOldID);
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
