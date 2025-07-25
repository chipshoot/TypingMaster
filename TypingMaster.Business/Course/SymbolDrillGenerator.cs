using System.Text;
using TypingMaster.Business.Contract;
using TypingMaster.Core.Constants;
using TypingMaster.Core.Models.Courses;

namespace TypingMaster.Business.Course;

public class SymbolDrillGenerator : ISymbolDrillGenerator
{
    private readonly Random _random = new ();

    private static readonly char[] BasicSymbols = [',', '.', '!', '?', ';', ':', '\'', '"'];
    
    private static readonly char[] IntermediateSymbols = ['@', '#', '$', '%', '&', '*', '(', ')', '-', '_', '=', '+'];
    
    private static readonly char[] AdvancedSymbols = ['[', ']', '{', '}', '<', '>', '|', '\\', '/', '~', '^', '`'];

    public int MaxCharacters { get; set; } = TypingMasterConstants.DefaultTypingWindowWidth;

    public string GenerateDrill(SymbolPracticePhases phase)
    {
        if (MaxCharacters <= 0)
        {
            return string.Empty;
        }

        return phase switch
        {
            SymbolPracticePhases.BasicSymbols => GenerateBasicSymbolDrill(),
            SymbolPracticePhases.SymbolCombinations => GenerateCombinationDrill(),
            SymbolPracticePhases.ProgrammingSymbols => GenerateProgrammingDrill(),
            SymbolPracticePhases.ProfessionalContexts => GenerateProfessionalDrill(),
            SymbolPracticePhases.SpeedChallenge => GenerateSpeedChallenge(),
            _ => "Default symbol drill"
        };
    }
    private string GenerateBasicSymbolDrill()
    {
        var sb = new StringBuilder();
        
        while (sb.Length <= MaxCharacters)
        {
            var word = GetRandomWord();
            var symbol = BasicSymbols[_random.Next(BasicSymbols.Length)];
            
            sb.Append(_random.Next(3) switch
            {
                0 => $"{word}{symbol} ",
                1 => $"{symbol}{word} ",
                _ => $"{word}{symbol}{word} " 
            });
        }
        
        var result = sb.ToString().Trim();
        return result[..Math.Min(result.Length, MaxCharacters)].Trim();
    }

    private string GenerateCombinationDrill()
    {
        var sb = new StringBuilder();
        string[] templates =
        [
            "word{s1}word{s2}",
            "{s1}word{s2}word",
            "word{s1}{s2}word",
            "{s1}{s2}word{s1}",
            "word{s1} and{s2}word"
        ];
        
        while (sb.Length <= MaxCharacters)
        {
            var template = templates[_random.Next(templates.Length)];
            var s1 = IntermediateSymbols[_random.Next(IntermediateSymbols.Length)];
            var s2 = AdvancedSymbols[_random.Next(AdvancedSymbols.Length)];
            
            var drill = template
                .Replace("{s1}", s1.ToString())
                .Replace("{s2}", s2.ToString())
                .Replace("word", GetRandomWord());
            
            sb.Append(drill).Append(' ');
        }
        
        var result = sb.ToString().Trim();
        return result[..Math.Min(result.Length, MaxCharacters)].Trim();
    }

    private string GenerateProgrammingDrill()
    {
        string[] drills =
        [
            "if (x > 0 && y < 10) { ... }",
            "function(arg1, arg2) => result;",
            "for (let i=0; i<10; i++) { ... }",
            "str = \"Hello, world!\"; // Comment",
            "dict = {'key': value};",
            "x = a ? b : c; // Ternary operator",
            "path = \"C:\\\\Program Files\\\\App\\\\file.txt\"",
            "regex = /^[a-z0-9_]+@[a-z]+\\.[a-z]{2,}$/;",
            "/* Multi-line\n * comment */",
            "#define MAX_SIZE 100"
        ];
        
        var result = drills[_random.Next(drills.Length)];
        return result[..Math.Min(result.Length, MaxCharacters)].Trim();
    }

    private string GenerateProfessionalDrill()
    {
        string[] contexts =
        [
            "E = mc²; ∫f(x)dx from 0 to ∞; α + β = γ",
            "Total: $1,250.75 (including 10% tax @ $125.08)",
            "Smith et al. (2023) reported a 25±3% increase - p<0.01*",
            "Dimensions: 100×50×25 mm; Tolerance: ±0.5%",
            "df['column'] = (df['value'] - μ) / σ # Normalization",
            "Party A agrees to pay Party B the sum of $10,000.00 (§4.2)"
        ];
        
        var result = contexts[_random.Next(contexts.Length)];
        return result[..Math.Min(result.Length, MaxCharacters)].Trim();
    }

    private string GenerateSpeedChallenge()
    {
        var sb = new StringBuilder();
        string[] components =
        [
            "Hello", "world", "123", "code", "function()", "$price", "25%", "array[]", 
            "key=value", "user@domain", "file.txt", "10>5", "a|b", "x*y", "result+=1"
        ];
        
        var count = _random.Next(5, 8);
        for (var i = 0; i < count; i++)
        {
            sb.Append(components[_random.Next(components.Length)]);
            
            if (i < count - 1)
            {
                sb.Append(GetRandomSymbol());
                if (_random.NextDouble() > 0.7) sb.Append(' ');
            }
        }
        
        var result = sb.ToString();
        return result[..Math.Min(result.Length, MaxCharacters)].Trim();
    }

    private string GetRandomWord()
    {
        string[] words = ["type", "fast", "symbol", "key", "practice", "drill", "code", "text"];
        return words[_random.Next(words.Length)];
    }

    private char GetRandomSymbol()
    {
        var allSymbols = BasicSymbols.Concat(IntermediateSymbols).Concat(AdvancedSymbols).ToArray();
        return allSymbols[_random.Next(allSymbols.Length)];
    }
}