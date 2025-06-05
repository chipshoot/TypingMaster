using System.Text;
using TypingMaster.Business.Contract;
using TypingMaster.Core.Constants;
using TypingMaster.Core.Models;
using TypingMaster.Core.Models.Courses;
using TypingMaster.Core.Utility;

namespace TypingMaster.Business.Course;

public class BeginnerCourse(Serilog.ILogger logger, IRandomNumberGenerator? randomGenerator = null, string lessonDataFileUrl = "") : ICourse
{
    private const string CourseDescription =
        "Master Touch Typing from Scratch: This structured beginner's course guides you from the home row keys (a, s, d, f, j, k, l, ;) to full keyboard proficiency. Starting with your finger placement on home keys, each lesson gradually introduces new keys while reinforcing previously learned ones. Progress at your own pace through interactive exercises designed to build muscle memory, improve accuracy, and increase typing speed. Perfect for new typists or anyone looking to develop proper touch typing technique without looking at the keyboard. Track your WPM and accuracy as you transform from hunt-and-peck to confident touch typing.";

    private const string RepetitionInst = "**Repetition Phase**: Focus on finger position and accuracy. Type the repeated characters.";
    private const string PatternsInst = "**Patterns Phase**: Practice key patterns to build finger coordination.";
    private const string RealWordsInst = "**RealWord Phase**: Apply your skills by typing these common words.";

    private const string KeyboardSectionNumber = "Number Row";

    private readonly IRandomNumberGenerator _randomGenerator = randomGenerator ?? new RandomNumberGenerator();
    private static readonly List<char> LeftHandDigits = ['1', '2', '3', '4', '5'];
    private static readonly List<char> RightHandDigits = ['6', '7', '8', '9', '0'];

    public Guid Id { get; set; }

    public string Name { get; set; } = TypingMasterConstants.BeginnerCourseName;

    public TrainingType Type { get; } = TrainingType.Course;

    public string LessonDataUrl { get; } = string.IsNullOrEmpty(lessonDataFileUrl)
        ? "Resources/LessonData/beginner-course-lessons.json"
        : lessonDataFileUrl;

    public IEnumerable<Lesson> Lessons { get; set; } = [];

    public string CompleteText { get; } = "";

    public int MaxCharacters { get; set; } = TypingMasterConstants.DefaultTypingWindowWidth;

    public CourseSetting Settings { get; set; } = null!;

    public string Description { get; } = CourseDescription;

    public ProcessResult ProcessResult { get; set; } = new(logger);

    // Add method to advance to next phase
    public PracticePhases AdvanceToNextPhase(PracticePhases phase, LessonType lessonType, StatsBase currentStats)
    {
        // Define thresholds for advancing to next phase
        var simpleRepetitionThreshold = new StatsBase { Wpm = 40, Accuracy = 95 };
        var patternsThreshold = new StatsBase { Wpm = 30, Accuracy = 90 };
        var realWordsThreshold = new StatsBase { Wpm = 35, Accuracy = 92 };

        switch (lessonType)
        {
            case LessonType.NotSet:
                return PracticePhases.NotSet;

            case LessonType.Practice:
            case LessonType.Test:
                return PracticePhases.RealWords;

            case LessonType.NewKey:

                // Only advance if user has met the threshold for current phase
                switch (phase)
                {
                    case PracticePhases.NotSet:
                        // When not set, always start with SimpleRepetition
                        return PracticePhases.SimpleRepetition;

                    case PracticePhases.SimpleRepetition when currentStats >= simpleRepetitionThreshold:
                        return PracticePhases.Patterns;

                    case PracticePhases.Patterns when currentStats >= patternsThreshold:
                        return PracticePhases.RealWords;

                    case PracticePhases.RealWords when currentStats >= realWordsThreshold:
                        // When mastering RealWords, set to NotSet to make service pick up next lesson
                        return PracticePhases.NotSet;

                    default:
                        // If threshold not met, stay in current phase
                        return phase;
                }
        }

        return phase;
    }

    public PracticeLessonResult? GetPracticeLesson(int curLessonId, StatsBase stats, PracticePhases phase)
    {
        Guard.AgainstNull(stats, nameof(stats));
        if (Settings?.TargetStats is null)
        {
            ProcessResult.AddError("Target stats not set.");
            return null;
        }

        var lesson = Lessons.FirstOrDefault(l => l.Id == curLessonId);
        if (lesson == null)
        {
            throw new Exception("Cannot found lesson");
        }

        if (stats >= Settings.TargetStats)
        {
            switch (lesson.Type)
            {
                case LessonType.NewKey:
                    if (phase == PracticePhases.NotSet)
                    {
                        // Target achieved, move to next lesson
                        var maxLessonId = Lessons.Max(l => l.Id);
                        if (maxLessonId == curLessonId)
                        {
                            return new PracticeLessonResult
                            {
                                Lesson = new Lesson
                                {
                                    Id = curLessonId,
                                    IsCourseComplete = true,
                                    Instruction = CompleteText,
                                    PracticeText = string.Empty
                                },
                                Phase = PracticePhases.NotSet,
                                LessonCount = Lessons.Count(),
                                TargetStats = Settings.TargetStats
                            };
                        }

                        lesson = Lessons.FirstOrDefault(l => l.Id == curLessonId + 1);
                    }

                    break;
                case LessonType.NotSet:
                case LessonType.Practice:
                case LessonType.Test:
                default:
                    lesson = Lessons.FirstOrDefault(l => l.Id == curLessonId + 1);
                    break;
            }
        }

        if (lesson == null)
        {
            throw new Exception("Cannot found lesson");
        }

        // Reset phase for new lesson
        var nextPhase = AdvanceToNextPhase(phase, lesson.Type, stats);
        if (!string.IsNullOrEmpty(lesson.Description))
        {
            lesson.PracticeText = lesson.Description.Contains(KeyboardSectionNumber)
                ? GeneratePracticeText(nextPhase)
                : GeneratePracticeText(lesson.Target, lesson.CommonWords, nextPhase);
        }
        else
        {
            lesson.PracticeText =
                GeneratePracticeText(lesson.Target, lesson.CommonWords, nextPhase);
        }

        lesson.Instruction = GetInstructionForPhase(lesson.Instruction, nextPhase);
        return new PracticeLessonResult
        {
            Lesson = lesson,
            Phase = nextPhase,
            LessonCount = Lessons.Count(),
            TargetStats = Settings.TargetStats
        };
    }

    private string GetInstructionForPhase(string baseInstruction, PracticePhases phase)
    {
        var cleanedInst= baseInstruction;
        if (cleanedInst.Contains(RepetitionInst))
        {
            cleanedInst= cleanedInst.Replace($"\n\n{RepetitionInst}", string.Empty);
        }
        if (cleanedInst.Contains(PatternsInst))
        {
            cleanedInst= cleanedInst.Replace($"\n\n{PatternsInst}", string.Empty);
        }
        if (cleanedInst.Contains(RealWordsInst))
        {
            cleanedInst= cleanedInst.Replace($"\n\n{RealWordsInst}", string.Empty);
        }

        var phaseText = phase switch
        {
            PracticePhases.SimpleRepetition => RepetitionInst,
            PracticePhases.Patterns => PatternsInst,
            PracticePhases.RealWords => RealWordsInst,
            _ => string.Empty
        };

        return $"{cleanedInst}\n\n{phaseText}";
    }

    public string GeneratePracticeText(IEnumerable<string> targetKeys, string[] commonWords, PracticePhases phase)
    {
        var practiceText = new StringBuilder();
        var targetKeysList = targetKeys.ToList();

        while (practiceText.Length < MaxCharacters)
        {
            switch (phase)
            {
                case PracticePhases.SimpleRepetition:
                    // PART 1: Simple repeated characters
                    foreach (var key in targetKeysList)
                    {
                        if (practiceText.Length > 0)
                        {
                            practiceText.Append(' ');
                        }

                        var repetitions = _randomGenerator.Next(2, 4);
                        practiceText.Append(new string(key[0], repetitions));
                    }

                    break;

                case PracticePhases.Patterns:
                    // PART 2: Patterns with the keys

                    // Generate 1-5 random strings from target keys
                    var randomKeyStringsCount = _randomGenerator.Next(1, 6); // Random number between 1 and 5
                    for (var i = 0; i < randomKeyStringsCount; i++)
                    {
                        if (practiceText.Length > 0)
                        {
                            practiceText.Append(' ');
                        }

                        // Create a random string from target keys
                        var keyStringLength = _randomGenerator.Next(2, 5); // Random length between 2 and 4
                        var keyString = new StringBuilder();
                        for (var j = 0; j < keyStringLength; j++)
                        {
                            keyString.Append(targetKeysList[_randomGenerator.Next(targetKeysList.Count)]);
                        }

                        practiceText.Append(keyString);
                    }

                    break;

                case PracticePhases.RealWords:
                case PracticePhases.NotSet:
                    // PART 3: Real words
                    var word = commonWords[_randomGenerator.Next(commonWords.Length)];
                    if (practiceText.Length > 0)
                    {
                        practiceText.Append(' ');
                    }

                    practiceText.Append(word);
                    break;
            }
        }

        // Trim practiceText beyond MaxCharacters
        if (practiceText.Length > MaxCharacters)
        {
            practiceText.Remove(MaxCharacters, practiceText.Length - MaxCharacters);
        }

        return practiceText.ToString();
    }

    public string GeneratePracticeText(PracticePhases phase)
    {
        var practiceText = new StringBuilder();
        var repeatCount = 3;
        var comboRepeat = 2;

        while (practiceText.Length < MaxCharacters)
        {
            switch (phase)
            {
                case PracticePhases.SimpleRepetition:
                    {
                        // Generate 111, 222, ..., 999, 000
                        practiceText.Append(GenerateSingleKeyPattern(LeftHandDigits, repeatCount)).Append(' ');
                        practiceText.Append(GenerateSingleKeyPattern(RightHandDigits, repeatCount)).Append(' ');

                        break;
                    }
                case PracticePhases.Patterns:
                    {
                        // Generate 10 10 29 29 38 38 etc.
                        practiceText.Append(GenerateVerticalCombos(comboRepeat)).Append(' ');
                        break;
                    }
                case PracticePhases.RealWords:
                    {
                        practiceText.Append(GenerateFingerSwapPattern());
                        break;
                    }
            }
        }


        // Trim practiceText beyond MaxCharacters
        if (practiceText.Length > MaxCharacters)
        {
            practiceText.Remove(MaxCharacters, practiceText.Length - MaxCharacters);
        }

        return practiceText.ToString().TrimEnd();
    }


    private string GenerateSingleKeyPattern(List<char> digits, int repetitions)
    {
        var sb = new StringBuilder();
        foreach (var digit in digits)
        {
            sb.Append(new string(digit, repetitions) + " ");
        }

        return sb.ToString().TrimEnd();
    }

    private string GenerateVerticalCombos(int repetitions)
    {
        var fingerPairs = new List<(char left, char right)>
        {
            ('1', '0'),
            ('2', '9'),
            ('3', '8'),
            ('4', '7'),
            ('5', '6'),
        };

        var sb = new StringBuilder();
        foreach (var combo in fingerPairs.Select(pair => $"{pair.left}{pair.right}"))
        {
            sb.Append(combo.Repeat(repetitions)).Append(' ');
        }

        return sb.ToString();
    }

    private static string GenerateFingerSwapPattern()
    {
        int[] digits = [1, 2, 3, 4, 5, 6, 7, 8, 9, 0];

        var patterns = digits.Select(digit => digit switch
            {
                1 => "1414",
                2 => "2525",
                3 => "3636",
                4 => "4747",
                5 => "5858",
                6 => "6969",
                9 => "9090",
                8 => "8080",
                _ => "0101",
            })
            .ToList();

        return patterns.Aggregate(new StringBuilder(), (sb, p) => sb.Append(p + " ")).ToString().TrimEnd();
    }
}