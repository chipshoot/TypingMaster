//using Serilog;
//using Moq;
//using TypingMaster.Business;
//using TypingMaster.Business.Contract;
//using TypingMaster.Business.Models;
//using TypingMaster.Business.Models.Courses;

//namespace TypingMaster.Tests
//{
//    public class TypingTrainerTests
//    {
//        private TypingTrainer _typingTrainer;

//        public TypingTrainerTests()
//        {
//            var mockLogger = new Mock<ILogger>();
//            var mockCourseService = new Mock<ICourseService>();
//            var course = new AdvancedLevelCourse
//            {
//                Id = 1,
//                Lessons =
//                [
//                    new Lesson
//                        { Id = 1, PracticeTexts = "Beginner Course.", Point = 1 },
//                    new Lesson
//                        { Id = 2, PracticeTexts = "Novice Course.", Point = 2 },
//                    new Lesson
//                        { Id = 3, PracticeTexts = "Intermediate Course.", Point = 3 },
//                    new Lesson
//                        { Id = 4, PracticeTexts = "Advanced Course.", Point = 4 },
//                    new Lesson
//                        { Id = 5, PracticeTexts = "Expert Course.", Point = 5 },
//                    new Lesson
//                        { Id = 6, PracticeTexts = "Beginner Course2.", Point = 1 }
//                ]
//            };
//            mockCourseService.Setup(x => x.GetCourse(1)).Returns(course);

//            List<Account> accounts = [
//                new Account
//                {
//                    Id = 1,
//                    AccountName = "BeginnerUser",
//                    Email = "testuser@example.com",
//                    User = new UserProfile { FirstName = "Beginner", LastName = "User", Title = "Mr." },
//                    GoalStats = new DrillStats { Wpm = 75, Accuracy = 95 },
//                    History = new PracticeLog
//                    {
//                        CurrentCourseId = 1,
//                        CurrentLessonId = 1,
//                        PracticeStats =
//                        [
//                            new DrillStats
//                            {
//                                Wpm = 20,
//                                Accuracy = 70,
//                                CourseId = 1,
//                                LessonId = 1,
//                                KeyEvents = new Queue<KeyEvent>(),
//                                StartTime = null,
//                                FinishTime = null,
//                                TypedText = ""
//                            }
//                        ]
//                    },
//                    CourseId = 1
//                },

//                new Account
//                {
//                    Id = 2,
//                    AccountName = "NoviceUser",
//                    Email = "testuser@example.com",
//                    User = new UserProfile { FirstName = "Novice", LastName = "User", Title = "Mr." },
//                    GoalStats = new DrillStats { Wpm = 75, Accuracy = 95 },
//                    History = new PracticeLog
//                    {
//                        CurrentCourseId = 1,
//                        CurrentLessonId = 1,
//                        PracticeStats =
//                        [
//                            new DrillStats
//                            {
//                                Wpm = 32,
//                                Accuracy = 82,
//                                CourseId = 1,
//                                LessonId = 2,
//                                KeyEvents = new Queue<KeyEvent>(),
//                                StartTime = null,
//                                FinishTime = null,
//                                TypedText = ""
//                            }
//                        ]
//                    },
//                    CourseId = 1
//                },

//                new Account
//                {
//                    Id = 3,
//                    AccountName = "IntermediateUser",
//                    Email = "testuser@example.com",
//                    User = new UserProfile { FirstName = "Intermediate", LastName = "User", Title = "Mr." },
//                    GoalStats = new DrillStats { Wpm = 75, Accuracy = 95 },
//                    History = new PracticeLog
//                    {
//                        CurrentCourseId = 1,
//                        CurrentLessonId = 1,
//                        PracticeStats =
//                        [
//                            new DrillStats
//                            {
//                                Wpm = 46,
//                                Accuracy = 86,
//                                CourseId = 1,
//                                LessonId = 3,
//                                KeyEvents = new Queue<KeyEvent>(),
//                                StartTime = null,
//                                FinishTime = null,
//                                TypedText = ""
//                            }
//                        ]
//                    },
//                    CourseId = 1
//                },

//                new Account
//                {
//                    Id = 4,
//                    AccountName = "AdvancedUser",
//                    Email = "testuser@example.com",
//                    User = new UserProfile { FirstName = "Advanced", LastName = "User", Title = "Mr." },
//                    GoalStats = new DrillStats { Wpm = 80, Accuracy = 95 },
//                    History = new PracticeLog
//                    {
//                        CurrentCourseId = 1,
//                        CurrentLessonId = 1,
//                        PracticeStats =
//                        [
//                            new DrillStats
//                            {
//                                Wpm = 61,
//                                Accuracy = 94,
//                                CourseId = 1,
//                                LessonId = 4,
//                                KeyEvents = new Queue<KeyEvent>(),
//                                StartTime = null,
//                                FinishTime = null,
//                                TypedText = ""
//                            }
//                        ],
//                        KeyStats = []
//                    },
//                    CourseId = 1
//                }
//            ];


//            _typingTrainer = new TypingTrainer(mockCourseService.Object, mockLogger.Object);
//        }

//        //[Theory]
//        //[InlineData(1, "Beginner Course.")]
//        //[InlineData(2, "Novice Course.")]
//        //[InlineData(3, "Intermediate Course.")]
//        //[InlineData(4, "Advanced Course.")]
//        //[InlineData(5, "Expert Course.")]
//        //public void CanGetTextBasedOnId(int lessonId, string expectedText)
//        //{
//        //    // Act
//        //    var actualText = _typingTrainer.GetPracticeText(lessonId);

//        //    // Assert
//        //    Assert.Equal(expectedText, actualText);
//        //}

//        //        [Theory]
//        //        [InlineData("BeginnerUser", "Beginner Course2.")]
//        //        [InlineData("NoviceUser", "Novice Course.")]
//        //        [InlineData("IntermediateUser", "Intermediate Course.")]
//        //        [InlineData("AdvancedUser", "Expert Course.")]
//        //        public void CanGetTextBasedOnAccount(string accountName, string expectedText)
//        //        {
//        //            // Arrange
//        //            var account = _accounts.FirstOrDefault(x => x.AccountName == accountName);
//        //            Assert.NotNull(account);
//        //            _typingTrainer = new TypingTrainer(account);

//        //            // Act
//        //            var (_, actualText) = _typingTrainer.GetPracticeText();

//        //            // Assert
//        //            Assert.Equal(expectedText, actualText);
//        //        }

//        //        [Fact]
//        //        public void CanGetTextForNewUser()
//        //        {
//        //            // Arrange
//        //            var account = _accounts.FirstOrDefault();
//        //            Assert.NotNull(account);
//        //            account.Progress = new List<LearningProgress>();

//        //            // Act
//        //            var (_, text) = _typingTrainer.GetPracticeText();

//        //            // Assert
//        //            Assert.Equal("Beginner Course.", text);
//        //        }

//        //        [Fact]
//        //        public void CanGetNextTextWithSameLevelWhenCurrentStatsDoesNotImprove()
//        //        {
//        //            // Arrange
//        //            var account = _accounts.FirstOrDefault();
//        //            Assert.NotNull(account);
//        //            account.Progress =
//        //            [
//        //                new LearningProgress { CourseId = 1, LessonId = 1, Stats = new DrillStats { Wpm = 20, Accuracy = 70 } }
//        //            ];

//        //            // Act
//        //            var (_, text) = _typingTrainer.GetPracticeText();

//        //            // Assert
//        //            Assert.Equal("Beginner Course2.", text);
//        //        }

//        //        [Fact]
//        //        public void CanGetNextTextWithNextLevelWhenCurrentStatsImproved()
//        //        {
//        //            // Arrange
//        //            var account = _accounts.FirstOrDefault();
//        //            Assert.NotNull(account);
//        //            account.Progress =
//        //            [
//        //                new LearningProgress { CourseId = 1, LessonId = 1, Stats = new DrillStats { Wpm = 20, Accuracy = 70 } },
//        //                new LearningProgress { CourseId = 1, LessonId = 6, Stats = new DrillStats { Wpm = 74, Accuracy = 90 } }
//        //            ];

//        //            // Act
//        //            var (_, text) = _typingTrainer.GetPracticeText();

//        //            // Assert
//        //            Assert.Equal("Novice Course.", text);
//        //        }

//        //        [Fact]
//        //        public void CanGetCourseFinishTextWhenAllLessonFinished()
//        //        {
//        //            // Arrange
//        //            const string expectedText = "Congratulation, You have completed all lessons in this course.";
//        //            var account = _accounts.FirstOrDefault();
//        //            Assert.NotNull(account);
//        //            account.Progress =
//        //            [
//        //                new LearningProgress { CourseId = 1, LessonId = 5, Stats = new DrillStats { Wpm = 20, Accuracy = 70 } },
//        //                new LearningProgress { CourseId = 1, LessonId = 5, Stats = new DrillStats { Wpm = 74, Accuracy = 90 } }
//        //            ];

//        //            _typingTrainer = new TypingTrainer(account);

//        //            // Act
//        //            var (_, actualText) = _typingTrainer.GetPracticeText();

//        //            // Assert
//        //            Assert.Equal(expectedText, actualText);
//        //        }
//    }
//}