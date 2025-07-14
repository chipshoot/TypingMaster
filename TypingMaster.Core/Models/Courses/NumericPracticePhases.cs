namespace TypingMaster.Core.Models.Courses;

public enum NumericPracticePhases
{
    // For course which not phase based
    NotSet = 0,

    // Part 1: Simple repeated characters
    SingleKeyFocus = 1,

    // Part 2: Simple patterns with keys
    SequenceCombos = 2,

    // Part 3: Simple patterns with keys
    HorizontalCombination = 3,

    // Part 4: real number patterns with keys
    PracticalPatterns = 4,

    // Part 5: full key and number mix application
    FullIntegration = 5,

    // Part 6: patterns with domain specific numbers
    DomainSpecialization = 6,

    // Part 7: competitive speed training
    CompetitiveSpeed = 7
}