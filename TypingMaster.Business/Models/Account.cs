namespace TypingMaster.Business.Models
{
    public class Account
    {
        public int Id { get; set; }

        public string AccountName { get; set; } = null!;

        public string Email { get; set; } = null!;

        public UserProfile User { get; set; } = null!;

        public TypingStats? GoalStats { get; set; }

        public IList<LearningProgress> Progress { get; set; } = null!;

        public double PracticeTime { get; set; } // Total practice time in hours

        public Course CurrentCourse { get; set; } = null!;
    }
}