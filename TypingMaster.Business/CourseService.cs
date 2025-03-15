using Serilog;
using TypingMaster.Business.Contract;
using TypingMaster.Business.Models;
using TypingMaster.Business.Models.Courses;

namespace TypingMaster.Business;

public class CourseService : ServiceBase, ICourseService
{
    private readonly List<ICourse> _courses = [];
    public static Guid CourseId1 = new("AB7E8988-4E54-435F-9DC3-25D3193EC378");
    public static Guid AllKeyTestCourseId = new("B326B0D9-F44C-4206-BE3B-301824817EEA");

    // todo: redesign the lesson instructions to HTML format
    private static readonly Dictionary<string, (string Instruction, string Description)> LessonInstructions = new()
    {
        {
            "asdf",
            ("Lessons begin with the home keys for the left hands: 'ASDF' Place your left hand fingers over the 'A' key; ring finger over 'S'; middle finger over 'D'; and index finger over 'F'. Keep the fingers curved, relaxed and poised just over the home keys.",
                "")
        },
        { "j", ("Use the index finger of the right hand to strike the 'J' key. ", "") },
        { "k", ("Use the middle finger of the right hand to strike the 'K' key. ", "") },
        { "l", ("Use the ring finger of the right hand to strike the 'L' key. ", "") },
        { ";", ("Use the little finger of the right hand to strike the ';' key. ", "") },
        { "rightHomeKeyBegin", ("The next lesson introduces a new home key '{0}'.", "") },
        {
            "rightHomeKeysEnd",
            ("Remember to position your fingers over the home keys and reach from there to all other parts of the keyboard",
                "")
        },
        {
            "capitalKey",
            ("So far, lessons have used lower case characters only. New lessons will use upper case letters as well. Upper case key letters are typed by holding down a Shift key at the same time the letter is pressed.\r\n\r\nThere are two Shift keys, one for each hand. Hold the Shift key down correct finger on the other hand.\r\n\r\nFor example, to type an upper case 'E', press the Shift key on the right side of the keyboard with the right little finger. While holding the Shift key down, press the 'E' key with the left middle finger.",
                "")
        },
        { "e", ("Use your left middle finger to strike both the 'd' and 'e' keys.", "") },
        { "i", ("Use your right middle finger to strike both the 'k' and 'i' keys.", "") },
    };

    // todo: create a new way to get all key test course
    // todo: move the text to a JSON data file
    public CourseService(ILogger logger) : base(logger)
    {
        var course = new AdvancedLevelCourse()
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

        // todo: create a new way to get speed test course
        // todo: move the text to a JSON data file
        course = new AdvancedLevelCourse()
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
        var course = id == null ? _courses.FirstOrDefault(c => c.Type == TrainingType.AllKeysTest) : _courses.FirstOrDefault(c => c.Type == TrainingType.AllKeysTest && c.Id == id);

        return Task.FromResult<ICourse>(course);
    }

    public Task<ICourse> GenerateBeginnerCourse(CourseSetting settings)
    {
        var course = new BeginnerCourse
        {
            Id = Guid.NewGuid(),
            Name = "Beginner Typing Course",
            Type = TrainingType.Course,
            Settings = settings,
            Lessons = GenerateBeginnerCourseLessons(settings),

            // Get complete text by combining all lesson texts
            //CompleteText = string.Join("\n\n", Lessons.Select(l => l.PracticeTexts));
        };

        return Task.FromResult<ICourse>(course);
    }

    // todo: move the text to a JSON data file
    private IEnumerable<Lesson> GenerateBeginnerCourseLessons(CourseSetting settings)
    {
        try
        {
            var lessons = new List<Lesson>
            {
                new()
                {
                    Id = 1,
                    Target = ["a", "s", "d", "f"],
                    Instruction = LessonInstructions["asdf"].Instruction,
                    Description = LessonInstructions["asdf"].Description,
                    Point = 1
                },
                new()
                {
                    Id = 2,
                    Target = ["a", "s", "d", "f", "j"],
                    Instruction =
                        $"{string.Format(LessonInstructions["rightHomeKeyBegin"].Instruction, "J")} {LessonInstructions["j"].Instruction} {LessonInstructions["rightHomeKeysEnd"].Instruction}",
                    Description = "",
                    Point = 2
                },
                new()
                {
                    Id = 3,
                    Target = ["a", "s", "d", "f", "j", "k"],
                    Instruction =
                        $"{string.Format(LessonInstructions["rightHomeKeyBegin"].Instruction, "K")} {LessonInstructions["k"].Instruction} {LessonInstructions["rightHomeKeysEnd"].Instruction}",
                    Description = "",
                    Point = 3
                },
                new()
                {
                    Id = 4,
                    Target = ["a", "s", "d", "f", "j", "k", "l"],
                    Instruction =
                        $"{string.Format(LessonInstructions["rightHomeKeyBegin"].Instruction, "L")} {LessonInstructions["l"].Instruction} {LessonInstructions["rightHomeKeysEnd"].Instruction}",
                    Description = "",
                    Point = 4
                },
                new()
                {
                    Id = 5,
                    Target = ["a", "s", "d", "f", "j", "k", "l", ";"],
                    Instruction =
                        $"{string.Format(LessonInstructions["rightHomeKeyBegin"].Instruction, ";")} {LessonInstructions[";"].Instruction} {LessonInstructions["rightHomeKeysEnd"].Instruction}",
                    Description = "",
                    Point = 5
                },
                new()
                {
                    Id = 6,
                    Target = ["d", "e"],
                    Instruction =
                        $"{LessonInstructions["e"].Instruction} {LessonInstructions["rightHomeKeysEnd"].Instruction}",
                    Description = "E",
                    Point = 1
                },
                new()
                {
                    Id = 7,
                    Target = ["k", "i"],
                    Instruction =
                        $"{LessonInstructions["i"].Instruction} {LessonInstructions["rightHomeKeysEnd"].Instruction}",
                    Description = "I",
                    Point = 1
                },
                new()
                {
                    Id = 8,
                    Target = ["a", "s", "d", "f", "j", "k", "l", "d", "e", "k", "i", ";"],
                    Instruction =
                        $"{LessonInstructions["i"].Instruction} {LessonInstructions["rightHomeKeysEnd"].Instruction}",
                    Description = "",
                    Point = 5
                },
                new()
                {
                    Id = 9,
                    Target = ["a", "s", "d", "f", "j", "k", "l", "d", "e", "k", "i", "r", ";"],
                    Instruction =
                        $"{LessonInstructions["i"].Instruction} {LessonInstructions["rightHomeKeysEnd"].Instruction}",
                    Description = "R",
                    Point = 5
                },
                new()
                {
                    Id = 10,
                    Target = ["a", "s", "d", "f", "j", "k", "l", "d", "e", "k", "i", "r", "u", ";"],
                    Instruction =
                        $"{LessonInstructions["i"].Instruction} {LessonInstructions["rightHomeKeysEnd"].Instruction}",
                    Description = "U",
                    Point = 5
                },
                new()
                {
                    Id = 11,
                    Target = ["a", "s", "d", "f", "j", "k", "l", "d", "e", "k", "i", "r", "u", "t", ";"],
                    Instruction =
                        $"{LessonInstructions["i"].Instruction} {LessonInstructions["rightHomeKeysEnd"].Instruction}",
                    Description = "T",
                    Point = 5
                },
                new()
                {
                    Id = 12,
                    Target = ["a", "s", "d", "f", "j", "k", "l", "d", "e", "k", "i", "r", "u", "t", "y", ";"],
                    Instruction =
                        $"{LessonInstructions["i"].Instruction} {LessonInstructions["rightHomeKeysEnd"].Instruction}",
                    Description = "Y",
                    Point = 5
                },
                new()
                {
                    Id = 13,
                    Target = ["a", "s", "d", "f", "j", "k", "l", "d", "e", "k", "i", "r", "u", "t", "y", "w", ";"],
                    Instruction =
                        $"{LessonInstructions["i"].Instruction} {LessonInstructions["rightHomeKeysEnd"].Instruction}",
                    Description = "W",
                    Point = 5
                },
                new()
                {
                    Id = 14,
                    Target = ["a", "s", "d", "f", "j", "k", "l", "d", "e", "k", "i", "r", "u", "t", "y", "w", "o", ";"],
                    Instruction =
                        $"{LessonInstructions["i"].Instruction} {LessonInstructions["rightHomeKeysEnd"].Instruction}",
                    Description = "O",
                    Point = 5
                },
                new()
                {
                    Id = 15,
                    Target =
                    [
                        "a", "s", "d", "f", "j", "k", "l", "d", "e", "k", "i", "r", "u", "t", "y", "w", "o", "q", ";"
                    ],
                    Instruction =
                        $"{LessonInstructions["i"].Instruction} {LessonInstructions["rightHomeKeysEnd"].Instruction}",
                    Description = "Q",
                    Point = 5
                },
                new()
                {
                    Id = 16,
                    Target =
                    [
                        "a", "s", "d", "f", "j", "k", "l", "d", "e", "k", "i", "r", "u", "t", "y", "w", "o", "q", "p",
                        ";"
                    ],
                    Instruction =
                        $"{LessonInstructions["i"].Instruction} {LessonInstructions["rightHomeKeysEnd"].Instruction}",
                    Description = "P",
                    Point = 5
                },
                new()
                {
                    Id = 17,
                    Target =
                    [
                        "a", "s", "d", "f", "j", "k", "l", "d", "e", "k", "i", "r", "u", "t", "y", "w", "o", "q", "p",
                        "c", ";"
                    ],
                    Instruction =
                        $"{LessonInstructions["i"].Instruction} {LessonInstructions["rightHomeKeysEnd"].Instruction}",
                    Description = "C",
                    Point = 5
                },
                new()
                {
                    Id = 18,
                    Target =
                    [
                        "a", "s", "d", "f", "j", "k", "l", "d", "e", "k", "i", "r", "u", "t", "y", "w", "o", "q", "p",
                        "c", "n", ";"
                    ],
                    Instruction =
                        $"{LessonInstructions["i"].Instruction} {LessonInstructions["rightHomeKeysEnd"].Instruction}",
                    Description = "N",
                    Point = 5
                },
                new()
                {
                    Id = 19,
                    Target =
                    [
                        "a", "s", "d", "f", "j", "k", "l", "d", "e", "k", "i", "r", "u", "t", "y", "w", "o", "q", "p",
                        "c", "n", "v", ";"
                    ],
                    Instruction =
                        $"{LessonInstructions["i"].Instruction} {LessonInstructions["rightHomeKeysEnd"].Instruction}",
                    Description = "V",
                    Point = 5
                },
                new()
                {
                    Id = 20,
                    Target =
                    [
                        "a", "s", "d", "f", "j", "k", "l", "d", "e", "k", "i", "r", "u", "t", "y", "w", "o", "q", "p",
                        "c", "n", "m", ";"
                    ],
                    Instruction =
                        $"{LessonInstructions["i"].Instruction} {LessonInstructions["rightHomeKeysEnd"].Instruction}",
                    Description = "M",
                    Point = 5
                },
                new()
                {
                    Id = 21,
                    Target =
                    [
                        "a", "s", "d", "f", "j", "k", "l", "d", "e", "k", "i", "r", "u", "t", "y", "w", "o", "q", "p",
                        "c", "n", "m", "x", ";"
                    ],
                    Instruction =
                        $"{LessonInstructions["i"].Instruction} {LessonInstructions["rightHomeKeysEnd"].Instruction}",
                    Description = "X",
                    Point = 5
                },
                new()
                {
                    Id = 22,
                    Target =
                    [
                        "a", "s", "d", "f", "j", "k", "l", "d", "e", "k", "i", "r", "u", "t", "y", "w", "o", "q", "p",
                        "c", "n", "m", "x", ",", ";"
                    ],
                    Instruction =
                        $"{LessonInstructions["i"].Instruction} {LessonInstructions["rightHomeKeysEnd"].Instruction}",
                    Description = ",",
                    Point = 5
                },
                new()
                {
                    Id = 23,
                    Target =
                    [
                        "a", "s", "d", "f", "j", "k", "l", "d", "e", "k", "i", "r", "u", "t", "y", "w", "o", "q", "p",
                        "c", "n", "m", "x", ",", "x", ";"
                    ],
                    Instruction =
                        $"{LessonInstructions["i"].Instruction} {LessonInstructions["rightHomeKeysEnd"].Instruction}",
                    Description = "Z",
                    Point = 5
                },
                new()
                {
                    Id = 24,
                    Target =
                    [
                        "a", "s", "d", "f", "j", "k", "l", "d", "e", "k", "i", "r", "u", "t", "y", "w", "o", "q", "p",
                        "c", "n", "m", "x", ",", "x", ".", ";"
                    ],
                    Instruction =
                        $"{LessonInstructions["i"].Instruction} {LessonInstructions["rightHomeKeysEnd"].Instruction}",
                    Description = ".",
                    Point = 5
                },
                new()
                {
                    Id = 24,
                    Target =
                    [
                        "a", "s", "d", "f", "j", "k", "l", "d", "e", "k", "i", "r", "u", "t", "y", "w", "o", "q", "p",
                        "c", "n", "m", "x", ",", "x", ".", ";"
                    ],
                    Instruction =
                        $"{LessonInstructions["capitalKey"].Instruction} {LessonInstructions["rightHomeKeysEnd"].Instruction}",
                    Description = ".",
                    Point = 5
                },
            };

            return lessons;
        }
        catch (Exception ex)
        {
            ProcessResult.AddException(ex);
            return null;
        }
    }
}