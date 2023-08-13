namespace CourseManagementSystem.Pages.Shared
{
    public partial class MainLayout
    {
        private string? _currentSchoolYearAndSemester;
        private readonly PeriodicTimer _timer = new(TimeSpan.FromSeconds(30));

        protected override async Task OnInitializedAsync()
        {
            _currentSchoolYearAndSemester = await SchoolTime.GetCurrentSemester(DateTime.Now);
            StateHasChanged();
            _ = StartClock();
        }

        protected async Task StartClock()
        {
            while (await _timer.WaitForNextTickAsync())
            {
                _currentSchoolYearAndSemester = await SchoolTime.GetCurrentSemester(DateTime.Now);
                await InvokeAsync(() => StateHasChanged());
            }
        }
    }
}
