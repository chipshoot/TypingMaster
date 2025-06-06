using System.Text;
using TypingMaster.Business.Contract;
using TypingMaster.Core.Constants;
using TypingMaster.Core.Models;
using TypingMaster.Core.Models.Courses;

namespace TypingMaster.Business.Course;

public class BeginnerCourse(Serilog.ILogger logger, string lessonDataFileUrl = "") : ServiceBase(logger), ICourse
{
    private const string CourseDescription =
        "Master Touch Typing from Scratch: This structured beginner's course guides you from the home row keys (a, s, d, f, j, k, l, ;) to full keyboard proficiency. Starting with your finger placement on home keys, each lesson gradually introduces new keys while reinforcing previously learned ones. Progress at your own pace through interactive exercises designed to build muscle memory, improve accuracy, and increase typing speed. Perfect for new typists or anyone looking to develop proper touch typing technique without looking at the keyboard. Track your WPM and accuracy as you transform from hunt-and-peck to confident touch typing.";

    private static readonly Random Random = new();

    public Guid Id { get; set; }

    public string Name { get; set; } = TypingMasterConstants.BeginnerCourseName;

    public TrainingType Type { get; } = TrainingType.Course;

    public string LessonDataUrl { get; } = string.IsNullOrEmpty(lessonDataFileUrl)
        ? "Resources/LessonData/beginner-course-lessons.json"
        : lessonDataFileUrl;

    public IEnumerable<Lesson> Lessons { get; set; } = [];

    public string CompleteText { get; } = "";

    public CourseSetting Settings { get; set; } = null!;

    public string Description { get; } = CourseDescription;

    public Lesson? GetPracticeLesson(int curLessonId, StatsBase stats)
    {
        ArgumentException.ThrowIfNullOrEmpty(nameof(stats));
        if (Settings?.TargetStats is null)
        {
            ProcessResult.AddError("Target stats not set.");
            return null;
        }

        Lesson? lesson;
        if (stats >= Settings.TargetStats)
        {
            // Target achieved, move to next lesson
            var maxLessonId = Lessons.Max(l => l.Id);
            if (maxLessonId == curLessonId)
            {
                var completeLesson = new Lesson
                {
                    Id = curLessonId,
                    IsCourseComplete = true,
                    Instruction = CompleteText,
                    PracticeText = string.Empty
                };
                return completeLesson;
            }

            lesson = Lessons.FirstOrDefault(l => l.Id == curLessonId + 1);
        }
        else
        {
            lesson = Lessons.FirstOrDefault(l => l.Id == curLessonId);
        }

        if (lesson == null)
        {
            throw new Exception("Cannot found lesson");
        }

        lesson.PracticeText = GeneratePracticeText(lesson.Target, lesson.CommonWords);
        return lesson;
    }

    private string GeneratePracticeText(IEnumerable<string> targetKeys, string[] commonWords)
    {
        var practiceText = new StringBuilder();
        var targetKeysList = targetKeys.ToList();

        // Generate 1-5 random strings from target keys
        var randomKeyStringsCount = Random.Next(1, 6); // Random number between 1 and 5
        for (var i = 0; i < randomKeyStringsCount; i++)
        {
            if (practiceText.Length > 0)
            {
                practiceText.Append(' ');
            }

            // Create a random string from target keys
            var keyStringLength = Random.Next(2, 5); // Random length between 2 and 4
            var keyString = new StringBuilder();
            for (var j = 0; j < keyStringLength; j++)
            {
                keyString.Append(targetKeysList[Random.Next(targetKeysList.Count)]);
            }

            practiceText.Append(keyString);
        }

        // Add random words from the dictionary
        while (practiceText.Length < Settings.PracticeTextLength)
        {
            var word = commonWords[Random.Next(commonWords.Length)];
            if (practiceText.Length + word.Length + 1 > Settings.PracticeTextLength)
            {
                break;
            }

            if (practiceText.Length > 0)
            {
                practiceText.Append(' ');
            }

            practiceText.Append(word);
        }

        return practiceText.ToString();
    }
}