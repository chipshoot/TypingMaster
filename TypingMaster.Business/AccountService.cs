using TypingMaster.Business.Contract;
using TypingMaster.Business.Models;

namespace TypingMaster.Business;

public class AccountService(ICourseService courseService) : IAccountService
{
    private readonly ICourseService _courseService = courseService ?? throw new ArgumentException(nameof(courseService));

    public Account GetAccount(int id)
    {
        return new Account
        {
            Id = id,
            AccountName = "SampleUser",
            Email = "sample.user@example.com",
            User = new UserProfile
            {
                FirstName = "Sample",
                LastName = "User",
                Title = "Mr."
            },
            GoalStats = new DrillStats
            {
                Wpm = 60,
                Accuracy = 98.5
            },
            History =
                new PracticeLog
                {
                    CurrentCourseId = 1,
                    CurrentLessonId = 1,
                    PracticeStats = [],
                    KeyStats = [],
                    PracticeDuration = 0
                },
            CurrentCourse = _courseService.GetCourse(1)
        };
    }
}