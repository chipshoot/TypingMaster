using System.Text;
using TypingMaster.Business.Contract;
using TypingMaster.Core.Constants;
using TypingMaster.Core.Models.Courses;

namespace TypingMaster.Business.Course;

public class ShiftDrillGenerator : IShiftDrillGenerator
{
    private static readonly Random Random = new Random();
    public int MaxCharacters { get; set; } = TypingMasterConstants.DefaultTypingWindowWidth;
    public List<string> CommonWords { get; set; } = [];

    public string GenerateDrill(PracticePhases phases)
    {
        if (MaxCharacters <= 0)
        {
            return string.Empty;
        }

        var practiceText = phases switch
        {
            PracticePhases.SimpleRepetition => GenerateBasicShiftDrill(),
            PracticePhases.Patterns => GenerateCapitalizationDrill(),
            PracticePhases.RealWords => GenerateFullCapsDrill(),
            _ => GenerateBasicShiftDrill()
        };

        return practiceText;
    }

    private string GenerateBasicShiftDrill()
    {
        var sb = new StringBuilder();
        const string letters = "abcdefghijklmnopqrstuvwxyz";

        for (var i = 0; i < MaxCharacters; i++)
        {
            var letter = letters[Random.Next(letters.Length)];
            sb.Append($"{letter}{char.ToUpper(letter)} ");
        }

        var result = sb.ToString().Trim();
        return result[..Math.Min(result.Length, MaxCharacters)].Trim();
    }

    private string GenerateCapitalizationDrill()
    {
        var sb = new StringBuilder();
        if (!CommonWords.Any())
        {
            CommonWords.AddRange("alice", "bob", "charlie", "david", "eva", "frank", "grace");
        }

        for (var i = 0; i < MaxCharacters; i++)
        {
            var word = CommonWords[Random.Next(CommonWords.Count)];
            word = $"{char.ToUpper(word[0])}{word[1..]}";
            sb.Append(word);
            sb.Append(' ');
        }

        var result = sb.ToString().Trim();
        return result[..Math.Min(result.Length, MaxCharacters)].Trim();
    }

    private string GenerateFullCapsDrill()
    {
        var sb = new StringBuilder();
        if (!CommonWords.Any())
        {
            CommonWords.AddRange("NASA", "FBI", "CIA", "HTML", "CSS", "JSON", "API", "CEO");
        }

        for (var i = 0; i < MaxCharacters; i++)
        {
            var word = CommonWords[Random.Next(CommonWords.Count)].ToUpper();
            sb.Append(word);
            sb.Append(' ');
        }

        var result = sb.ToString().Trim();
        return result[..Math.Min(result.Length, MaxCharacters)].Trim();
    }
}