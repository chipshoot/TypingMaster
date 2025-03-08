using TypingMaster.Business.Contract;
using TypingMaster.Business.Models;
using TypingMaster.Business.Models.Courses;

namespace TypingMaster.Business;

public class CourseService : ICourseService
{
    private readonly List<ICourse> _courses = [];
    public static Guid CourseId1 = new ("AB7E8988-4E54-435F-9DC3-25D3193EC378");
    public static Guid AllKeyTestCourseId = new ("B326B0D9-F44C-4206-BE3B-301824817EEA");

    public CourseService()
    {
        var course = new AdvancedLevelCourse(CourseType.Practice)
        {
            Id = CourseId1,
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
        _courses.Add(course);


        course = new AdvancedLevelCourse(CourseType.AllKeysTest)
        {
            Id = AllKeyTestCourseId,
            Lessons =
                [
                    new Lesson
                    {

                        Id = 1,
                        PracticeText = "hAnother sharp-witted character, _;  The aerialist's phone number was (217) 389-7557.  The freeloaders expanded by this den located at 97191 Straus Square, Piscataway, Ohio 67107-0614! \"Boy!\" she vocalized.  This understandably zealous physician was jesting regarding my executive around 23 Heyl Plaza, Quantico, Alabama 92780-8450!  The jaywalkers hurled beside the piazza located at 57 Arrington Center, Hatfield, DE",
                        Point = 1,
                        Description = "The course advances to the next level of lessons if the current typing performance level is equal to or above the advanced level."
                    },
                    new Lesson
                    {

                        Id = 2,
                        PracticeText = "Taking a test will give this program a basic for customizing individual lessons. Don't be concerned if you aren't typing as quickly as you can; you will soon excel.  Everyone has a quandary with number and symbol keys.  Don't expect to zoom through these next sentences - just do your best! \"Her birthday is 08/154/53.\"  Does 73% of $546.00 = $98.58? Item #32 (jonquils)  sold 2 units @ $6 & 1 @ $73. *+* That's all folks! +*+",
                        Point = 1,
                        Description = "The course advances to the next level of lessons if the current typing performance level is equal to or above the advanced level."
                    },
                    new Lesson
                    {
                        Id = 3,
                        PracticeText = "SHIFTING thoughts, typing FLUENTLY, developing SPEED! Caps Lock may be a friend or foe—KNOW when to use it. Function keys (F1-F12) aid shortcuts. Ctrl + C copies, Ctrl + V pastes, and Alt + Tab swaps. The Escape key, a way out. Don't ignore the power of the ENTER key—confirming, submitting, breaking lines.  Every key counts, so practice with purpose!",
                        Point = 1,
                        Description = "The course advances to the next level of lessons if the current typing performance level is equal to or above the advanced level."
                    },
                    new Lesson
                    {
                        Id = 4,
                        PracticeText = "Coding requires symbols: {} for blocks, [] for arrays, <> for comparisons. The backslash \\ is essential in file paths, escaping characters. Quotes \"\" and '' frame strings, while ` allows inline code. The spacebar, a silent hero, controls rhythm. Typing, like music, has a flow—staccato or smooth.  To improve, practice! Type, correct, repeat, master.",
                        Point = 2,
                        Description = "The course advances to the next level of lessons if the current typing performance level is equal to or above the advanced level."
                    },
                    new Lesson
                    {
                        Id = 5,
                        PracticeText = "Emails need @, URLs need .com, passwords mix Aa1! Common shortcuts: Ctrl + S saves, Ctrl + Z undoes, Ctrl + X cuts. Mac users prefer Cmd over Ctrl. The numpad—quicker digits, faster math. Ergonomic posture boosts accuracy. Fingers rest on ASDF JKL;—home row anchors speed. Keep going, improving, perfecting!  Type on, challenge yourself, enjoy the process.",
                        Point = 3,
                        Description = "The course advances to the next level of lessons if the current typing performance level is equal to or above the advanced level."
                    },
                    new Lesson
                    {
                        Id = 6,
                        PracticeText = "Practice drills: Type \"The $ sign matters\"—spot it? Press Tab \u2192 it indents. Press Backspace \u2190 it erases. Use Delete \u27a1 it clears ahead. Adjust Caps \u21ea if needed. Special characters? Try Alt + 0153 (\u2122) or Alt + 0169 (\u00a9). Ever typed in different languages? ¡Sí! Voilà! 汉字也行. Typing unites, connects, empowers.  Never stop—keep practicing!",
                        Point = 4,
                        Description = "The course advances to the next level of lessons if the current typing performance level is equal to or above the advanced level."
                    }
                ]
        };
        _courses.Add(course);
    }

    public Task<ICourse?> GetCourse(Guid id)
    {
        var course = _courses.FirstOrDefault(c => c.Id == id);
        return Task.FromResult<ICourse>(course);
    }

    public Task<ICourse?> GetAllKeysCourse(Guid? id)
    {
        var course = id == null ? _courses.FirstOrDefault(c => c.Type == CourseType.AllKeysTest) : _courses.FirstOrDefault(c => c.Type == CourseType.AllKeysTest && c.Id == id);

        return Task.FromResult<ICourse>(course);
    }
}