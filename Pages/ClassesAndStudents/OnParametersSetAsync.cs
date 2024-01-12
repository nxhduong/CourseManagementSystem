using Microsoft.Data.Sqlite;
using Microsoft.AspNetCore.Components;
using CourseManagementSystem.Models;

namespace CourseManagementSystem.Pages.ClassesAndStudents
{
    public partial class ClassesAndStudentsPage : ComponentBase
    {
        protected override async Task OnParametersSetAsync()
        {
            // Authentication
            var sessionResult = await SessionStorage.GetAsync<PersonModel>("cms_access_token");
            _user = sessionResult.Success ? sessionResult.Value : null;

            if (_user?.IsStaff == true)
            {
                StateHasChanged();

                using var connection = new SqliteConnection("Data Source=Data/CMS_DATABASE.db;Mode=ReadOnly");
                connection.Open();
                var command = connection.CreateCommand();

                // Display classes upon successful authentication
                command.CommandText = "SELECT * FROM Classes";

                using (var reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        if (!_classesList.Contains(reader.GetString(0)))
                        {
                            _classesList.Add(reader.GetString(0));
                        }
                    }
                }

                // Show students when a class is selected
                if (Class is not null && _classesList.Contains(Class))
                {
                    command = connection.CreateCommand();
                    command.CommandText = "SELECT Class, StudentName, ID, Male, DOB, SocialID FROM Students ORDER BY ID";

                    using var reader = command.ExecuteReader();
                    while (reader.Read())
                    {
                        if (!_studentsList.Where(student => student[2] == reader.GetString(2)).Any())
                        {
                            var studentInfo = new string[6];
                            for (var i = 0; i < 6; i++)
                            {
                                studentInfo[i] = reader.GetString(i);
                            }
                            _studentsList.Add(studentInfo);
                        }
                    }

                    _class.OldClass = Class;
                }
                else if (_classesList.Count != 0)
                {
                    Navigation?.NavigateTo("/classes");
                }

                StateHasChanged();
            }
            else
            {
                Navigation?.NavigateTo("/login");
            }
        }
    }
}
