using System.Text;
using TypingMaster.Business.Contract;
using TypingMaster.Core.Models;
using TypingMaster.Core.Models.Courses;

namespace TypingMaster.Business.Course;

public class BeginnerCourse : ServiceBase, ICourse
{
    public BeginnerCourse(Serilog.ILogger logger, string lessonDataFileUrl = "") : base(logger)
    {
        Type = TrainingType.Course;
        LessonDataUrl = string.IsNullOrEmpty(lessonDataFileUrl)
            ? "Resources/LessonData/beginner-course-lessons.json"
            : lessonDataFileUrl;
        CompleteText = "";
        Description = CourseDescription;
    }

    private const string CourseDescription =
        "Master Touch Typing from Scratch: This structured beginner's course guides you from the home row keys (a, s, d, f, j, k, l, ;) to full keyboard proficiency. Starting with your finger placement on home keys, each lesson gradually introduces new keys while reinforcing previously learned ones. Progress at your own pace through interactive exercises designed to build muscle memory, improve accuracy, and increase typing speed. Perfect for new typists or anyone looking to develop proper touch typing technique without looking at the keyboard. Track your WPM and accuracy as you transform from hunt-and-peck to confident touch typing.";

    // List of common English words with limited character sets
    private static readonly Dictionary<string, string[]> CommonWords = new()
    {
        ["asdf"] = ["as", "sad", "fad", "dad", "add", "sass", "fads", "ads"],
        ["asdfj"] = ["ads", "sad", "fad", "dad", "add", "jazz", "fads", "jaff"],
        ["asdfjk"] = ["ask", "ads", "sad", "fad", "dad", "jack", "fads", "skaj"],
        ["asdfjkl"] = ["ask", "all", "fall", "dads", "laff", "flask", "fall", "lads"],
        ["asdfjkl;"] = ["all;", "fall;", "dad;", "ask;", "jak;", "lads;", "ads;", "sad;"],
        // Additional dictionaries will be populated dynamically as needed
    };

    private static readonly Random Random = new Random();

    public Guid Id { get; set; }

    public string Name { get; set; }

    public TrainingType Type { get; }

    public string LessonDataUrl { get; }

    public IEnumerable<Lesson> Lessons { get; set; }

    public string CompleteText { get; }

    public CourseSetting Settings { get; set; }

    public string Description { get; }

    public Lesson? GetPracticeLesson(int curLessonId, StatsBase stats)
    {
        ArgumentException.ThrowIfNullOrEmpty(nameof(stats));
        if (Settings?.TargetStats is null)
        {
            ProcessResult.AddError("Target stats not set.");
            return null;
        }

        Lesson lesson;
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

        lesson.PracticeText = GeneratePracticeText(lesson.Target);
        return lesson;
    }

    private string GeneratePracticeText(IEnumerable<string> targetKeys)
    {
        var enumerable = targetKeys.ToList();
        var targetKeySet = string.Concat(enumerable.Select(k => k));
        if (!CommonWords.TryGetValue(targetKeySet, out var words))
        {
            throw new InvalidOperationException($"No common words found for the target keys: {targetKeySet}");
        }

        var practiceText = new StringBuilder();
        var targetKeysList = enumerable.ToList();

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
            var word = words[Random.Next(words.Length)];
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