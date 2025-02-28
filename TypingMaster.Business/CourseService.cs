using TypingMaster.Business.Contract;
using TypingMaster.Business.Models;
using TypingMaster.Business.Models.Courses;

namespace TypingMaster.Business;

public class CourseService : ICourseService
{
    public Task<ICourse> GetCourse(int id)
    {
        var course = new AdvancedLevelCourse()
        {
            Id = 1,
            Lessons =
                [
                    new Lesson
                    {
                        Id = 1,
                        PracticeText = "This is the introductory material for typing.",
                        Point = 1,
                        Description = "The course advances to the next level of lessons if the current typing performance level is equal to or above the advanced level."
                    },

                    new Lesson
                    {
                        Id = 2,
                        PracticeText = "This is the introductory beginner material for typing.",
                        Point = 1,
                        Description = "The course advances to the next level of lessons if the current typing performance level is equal to or above the advanced level."
                    },
                    new Lesson
                    {
                        Id = 3,
                        PracticeText = "This is the introductory Novice material for typing.",
                        Point = 2,
                        Description = "The course advances to the next level of lessons if the current typing performance level is equal to or above the advanced level."
                    },
                    new Lesson
                    {
                        Id = 4,
                        PracticeText = "This is the introductory Intermediate material for typing.",
                        Point = 3,
                        Description = "The course advances to the next level of lessons if the current typing performance level is equal to or above the advanced level."
                    },
                    new Lesson
                    {
                        Id = 5,
                        PracticeText = "This is the introductory Advanced material for typing.",
                        Point = 4,
                        Description = "The course advances to the next level of lessons if the current typing performance level is equal to or above the advanced level."
                    },
                    new Lesson
                    {
                        Id = 6,
                        PracticeText = "This is the introductory Expert material for typing.",
                        Point = 5,
                        Description = "The course advances to the next level of lessons if the current typing performance level is equal to or above the advanced level."
                    },
                ]
        };

        return Task.FromResult<ICourse>(course);
    }
}