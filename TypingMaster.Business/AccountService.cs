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
            GoalStats = new TypingStats
            {
                Wpm = 60,
                Accuracy = 98.5
            },
            Progress =
            [
                new LearningProgress
                {
                    CourseId = 1,
                    LessonId = 1,
                    Stats = new TypingStats
                    {
                        Wpm = 30,
                        Accuracy = 79.0
                    }
                }
            ],
            PracticeTime = 15.0, // Total practice time in hours
            CurrentCourse = _courseService.GetCourse(1)
        };
    }
}