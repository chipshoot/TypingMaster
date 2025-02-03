namespace TypingMaster.Business.Models
{
    public class Account
    {
        public int Id { get; set; }
        public string AccountName { get; set; }
        public string Email { get; set; }
        public UserProfile User { get; set; }
        public TypingStats GoalStats { get; set; }
        public TypingStats CurrentStats { get; set; }
        public double PracticeTime { get; set; } // Total practice time in hours
        public Course CurrentCourse { get; set; }
    }
}