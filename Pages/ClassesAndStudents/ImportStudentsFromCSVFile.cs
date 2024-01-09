using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;
using Microsoft.Data.Sqlite;
using System.Text;

namespace CourseManagementSystem.Pages.ClassesAndStudents
{
    public partial class ClassesAndStudentsPage : ComponentBase
    {
        private async Task ImportStudentsFromCSVFile(InputFileChangeEventArgs eventArgs)
        {
            // Add rows from valid CSV file to list and database
            if (eventArgs.File.Size <= 2000000 && eventArgs.File.Name.Substring(eventArgs.File.Name.Length - 3, 3).Equals("csv", StringComparison.CurrentCultureIgnoreCase))
            {
                using var connection = new SqliteConnection("Data Source=Data/CMS_DATABASE.db;Mode=ReadWrite");
                connection.Open();

                var memoryStream = new MemoryStream();
                await eventArgs.File.OpenReadStream().CopyToAsync(memoryStream);
                var rows = Encoding.UTF8.GetString(memoryStream.ToArray()).Split("\n");


                for (var i = 1; i < rows.Length; i++)
                {
                    var data = rows[i].Split(",");
                    _duplicates = from student in _studentsList
                                  where student[2].Equals(student[2], StringComparison.CurrentCultureIgnoreCase)
                                  select student[2];

                    if (data.Length != 5)
                    {
                        _errorMessage = "Number of columns must be exactly 5";
                        return;
                    }
                    if (!_duplicates.Any())
                    {
                        _studentsList.Add([Class!, data[0], data[1], data[2], data[3], data[4]]);

                        var command = connection.CreateCommand();
                        command.CommandText = $"INSERT INTO Students VALUES ($class, $studentname, $id, $male, $dob, $socialid, '{_defaultHashPass}')";
                        command.Parameters.AddWithValue("$class", Class);
                        command.Parameters.AddWithValue("$studentname", data[0]);
                        command.Parameters.AddWithValue("$id", data[1]);
                        command.Parameters.AddWithValue("$male", data[2]);
                        command.Parameters.AddWithValue("$dob", data[3]);
                        command.Parameters.AddWithValue("$socialid", data[4]);
                        command.ExecuteNonQuery();
                    }
                }

                StateHasChanged();
            }
        }
    }
}
