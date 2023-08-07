using Microsoft.Data.Sqlite;
using System.Globalization;

namespace CourseManagementSystem.Data
{
    public class SchoolTimeService
    {
        public Task<string> GetCurrentSemester(DateTime dateTime)
        {
            var today = dateTime.Date;
            using (var connection = new SqliteConnection("Data Source=Data/CMS_DATABASE.db;Mode=ReadOnly"))
            {
                connection.Open();

                var command = connection.CreateCommand();
                command.CommandText = "SELECT * FROM Semesters";

                using var reader = command.ExecuteReader();

                DateTime startDate, endDate;
                CultureInfo local = new("vi-VN");

                while (reader.Read())
                {
                    try
                    {
                        startDate = DateTime.ParseExact(reader.GetString(2), "dd/MM/yyyy", local);
                        endDate = DateTime.ParseExact(reader.GetString(3), "dd/MM/yyyy", local);

                        if (DateTime.Compare(today, startDate) >= 0 && DateTime.Compare(today, endDate) <= 0)
                        {
                            return Task.FromResult($"Current Semester: {reader.GetString(1)} ({reader.GetString(0)})");
                        }
                    }
                    catch (FormatException err)
                    {
                        Console.WriteLine(err);
                        break;
                    }
                }
            }
            return Task.FromResult(today.ToString());
        }
    }
}
