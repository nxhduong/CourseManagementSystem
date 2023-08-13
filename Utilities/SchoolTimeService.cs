using Microsoft.Data.Sqlite;
using System.Globalization;

namespace CourseManagementSystem.Utilities
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

                while (reader.Read())
                {
                    try
                    {
                        startDate = DateTime.ParseExact(reader.GetString(2), "dd/MM/yyyy", CultureInfo.InvariantCulture);
                        endDate = DateTime.ParseExact(reader.GetString(3), "dd/MM/yyyy", CultureInfo.InvariantCulture);

                        if (DateTime.Compare(today, startDate) >= 0 && DateTime.Compare(today, endDate) <= 0)
                        {
                            return Task.FromResult($"Current Semester: {reader.GetString(1)} ({reader.GetString(0)})");
                        }
                    }
                    catch (FormatException err)
                    {
                        Console.Error.WriteLine(err);
                        break;
                    }
                }
            }
            return Task.FromResult(today.ToString());
        }
    }
}
