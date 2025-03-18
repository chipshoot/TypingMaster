namespace TypingMaster.Business.Models
{
    public class Account
    {
        public int Id { get; set; }

        public string AccountName { get; set; } = null!;

        /// <summary>
        /// Unique email address of the account and also in system
        /// </summary>
        public string AccountEmail { get; set; } = null!;

        public UserProfile User { get; set; } = null!;

        public StatsBase? GoalStats { get; set; }

        public PracticeLog History { get; set; } = null!;

        public Guid CourseId { get; set; }

        public Guid TestCourseId { get; set; }

        public Guid GameCourseId { get; set; }

        public bool IsDeleted { get; set; } = false;

        public DateTime? DeletedAt { get; set; }

        /// <summary>
        /// Version number for optimistic concurrency control
        /// </summary>
        public int Version { get; set; }
    }
}