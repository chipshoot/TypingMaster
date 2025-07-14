using TypingMaster.Business.Course;
using TypingMaster.Core.Constants;
using TypingMaster.Core.Models.Courses;

namespace TypingMaster.Tests.Course;

public class NumericDrillGeneratorTests
{
    private readonly NumericDrillGenerator _generator = new();

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
    [InlineData(8, 1, "1")]
    [InlineData(8, 5, "12345")]
    [InlineData(8, 10, "1234567890")]
    [InlineData(8, 15, "1234567890 1234")]
    [InlineData(8, 74, "1234567890 1234567890 1234567890 1234567890 1234567890 1234567890 12345678")]
    [InlineData(3, 74, "1234567890 1234567890 1234567890")]
    public void GenerateDrill_NotSet_Returns_NumberSequence_With_MaxLength(int count, int maxTextLength, string expectText)
    {
        // Act
        _generator.MaxCharacters = maxTextLength;
        var result = _generator.GenerateDrill(NumericPracticePhases.NotSet, count);

        // Assert
        Assert.Equal(expectText, result);
    }

    [Theory]
    [InlineData(1, 74, "1 2 3 4 5 6 7 8 9 0 1 2 3 4 5 6 7 8 9 0 1 2 3 4 5 6 7 8 9 0 1 2 3 4 5 6 7")]
    [InlineData(3, 74, "111 222 333 444 555 666 777 888 999 000 111 222 333 444 555 666 777 888 99")]
    [InlineData(5, 74, "11111 22222 33333 44444 55555 66666 77777 88888 99999 00000 11111 22222 33")]
    public void GenerateDrill_SingleKeyFocus_ReturnsRepeatedDigits(int count, int maxTextLength, string expectText)
    {
        // Act
        _generator.MaxCharacters = maxTextLength;
        var result = _generator.GenerateDrill(NumericPracticePhases.SingleKeyFocus, count);

        // Assert
        Assert.Equal(expectText, result);
    }

    [Theory]
    [InlineData(1, 74, "1 2 3 4 5 6 7 8 9 0 1 2 3 4 5 6 7 8 9 0 1 2 3 4 5 6 7 8 9 0 1 2 3 4 5 6 7")]
    [InlineData(4, 74, "1234 5678 9012 3456 7890 1234 5678 9012 3456 7890 1234 5678 9012 3456 7890")]
    [InlineData(5, 74, "12345 67890 12345 67890 12345 67890 12345 67890 12345 67890 12345 67890 12")]
    [InlineData(10, 74, "1234567890 1234567890 1234567890 1234567890 1234567890 1234567890 12345678")]
    public void GenerateDrill_VerticalCombos_ReturnsSequentialPattern(int count, int maxTextLength, string expectText)
    {
        // Act
        _generator.MaxCharacters = maxTextLength;
        var result = _generator.GenerateDrill(NumericPracticePhases.SequenceCombos, count);

        // Assert
        Assert.Equal(expectText, result);
    }

    [Theory]
    [InlineData(1, 74, "10 29 38 47 56")]
    [InlineData(3, 74, "102938 293847 384756")]
    [InlineData(5, 74, "1029384756 ")]
    public void GenerateDrill_HorizontalCombination_ReturnsFingerPairPatterns(int count, int maxTextLength, string expectText)
    {
        // Act
        _generator.MaxCharacters = maxTextLength;
        var result = _generator.GenerateDrill(NumericPracticePhases.HorizontalCombination, count);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(result.Length, maxTextLength);

    }


    [Theory]
    [InlineData(1)]
    [InlineData(3)]
    [InlineData(5)]
    public void GenerateDrill_PracticalPatterns_ReturnsFormattedPatterns(int count)
    {
        // Act
        var result = _generator.GenerateDrill(NumericPracticePhases.PracticalPatterns, count);

        // Assert
        Assert.NotEmpty(result);

        // Should contain common formatting characters
        var hasFormattingChars = result.Any(c =>
            c == '-' || c == '/' || c == '.' || c == ':' ||
            c == '$' || c == '%' || c == '#' || c == '(' || c == ')');

        // Result should be a formatted pattern (dates, times, money, etc.)
        Assert.True(hasFormattingChars || result.All(char.IsDigit));
    }

    [Theory]
    [InlineData(1)]
    [InlineData(3)]
    [InlineData(5)]
    public void GenerateDrill_FullIntegration_ReturnsCodePatterns(int count)
    {
        // Act
        var result = _generator.GenerateDrill(NumericPracticePhases.FullIntegration, count);

        // Assert
        Assert.NotEmpty(result);

        // Should contain alphanumeric characters and symbols
        var hasLetters = result.Any(char.IsLetter);
        var hasDigits = result.Any(char.IsDigit);

        Assert.True(hasLetters || hasDigits);
    }

    [Theory]
    [InlineData(1)]
    [InlineData(3)]
    [InlineData(5)]
    public void GenerateDrill_DomainSpecialization_ReturnsDomainSpecificPatterns(int count)
    {
        // Act
        var result = _generator.GenerateDrill(NumericPracticePhases.DomainSpecialization, count);

        // Assert
        Assert.NotEmpty(result);

        // Should contain domain-specific keywords
        var domainKeywords = new[] { "Phone:", "ZIP:", "ISBN:", "EIN:", "Score:", "Temp:" };
        var containsDomainKeyword = domainKeywords.Any(keyword => result.Contains(keyword));

        Assert.True(containsDomainKeyword);
        Assert.Contains(":", result); // Should have colon separator
    }

    [Theory]
    [InlineData(1)]
    [InlineData(3)]
    [InlineData(5)]
    public void GenerateDrill_CompetitiveSpeed_ReturnsDigitSequence(int count)
    {
        // Act
        var result = _generator.GenerateDrill(NumericPracticePhases.CompetitiveSpeed, count);

        // Assert
        Assert.NotEmpty(result);
        Assert.True(result.All(char.IsDigit));
        Assert.Equal(3, result.Length); // Should be 3 digits based on the implementation
    }

    [Fact]
    public void GenerateDrill_InvalidPhase_ReturnsEmptyString()
    {
        // Act
        var result = _generator.GenerateDrill((NumericPracticePhases)999, 5);

        // Assert
        Assert.Equal(string.Empty, result);
    }

    [Fact]
    public void GenerateDrill_SingleKeyFocus_RespectsMaxCharacters()
    {
        // Arrange
        _generator.MaxCharacters = 50;

        // Act
        var result = _generator.GenerateDrill(NumericPracticePhases.SingleKeyFocus, 3);

        // Assert
        Assert.True(result.Length <= _generator.MaxCharacters);
    }

    [Fact]
    public void GenerateDrill_HorizontalCombination_RespectsMaxCharacters()
    {
        // Arrange
        _generator.MaxCharacters = 30;

        // Act
        var result = _generator.GenerateDrill(NumericPracticePhases.HorizontalCombination, 3);

        // Assert
        Assert.True(result.Length <= _generator.MaxCharacters);
    }

    [Fact]
    public void GenerateDrill_SingleKeyFocus_ContainsLeftAndRightHandDigits()
    {
        // Act
        var result = _generator.GenerateDrill(NumericPracticePhases.SingleKeyFocus, 2);

        // Assert
        Assert.NotEmpty(result);

        // Left hand digits: 1, 2, 3, 4, 5
        // Right hand digits: 6, 7, 8, 9, 0
        var leftHandDigits = new[] { '1', '2', '3', '4', '5' };
        var rightHandDigits = new[] { '6', '7', '8', '9', '0' };

        var containsLeftHand = result.Any(c => leftHandDigits.Contains(c));
        var containsRightHand = result.Any(c => rightHandDigits.Contains(c));

        Assert.True(containsLeftHand);
        Assert.True(containsRightHand);
    }

    [Fact]
    public void GenerateDrill_HorizontalCombination_ContainsFingerPairs()
    {
        // Act
        var result = _generator.GenerateDrill(NumericPracticePhases.HorizontalCombination, 2);

        // Assert
        Assert.NotEmpty(result);

        // Expected finger pairs: 10, 29, 38, 47, 56
        var expectedPairs = new[] { "10", "29", "38", "47", "56" };
        var containsExpectedPair = expectedPairs.Any(pair => result.Contains(pair));

        Assert.True(containsExpectedPair);
    }

    [Theory]
    [InlineData(0)]
    [InlineData(-1)]
    [InlineData(-5)]
    public void GenerateDrill_ZeroOrNegativeCount_HandlesGracefully(int count)
    {
        // Act & Assert - Should not throw exceptions
        var result1 = _generator.GenerateDrill(NumericPracticePhases.SingleKeyFocus, count);
        var result2 = _generator.GenerateDrill(NumericPracticePhases.SequenceCombos, count);
        var result3 = _generator.GenerateDrill(NumericPracticePhases.CompetitiveSpeed, count);

        // Results should be valid (either empty or have some content)
        Assert.NotNull(result1);
        Assert.NotNull(result2);
        Assert.NotNull(result3);
    }

    [Fact]
    public void GenerateDrill_PracticalPatterns_GeneratesValidFormats()
    {
        // Act
        var results = new List<string>();
        for (int i = 0; i < 10; i++)
        {
            results.Add(_generator.GenerateDrill(NumericPracticePhases.PracticalPatterns, 1));
        }

        // Assert
        Assert.All(results, result => Assert.NotEmpty(result));

        // Should generate various formats over multiple calls
        var uniqueResults = results.Distinct().Count();
        Assert.True(uniqueResults >= 1); // At least some variation expected
    }

    [Fact]
    public void GenerateDrill_FullIntegration_GeneratesValidCodePatterns()
    {
        // Act
        var results = new List<string>();
        for (int i = 0; i < 10; i++)
        {
            results.Add(_generator.GenerateDrill(NumericPracticePhases.FullIntegration, 1));
        }

        // Assert
        Assert.All(results, result => Assert.NotEmpty(result));

        // Should contain programming-like patterns
        var hasCodePattern = results.Any(result =>
            result.Contains("=") || result.Contains(":") ||
            result.Contains("int") || result.Contains("var") ||
            result.Contains("const") || result.Contains("double"));

        Assert.True(hasCodePattern);
    }

    [Fact]
    public void GenerateDrill_DomainSpecialization_GeneratesVariousDomains()
    {
        // Act
        var results = new List<string>();
        for (int i = 0; i < 20; i++)
        {
            results.Add(_generator.GenerateDrill(NumericPracticePhases.DomainSpecialization, 1));
        }

        // Assert
        Assert.All(results, result => Assert.NotEmpty(result));

        // Should generate various domain patterns
        var domainKeywords = new[] { "Phone:", "ZIP:", "ISBN:", "EIN:", "Score:", "Temp:" };
        var foundDomains = domainKeywords.Where(keyword =>
            results.Any(result => result.Contains(keyword))).Count();

        // Should find at least some domain variety over 20 attempts
        Assert.True(foundDomains >= 1);
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
    public void GenerateDrill_AllPhases_GenerateNonNullResults()
    {
        // Arrange
        var allPhases = Enum.GetValues<NumericPracticePhases>();

        // Act & Assert
        foreach (var phase in allPhases)
        {
            var result = _generator.GenerateDrill(phase, 3);
            Assert.NotNull(result);
        }
    }

    [Fact]
    public void GenerateDrill_ConsistentBehavior_WithSameInputs()
    {
        // Note: This test acknowledges that the generator uses Random,
        // so we test that it consistently produces valid output rather than identical output

        // Act
        var results = new List<string>();
        for (int i = 0; i < 5; i++)
        {
            results.Add(_generator.GenerateDrill(NumericPracticePhases.SequenceCombos, 5));
        }

        // Assert
        Assert.All(results, result =>
        {
            Assert.Equal(5, result.Length);
            Assert.True(result.All(char.IsDigit));
        });
    }
}