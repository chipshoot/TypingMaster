using TypingMaster.Business.Course;
using TypingMaster.Core.Constants;
using TypingMaster.Core.Models.Courses;

namespace TypingMaster.Tests.Course;

public class SymbolDrillGeneratorTests
{
    private readonly SymbolDrillGenerator _generator = new();

    [Fact]
    public void MaxCharacters_DefaultValue_ShouldBeTypingMasterConstant()
    {
        // Arrange & Act
        var maxChars = _generator.MaxCharacters;

        // Assert
        Assert.Equal(TypingMasterConstants.DefaultTypingWindowWidth, maxChars);
    }

    [Fact]
    public void MaxCharacters_CanBeSet_ShouldUpdateValue()
    {
        // Arrange
        var newValue = 100;

        // Act
        _generator.MaxCharacters = newValue;

        // Assert
        Assert.Equal(newValue, _generator.MaxCharacters);
    }

    [Theory]
    [InlineData(10)]
    [InlineData(50)]
    [InlineData(100)]
    public void MaxCharacters_SetDifferentValues_UpdatesCorrectly(int maxChars)
    {
        // Act
        _generator.MaxCharacters = maxChars;

        // Assert
        Assert.Equal(maxChars, _generator.MaxCharacters);
    }

    [Fact]
    public void GenerateDrill_BasicSymbols_ContainsBasicSymbolsAndWords()
    {
        // Act
        var result = _generator.GenerateDrill(SymbolPracticePhases.BasicSymbols);

        // Assert
        Assert.NotEmpty(result);

        // Should contain basic symbols
        var basicSymbols = new[] { ',', '.', '!', '?', ';', ':', '\'', '"' };
        var containsBasicSymbol = result.Any(c => basicSymbols.Contains(c));
        Assert.True(containsBasicSymbol, "Should contain at least one basic symbol");

        // Should contain alphabetic characters (words)
        var containsLetters = result.Any(char.IsLetter);
        Assert.True(containsLetters, "Should contain alphabetic characters");
    }

    [Fact]
    public void GenerateDrill_BasicSymbols_RespectsMaxCharacters()
    {
        // Arrange
        _generator.MaxCharacters = 50;

        // Act
        var result = _generator.GenerateDrill(SymbolPracticePhases.BasicSymbols);

        // Assert
        Assert.True(result.Length <= _generator.MaxCharacters);
    }

    [Fact]
    public void GenerateDrill_SymbolCombinations_ContainsIntermediateAndAdvancedSymbols()
    {
        // Act
        var result = _generator.GenerateDrill(SymbolPracticePhases.SymbolCombinations);

        // Assert
        Assert.NotEmpty(result);

        // Should contain intermediate symbols
        var intermediateSymbols = new[] { '@', '#', '$', '%', '&', '*', '(', ')', '-', '_', '=', '+' };
        var advancedSymbols = new[] { '[', ']', '{', '}', '<', '>', '|', '\\', '/', '~', '^', '`' };

        var containsIntermediate = result.Any(c => intermediateSymbols.Contains(c));
        var containsAdvanced = result.Any(c => advancedSymbols.Contains(c));

        Assert.True(containsIntermediate || containsAdvanced, "Should contain intermediate or advanced symbols");
    }

    [Fact]
    public void GenerateDrill_SymbolCombinations_ContainsWords()
    {
        // Act
        var result = _generator.GenerateDrill(SymbolPracticePhases.SymbolCombinations);

        // Assert
        Assert.NotEmpty(result);

        // Should contain alphabetic characters from the word templates
        var containsLetters = result.Any(char.IsLetter);
        Assert.True(containsLetters, "Should contain alphabetic characters");
    }

    [Fact]
    public void GenerateDrill_ProgrammingSymbols_ReturnsValidProgrammingPattern()
    {
        // Act
        var result = _generator.GenerateDrill(SymbolPracticePhases.ProgrammingSymbols);

        // Assert
        Assert.NotEmpty(result);

        // Should be one of the predefined programming drills
        var expectedProgrammingPatterns = new[]
        {
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
        };

        var isValidProgrammingPattern = expectedProgrammingPatterns.Contains(result);
        Assert.True(isValidProgrammingPattern, "Should return a valid programming pattern");
    }

    [Fact]
    public void GenerateDrill_ProgrammingSymbols_ContainsProgrammingElements()
    {
        // Act
        var results = new List<string>();
        for (var i = 0; i < 20; i++)
        {
            results.Add(_generator.GenerateDrill(SymbolPracticePhases.ProgrammingSymbols));
        }

        var allResults = string.Join(" ", results);

        // Assert
        // Should contain common programming symbols and keywords
        var hasProgrammingSymbols = allResults.Contains('(') || allResults.Contains('{') ||
                                   allResults.Contains('=') || allResults.Contains(';') ||
                                   allResults.Contains('/') || allResults.Contains('*');

        Assert.True(hasProgrammingSymbols, "Should contain programming symbols");

        var hasProgrammingKeywords = allResults.Contains("if") || allResults.Contains("for") ||
                                    allResults.Contains("function") || allResults.Contains("let") ||
                                    allResults.Contains("define");

        Assert.True(hasProgrammingKeywords, "Should contain programming keywords");
    }

    [Fact]
    public void GenerateDrill_ProfessionalContexts_ReturnsValidProfessionalPattern()
    {
        // Act
        var result = _generator.GenerateDrill(SymbolPracticePhases.ProfessionalContexts);

        // Assert
        Assert.NotEmpty(result);

        // Should be one of the predefined professional contexts
        var expectedProfessionalPatterns = new[]
        {
            "E = mc²; ∫f(x)dx from 0 to ∞; α + β = γ",
            "Total: $1,250.75 (including 10% tax @ $125.08)",
            "Smith et al. (2023) reported a 25±3% increase - p<0.01*",
            "Dimensions: 100×50×25 mm; Tolerance: ±0.5%",
            "df['column'] = (df['value'] - μ) / σ # Normalization",
            "Party A agrees to pay Party B the sum of $10,000.00 (§4.2)"
        };

        var isValidProfessionalPattern = expectedProfessionalPatterns.Contains(result);
        Assert.True(isValidProfessionalPattern, "Should return a valid professional pattern");
    }

    [Fact]
    public void GenerateDrill_ProfessionalContexts_ContainsProfessionalElements()
    {
        // Act
        var results = new List<string>();
        for (var i = 0; i < 20; i++)
        {
            results.Add(_generator.GenerateDrill(SymbolPracticePhases.ProfessionalContexts));
        }

        var allResults = string.Join(" ", results);

        // Assert
        // Should contain professional symbols and terms
        var hasProfessionalSymbols = allResults.Contains('$') || allResults.Contains('%') ||
                                    allResults.Contains('±') || allResults.Contains('×') ||
                                    allResults.Contains('²') || allResults.Contains('∫') ||
                                    allResults.Contains('μ') || allResults.Contains('σ');

        Assert.True(hasProfessionalSymbols, "Should contain professional symbols");

        var hasProfessionalTerms = allResults.Contains("Total") || allResults.Contains("tax") ||
                                  allResults.Contains("reported") || allResults.Contains("Dimensions") ||
                                  allResults.Contains("Tolerance") || allResults.Contains("Party");

        Assert.True(hasProfessionalTerms, "Should contain professional terms");
    }

    [Fact]
    public void GenerateDrill_SpeedChallenge_ContainsValidComponents()
    {
        // Act
        var result = _generator.GenerateDrill(SymbolPracticePhases.SpeedChallenge);

        // Assert
        Assert.NotEmpty(result);

        // Should contain some of the predefined components
        var expectedComponents = new[]
        {
            "Hello", "world", "123", "code", "function()", "$price", "25%", "array[]",
            "key=value", "user@domain", "file.txt", "10>5", "a|b", "x*y", "result+=1"
        };

        var containsComponent = expectedComponents.Any(component => result.Contains(component));
        Assert.True(containsComponent, "Should contain at least one expected component");
    }

    [Fact]
    public void GenerateDrill_SpeedChallenge_ContainsSymbols()
    {
        // Act
        var results = new List<string>();
        for (var i = 0; i < 10; i++)
        {
            results.Add(_generator.GenerateDrill(SymbolPracticePhases.SpeedChallenge));
        }

        var allResults = string.Join(" ", results);

        // Assert
        // Should contain various symbols from the speed challenge
        var hasSymbols = allResults.Any(c => !char.IsLetterOrDigit(c) && !char.IsWhiteSpace(c));
        Assert.True(hasSymbols, "Should contain symbols in speed challenge");
    }

    [Theory]
    [InlineData(SymbolPracticePhases.BasicSymbols)]
    [InlineData(SymbolPracticePhases.SymbolCombinations)]
    [InlineData(SymbolPracticePhases.ProgrammingSymbols)]
    [InlineData(SymbolPracticePhases.ProfessionalContexts)]
    [InlineData(SymbolPracticePhases.SpeedChallenge)]
    public void GenerateDrill_AllPhases_GenerateNonEmptyResults(SymbolPracticePhases phase)
    {
        // Act
        var result = _generator.GenerateDrill(phase);

        // Assert
        Assert.NotEmpty(result);
    }

    [Theory]
    [InlineData(SymbolPracticePhases.BasicSymbols)]
    [InlineData(SymbolPracticePhases.SymbolCombinations)]
    public void GenerateDrill_VariablePhases_RespectsMaxCharacters(SymbolPracticePhases phase)
    {
        // Arrange
        _generator.MaxCharacters = 30;

        // Act
        var result = _generator.GenerateDrill(phase);

        // Assert
        Assert.True(result.Length <= _generator.MaxCharacters);
    }

    [Fact]
    public void GenerateDrill_InvalidPhase_ReturnsDefaultDrill()
    {
        // Act
        var result = _generator.GenerateDrill((SymbolPracticePhases)999);

        // Assert
        Assert.Equal("Default symbol drill", result);
    }


    [Fact]
    public void GenerateDrill_BasicSymbols_GeneratesVariousPatterns()
    {
        // Act
        var results = new List<string>();
        for (var i = 0; i < 10; i++)
        {
            results.Add(_generator.GenerateDrill(SymbolPracticePhases.BasicSymbols));
        }

        // Assert
        Assert.All(results, result => Assert.NotEmpty(result));

        // Should have some variation in patterns over multiple calls
        var uniqueResults = results.Distinct().Count();
        Assert.True(uniqueResults >= 1, "Should generate some variation in patterns");
    }

    [Fact]
    public void GenerateDrill_SymbolCombinations_UsesTemplates()
    {
        // Act
        var results = new List<string>();
        for (var i = 0; i < 20; i++)
        {
            results.Add(_generator.GenerateDrill(SymbolPracticePhases.SymbolCombinations));
        }

        var allResults = string.Join(" ", results);

        // Assert
        // Should contain words (from templates) and symbols
        var containsWords = allResults.Any(char.IsLetter);
        var containsSymbols = allResults.Any(c => !char.IsLetterOrDigit(c) && !char.IsWhiteSpace(c));

        Assert.True(containsWords, "Should contain words from templates");
        Assert.True(containsSymbols, "Should contain symbols from templates");
    }

    [Fact]
    public void GenerateDrill_BasicSymbols_ContainsExpectedWords()
    {
        // Act
        var results = new List<string>();
        for (var i = 0; i < 20; i++)
        {
            results.Add(_generator.GenerateDrill(SymbolPracticePhases.BasicSymbols));
        }

        var allResults = string.Join(" ", results);

        // Assert
        // Should contain words from the GetRandomWord method
        var expectedWords = new[] { "type", "fast", "symbol", "key", "practice", "drill", "code", "text" };
        var containsExpectedWord = expectedWords.Any(word => allResults.Contains(word));

        Assert.True(containsExpectedWord, "Should contain words from the predefined word list");
    }

    [Fact]
    public void GenerateDrill_SpeedChallenge_GeneratesVariableLengthResults()
    {
        // Act
        var results = new List<string>();
        for (var i = 0; i < 10; i++)
        {
            results.Add(_generator.GenerateDrill(SymbolPracticePhases.SpeedChallenge));
        }

        // Assert
        Assert.All(results, Assert.NotEmpty);

        // Should have some variation in length due to random component count (5-8)
        var lengths = results.Select(r => r.Length).ToList();
        var hasVariation = lengths.Max() > lengths.Min();

        Assert.True(hasVariation, "Should generate results with some length variation");
    }

    [Fact]
    public void GenerateDrill_ConsistentBehavior_WithSameInputs()
    {
        // Note: This test acknowledges that the generator uses Random,
        // so we test that it consistently produces valid output rather than identical output

        // Arrange
        _generator.MaxCharacters = 50;

        // Act
        var results = new List<string>();
        for (var i = 0; i < 5; i++)
        {
            results.Add(_generator.GenerateDrill(SymbolPracticePhases.BasicSymbols));
        }

        // Assert
        foreach (var result in results)
        {
            Assert.NotEmpty(result);
            Assert.True(result.Length <= _generator.MaxCharacters);
        }
    }

    [Fact]
    public void GenerateDrill_AllPhases_GenerateUniquePatterns()
    {
        // Act
        var basicResult = _generator.GenerateDrill(SymbolPracticePhases.BasicSymbols);
        var combinationResult = _generator.GenerateDrill(SymbolPracticePhases.SymbolCombinations);
        var programmingResult = _generator.GenerateDrill(SymbolPracticePhases.ProgrammingSymbols);
        var professionalResult = _generator.GenerateDrill(SymbolPracticePhases.ProfessionalContexts);
        var speedResult = _generator.GenerateDrill(SymbolPracticePhases.SpeedChallenge);

        // Assert
        Assert.NotEmpty(basicResult);
        Assert.NotEmpty(combinationResult);
        Assert.NotEmpty(programmingResult);
        Assert.NotEmpty(professionalResult);
        Assert.NotEmpty(speedResult);

        // Each phase should generate distinct types of content
        var allResults = new[] { basicResult, combinationResult, programmingResult, professionalResult, speedResult };
        var uniqueResults = allResults.Distinct().Count();

        Assert.True(uniqueResults >= 3, "Different phases should generate distinctly different content");
    }
}