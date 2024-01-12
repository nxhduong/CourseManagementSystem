using Microsoft.AspNetCore.Components;
using Microsoft.Data.Sqlite;

namespace CourseManagementSystem.Pages.ClassesAndStudents
{
    public partial class ClassesAndStudentsPage : ComponentBase
    {
        private void ModifyClass()
        {
            var classFound = _classesList.Contains(_class.NewClass ?? "");

            using var connection = new SqliteConnection("Data Source=Data/CMS_DATABASE.db;Mode=ReadWrite");
            connection.Open();
            var command = connection.CreateCommand();

            // Create class
            if (string.IsNullOrWhiteSpace(_class.OldClass) && !string.IsNullOrWhiteSpace(_class.NewClass) && !classFound)
            {
                command.CommandText = "INSERT INTO Classes VALUES ($class)";
                command.Parameters.AddWithValue("$class", _class.NewClass);
            }
            // Delete class
            else if (!string.IsNullOrWhiteSpace(_class.OldClass) && string.IsNullOrWhiteSpace(_class.NewClass) && classFound)
            {
                command.CommandText = "DELETE FROM Classes WHERE Class = $class";
                command.Parameters.AddWithValue("$class", _class.OldClass);
            }
            // Rename class
            else if (!string.IsNullOrWhiteSpace(_class.OldClass) && !string.IsNullOrWhiteSpace(_class.NewClass) && classFound)
            {
                command.CommandText = "UPDATE Classes SET Classes = $newclass WHERE Classes = $oldclass";
                command.Parameters.AddWithValue("$oldclass", _class.OldClass);
                command.Parameters.AddWithValue("$newclass", _class.NewClass);
            }
            else
            {
                _errorMessage = "Invalid input";
            }

            command.ExecuteNonQuery();
            StateHasChanged();
        }
    }
}
