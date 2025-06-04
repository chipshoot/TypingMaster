using Moq;
using TypingMaster.Business.Course;
using TypingMaster.Core.Models.Courses;
using Xunit;

namespace TypingMaster.Tests.Course;

public class BeginnerCourseTests
{
    [Fact]
    public void GeneratePracticeText_WithMockRandom_ReturnsExpectedPattern()
    {
        // Arrange
        var mockLogger = new Mock<Serilog.ILogger>();
        var mockRandom = new Mock<IRandomNumberGenerator>();

        // Setup mock to return predictable values
        mockRandom.SetupSequence(x => x.Next(2, 4))
            .Returns(3); // repetitions for first key

        var course = new BeginnerCourse(mockLogger.Object, mockRandom.Object);
        var targetKeys = new[] { "a", "s" };
        var commonWords = new[] { "test", "word" };

        // Act
        var result = course.GeneratePracticeText(targetKeys, commonWords, PracticePhases.SimpleRepetition);

        // Assert
        Assert.NotEmpty(result);
        Assert.Contains("aaa", result);
    }

    [Fact]
    public void GeneratePracticeText_NumberRow_ReturnsExpectedPattern()
    {
        // Arrange
        var mockLogger = new Mock<Serilog.ILogger>();
        var course = new BeginnerCourse(mockLogger.Object);

        // Act
        var result = course.GeneratePracticeText(PracticePhases.SimpleRepetition);

        // Assert
        Assert.NotEmpty(result);
        Assert.Contains("111", result);
        Assert.Contains("222", result);
    }

    [Fact]
    public void GeneratePracticeText_WithPatternPhase_GeneratesPatterns()
    {
        // Arrange
        var mockLogger = new Mock<Serilog.ILogger>();
        var mockRandom = new Mock<IRandomNumberGenerator>();

        // Setup mock for predictable pattern generation
        mockRandom.SetupSequence(x => x.Next(1, 6)).Returns(2); // randomKeyStringsCount
        mockRandom.SetupSequence(x => x.Next(2, 5)).Returns(3); // keyStringLength
        mockRandom.SetupSequence(x => x.Next(It.IsAny<int>())).Returns(0); // key index

        var course = new BeginnerCourse(mockLogger.Object, mockRandom.Object);
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

        var course = new BeginnerCourse(mockLogger.Object, mockRandom.Object);
        var targetKeys = new[] { "a", "s" };
        var commonWords = new[] { "test", "word" };

        // Act
        var result = course.GeneratePracticeText(targetKeys, commonWords, PracticePhases.RealWords);

        // Assert
        Assert.NotEmpty(result);
        Assert.Contains("test", result);
    }
}