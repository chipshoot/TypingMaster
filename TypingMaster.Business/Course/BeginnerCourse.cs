using System.Text;
using TypingMaster.Business.Contract;
using TypingMaster.Core.Constants;
using TypingMaster.Core.Models;
using TypingMaster.Core.Models.Courses;
using TypingMaster.Core.Utility;

namespace TypingMaster.Business.Course;

public class BeginnerCourse(
    INumericDrillGenerator numericDrillGenerator,
    IShiftDrillGenerator shiftDrillGenerator,
    ISymbolDrillGenerator symbolDrillGenerator,
    Serilog.ILogger logger,
    IRandomNumberGenerator? randomGenerator = null,
    string lessonDataFileUrl = "") : ICourse
{
    private const string CourseDescription =
        "Master Touch Typing from Scratch: This structured beginner's course guides you from the home row keys (a, s, d, f, j, k, l, ;) to full keyboard proficiency. Starting with your finger placement on home keys, each lesson gradually introduces new keys while reinforcing previously learned ones. Progress at your own pace through interactive exercises designed to build muscle memory, improve accuracy, and increase typing speed. Perfect for new typists or anyone looking to develop proper touch typing technique without looking at the keyboard. Track your WPM and accuracy as you transform from hunt-and-peck to confident touch typing.";

    private const string RepetitionInst = "**Repetition Phase**: Focus on finger position and accuracy. Type the repeated characters.";
    private const string PatternsInst = "**Patterns Phase**: Practice key patterns to build finger coordination.";
    private const string RealWordsInst = "**RealWord Phase**: Apply your skills by typing these common words.";

    private const string NumberSection = "Number Row";
    private const string ShiftSection = "Shift Row";
    private const string SymbolSection = "Symbol Row";
    private const int RepeatCount = 3;

    private readonly IRandomNumberGenerator _randomGenerator = randomGenerator ?? new RandomNumberGenerator();

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
                        phase = PracticePhases.NotSet; // Reset phase for next lesson
                    }

                    break;
                case LessonType.NotSet:
                case LessonType.Practice:
                case LessonType.Test:
                default:
                    lesson = Lessons.FirstOrDefault(l => l.Id == curLessonId + 1);
                    phase = PracticePhases.NotSet; // Reset phase for next lesson
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
            if (lesson.Description.Contains(NumberSection))
            {
                lesson.PracticeText = GeneratePracticeText(nextPhase, lesson.Point);
            }
            else if(lesson.Description.Contains(ShiftSection))
            {
                // If the lesson is about Shift keys, generate practice text for Shift keys
                lesson.PracticeText = GeneratePracticeTextForShift(lesson.CommonWords, nextPhase);
            }
            else if(lesson.Description.Contains(SymbolSection))
            {
                // If the lesson is about Symbol keys, generate practice text for Shift keys
                lesson.PracticeText = GeneratePracticeTextForSymbol(lesson.CommonWords, nextPhase);
            }
            else
            {
                lesson.PracticeText = GeneratePracticeText(lesson.Target, lesson.CommonWords, nextPhase);
            }
        }
        else
        {
            lesson.PracticeText = GeneratePracticeText(lesson.Target, lesson.CommonWords, nextPhase);
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
        var cleanedInst = baseInstruction;
        if (cleanedInst.Contains(RepetitionInst))
        {
            cleanedInst = cleanedInst.Replace($"\n\n{RepetitionInst}", string.Empty);
        }
        if (cleanedInst.Contains(PatternsInst))
        {
            cleanedInst = cleanedInst.Replace($"\n\n{PatternsInst}", string.Empty);
        }
        if (cleanedInst.Contains(RealWordsInst))
        {
            cleanedInst = cleanedInst.Replace($"\n\n{RealWordsInst}", string.Empty);
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

    public string GeneratePracticeTextForShift(string[] commonWords, PracticePhases phase)
    {
        shiftDrillGenerator.MaxCharacters = MaxCharacters;
        shiftDrillGenerator.CommonWords.Clear();
        shiftDrillGenerator.CommonWords.AddRange(commonWords);
        var practiceText = shiftDrillGenerator.GenerateDrill(phase);

        return practiceText;

    }


    /// <summary>
    /// Get practice text for the numeric drill.
    /// </summary>
    /// <param name="phase">The current <see cref="PracticePhases"/> of practice and will map to <see cref="NumericPracticePhases" /> </param>
    ///     PracticePhases        NumericPracticePhases
    ///     NotSet            ->  NotSet 
    ///     SimpleRepetition  ->  SingleKeyFocus (point 1)
    ///                       ->  PracticalPatterns (point 2)
    ///     Patterns          ->  SequenceCombos (point 1)
    ///                       ->  FullIntegration (point 2)
    ///     RealWords         ->  HorizontalCombination (point 1)
    ///                       ->  DomainSpecialization (point 2)
    /// <param name="point">The point value used for control mapping from <see cref="PracticePhases"/> of practice to <see cref="NumericPracticePhases" /> </param>
    /// <returns>The practice text for training number keys</returns>
    public string GeneratePracticeText(PracticePhases phase, int point)
    {
        NumericPracticePhases numericPhase;
        switch (phase)
        {
            case PracticePhases.NotSet:
                numericPhase = NumericPracticePhases.SingleKeyFocus;
                break;
            case PracticePhases.SimpleRepetition:
                numericPhase = point switch
                {
                    1 => NumericPracticePhases.SingleKeyFocus,
                    2 => NumericPracticePhases.PracticalPatterns,
                    _ => NumericPracticePhases.SingleKeyFocus
                };
                break;
            case PracticePhases.Patterns:
                numericPhase = point switch
                {
                    1 => NumericPracticePhases.SequenceCombos,
                    2 => NumericPracticePhases.FullIntegration,
                    _ => NumericPracticePhases.SequenceCombos
                };
                break;
            case PracticePhases.RealWords:
                numericPhase = point switch
                {
                    1 => NumericPracticePhases.HorizontalCombination,
                    2 => NumericPracticePhases.DomainSpecialization,
                    _ => NumericPracticePhases.HorizontalCombination
                };
                break;
            default:
                numericPhase = NumericPracticePhases.NotSet;
                break;
        }

        numericDrillGenerator.EnableCapital = false;
        numericDrillGenerator.EnableSymbol = false;
        numericDrillGenerator.MaxCharacters = MaxCharacters;
        
        var practiceText = numericDrillGenerator.GenerateDrill(numericPhase, RepeatCount);
        return practiceText.TrimEnd();
    }

    public string GeneratePracticeTextForSymbol(string[] commonWords, PracticePhases phase)
    {
        var symbolPhase = phase switch
        {
            PracticePhases.NotSet => SymbolPracticePhases.BasicSymbols,
            PracticePhases.SimpleRepetition => SymbolPracticePhases.BasicSymbols,
            PracticePhases.Patterns => SymbolPracticePhases.SymbolCombinations,
            PracticePhases.RealWords => SymbolPracticePhases.ProgrammingSymbols,
            _ => SymbolPracticePhases.BasicSymbols
        };

        symbolDrillGenerator.MaxCharacters = MaxCharacters;

        
        var practiceText = symbolDrillGenerator.GenerateDrill(symbolPhase);
        return practiceText.TrimEnd();
    }
}