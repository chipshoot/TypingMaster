using TypingMaster.Business.Contract;

namespace TypingMaster.Business.Models.Courses
{
    /// <summary>
    /// The course that increase the level of lesson based on the user's performance.
    /// If the user performs is better than advanced level, the lesson level will be increased.
    /// </summary>
    public class AdvancedLevelCourse : ICourse
    {
        private const string CourseDescription = "The course advances to the next level of lessons if the current typing performance level is equal to or above the advanced level.";

        private const string CourseCompleteText = "Congratulation, You have completed all lessons in this course.";

        public int Id { get; set; }

        public string Name { get; set; } =  "Skill Level Charge Course";

        public string CompleteText => CourseCompleteText;

        public IEnumerable<Lesson> Lessons { get; set; } = [];

        public Lesson? GetPracticeLesson(DrillStats stats)
        {
            Lesson? nextLesson;
            if (stats.LessonId == 0)
            {
                nextLesson = Lessons.FirstOrDefault(l => l.Id == 1);
            }
            else
            {
                var curLessonId = stats.LessonId;
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
            return nextLesson;
        }

        public bool IsCompleted(DrillStats stats)
        {
            ArgumentNullException.ThrowIfNull(stats);

            var curLesson = Lessons.FirstOrDefault(l => l.Id == stats.LessonId) ??
                            throw new InvalidOperationException("Lesson not found.");

            Lesson? nextLesson;
            var curSkill = stats.GetSkillLevel();
            if (curSkill >= SkillLevel.Advanced)
            {
                nextLesson = Lessons.FirstOrDefault(l => l.Point > curLesson.Point);
            }
            else
            {
                nextLesson = Lessons.FirstOrDefault(l => l.Point == curLesson.Point && l.Id > curLesson.Id)
                             ?? Lessons.FirstOrDefault(l => l.Point == curLesson.Point && l.Id == curLesson.Id);
            }

            return nextLesson == null;
        }

        public DrillStats GenerateStartStats()
        {
            return new DrillStats
            {
                CourseId = Id,
                LessonId = 1,
                Wpm = 0,
                Accuracy = 0,
                KeyEvents = [],
                TypedText = string.Empty,
                StartTime = DateTime.Now,
                FinishTime = DateTime.UtcNow
            };
        }

        public string Description { get; } = CourseDescription;
    }
}