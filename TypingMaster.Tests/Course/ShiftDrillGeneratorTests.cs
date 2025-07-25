using TypingMaster.Business.Course;
using TypingMaster.Core.Constants;
using TypingMaster.Core.Models.Courses;

namespace TypingMaster.Tests.Course;

public class ShiftDrillGeneratorTests
{
    private readonly ShiftDrillGenerator _generator = new();

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
    [InlineData(200)]
    public void MaxCharacters_SetDifferentValues_UpdatesCorrectly(int maxChars)
    {
        // Act
        _generator.MaxCharacters = maxChars;

        // Assert
        Assert.Equal(maxChars, _generator.MaxCharacters);
    }

    [Fact]
    public void CommonWords_DefaultValue_ShouldBeEmptyList()
    {
        // Arrange & Act
        var commonWords = _generator.CommonWords;

        // Assert
        Assert.NotNull(commonWords);
        Assert.Empty(commonWords);
    }

    [Fact]
    public void CommonWords_CanBeSet_ShouldUpdateValue()
    {
        // Arrange
        var newWords = new List<string> { "test", "word", "example" };

        // Act
        _generator.CommonWords = newWords;

        // Assert
        Assert.Equal(newWords, _generator.CommonWords);
        Assert.Equal(3, _generator.CommonWords.Count);
    }

    #region GenerateDrill - SimpleRepetition Phase Tests

    [Fact]
    public void GenerateDrill_SimpleRepetition_GeneratesBasicShiftDrill()
    {
        // Act
        var result = _generator.GenerateDrill(PracticePhases.SimpleRepetition);

        // Assert
        Assert.NotEmpty(result);
        Assert.True(result.Length <= _generator.MaxCharacters);
    }

    [Fact]
    public void GenerateDrill_SimpleRepetition_ContainsLowercaseAndUppercasePairs()
    {
        // Arrange
        _generator.MaxCharacters = 100;

        // Act
        var result = _generator.GenerateDrill(PracticePhases.SimpleRepetition);

        // Assert
        Assert.NotEmpty(result);
        
        // Should contain both lowercase and uppercase letters
        var hasLowercase = result.Any(char.IsLower);
        var hasUppercase = result.Any(char.IsUpper);
        
        Assert.True(hasLowercase, "Should contain lowercase letters");
        Assert.True(hasUppercase, "Should contain uppercase letters");
    }

    [Fact]
    public void GenerateDrill_SimpleRepetition_RespectsMaxCharacters()
    {
        // Arrange
        _generator.MaxCharacters = 50;

        // Act
        var result = _generator.GenerateDrill(PracticePhases.SimpleRepetition);

        // Assert
        Assert.True(result.Length <= _generator.MaxCharacters);
    }

    [Fact]
    public void GenerateDrill_SimpleRepetition_ResultIsTrimmed()
    {
        // Act
        var result = _generator.GenerateDrill(PracticePhases.SimpleRepetition);

        // Assert
        Assert.NotEmpty(result);
        Assert.False(result.StartsWith(' '), "Result should not start with space");
        Assert.False(result.EndsWith(' '), "Result should not end with space");
    }

    [Fact]
    public void GenerateDrill_SimpleRepetition_ContainsOnlyAlphabeticCharactersAndSpaces()
    {
        // Act
        var result = _generator.GenerateDrill(PracticePhases.SimpleRepetition);

        // Assert
        Assert.NotEmpty(result);
        Assert.True(result.All(c => char.IsLetter(c) || char.IsWhiteSpace(c)), 
            "Should contain only letters and spaces");
    }

    #endregion

    #region GenerateDrill - Patterns Phase Tests

    [Fact]
    public void GenerateDrill_Patterns_GeneratesCapitalizationDrill()
    {
        // Act
        var result = _generator.GenerateDrill(PracticePhases.Patterns);

        // Assert
        Assert.NotEmpty(result);
        Assert.True(result.Length <= _generator.MaxCharacters);
    }

    [Fact]
    public void GenerateDrill_Patterns_UsesDefaultWordsWhenCommonWordsEmpty()
    {
        // Arrange
        _generator.CommonWords.Clear();

        // Act
        var result = _generator.GenerateDrill(PracticePhases.Patterns);

        // Assert
        Assert.NotEmpty(result);
        
        // Should have populated CommonWords with default values
        Assert.NotEmpty(_generator.CommonWords);
        Assert.Contains("alice", _generator.CommonWords);
        Assert.Contains("bob", _generator.CommonWords);
        Assert.Contains("charlie", _generator.CommonWords);
        Assert.Contains("david", _generator.CommonWords);
        Assert.Contains("eva", _generator.CommonWords);
        Assert.Contains("frank", _generator.CommonWords);
        Assert.Contains("grace", _generator.CommonWords);
    }

    [Fact]
    public void GenerateDrill_Patterns_UsesProvidedCommonWords()
    {
        // Arrange
        var customWords = new List<string> { "custom", "words", "test" };
        _generator.CommonWords = customWords;

        // Act
        var result = _generator.GenerateDrill(PracticePhases.Patterns);

        // Assert
        Assert.NotEmpty(result);
        
        // Should use the custom words provided
        var wordsInResult = result.Split(' ', StringSplitOptions.RemoveEmptyEntries);
        var hasCustomWord = wordsInResult.Any(word => 
            customWords.Any(cw => word.Equals(char.ToUpper(cw[0]) + cw[1..], StringComparison.OrdinalIgnoreCase)));
        
        Assert.True(hasCustomWord, "Should contain words from the custom CommonWords list");
    }

    [Fact]
    public void GenerateDrill_Patterns_ContainsCapitalizedWords()
    {
        // Act
        var result = _generator.GenerateDrill(PracticePhases.Patterns);

        // Assert
        Assert.NotEmpty(result);
        
        var words = result.Split(' ', StringSplitOptions.RemoveEmptyEntries);
        Assert.True(words.Length > 0, "Should contain words");
        
        // Each word should start with an uppercase letter
        foreach (var word in words)
        {
            Assert.True(char.IsUpper(word[0]), $"Word '{word}' should start with uppercase letter");
            if (word.Length > 1)
            {
                Assert.True(word[1..].All(char.IsLower), $"Word '{word}' should have lowercase letters after first character");
            }
        }
    }

    [Fact]
    public void GenerateDrill_Patterns_RespectsMaxCharacters()
    {
        // Arrange
        _generator.MaxCharacters = 30;

        // Act
        var result = _generator.GenerateDrill(PracticePhases.Patterns);

        // Assert
        Assert.True(result.Length <= _generator.MaxCharacters);
    }

    #endregion

    #region GenerateDrill - RealWords Phase Tests

    [Fact]
    public void GenerateDrill_RealWords_GeneratesFullCapsDrill()
    {
        // Act
        var result = _generator.GenerateDrill(PracticePhases.RealWords);

        // Assert
        Assert.NotEmpty(result);
        Assert.True(result.Length <= _generator.MaxCharacters);
    }

    [Fact]
    public void GenerateDrill_RealWords_UsesDefaultWordsWhenCommonWordsEmpty()
    {
        // Arrange
        _generator.CommonWords.Clear();

        // Act
        var result = _generator.GenerateDrill(PracticePhases.RealWords);

        // Assert
        Assert.NotEmpty(result);
        
        // Should have populated CommonWords with default acronyms
        Assert.NotEmpty(_generator.CommonWords);
        Assert.Contains("NASA", _generator.CommonWords);
        Assert.Contains("FBI", _generator.CommonWords);
        Assert.Contains("CIA", _generator.CommonWords);
        Assert.Contains("HTML", _generator.CommonWords);
        Assert.Contains("CSS", _generator.CommonWords);
        Assert.Contains("JSON", _generator.CommonWords);
        Assert.Contains("API", _generator.CommonWords);
        Assert.Contains("CEO", _generator.CommonWords);
    }

    [Fact]
    public void GenerateDrill_RealWords_UsesProvidedCommonWords()
    {
        // Arrange
        var customWords = new List<string> { "xml", "sql", "ide" };
        _generator.CommonWords = customWords;

        // Act
        var result = _generator.GenerateDrill(PracticePhases.RealWords);

        // Assert
        Assert.NotEmpty(result);
        
        // Should use the custom words provided (in uppercase)
        var wordsInResult = result.Split(' ', StringSplitOptions.RemoveEmptyEntries);
        var hasCustomWord = wordsInResult.Any(word => 
            customWords.Any(cw => word.Equals(cw.ToUpper(), StringComparison.OrdinalIgnoreCase)));
        
        Assert.True(hasCustomWord, "Should contain words from the custom CommonWords list");
    }

    [Fact]
    public void GenerateDrill_RealWords_ContainsUppercaseWords()
    {
        // Act
        var result = _generator.GenerateDrill(PracticePhases.RealWords);

        // Assert
        Assert.NotEmpty(result);
        
        var words = result.Split(' ', StringSplitOptions.RemoveEmptyEntries);
        Assert.True(words.Length > 0, "Should contain words");
        
        // Each word should be entirely uppercase
        foreach (var word in words)
        {
            Assert.True(word.All(char.IsUpper), $"Word '{word}' should be entirely uppercase");
        }
    }

    [Fact]
    public void GenerateDrill_RealWords_RespectsMaxCharacters()
    {
        // Arrange
        _generator.MaxCharacters = 40;

        // Act
        var result = _generator.GenerateDrill(PracticePhases.RealWords);

        // Assert
        Assert.True(result.Length <= _generator.MaxCharacters);
    }

    #endregion

    #region GenerateDrill - Edge Cases and Default Behavior Tests

    [Fact]
    public void GenerateDrill_NotSetPhase_UsesDefaultBehavior()
    {
        // Act
        var result = _generator.GenerateDrill(PracticePhases.NotSet);

        // Assert
        Assert.NotEmpty(result);
        // Should default to SimpleRepetition behavior (contains both upper and lowercase)
        var hasLowercase = result.Any(char.IsLower);
        var hasUppercase = result.Any(char.IsUpper);
        
        Assert.True(hasLowercase, "Should contain lowercase letters (default behavior)");
        Assert.True(hasUppercase, "Should contain uppercase letters (default behavior)");
    }

    [Fact]
    public void GenerateDrill_InvalidPhase_UsesDefaultBehavior()
    {
        // Act
        var result = _generator.GenerateDrill((PracticePhases)999);

        // Assert
        Assert.NotEmpty(result);
        // Should default to SimpleRepetition behavior
        var hasLowercase = result.Any(char.IsLower);
        var hasUppercase = result.Any(char.IsUpper);
        
        Assert.True(hasLowercase, "Should contain lowercase letters (default behavior)");
        Assert.True(hasUppercase, "Should contain uppercase letters (default behavior)");
    }

    [Theory]
    [InlineData(PracticePhases.SimpleRepetition)]
    [InlineData(PracticePhases.Patterns)]
    [InlineData(PracticePhases.RealWords)]
    [InlineData(PracticePhases.NotSet)]
    public void GenerateDrill_AllValidPhases_GenerateNonEmptyResults(PracticePhases phase)
    {
        // Act
        var result = _generator.GenerateDrill(phase);

        // Assert
        Assert.NotEmpty(result);
        Assert.True(result.Length <= _generator.MaxCharacters);
    }

    [Theory]
    [InlineData(1)]
    [InlineData(10)]
    [InlineData(50)]
    [InlineData(100)]
    public void GenerateDrill_DifferentMaxCharacters_RespectsLimits(int maxChars)
    {
        // Arrange
        _generator.MaxCharacters = maxChars;

        // Act
        var result = _generator.GenerateDrill(PracticePhases.SimpleRepetition);

        // Assert
        Assert.True(result.Length <= maxChars);
    }

    [Fact]
    public void GenerateDrill_ZeroMaxCharacters_ReturnsEmptyString()
    {
        // Arrange
        _generator.MaxCharacters = 0;

        // Act
        var result = _generator.GenerateDrill(PracticePhases.SimpleRepetition);

        // Assert
        Assert.Equal(string.Empty, result);
    }

    [Fact]
    public void GenerateDrill_NegativeMaxCharacters_ReturnsEmptyString()
    {
        // Arrange
        _generator.MaxCharacters = -10;

        // Act
        var result = _generator.GenerateDrill(PracticePhases.SimpleRepetition);

        // Assert
        Assert.Equal(string.Empty, result);
    }

    #endregion

    #region Randomness and Variation Tests

    [Fact]
    public void GenerateDrill_SimpleRepetition_GeneratesVariation()
    {
        // Arrange
        _generator.MaxCharacters = 100;
        var results = new List<string>();

        // Act
        for (var i = 0; i < 10; i++)
        {
            results.Add(_generator.GenerateDrill(PracticePhases.SimpleRepetition));
        }

        // Assert
        Assert.All(results, result => Assert.NotEmpty(result));
        
        // Should have some variation due to randomness
        var uniqueResults = results.Distinct().Count();
        Assert.True(uniqueResults >= 1, "Should generate some variation in results");
    }

    [Fact]
    public void GenerateDrill_Patterns_GeneratesVariation()
    {
        // Arrange
        _generator.MaxCharacters = 100;
        var results = new List<string>();

        // Act
        for (var i = 0; i < 10; i++)
        {
            results.Add(_generator.GenerateDrill(PracticePhases.Patterns));
        }

        // Assert
        Assert.All(results, result => Assert.NotEmpty(result));
        
        // Should have some variation due to randomness
        var uniqueResults = results.Distinct().Count();
        Assert.True(uniqueResults >= 1, "Should generate some variation in results");
    }

    [Fact]
    public void GenerateDrill_RealWords_GeneratesVariation()
    {
        // Arrange
        _generator.MaxCharacters = 100;
        var results = new List<string>();

        // Act
        for (var i = 0; i < 10; i++)
        {
            results.Add(_generator.GenerateDrill(PracticePhases.RealWords));
        }

        // Assert
        Assert.All(results, result => Assert.NotEmpty(result));
        
        // Should have some variation due to randomness
        var uniqueResults = results.Distinct().Count();
        Assert.True(uniqueResults >= 1, "Should generate some variation in results");
    }

    #endregion

    #region Integration and State Tests

    [Fact]
    public void GenerateDrill_MultiplePhases_MaintainsCommonWordsState()
    {
        // Arrange
        _generator.CommonWords.Clear();

        // Act
        var patternsResult = _generator.GenerateDrill(PracticePhases.Patterns);
        var realWordsResult = _generator.GenerateDrill(PracticePhases.RealWords);

        // Assert
        Assert.NotEmpty(patternsResult);
        Assert.NotEmpty(realWordsResult);
        
        // CommonWords should contain both default sets after both calls
        Assert.Contains("alice", _generator.CommonWords); // From Patterns phase
        Assert.Contains("frank", _generator.CommonWords);  // From RealWords phase
    }

    [Fact]
    public void GenerateDrill_ConsistentBehavior_WithSameConfiguration()
    {
        // Arrange
        _generator.MaxCharacters = 50;
        _generator.CommonWords = ["test", "word"];

        // Act & Assert - Should consistently produce valid output
        for (var i = 0; i < 5; i++)
        {
            var result = _generator.GenerateDrill(PracticePhases.Patterns);
            Assert.NotEmpty(result);
            Assert.True(result.Length <= _generator.MaxCharacters);
            
            var words = result.Split(' ', StringSplitOptions.RemoveEmptyEntries);
            Assert.True(words.All(word => char.IsUpper(word[0])), "All words should be capitalized");
        }
    }

    [Fact]
    public void GenerateDrill_AllPhases_ProduceDistinctContent()
    {
        // Arrange
        _generator.MaxCharacters = 100;

        // Act
        var simpleResult = _generator.GenerateDrill(PracticePhases.SimpleRepetition);
        var patternsResult = _generator.GenerateDrill(PracticePhases.Patterns);
        var realWordsResult = _generator.GenerateDrill(PracticePhases.RealWords);

        // Assert
        Assert.NotEmpty(simpleResult);
        Assert.NotEmpty(patternsResult);
        Assert.NotEmpty(realWordsResult);

        // Simple repetition should have mixed case single letters
        var simpleHasSingleLetters = simpleResult.Contains(' ') && 
            simpleResult.Split(' ').Any(part => part.Length <= 2);
        
        // Patterns should have capitalized words
        var patternsHasCapitalizedWords = patternsResult.Split(' ', StringSplitOptions.RemoveEmptyEntries)
            .Any(word => word.Length > 1 && char.IsUpper(word[0]) && word[1..].All(char.IsLower));
        
        // RealWords should have fully uppercase words
        var realWordsHasUppercaseWords = realWordsResult.Split(' ', StringSplitOptions.RemoveEmptyEntries)
            .Any(word => word.All(char.IsUpper));

        Assert.True(simpleHasSingleLetters, "Simple repetition should contain short letter combinations");
        Assert.True(patternsHasCapitalizedWords, "Patterns should contain capitalized words");
        Assert.True(realWordsHasUppercaseWords, "RealWords should contain fully uppercase words");
    }

    #endregion
}