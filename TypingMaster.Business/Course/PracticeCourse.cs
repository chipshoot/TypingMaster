using TypingMaster.Business.Contract;
using TypingMaster.Core.Models;
using TypingMaster.Core.Models.Courses;

namespace TypingMaster.Business.Course
{
    /// <summary>
    /// The course that increase the level of lesson based on the user's performance.
    /// If the user performs is better than advanced level, the lesson level will be increased.
    /// </summary>
    public class PracticeCourse : ServiceBase, ICourse
    {
        public PracticeCourse(string name, Serilog.ILogger logger, string lessonDataFileUrl, string description) : base(logger)
        {
            Type = TrainingType.Course;
            LessonDataUrl = lessonDataFileUrl;
            CompleteText = CourseCompleteText;
            Name = name;
            Description = description;
            Settings = new CourseSetting
            {
                Minutes = 0,
                TargetStats = new StatsBase
                {
                    Wpm = 0,
                    Accuracy = 0,
                },
                NewKeysPerStep = 0,
                PracticeTextLength = 74
            };
        }

        private const string CourseCompleteText = "Congratulation, You have completed all lessons in this course.";

        public Guid Id { get; set; }

        public string Name { get; set; }

        public TrainingType Type { get; }

        public string LessonDataUrl { get; }

        public IEnumerable<Lesson> Lessons { get; set; } = [];

        public string CompleteText { get; }

        public CourseSetting Settings { get; set; }

        public string Description { get; } = string.Empty;

        public Lesson? GetPracticeLesson(int curLessonId, StatsBase stats)
        {
            Lesson? nextLesson;
            if (curLessonId == 0)
            {
                nextLesson = Lessons.FirstOrDefault(l => l.Id == 1);
            }
            else
            {
                var curLessonPoint = Lessons.FirstOrDefault(x => x.Id == curLessonId)?.Point ?? 1;
                var curSkill = stats.GetSkillLevel();

                // if current skill level above advanced, return the lesson with bigger point, otherwise return the lesson with same point
                if (curSkill >= SkillLevel.Advanced)
                {
                    nextLesson = Lessons.FirstOrDefault(l => l.Point > curLessonPoint);
                }
                else
                {
                    nextLesson = Lessons.FirstOrDefault(l => l.Point == curLessonPoint && l.Id > curLessonId)
                                 ?? Lessons.FirstOrDefault(l => l.Point == curLessonPoint && l.Id == curLessonId);
                }
            }

            // get next lesson of the current level
            if (nextLesson == null)
            {
                nextLesson = new Lesson
                {
                    Id = curLessonId,
                    IsCourseComplete = true,
                    Instruction = CompleteText,
                    PracticeText = string.Empty
                };
            }

            return nextLesson;
        }
    }
}