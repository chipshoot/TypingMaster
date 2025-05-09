//using AutoMapper;
//using Moq;
//using Serilog;
//using TypingMaster.Business;
//using TypingMaster.Core.Models;
//using TypingMaster.Core.Models.Courses;
//using TypingMaster.DataAccess.Data;
//using TypingMaster.Tests.Utility;

//namespace TypingMaster.Tests
//{
//    public class BeginnerCourseTests : IDisposable
//    {
//        private readonly Mock<ILogger> _mockLogger;
//        private readonly Mock<IMapper> _mockMapper;
//        private readonly Mock<ICourseRepository> _mockRepository;
//        private const int PhaseAttemptThreshold = 30;
//        private static BeginnerCourse _course;
//        private static bool _initialized = false;
//        private static readonly SemaphoreSlim _initializationLock = new SemaphoreSlim(1, 1);
//        private static string _tempLessonDataPath;

//        //todo: update test class to meet the new design of the ReportService class
//        public BeginnerCourseTests()
//        {
//            _mockLogger = new Mock<ILogger>();
//            _mockMapper = new Mock<IMapper>();
//            _mockRepository = new Mock<ICourseRepository>();
//            InitializeAsync().GetAwaiter().GetResult();
//        }

//        private async Task InitializeAsync()
//        {
//            if (_initialized)
//                return;

//            await _initializationLock.WaitAsync();
//            try
//            {
//                if (!_initialized)
//                {
//                    // Create a temporary directory for test resources
//                    var testResourcesDir = Path.Combine(Path.GetTempPath(), "TypingMasterTests", "Resources", "LessonData");
//                    Directory.CreateDirectory(testResourcesDir);
//                    _tempLessonDataPath = Path.Combine(testResourcesDir, "beginner-course-lessons.json");

//                    // Create a minimal lesson data file for testing
//                    var lessonData = @"{
//                        ""lessonInstructions"": {
//                            ""homeRow"": {
//                                ""instruction"": ""Place your fingers on the home row keys: a s d f j k l ;"",
//                                ""description"": ""Home row key practice""
//                            }
//                        },
//                        ""lessons"": [
//                            {
//                                ""id"": 1,
//                                ""target"": [""a"", ""s"", ""d"", ""f""],
//                                ""instructionKey"": ""homeRow"",
//                                ""description"": ""Practice the left hand home row keys"",
//                                ""point"": 1
//                            },
//                            {
//                                ""id"": 2,
//                                ""target"": [""a"", ""s"", ""d"", ""f"", ""j""],
//                                ""instructionKey"": ""homeRow"",
//                                ""description"": ""Practice the left and right hand home row keys"",
//                                ""point"": 1
//                            },
//                            {
//                                ""id"": 3,
//                                ""target"": [""a"", ""s"", ""d"", ""f"", ""j"", ""k""],
//                                ""instructionKey"": ""homeRow"",
//                                ""description"": ""Practice the left and right hand home row keys"",
//                                ""point"": 1
//                            }
//                        ]
//                    }";
//                    await File.WriteAllTextAsync(_tempLessonDataPath, lessonData);

//                    var settings = new CourseSetting
//                    {
//                        Minutes = 120,
//                        NewKeysPerStep = 1,
//                        TargetStats = new StatsBase
//                        {
//                            Accuracy = 85,
//                            Wpm = 30
//                        },
//                        PhaseAttemptThreshold = PhaseAttemptThreshold
//                    };

//                    var courseService = new CourseService(_mockRepository.Object, _mockMapper.Object, _mockLogger.Object);
//                    // Override the LessonDataUrl for testing
//                    _course = await courseService.GenerateBeginnerCourse(settings) as BeginnerCourse;
//                    _initialized = true;
//                }
//            }
//            finally
//            {
//                _initializationLock.Release();
//            }
//        }

//        public void Dispose()
//        {
//            // Clean up the temporary test files
//            if (File.Exists(_tempLessonDataPath))
//            {
//                try
//                {
//                    File.Delete(_tempLessonDataPath);
//                    Directory.Delete(Path.GetDirectoryName(_tempLessonDataPath)!, true);
//                }
//                catch
//                {
//                    // Ignore cleanup errors
//                }
//            }
//        }

//        [Fact]
//        public void HasNoEmptyId()
//        {
//            // Arrange
//            // Act
//            var id = _course.Id;

//            // Assert
//            Assert.NotEqual(Guid.Empty, id);
//        }

//        [Fact]
//        public void HasNoEmptyName()
//        {
//            // Arrange
//            // Act
//            var name = _course.Name;

//            // Assert
//            Assert.Equal("Beginner Typing Course", name);
//        }

//        [Fact]
//        public void HasPracticeType()
//        {
//            // Arrange
//            // Act
//            var type = _course.Type;

//            // Assert
//            Assert.Equal(TrainingType.Course, type);
//        }

//        [Fact]
//        public void HasCourseDescription()
//        {
//            // Arrange
//            // Act
//            var description = _course.Description;

//            // Assert
//            Assert.NotEmpty(description);
//        }

//        [Fact]
//        public void GetLeftHomeKeyTestWords()
//        {
//            // Arrange
//            var stats = new StatsBase
//            {
//                Accuracy = 50,
//                Wpm = 20,
//            };

//            // Act
//            var lesson = _course.GetPracticeLesson(1, stats);

//            // Assert
//            Assert.NotNull(lesson);
//            Assert.NotEmpty(lesson.PracticeText);
//            Assert.True(PhaseAttemptThreshold >= lesson.PracticeText.Length);
//            Assert.True(lesson.PracticeText.ContainsOnlyAllowedChars(['a', 's', 'd', 'f']));
//        }

//        [Fact]
//        public void GeneratePracticeText_RespectsLengthLimit()
//        {
//            // Arrange
//            var stats = new StatsBase
//            {
//                Accuracy = 50,
//                Wpm = 20,
//            };

//            // Act
//            var lesson = _course.GetPracticeLesson(1, stats);

//            // Assert
//            Assert.NotNull(lesson);
//            Assert.NotEmpty(lesson.PracticeText);
//            Assert.True(lesson.PracticeText.Length <= _course.Settings.PhaseAttemptThreshold,
//                $"Practice text length {lesson.PracticeText.Length} should be less than or equal to {_course.Settings.PhaseAttemptThreshold}");
//        }

//        [Fact]
//        public void GeneratePracticeText_DifferentTextsForSameLesson()
//        {
//            // Arrange
//            var stats = new StatsBase
//            {
//                Accuracy = 50,
//                Wpm = 20,
//            };

//            // Act - get practice text twice for the same lesson
//            var lesson1 = _course.GetPracticeLesson(1, stats);
//            var lesson2 = _course.GetPracticeLesson(1, stats);

//            // Assert - texts should be different due to randomization
//            Assert.NotNull(lesson1);
//            Assert.NotNull(lesson2);
//            Assert.NotEmpty(lesson1.PracticeText);
//            Assert.NotEmpty(lesson2.PracticeText);
//            Assert.NotEqual(lesson1.PracticeText, lesson2.PracticeText);
//        }

//        [Fact]
//        public void GeneratePracticeText_HandlesMultipleLessons()
//        {
//            // Arrange
//            var stats1 = new StatsBase
//            {
//                Accuracy = 10, // Perfect stats to move to next lesson
//                Wpm = 10,
//            };
//            var stats2 = new StatsBase
//            {
//                Accuracy = 100, // Perfect stats to move to next lesson
//                Wpm = 100,
//            };

//            // Act - get practice texts for two consecutive lessons
//            var lesson1 = _course.GetPracticeLesson(1, stats1);
//            var lesson2 = _course.GetPracticeLesson(1, stats2);

//            // Assert - both lessons should have valid practice texts
//            Assert.NotNull(lesson1);
//            Assert.NotNull(lesson2);
//            Assert.NotEmpty(lesson1.PracticeText);
//            Assert.NotEmpty(lesson2.PracticeText);

//            // Lesson texts should differ as they have different target keys
//            Assert.NotEqual(lesson1.PracticeText, lesson2.PracticeText);
//        }
//    }
//}