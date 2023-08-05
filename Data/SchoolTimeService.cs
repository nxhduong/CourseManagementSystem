namespace CourseManagementSystem.Data
{
    public class SchoolTimeService
    {
        public Task<string> GetCurrentSemester(DateTime dateTime)
        {
            return Task.FromResult(dateTime.ToString());
        }
    }
}
