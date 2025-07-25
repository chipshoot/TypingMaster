using TypingMaster.Core.Models.Courses;

namespace TypingMaster.Business.Contract;

public interface IShiftDrillGenerator
{
    
    string GenerateDrill(PracticePhases phases);

    int MaxCharacters { get; set; }

    List<string> CommonWords { get; set; }
}