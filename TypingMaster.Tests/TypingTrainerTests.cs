using Serilog;
using Moq;
using TypingMaster.Business;
using TypingMaster.Business.Contract;
using TypingMaster.Business.Course;
using TypingMaster.Core.Models;
using TypingMaster.Core.Models.Courses;

namespace TypingMaster.Tests
{
    public class TypingTrainerTests
    {
        private const string CourseId = "534751AD-3817-404A-BA0F-9C83820C6A37";
        private TypingTrainer _typingTrainer;
        private readonly Guid _testCourseId = new Guid(CourseId);
        private readonly List<Account> _accounts;

        public TypingTrainerTests()
        {
            var mockLogger = new Mock<ILogger>();
            var mockCourseService = new Mock<ICourseService>();
            var advancedCourse = new AdvancedLevelCourse(mockLogger.Object)
            {
                Id = new Guid(CourseId),
                Lessons =
                [
                    new Lesson
                    {
                        Id = 1,
                        Target = null,
                        Instruction = null,
                        IsCourseComplete = false,
                        PracticeText = "Beginner Course.",
                        Point = 1,
                        Description = null
                    },
                    new Lesson
                        { Id = 2, PracticeText = "Novice Course.", Point = 2 },
                    new Lesson
                        { Id = 3, PracticeText = "Intermediate Course.", Point = 3 },
                    new Lesson
                        { Id = 4, PracticeText = "Advanced Course.", Point = 4 },
                    new Lesson
                        { Id = 5, PracticeText = "Expert Course.", Point = 5 },
                    new Lesson
                        { Id = 6, PracticeText = "Beginner Course2.", Point = 1 }
                ]
            };

            var courseDto = new CourseDto
            {
                Id = new Guid(CourseId),
                Name = "AdvancedLevelCourse",
                Type = TrainingType.Course,
                LessonDataUrl = "",
                Description = "Advanced Level Course",
                Lessons = advancedCourse.Lessons,
                Settings = new CourseSetting
                {
                    Minutes = 30,
                    NewKeysPerStep = 1,
                    PracticeTextLength = 50,
                    TargetStats = new StatsBase
                    {
                        Wpm = 50,
                        Accuracy = 90
                    }
                }
            };
            mockCourseService.Setup(x => x.GetCourse(_testCourseId))
                .Returns(Task.FromResult(courseDto));

            _accounts = [
                new Account
                {
                    Id = 1,
                    AccountName = "BeginnerUser",
                    AccountEmail = "testuser@example.com",
                    User = new UserProfile { FirstName = "Beginner", LastName = "User", Title = "Mr." },
                    GoalStats = new DrillStats { Wpm = 75, Accuracy = 95 },
                    History = new PracticeLog
                    {
                        CurrentCourseId = _testCourseId,
                        CurrentLessonId = 1,
                        PracticeStats =
                        [
                            new DrillStats
                            {
                                Wpm = 20,
                                Accuracy = 70,
                                CourseId = _testCourseId,
                                LessonId = 1,
                                KeyEvents = new Queue<KeyEvent>(),
                                StartTime = null,
                                FinishTime = null,
                                TypedText = ""
                            }
                        ]
                    },
                    CourseId = _testCourseId
                },

                new Account
                {
                    Id = 2,
                    AccountName = "NoviceUser",
                    AccountEmail = "testuser@example.com",
                    User = new UserProfile { FirstName = "Novice", LastName = "User", Title = "Mr." },
                    GoalStats = new DrillStats { Wpm = 75, Accuracy = 95 },
                    History = new PracticeLog
                    {
                        CurrentCourseId = _testCourseId,
                        CurrentLessonId = 1,
                        PracticeStats =
                        [
                            new DrillStats
                            {
                                Wpm = 32,
                                Accuracy = 82,
                                CourseId = _testCourseId,
                                LessonId = 2,
                                KeyEvents = new Queue<KeyEvent>(),
                                StartTime = null,
                                FinishTime = null,
                                TypedText = ""
                            }
                        ]
                    },
                    CourseId = _testCourseId
                },

                new Account
                {
                    Id = 3,
                    AccountName = "IntermediateUser",
                    AccountEmail = "testuser@example.com",
                    User = new UserProfile { FirstName = "Intermediate", LastName = "User", Title = "Mr." },
                    GoalStats = new DrillStats { Wpm = 75, Accuracy = 95 },
                    History = new PracticeLog
                    {
                        CurrentCourseId = _testCourseId,
                        CurrentLessonId = 1,
                        PracticeStats =
                        [
                            new DrillStats
                            {
                                Wpm = 46,
                                Accuracy = 86,
                                CourseId = _testCourseId,
                                LessonId = 3,
                                KeyEvents = new Queue<KeyEvent>(),
                                StartTime = null,
                                FinishTime = null,
                                TypedText = ""
                            }
                        ]
                    },
                    CourseId = _testCourseId
                },

                new Account
                {
                    Id = 4,
                    AccountName = "AdvancedUser",
                    AccountEmail = "testuser@example.com",
                    User = new UserProfile { FirstName = "Advanced", LastName = "User", Title = "Mr." },
                    GoalStats = new DrillStats { Wpm = 80, Accuracy = 95 },
                    History = new PracticeLog
                    {
                        CurrentCourseId = _testCourseId,
                        CurrentLessonId = 1,
                        PracticeStats =
                        [
                            new DrillStats
                            {
                                Wpm = 61,
                                Accuracy = 94,
                                CourseId = _testCourseId,
                                LessonId = 4,
                                KeyEvents = new Queue<KeyEvent>(),
                                StartTime = null,
                                FinishTime = null,
                                TypedText = ""
                            }
                        ],
                        KeyStats = []
                    },
                    CourseId = _testCourseId
                }
            ];


            _typingTrainer = new TypingTrainer(mockCourseService.Object, mockLogger.Object);
        }

        //[Theory]
        //[InlineData(1, "Beginner Course.")]
        //[InlineData(2, "Novice Course.")]
        //[InlineData(3, "Intermediate Course.")]
        //[InlineData(4, "Advanced Course.")]
        //[InlineData(5, "Expert Course.")]
        //public void CanGetTextBasedOnId(int lessonId, string expectedText)
        //{
        //    // Act
        //    var actualText = _typingTrainer.GetPracticeText(lessonId);

        //    // Assert
        //    Assert.Equal(expectedText, actualText);
        //}

        //        [Theory]
        //        [InlineData("BeginnerUser", "Beginner Course2.")]
        //        [InlineData("NoviceUser", "Novice Course.")]
        //        [InlineData("IntermediateUser", "Intermediate Course.")]
        //        [InlineData("AdvancedUser", "Expert Course.")]
        //        public void CanGetTextBasedOnAccount(string accountName, string expectedText)
        //        {
        //            // Arrange
        //            var account = _accounts.FirstOrDefault(x => x.AccountName == accountName);
        //            Assert.NotNull(account);
        //            _typingTrainer = new TypingTrainer(account);

        //            // Act
        //            var (_, actualText) = _typingTrainer.GetPracticeText();

        //            // Assert
        //            Assert.Equal(expectedText, actualText);
        //        }

        //[Fact]
        //public void CanGetTextForNewUser()
        //{
        //    // Arrange
        //    var account = _accounts.FirstOrDefault();
        //    Assert.NotNull(account);
        //    account.Progress = new List<LearningProgress>();

        //    // Act
        //    var (_, text) = _typingTrainer.GetPracticeText();

        //    // Assert
        //    Assert.Equal("Beginner Course.", text);
        //}

        //        [Fact]
        //        public void CanGetNextTextWithSameLevelWhenCurrentStatsDoesNotImprove()
        //        {
        //            // Arrange
        //            var account = _accounts.FirstOrDefault();
        //            Assert.NotNull(account);
        //            account.Progress =
        //            [
        //                new LearningProgress { CourseId = 1, LessonId = 1, Stats = new DrillStats { Wpm = 20, Accuracy = 70 } }
        //            ];

        //            // Act
        //            var (_, text) = _typingTrainer.GetPracticeText();

        //            // Assert
        //            Assert.Equal("Beginner Course2.", text);
        //        }

        //        [Fact]
        //        public void CanGetNextTextWithNextLevelWhenCurrentStatsImproved()
        //        {
        //            // Arrange
        //            var account = _accounts.FirstOrDefault();
        //            Assert.NotNull(account);
        //            account.Progress =
        //            [
        //                new LearningProgress { CourseId = 1, LessonId = 1, Stats = new DrillStats { Wpm = 20, Accuracy = 70 } },
        //                new LearningProgress { CourseId = 1, LessonId = 6, Stats = new DrillStats { Wpm = 74, Accuracy = 90 } }
        //            ];

        //            // Act
        //            var (_, text) = _typingTrainer.GetPracticeText();

        //            // Assert
        //            Assert.Equal("Novice Course.", text);
        //        }

        //        [Fact]
        //        public void CanGetCourseFinishTextWhenAllLessonFinished()
        //        {
        //            // Arrange
        //            const string expectedText = "Congratulation, You have completed all lessons in this course.";
        //            var account = _accounts.FirstOrDefault();
        //            Assert.NotNull(account);
        //            account.Progress =
        //            [
        //                new LearningProgress { CourseId = 1, LessonId = 5, Stats = new DrillStats { Wpm = 20, Accuracy = 70 } },
        //                new LearningProgress { CourseId = 1, LessonId = 5, Stats = new DrillStats { Wpm = 74, Accuracy = 90 } }
        //            ];

        //            _typingTrainer = new TypingTrainer(account);

        //            // Act
        //            var (_, actualText) = _typingTrainer.GetPracticeText();

        //            // Assert
        //            Assert.Equal(expectedText, actualText);
        //        }
    }
}