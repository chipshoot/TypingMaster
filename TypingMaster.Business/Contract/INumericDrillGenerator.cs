using TypingMaster.Core.Models.Courses;

namespace TypingMaster.Business.Contract;

public interface INumericDrillGenerator
{
    string GenerateDrill(NumericPracticePhases phases, int count);

    int MaxCharacters { get; set; }

    bool EnableCapital { get; set; }

    bool EnableSymbol { get; set; }
}