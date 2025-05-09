namespace TypingMaster.Core.Models.Courses;

public enum PracticePhases
{
    // For course which not phase based
    NotSet = 0,

    // Part 1: Simple repeated characters
    SimpleRepetition = 1,

    // Part 2: Simple patterns with keys
    Patterns = 2,

    // Part 3: Real words practice
    RealWords = 3
}