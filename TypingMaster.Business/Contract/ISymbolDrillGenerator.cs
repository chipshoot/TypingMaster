using TypingMaster.Core.Models.Courses;

namespace TypingMaster.Business.Contract;

public interface ISymbolDrillGenerator
{
    string GenerateDrill(SymbolPracticePhases phase);

    int MaxCharacters { get; set; }
}