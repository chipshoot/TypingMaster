using Moq;
using TypingMaster.Business.Contract;
using TypingMaster.Business.Course;
using TypingMaster.Core.Models.Courses;

namespace TypingMaster.Tests.Course;

public class BeginnerCourseTests
{
    private readonly NumericDrillGenerator _numericDrillGenerator = new();
    private readonly ShiftDrillGenerator _shiftDrillGenerator = new();
    private readonly SymbolDrillGenerator _symbolDrillGenerator = new();

    [Fact]
    public void GeneratePracticeText_WithMockRandom_ReturnsExpectedPattern()
    {
        // Arrange
        var mockLogger = new Mock<Serilog.ILogger>();
        var mockRandom = new Mock<IRandomNumberGenerator>();

        // Setup mock to return predictable values
        mockRandom.SetupSequence(x => x.Next(2, 4))
            .Returns(3); // repetitions for first key

        var course = new BeginnerCourse(_numericDrillGenerator, _shiftDrillGenerator, _symbolDrillGenerator, mockLogger.Object, mockRandom.Object);
        var targetKeys = new[] { "a", "s" };
        var commonWords = new[] { "test", "word" };

        // Act
        var result = course.GeneratePracticeText(targetKeys, commonWords, PracticePhases.SimpleRepetition);

        // Assert
        Assert.NotEmpty(result);
        Assert.Contains("aaa", result);
    }

    [Theory]
    [InlineData(1)]
    [InlineData(2)]
    [InlineData(3)]
    public void GeneratePracticeText_NumberRow_ReturnsExpectedPattern(int point)
    {
        // Arrange
        var mockLogger = new Mock<Serilog.ILogger>();
        var course = new BeginnerCourse(_numericDrillGenerator, _shiftDrillGenerator, _symbolDrillGenerator, mockLogger.Object);

        // Act
        var result = course.GeneratePracticeText(PracticePhases.SimpleRepetition, point);

        // Assert
        Assert.NotEmpty(result);
        //Assert.Contains("111", result);
        //Assert.Contains("222", result);
    }

    [Fact]
    public void GeneratePracticeText_WithPatternPhase_GeneratesPatterns()
    {
        // Arrange
        var mockLogger = new Mock<Serilog.ILogger>();
        var mockRandom = new Mock<IRandomNumberGenerator>();

        // Setup mock for predictable pattern generation
        mockRandom.Setup(x => x.Next(1, 6)).Returns(2); // randomKeyStringsCount
        mockRandom.Setup(x => x.Next(2, 5)).Returns(3); // keyStringLength
        var callIdx = -1;
        mockRandom.Setup(x => x.Next(2)).Returns(()=> ++callIdx % 2); // key index

        var course = new BeginnerCourse(_numericDrillGenerator, _shiftDrillGenerator, _symbolDrillGenerator, mockLogger.Object, mockRandom.Object);
        var targetKeys = new[] { "a", "s" };
        var commonWords = new[] { "test" };

        // Act
        var result = course.GeneratePracticeText(targetKeys, commonWords, PracticePhases.Patterns);

        // Assert
        Assert.NotEmpty(result);
    }

    [Fact]
    public void GeneratePracticeText_WithRealWordsPhase_ReturnsWords()
    {
        // Arrange
        var mockLogger = new Mock<Serilog.ILogger>();
        var mockRandom = new Mock<IRandomNumberGenerator>();

        mockRandom.Setup(x => x.Next(It.IsAny<int>())).Returns(0); // Always return first word

        var course = new BeginnerCourse(_numericDrillGenerator, _shiftDrillGenerator, _symbolDrillGenerator, mockLogger.Object, mockRandom.Object);
        var targetKeys = new[] { "a", "s" };
        var commonWords = new[] { "test", "word" };

        // Act
        var result = course.GeneratePracticeText(targetKeys, commonWords, PracticePhases.RealWords);

        // Assert
        Assert.NotEmpty(result);
        Assert.Contains("test", result);
    }


    [Fact]
    public void GeneratePracticeText_NotSetPhase_MapsToSingleKeyFocus()
    {
        // Arrange
        var mockLogger = new Mock<Serilog.ILogger>();
        var mockNumericDrillGenerator = new Mock<INumericDrillGenerator>();
        mockNumericDrillGenerator.Setup(x => x.GenerateDrill(NumericPracticePhases.SingleKeyFocus, 5))
            .Returns(new System.Text.StringBuilder("111 222 333").ToString());

        var course = new BeginnerCourse(mockNumericDrillGenerator.Object, _shiftDrillGenerator, _symbolDrillGenerator, mockLogger.Object);

        // Act
        var result = course.GeneratePracticeText(PracticePhases.NotSet, 1);

        // Assert
        mockNumericDrillGenerator.Verify(x => x.GenerateDrill(NumericPracticePhases.SingleKeyFocus, 5), Times.Once);
        Assert.Equal("111 222 333", result);
    }

    [Theory]
    [InlineData(1, NumericPracticePhases.SingleKeyFocus)]
    [InlineData(2, NumericPracticePhases.PracticalPatterns)]
    [InlineData(0, NumericPracticePhases.SingleKeyFocus)] // Edge case: point 0 defaults to SingleKeyFocus
    [InlineData(3, NumericPracticePhases.SingleKeyFocus)] // Edge case: point 3 defaults to SingleKeyFocus
    public void GeneratePracticeText_SimpleRepetitionPhase_MapsCorrectlyBasedOnPoint(int point,
        NumericPracticePhases expectedPhase)
    {
        // Arrange
        var mockLogger = new Mock<Serilog.ILogger>();
        var mockNumericDrillGenerator = new Mock<INumericDrillGenerator>();
        var expectedText = $"drill_for_{expectedPhase}";
        mockNumericDrillGenerator.Setup(x => x.GenerateDrill(expectedPhase, 5))
            .Returns(new System.Text.StringBuilder(expectedText).ToString());

        var course = new BeginnerCourse(mockNumericDrillGenerator.Object, _shiftDrillGenerator, _symbolDrillGenerator, mockLogger.Object);

        // Act
        var result = course.GeneratePracticeText(PracticePhases.SimpleRepetition, point);

        // Assert
        mockNumericDrillGenerator.Verify(x => x.GenerateDrill(expectedPhase, 5), Times.Once);
        Assert.Equal(expectedText, result);
    }


    [Theory]
    [InlineData(1, NumericPracticePhases.SequenceCombos)]
    [InlineData(2, NumericPracticePhases.FullIntegration)]
    [InlineData(0, NumericPracticePhases.SequenceCombos)] // Edge case: point 0 defaults to SequenceCombos
    [InlineData(3, NumericPracticePhases.SequenceCombos)] // Edge case: point 3 defaults to SequenceCombos
    public void GeneratePracticeText_PatternsPhase_MapsCorrectlyBasedOnPoint(int point,
        NumericPracticePhases expectedPhase)
    {
        // Arrange
        var mockLogger = new Mock<Serilog.ILogger>();
        var mockNumericDrillGenerator = new Mock<INumericDrillGenerator>();
        var expectedText = $"drill_for_{expectedPhase}";
        mockNumericDrillGenerator.Setup(x => x.GenerateDrill(expectedPhase, 5))
            .Returns(new System.Text.StringBuilder(expectedText).ToString());

        var course = new BeginnerCourse(mockNumericDrillGenerator.Object, _shiftDrillGenerator, _symbolDrillGenerator, mockLogger.Object);

        // Act
        var result = course.GeneratePracticeText(PracticePhases.Patterns, point);

        // Assert
        mockNumericDrillGenerator.Verify(x => x.GenerateDrill(expectedPhase, 5), Times.Once);
        Assert.Equal(expectedText, result);
    }

    [Theory]
    [InlineData(1, NumericPracticePhases.HorizontalCombination)]
    [InlineData(2, NumericPracticePhases.DomainSpecialization)]
    [InlineData(0, NumericPracticePhases.HorizontalCombination)] // Edge case: point 0 defaults to HorizontalCombination
    [InlineData(3, NumericPracticePhases.HorizontalCombination)] // Edge case: point 3 defaults to HorizontalCombination
    public void GeneratePracticeText_RealWordsPhase_MapsCorrectlyBasedOnPoint(int point,
        NumericPracticePhases expectedPhase)
    {
        // Arrange
        var mockLogger = new Mock<Serilog.ILogger>();
        var mockNumericDrillGenerator = new Mock<INumericDrillGenerator>();
        var expectedText = $"drill_for_{expectedPhase}";
        mockNumericDrillGenerator.Setup(x => x.GenerateDrill(expectedPhase, 5))
            .Returns(new System.Text.StringBuilder(expectedText).ToString());

        var course = new BeginnerCourse(mockNumericDrillGenerator.Object, _shiftDrillGenerator, _symbolDrillGenerator, mockLogger.Object);

        // Act
        var result = course.GeneratePracticeText(PracticePhases.RealWords, point);

        // Assert
        mockNumericDrillGenerator.Verify(x => x.GenerateDrill(expectedPhase, 5), Times.Once);
        Assert.Equal(expectedText, result);
    }


    [Fact]
    public void GeneratePracticeText_NumericDrill_ConfiguresGeneratorCorrectly()
    {
        // Arrange
        var mockLogger = new Mock<Serilog.ILogger>();
        var mockNumericDrillGenerator = new Mock<INumericDrillGenerator>();
        mockNumericDrillGenerator.SetupAllProperties();
        mockNumericDrillGenerator.Setup(x => x.GenerateDrill(It.IsAny<NumericPracticePhases>(), It.IsAny<int>()))
            .Returns(new System.Text.StringBuilder("test").ToString());

        var course = new BeginnerCourse(mockNumericDrillGenerator.Object, _shiftDrillGenerator, _symbolDrillGenerator, mockLogger.Object)
        {
            MaxCharacters = 150
        };

        // Act
        _ = course.GeneratePracticeText(PracticePhases.SimpleRepetition, 1);

        // Assert
        mockNumericDrillGenerator.VerifySet(x => x.EnableCapital = false, Times.Once);
        mockNumericDrillGenerator.VerifySet(x => x.EnableSymbol = false, Times.Once);
        mockNumericDrillGenerator.VerifySet(x => x.MaxCharacters = 150, Times.Once);
        mockNumericDrillGenerator.Verify(x => x.GenerateDrill(NumericPracticePhases.SingleKeyFocus, 5), Times.Once);
    }


    [Fact]
    public void GeneratePracticeText_NumericDrill_TrimsEndWhitespace()
    {
        // Arrange
        var mockLogger = new Mock<Serilog.ILogger>();
        var mockNumericDrillGenerator = new Mock<INumericDrillGenerator>();
        mockNumericDrillGenerator.Setup(x => x.GenerateDrill(It.IsAny<NumericPracticePhases>(), It.IsAny<int>()))
            .Returns(new System.Text.StringBuilder("123 456   \n  ").ToString());

        var course = new BeginnerCourse(mockNumericDrillGenerator.Object, _shiftDrillGenerator, _symbolDrillGenerator, mockLogger.Object);

        // Act
        var result = course.GeneratePracticeText(PracticePhases.SimpleRepetition, 1);

        // Assert
        Assert.Equal("123 456", result);
        Assert.False(result.EndsWith(' '));
        Assert.False(result.EndsWith('\n'));
    }

    [Theory]
    [InlineData(PracticePhases.SimpleRepetition)]
    [InlineData(PracticePhases.Patterns)]
    [InlineData(PracticePhases.RealWords)]
    [InlineData(PracticePhases.NotSet)]
    public void GeneratePracticeText_AllValidPhases_ProducesNonEmptyResult(PracticePhases phase)
    {
        // Arrange
        var mockLogger = new Mock<Serilog.ILogger>();
        var mockNumericDrillGenerator = new Mock<INumericDrillGenerator>();
        mockNumericDrillGenerator.Setup(x => x.GenerateDrill(It.IsAny<NumericPracticePhases>(), It.IsAny<int>()))
            .Returns(new System.Text.StringBuilder("numeric drill content").ToString());

        var course = new BeginnerCourse(mockNumericDrillGenerator.Object, _shiftDrillGenerator, _symbolDrillGenerator, mockLogger.Object);

        // Act
        var result = course.GeneratePracticeText(phase, 1);

        // Assert
        Assert.NotNull(result);
        Assert.NotEmpty(result);
    }

    [Theory]
    [InlineData(-1)]
    [InlineData(int.MinValue)]
    [InlineData(int.MaxValue)]
    [InlineData(100)]
    public void GeneratePracticeText_EdgeCasePoints_HandlesGracefully(int point)
    {
        // Arrange
        var mockLogger = new Mock<Serilog.ILogger>();
        var mockNumericDrillGenerator = new Mock<INumericDrillGenerator>();
        mockNumericDrillGenerator.Setup(x => x.GenerateDrill(It.IsAny<NumericPracticePhases>(), It.IsAny<int>()))
            .Returns(new System.Text.StringBuilder("edge case content").ToString());

        var course = new BeginnerCourse(mockNumericDrillGenerator.Object, _shiftDrillGenerator, _symbolDrillGenerator, mockLogger.Object);

        // Act & Assert - Should not throw exception
        var result = course.GeneratePracticeText(PracticePhases.SimpleRepetition, point);
        Assert.NotNull(result);
    }


    [Fact]
    public void GeneratePracticeText_DefaultPhase_MapsToNotSetPhase()
    {
        // Arrange
        var mockLogger = new Mock<Serilog.ILogger>();
        var mockNumericDrillGenerator = new Mock<INumericDrillGenerator>();
        mockNumericDrillGenerator.Setup(x => x.GenerateDrill(NumericPracticePhases.NotSet, 5))
            .Returns(new System.Text.StringBuilder("default phase content").ToString());

        var course = new BeginnerCourse(mockNumericDrillGenerator.Object, _shiftDrillGenerator, _symbolDrillGenerator, mockLogger.Object);

        // Act
        var result = course.GeneratePracticeText((PracticePhases)999, 1); // Invalid enum value

        // Assert
        mockNumericDrillGenerator.Verify(x => x.GenerateDrill(NumericPracticePhases.NotSet, 5), Times.Once);
        Assert.Equal("default phase content", result);
    }

    [Fact]
    public void GeneratePracticeText_NumericDrill_UsesCorrectDrillCount()
    {
        // Arrange
        var mockLogger = new Mock<Serilog.ILogger>();
        var mockNumericDrillGenerator = new Mock<INumericDrillGenerator>();
        mockNumericDrillGenerator.Setup(x => x.GenerateDrill(It.IsAny<NumericPracticePhases>(), 5))
            .Returns(new System.Text.StringBuilder("counted drill").ToString());

        var course = new BeginnerCourse(mockNumericDrillGenerator.Object, _shiftDrillGenerator, _symbolDrillGenerator, mockLogger.Object);

        // Act
        _ = course.GeneratePracticeText(PracticePhases.SimpleRepetition, 1);

        // Assert
        mockNumericDrillGenerator.Verify(x => x.GenerateDrill(It.IsAny<NumericPracticePhases>(), 5), Times.Once);
    }

    [Fact]
    public void GeneratePracticeText_EmptyDrillResult_ReturnsEmptyString()
    {
        // Arrange
        var mockLogger = new Mock<Serilog.ILogger>();
        var mockNumericDrillGenerator = new Mock<INumericDrillGenerator>();
        mockNumericDrillGenerator.Setup(x => x.GenerateDrill(It.IsAny<NumericPracticePhases>(), It.IsAny<int>()))
            .Returns(new System.Text.StringBuilder("").ToString());

        var course = new BeginnerCourse(mockNumericDrillGenerator.Object, _shiftDrillGenerator, _symbolDrillGenerator, mockLogger.Object);

        // Act
        var result = course.GeneratePracticeText(PracticePhases.SimpleRepetition, 1);

        // Assert
        Assert.Equal(string.Empty, result);
    }

    [Fact]
    public void GeneratePracticeText_WhitespaceOnlyDrillResult_ReturnsEmptyString()
    {
        // Arrange
        var mockLogger = new Mock<Serilog.ILogger>();
        var mockNumericDrillGenerator = new Mock<INumericDrillGenerator>();
        mockNumericDrillGenerator.Setup(x => x.GenerateDrill(It.IsAny<NumericPracticePhases>(), It.IsAny<int>()))
            .Returns(new System.Text.StringBuilder("   \t\n  ").ToString());

        var course = new BeginnerCourse(mockNumericDrillGenerator.Object, _shiftDrillGenerator, _symbolDrillGenerator, mockLogger.Object);

        // Act
        var result = course.GeneratePracticeText(PracticePhases.SimpleRepetition, 1);

        // Assert
        Assert.Equal(string.Empty, result);
    }
}