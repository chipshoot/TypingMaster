using TypingMaster.Business.Course;
using TypingMaster.Core.Models.Courses;
using Xunit;
using System.Text.RegularExpressions;

namespace TypingMaster.Tests.Course;

public class NumericDrillGeneratorTests
{
    private readonly NumericDrillGenerator _generator = new();


    [Theory]
    [InlineData(PracticePhases.SimpleRepetition, 5, "11111")]
    [InlineData(PracticePhases.RealWords, 3, "1 2 3")]
    public void GenerateDrill_ReturnsExpectedResult(PracticePhases phase, int count, string expected)
    {
        // Act
        var result = _generator.GenerateDrill(phase, count);

        // Assert
        Assert.Equal(expected, result);
    }

    [Fact]
    public void GenerateDrill_Patterns_ReturnsRealisticNumericPatterns()
    {
        // Act
        var result = _generator.GenerateDrill(PracticePhases.Patterns, 10);

        // Assert
        Assert.NotEmpty(result);

        // Check that the result contains realistic patterns
        var patterns = result.Split(' ', StringSplitOptions.RemoveEmptyEntries);
        Assert.Equal(10, patterns.Length);

        // Verify at least some common pattern types are present
        var hasDatePattern = patterns.Any(p => Regex.IsMatch(p, @"\d{4}-\d{2}-\d{2}") ||
                                                Regex.IsMatch(p, @"\d{2}/\d{2}/\d{4}") ||
                                                Regex.IsMatch(p, @"\d{2}\.\d{2}\.\d{4}"));

        var hasTimePattern = patterns.Any(p => Regex.IsMatch(p, @"\d{2}:\d{2}") ||
                                               Regex.IsMatch(p, @"\d{2}:\d{2}:\d{2}") ||
                                               Regex.IsMatch(p, @"\d{1,2}:\d{2} (AM|PM)"));

        var hasCurrencyPattern = patterns.Any(p => Regex.IsMatch(p, @"[\$€£]\d+\.\d{2}"));

        var hasPercentagePattern = patterns.Any(p => Regex.IsMatch(p, @"\d+\.\d+%"));

        var hasFractionPattern = patterns.Any(p => Regex.IsMatch(p, @"\d+/\d+"));

        var hasCoordinatePattern = patterns.Any(p => Regex.IsMatch(p, @"\d+\.\d+°[NS], \d+\.\d+°[EW]"));

        var hasPhonePattern = patterns.Any(p => Regex.IsMatch(p, @"\(\d{3}\) \d{3}-\d{4}"));

        var hasCreditCardPattern = patterns.Any(p => Regex.IsMatch(p, @"\*{4} \*{4} \*{4} \d{4}"));

        // At least one pattern type should be present (since we're generating 10 patterns)
        Assert.True(hasDatePattern || hasTimePattern || hasCurrencyPattern || hasPercentagePattern ||
                   hasFractionPattern || hasCoordinatePattern || hasPhonePattern || hasCreditCardPattern,
                   "Generated patterns should contain at least one realistic numeric pattern type");
    }

    [Fact]
    public void GenerateDrill_Patterns_GeneratesCorrectCount()
    {
        // Arrange
        var count = 5;

        // Act
        var result = _generator.GenerateDrill(PracticePhases.Patterns, count);

        // Assert
        var patterns = result.Split(' ', StringSplitOptions.RemoveEmptyEntries);
        Assert.Equal(count, patterns.Length);
    }

    [Fact]
    public void GenerateDrill_UnsupportedPhase_ThrowsArgumentOutOfRangeException()
    {
        // Arrange
        var invalidPhase = (PracticePhases)999;

        // Act & Assert
        Assert.Throws<ArgumentOutOfRangeException>(() => _generator.GenerateDrill(invalidPhase, 5));
    }

    [Fact]
    public void GenerateDrill_Patterns_GeneratesDifferentResults()
    {
        // Act - Generate multiple results to check for randomness
        var result1 = _generator.GenerateDrill(PracticePhases.Patterns, 5);
        var result2 = _generator.GenerateDrill(PracticePhases.Patterns, 5);

        // Assert - Results should be different due to randomness
        // Note: There's a small chance they could be the same, but very unlikely with 5 patterns
        Assert.NotEqual(result1, result2);
    }
}