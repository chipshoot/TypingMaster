using TypingMaster.Business.Contract;
using TypingMaster.Business.Models;

namespace TypingMaster.Business;

public class CourseService : ICourseService
{
    public Course GetCourse(int id)
    {
        return new Course
        {
            Id = 1,
            Lessons =
                [
                    new Lesson
                    {
                        Id = 1,
                        PracticeText = "This is the introductory material for typing.",
                        Point = 1
                    },

                    new Lesson
                    {
                        Id = 2,
                        PracticeText = "This is the introductory beginner material for typing.",
                        Point = 1
                    },
                    new Lesson
                    {
                        Id = 3,
                        PracticeText = "This is the introductory Novice material for typing.",
                        Point = 2
                    },
                    new Lesson
                    {
                        Id = 4,
                        PracticeText = "This is the introductory Intermediate material for typing.",
                        Point = 3
                    },
                    new Lesson
                    {
                        Id = 5,
                        PracticeText = "This is the introductory Advanced material for typing.",
                        Point = 4
                    },
                    new Lesson
                    {
                        Id = 6,
                        PracticeText = "This is the introductory Expert material for typing.",
                        Point = 5
                    },
                ]
        };
    }
}