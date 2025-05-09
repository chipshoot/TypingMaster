//using Moq;
//using Serilog;
//using TypingMaster.Client.Services;
//using TypingMaster.Core.Models;
//using TypingMaster.Core.Models.Courses;

//namespace TypingMaster.Tests.Course
//{
//    public class PhaseManagementTests
//    {
//        private readonly Mock<ILogger> _mockLogger;
//        private readonly Mock<IAccountWebService> _mockAccountWebService;
//        private readonly TypingTrainer _typingTrainer;

//        // Define some test stats
//        private readonly StatsBase _belowThreshold = new() { Wpm = 25, Accuracy = 0.85 };
//        private readonly StatsBase _simpleRepetitionPassed = new() { Wpm = 32, Accuracy = 0.92 };
//        private readonly StatsBase _patternsPassed = new() { Wpm = 38, Accuracy = 0.94 };
//        private readonly StatsBase _realWordsPassed = new() { Wpm = 45, Accuracy = 0.97 };

//        public PhaseManagementTests()
//        {
//            _mockLogger = new Mock<ILogger>();
//            _mockAccountWebService = new Mock<IAccountWebService>();
//            _typingTrainer = new TypingTrainer(_mockAccountWebService.Object, _mockLogger.Object);
//        }

//        [Fact]
//        public void AdvanceToNextPhase_FromSimpleRepetition_WithGoodStats_ReturnsPatterns()
//        {
//            // Arrange
//            var initialPhase = PracticePhases.SimpleRepetition;
//            var expectedNextPhase = PracticePhases.Patterns;

//            // Act
//            var resultPhase = _typingTrainer.AdvanceToNextPhase(initialPhase, _simpleRepetitionPassed);

//            // Assert
//            Assert.Equal(expectedNextPhase, resultPhase);
//        }

//        [Fact]
//        public void AdvanceToNextPhase_FromSimpleRepetition_WithPoorStats_StaysInPhase()
//        {
//            // Arrange
//            var initialPhase = PracticePhases.SimpleRepetition;

//            // Act
//            var resultPhase = _typingTrainer.AdvanceToNextPhase(initialPhase, _belowThreshold);

//            // Assert
//            Assert.Equal(initialPhase, resultPhase);
//        }

//        [Fact]
//        public void AdvanceToNextPhase_FromPatterns_WithGoodStats_ReturnsRealWords()
//        {
//            // Arrange
//            var initialPhase = PracticePhases.Patterns;
//            var expectedNextPhase = PracticePhases.RealWords;

//            // Act
//            var resultPhase = _typingTrainer.AdvanceToNextPhase(initialPhase, _patternsPassed);

//            // Assert
//            Assert.Equal(expectedNextPhase, resultPhase);
//        }

//        [Fact]
//        public void AdvanceToNextPhase_FromPatterns_WithPoorStats_StaysInPhase()
//        {
//            // Arrange
//            var initialPhase = PracticePhases.Patterns;

//            // Act
//            var resultPhase = _typingTrainer.AdvanceToNextPhase(initialPhase, _belowThreshold);

//            // Assert
//            Assert.Equal(initialPhase, resultPhase);
//        }

//        [Fact]
//        public void AdvanceToNextPhase_FromRealWords_WithGoodStats_CyclesBackToSimpleRepetition()
//        {
//            // Arrange
//            var initialPhase = PracticePhases.RealWords;
//            var expectedNextPhase = PracticePhases.SimpleRepetition;

//            // Act
//            var resultPhase = _typingTrainer.AdvanceToNextPhase(initialPhase, _realWordsPassed);

//            // Assert
//            Assert.Equal(expectedNextPhase, resultPhase);
//        }

//        [Fact]
//        public void AdvanceToNextPhase_FromRealWords_WithPoorStats_StaysInPhase()
//        {
//            // Arrange
//            var initialPhase = PracticePhases.RealWords;

//            // Act
//            var resultPhase = _typingTrainer.AdvanceToNextPhase(initialPhase, _belowThreshold);

//            // Assert
//            Assert.Equal(initialPhase, resultPhase);
//        }

//        [Fact]
//        public void AdvanceToNextPhase_FromNotSet_DefaultsToSimpleRepetition()
//        {
//            // Arrange
//            var initialPhase = PracticePhases.NotSet;
//            var expectedNextPhase = PracticePhases.SimpleRepetition;

//            // Act
//            var resultPhase = _typingTrainer.AdvanceToNextPhase(initialPhase, _belowThreshold);

//            // Assert
//            Assert.Equal(expectedNextPhase, resultPhase);
//        }

//        [Theory]
//        [InlineData(PracticePhases.SimpleRepetition, PracticePhases.Patterns)]
//        [InlineData(PracticePhases.Patterns, PracticePhases.RealWords)]
//        [InlineData(PracticePhases.RealWords, PracticePhases.SimpleRepetition)]
//        [InlineData(PracticePhases.NotSet, PracticePhases.SimpleRepetition)]
//        public void AdvanceToNextPhase_TheoryTest_WithGoodStats_ReturnsCorrectNextPhase(PracticePhases initialPhase, PracticePhases expectedNextPhase)
//        {
//            // Arrange - create stats sufficient to pass any threshold
//            var excellentStats = new StatsBase { Wpm = 50, Accuracy = 0.98 };

//            // Act
//            var resultPhase = _typingTrainer.AdvanceToNextPhase(initialPhase, excellentStats);

//            // Assert
//            Assert.Equal(expectedNextPhase, resultPhase);
//        }

//        [Fact]
//        public void AdvanceToNextPhase_FullCycle_WithGoodStats_ReturnsToOriginalPhase()
//        {
//            // Arrange
//            var initialPhase = PracticePhases.SimpleRepetition;
//            var excellentStats = new StatsBase { Wpm = 50, Accuracy = 0.98 };

//            // Act - go through the entire cycle
//            var phase1 = _typingTrainer.AdvanceToNextPhase(initialPhase, excellentStats);           // SimpleRepetition -> Patterns
//            var phase2 = _typingTrainer.AdvanceToNextPhase(phase1, excellentStats);                 // Patterns -> RealWords
//            var phase3 = _typingTrainer.AdvanceToNextPhase(phase2, excellentStats);                 // RealWords -> SimpleRepetition

//            // Assert
//            Assert.Equal(PracticePhases.Patterns, phase1);
//            Assert.Equal(PracticePhases.RealWords, phase2);
//            Assert.Equal(initialPhase, phase3);  // Should be back to SimpleRepetition
//        }
//    }
//}