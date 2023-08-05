namespace CourseManagementSystem.Data
{
    public enum VisitorType
    {
        Anononymous,
        Student,
        Staff
    }

    public class UserSessionService
    {
        public VisitorType CheckLoginCredentials(string cookie)
        {
            return VisitorType.Anononymous;
        }
    }
}
