using TypingMaster.Business.Contract;

namespace TypingMaster.Business.Models
{
    public class Account
    {
        public int Id { get; set; }

        public string AccountName { get; set; } = null!;

        public string Email { get; set; } = null!;

        public UserProfile User { get; set; } = null!;

        public DrillStats? GoalStats { get; set; }

        public PracticeLog History { get; set; } = null!;

        public ICourse CurrentCourse { get; set; } = null!;
    }
}